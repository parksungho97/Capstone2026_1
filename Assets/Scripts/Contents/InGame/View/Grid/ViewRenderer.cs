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

        // [Step 1]
        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        var frustumObstacles = viewInfo.Obstacles
            .Where(r => r != null && GeometryUtility.TestPlanesAABB(cameraPlanes, r.bounds))
            .ToList();

        // [Step 2] 프러스텀 내 장애물 중 카메라→플레이어 레이에 맞는 것 선별
        List<Renderer> xRayObstacles = FindXRayObstacles(mainCamera.transform.position, playerPos, frustumObstacles);

        // [Step 3] 레이에 맞은 장애물 → 원본 복사+투명화, 해제된 장애물 → 원래 재질 복원
        var newXRaySet = new HashSet<Renderer>(xRayObstacles);

        var toRestore = new List<Renderer>();
        foreach (var kvp in _xrayActive)
        {
            if (kvp.Key == null || !newXRaySet.Contains(kvp.Key))
            {
                if (kvp.Key != null) kvp.Key.sharedMaterials = kvp.Value.originals;
                foreach (var m in kvp.Value.transparents)
                    if (m != null) Object.Destroy(m);
                toRestore.Add(kvp.Key);
            }
        }
        foreach (var r in toRestore)
            _xrayActive.Remove(r);

        foreach (var r in xRayObstacles)
        {
            if (r == null || _xrayActive.ContainsKey(r)) continue;
            var originals = r.sharedMaterials;
            var transparents = new Material[originals.Length];
            for (int i = 0; i < originals.Length; i++)
                transparents[i] = originals[i] != null ? MakeTransparentCopy(originals[i]) : null;
            _xrayActive[r] = (originals, transparents);
            r.sharedMaterials = transparents;
        }

        commandBuffer.Clear();

        // 탑다운 카메라를 월드 센터 위에 고정 (안개 레이스테핑용)
        // aspect=1 (512×512 정사각형 RT), orthographicSize는 맵의 넓은 쪽 절반을 커버
        topDownCamera.transform.position = new Vector3(0f, topDownHeight, 0f);
        topDownCamera.orthographicSize = Mathf.Max(viewInfo.WorldX, viewInfo.WorldY) * 0.5f;

        Matrix4x4 topDownGpuProj = GL.GetGPUProjectionMatrix(topDownCamera.projectionMatrix, false);
        Matrix4x4 topDownVP = topDownGpuProj * topDownCamera.worldToCameraMatrix;
        viewRenderMaterial.SetMatrix("_ObstacleMaskVP", topDownVP);
        // ── Phase 2: ViewRender 안개 후처리 ──────────────────────────────────────
        // [Step 6] 탑다운 장애물 마스크 생성 후 안개 셰이더 적용
        int fogTempRT = Shader.PropertyToID("_FogTemp");
        commandBuffer.GetTemporaryRT(fogTempRT, mainCamera.pixelWidth, mainCamera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.Default);

        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, fogTempRT);

        commandBuffer.SetRenderTarget(new RenderTargetIdentifier(obstacleMaskTexture));
        commandBuffer.ClearRenderTarget(true, true, Color.clear, 1f);
        commandBuffer.SetViewProjectionMatrices(topDownCamera.worldToCameraMatrix, topDownGpuProj);

        Plane[] topDownPlanes = GeometryUtility.CalculateFrustumPlanes(topDownCamera);
        foreach (var r in frustumObstacles)
            commandBuffer.DrawRenderer(r, stencilMat);

        commandBuffer.SetGlobalTexture("_ObstacleMask", obstacleMaskTexture);
        //commandBuffer.Blit(obstacleMaskTexture, BuiltinRenderTextureType.CameraTarget);
        commandBuffer.Blit(fogTempRT, BuiltinRenderTextureType.CameraTarget, viewRenderMaterial);
        commandBuffer.ReleaseTemporaryRT(fogTempRT);
    }

    public void Cleanup()
    {
        foreach (var kvp in _xrayActive)
        {
            if (kvp.Key != null) kvp.Key.sharedMaterials = kvp.Value.originals;
            foreach (var m in kvp.Value.transparents)
                if (m != null) Object.Destroy(m);
        }
        _xrayActive.Clear();

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
        if (topDownCameraGO != null)
        {
            Object.Destroy(topDownCameraGO);
            topDownCameraGO = null;
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

    private static Material MakeTransparentCopy(Material original, float alpha = 0.1f)
    {
        var mat = new Material(original);
        if (mat.HasProperty("_SrcBlend"))
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        if (mat.HasProperty("_Color"))
        {
            Color c = mat.GetColor("_Color");
            c.a = alpha;
            mat.SetColor("_Color", c);
        }
        return mat;
    }

    private Camera mainCamera;
    private Material viewRenderMaterial;
    private CommandBuffer commandBuffer;

    private RenderTexture obstacleMaskTexture;
    private Shader stencilShader;
    private Material stencilMat;

    private Camera topDownCamera;
    private GameObject topDownCameraGO;
    private float topDownHeight;

    private readonly Dictionary<Renderer, (Material[] originals, Material[] transparents)> _xrayActive =
        new Dictionary<Renderer, (Material[] originals, Material[] transparents)>();
}
