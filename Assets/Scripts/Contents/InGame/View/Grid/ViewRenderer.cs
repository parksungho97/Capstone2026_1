using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ViewRenderer
{
    public ViewRenderer(Camera mainCamera)
    {
        this.mainCamera = mainCamera;
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;

        Shader s = Shader.Find("jjh/ViewRender");
        viewRenderMaterial = new Material(s);

        commandBuffer = new CommandBuffer { name = "ViewGrid Fog Overlay" };
        mainCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);

        stencilShader = Shader.Find("jjh/ObstacleMask");
        stencilMat = new Material(stencilShader);

        obstacleMaskTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.R8)
        {
            name = "ObstacleMaskTexture",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };
        obstacleMaskTexture.Create();
    }

    public void SetFogColor(Color color, float opacity)
    {
        viewRenderMaterial.SetColor("_FogColor", color);
        viewRenderMaterial.SetFloat("_FogOpacity", opacity);
    }

    public void RenderView(RenderTexture viewGridBuffer, ViewInfo viewInfo, Vector3 playerPos)
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
        viewRenderMaterial.SetMatrix("_ViewProj", vp);

        viewRenderMaterial.SetTexture("_GridTex", viewGridBuffer);
        viewRenderMaterial.SetVector("_GridTex_TexelSize", new Vector4(1f / viewInfo.GridX, 1f / viewInfo.GridY, viewInfo.GridX, viewInfo.GridY));

        viewRenderMaterial.SetVector("_PlayerPos", playerPos);

        commandBuffer.Clear();

        int tempRT = Shader.PropertyToID("_TempViewRender");
        commandBuffer.GetTemporaryRT(tempRT, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.Default);

        // [1] 현재 화면 임시 텍스처에 복사
        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, tempRT);

        // [2] obstacleMaskTexture 클리어 후 장애물만 R=1로 그리기
        //     카메라 depth buffer를 직접 연결 → ZTest LEqual이 정상 작동
        commandBuffer.SetRenderTarget(
            new RenderTargetIdentifier(obstacleMaskTexture),
            BuiltinRenderTextureType.Depth);
        commandBuffer.ClearRenderTarget(false, true, Color.clear);

        foreach (var r in viewInfo.Obstacles)
            if (r != null) commandBuffer.DrawRenderer(r, stencilMat);

        // [3] 마스크를 셰이더에 넘기고 후처리 적용
        commandBuffer.SetGlobalTexture("_ObstacleMask", obstacleMaskTexture);
        commandBuffer.Blit(tempRT, BuiltinRenderTextureType.CameraTarget, viewRenderMaterial);

        commandBuffer.ReleaseTemporaryRT(tempRT);
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
    }

    private Camera mainCamera;
    private Material viewRenderMaterial;
    private CommandBuffer commandBuffer;

    private RenderTexture obstacleMaskTexture;
    private Shader stencilShader;
    private Material stencilMat;
}
