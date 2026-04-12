using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class ViewRenderer
{
    public ViewRenderer(Camera mainCamera, float topDownHeight = 20f)
    {
        this.mainCamera = mainCamera;
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;

        viewRenderMaterial = new Material(Shader.Find("jjh/ViewRender"));
        stencilWriteMat    = new Material(Shader.Find("jjh/StencilWrite"));
        xrayBlendMat       = new Material(Shader.Find("jjh/XRayBlend"));

        commandBuffer = new CommandBuffer { name = "ViewGrid Fog Overlay" };
        mainCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);

        stencilShader = Shader.Find("jjh/ObstacleMask");
        stencilMat = new Material(stencilShader);

        int um = 512;
        obstacleMaskTexture = new RenderTexture(um, um, 16, RenderTextureFormat.R8)
        {
            name = "ObstacleMaskTexture",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };
        obstacleMaskTexture.Create();

        this.topDownHeight = topDownHeight;
        topDownCameraGO = new GameObject("ObstacleMaskTopDownCamera");
        topDownCamera = topDownCameraGO.AddComponent<Camera>();
        topDownCamera.orthographic = true;
        topDownCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        topDownCamera.nearClipPlane = 0.1f;
        topDownCamera.farClipPlane = topDownHeight * 2f;
        topDownCamera.cullingMask = 0;
        topDownCamera.enabled = false;
        topDownCamera.aspect = 1;

        noObstacleCameraGO = new GameObject("NoObstacleCamera");
        noObstacleCamera = noObstacleCameraGO.AddComponent<Camera>();
        noObstacleCamera.enabled = false;
    }

    public void SetFogColor(Color color, float opacity)
    {
        viewRenderMaterial.SetColor("_FogColor", color);
        viewRenderMaterial.SetFloat("_FogOpacity", opacity);
    }

    public void RenderView(RenderTexture viewGridBuffer, ViewInfo viewInfo, Vector3 playerPos, float xRayRadius = 3f)
    {
        viewRenderMaterial.SetFloat("_GridX", viewInfo.GridX);
        viewRenderMaterial.SetFloat("_GridY", viewInfo.GridY);
        viewRenderMaterial.SetFloat("_WorldX", viewInfo.WorldX);
        viewRenderMaterial.SetFloat("_WorldY", viewInfo.WorldY);
        Vector3 origin = viewInfo.GetLeftBottom();
        viewRenderMaterial.SetVector("_WorldOrigin", new Vector4(origin.x, origin.y, origin.z, 1.0f));

        // 1. 역VP 행렬 갱신
        Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(mainCamera.projectionMatrix, false);
        Matrix4x4 vp = gpuProj * mainCamera.worldToCameraMatrix;
        viewRenderMaterial.SetMatrix("_InvVP", vp.inverse);

        viewRenderMaterial.SetTexture("_GridTex", viewGridBuffer);
        viewRenderMaterial.SetVector("_GridTex_TexelSize", new Vector4(1f / viewInfo.GridX, 1f / viewInfo.GridY, viewInfo.GridX, viewInfo.GridY));

        viewRenderMaterial.SetVector("_PlayerPos", playerPos);

        // [Step 1] 메인 카메라 프러스텀 내 장애물만 추림
        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        var frustumObstacles = viewInfo.Obstacles
            .Where(r => r != null && GeometryUtility.TestPlanesAABB(cameraPlanes, r.bounds))
            .ToList();

        // [Step 2] 프러스텀 내 장애물 중 카메라→플레이어 레이에 맞는 것 선별
        List<Renderer> xRayObstacles = FindXRayObstacles(mainCamera.transform.position, playerPos, frustumObstacles);

        // [Step 3] XRay 장애물 레이어 이동 후 배경 렌더링
        var layerBackup = new Dictionary<Renderer, int>();
        foreach (var r in xRayObstacles)
        {
            layerBackup[r] = r.gameObject.layer;
            r.gameObject.layer = xRayTempLayer;
        }
        EnsureNoObstacleTexture();
        noObstacleCameraGO.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);
        noObstacleCamera.CopyFrom(mainCamera);
        noObstacleCamera.targetTexture = noObstacleTexture;
        noObstacleCamera.cullingMask = mainCamera.cullingMask & ~(1 << xRayTempLayer);
        noObstacleCamera.depthTextureMode = DepthTextureMode.None;
        noObstacleCamera.RemoveAllCommandBuffers();
        noObstacleCamera.enabled = false;
        noObstacleCamera.Render();
        foreach (var kvp in layerBackup)
            if (kvp.Key != null) kvp.Key.gameObject.layer = kvp.Value;

        // 탑다운 카메라를 월드 센터 위에 고정 (안개 레이스테핑용)
        // aspect=1 (512×512 정사각형 RT), orthographicSize는 맵의 넓은 쪽 절반을 커버
        topDownCamera.transform.position = new Vector3(0f, topDownHeight, 0f);
        topDownCamera.orthographicSize = Mathf.Max(viewInfo.WorldX, viewInfo.WorldY) * 0.5f;

        Matrix4x4 topDownGpuProj = GL.GetGPUProjectionMatrix(topDownCamera.projectionMatrix, false);
        Matrix4x4 topDownVP = topDownGpuProj * topDownCamera.worldToCameraMatrix;
        viewRenderMaterial.SetMatrix("_ObstacleMaskVP", topDownVP);

        commandBuffer.Clear();

        // ── Phase 1: XRay 스텐실 블랜드 ─────────────────────────────────────────
        // [Step 4] 카메라→플레이어 레이에 걸린 장애물 픽셀에 스텐실=1 기록
        // [Step 5] 스텐실=1 픽셀을 noObstacleRT(배경)와 블랜딩
        if (xRayObstacles.Count > 0)
        {
            xrayBlendMat.SetTexture("_NoObstacleRT", noObstacleTexture);

            commandBuffer.SetViewProjectionMatrices(mainCamera.worldToCameraMatrix, gpuProj);
            foreach (var r in xRayObstacles)
                if (r != null) commandBuffer.DrawRenderer(r, stencilWriteMat);

            int xrayTempRT = Shader.PropertyToID("_XRayTemp");
            commandBuffer.GetTemporaryRT(xrayTempRT, mainCamera.pixelWidth, mainCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
            commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, xrayTempRT);
            commandBuffer.Blit(xrayTempRT, BuiltinRenderTextureType.CameraTarget, xrayBlendMat);
            commandBuffer.ReleaseTemporaryRT(xrayTempRT);
        }

        // ── Phase 2: ViewRender 안개 후처리 ──────────────────────────────────────
        // [Step 6] 탑다운 장애물 마스크 생성 후 안개 셰이더 적용
        int fogTempRT = Shader.PropertyToID("_FogTemp");
        commandBuffer.GetTemporaryRT(fogTempRT, mainCamera.pixelWidth, mainCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.Default);

        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, fogTempRT);

        commandBuffer.SetRenderTarget(new RenderTargetIdentifier(obstacleMaskTexture));
        commandBuffer.ClearRenderTarget(true, true, Color.clear, 1f);
        commandBuffer.SetViewProjectionMatrices(topDownCamera.worldToCameraMatrix, topDownGpuProj);

        Plane[] topDownPlanes = GeometryUtility.CalculateFrustumPlanes(topDownCamera);
        foreach (var r in viewInfo.Obstacles)
            if (r != null && GeometryUtility.TestPlanesAABB(topDownPlanes, r.bounds))
                commandBuffer.DrawRenderer(r, stencilMat);

        commandBuffer.SetGlobalTexture("_ObstacleMask", obstacleMaskTexture);
        commandBuffer.Blit(fogTempRT, BuiltinRenderTextureType.CameraTarget, viewRenderMaterial);
        commandBuffer.ReleaseTemporaryRT(fogTempRT);
    }

    public void Cleanup()
    {
        if (commandBuffer != null)
        {
            if (mainCamera)
                mainCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);
            commandBuffer.Dispose();
            commandBuffer = null;
        }
        if (viewRenderMaterial != null)
        {
            Object.Destroy(viewRenderMaterial);
            viewRenderMaterial = null;
        }
        if (stencilWriteMat != null)
        {
            Object.Destroy(stencilWriteMat);
            stencilWriteMat = null;
        }
        if (xrayBlendMat != null)
        {
            Object.Destroy(xrayBlendMat);
            xrayBlendMat = null;
        }
        if (topDownCameraGO != null)
        {
            Object.Destroy(topDownCameraGO);
            topDownCameraGO = null;
        }
        if (noObstacleTexture != null)
        {
            noObstacleTexture.Release();
            Object.Destroy(noObstacleTexture);
            noObstacleTexture = null;
        }
        if (noObstacleCameraGO != null)
        {
            Object.Destroy(noObstacleCameraGO);
            noObstacleCameraGO = null;
        }
    }

    private static readonly Vector2[] kSurroundDirs =
    {
        Vector2.zero,
        new Vector2( 1f, 0f), new Vector2(-1f, 0f),
        new Vector2( 0f, 1f), new Vector2( 0f,-1f),
        new Vector2( 1f, 1f), new Vector2( 1f,-1f),
        new Vector2(-1f, 1f), new Vector2(-1f,-1f),
    };

    private List<Renderer> FindXRayObstacles(Vector3 camPos, Vector3 playerPos, List<Renderer> obstacles)
    {
        var obstacleSet = new HashSet<Renderer>(obstacles.Where(r => r != null));
        var result = new HashSet<Renderer>();
        foreach (var dir2D in kSurroundDirs)
        {
            Vector3 target = playerPos + new Vector3(dir2D.x, 0f, dir2D.y) * 0.5f;
            Vector3 d = target - camPos;
            float dist = d.magnitude;
            if (dist < 0.001f) continue;
            foreach (var hit in Physics.RaycastAll(camPos, d / dist, dist))
            {
                Transform t = hit.transform;
                while (t != null)
                {
                    Renderer r = t.GetComponent<Renderer>();
                    if (r != null && obstacleSet.Contains(r)) { result.Add(r); break; }
                    t = t.parent;
                }
            }
        }
        return new List<Renderer>(result);
    }

    private void EnsureNoObstacleTexture()
    {
        int w = mainCamera.pixelWidth;
        int h = mainCamera.pixelHeight;
        if (noObstacleTexture != null && noObstacleTexture.width == w && noObstacleTexture.height == h)
            return;
        if (noObstacleTexture != null) { noObstacleTexture.Release(); Object.Destroy(noObstacleTexture); }
        noObstacleTexture = new RenderTexture(w, h, 24, RenderTextureFormat.Default)
        {
            name = "NoObstacleTexture",
            filterMode = FilterMode.Bilinear,
        };
        noObstacleTexture.Create();
    }

    private Camera mainCamera;
    private Material viewRenderMaterial;
    private Material stencilWriteMat;
    private Material xrayBlendMat;
    private CommandBuffer commandBuffer;

    private RenderTexture obstacleMaskTexture;
    private Shader stencilShader;
    private Material stencilMat;

    private Camera topDownCamera;
    private GameObject topDownCameraGO;
    private float topDownHeight;

    private Camera noObstacleCamera;
    private GameObject noObstacleCameraGO;
    private RenderTexture noObstacleTexture;
    private const int xRayTempLayer = 31;
}
