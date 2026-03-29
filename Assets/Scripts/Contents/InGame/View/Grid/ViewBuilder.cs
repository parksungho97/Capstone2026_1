
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ViewBuilder
{
    public RenderTexture ViewGridBuffer { get; private set; }

    public ViewBuilder(ComputeShader fovBuildShader)
    {
        this.fovBuildShader = fovBuildShader;
        fovBuildKernel = fovBuildShader.FindKernel("CSClearAndUpdate");
        Debug.Assert(fovBuildKernel >= 0, "CSClearAndUpdate kernel not found in fovBuildShader");
    }

    // 한번만 호출해서 버퍼 바인딩
    public void BindViewGridBuffer(int gridX, int gridY)
    {
        if (ViewGridBuffer)
            ViewGridBuffer.Release();

        ViewGridBuffer = new RenderTexture(gridX, gridY, 0, RenderTextureFormat.R8);
        ViewGridBuffer.enableRandomWrite = true;
        ViewGridBuffer.filterMode = FilterMode.Bilinear;
        ViewGridBuffer.wrapMode = TextureWrapMode.Clamp;
        ViewGridBuffer.Create();

        fovBuildShader.SetTexture(fovBuildKernel, "_Grid", ViewGridBuffer);
    }

    // 매 프레임마다 호출해서 FOV 정보 업데이트
    public void BuildViewGrid(ViewInfo viewInfo)
    {
        // FOV버퍼 업데이트
        List<FovInfo> fovInfos = viewInfo.FovInfos;
        fovBuffer = new ComputeBuffer(fovInfos.Count, UnsafeUtility.SizeOf<FovInfo>());
        fovBuffer.SetData(fovInfos);

        // 버퍼 바인딩
        fovBuildShader.SetBuffer(fovBuildKernel, "_FOVs", fovBuffer);

        // Constant 설정
        Vector3 worldOrigin = viewInfo.GetLeftBottom();
        fovBuildShader.SetInt("_GridX", (int)viewInfo.GridX);
        fovBuildShader.SetInt("_GridY", (int)viewInfo.GridY);
        fovBuildShader.SetFloat("_WorldX", viewInfo.WorldX);
        fovBuildShader.SetFloat("_WorldY", viewInfo.WorldY);
        fovBuildShader.SetVector("_WorldOrigin", (Vector4)worldOrigin);
        fovBuildShader.SetInt("_FOVCount", fovInfos.Count);

        fovBuildShader.Dispatch(fovBuildKernel, Mathf.CeilToInt(viewInfo.GridX / 8f), Mathf.CeilToInt(viewInfo.GridY / 8f), 1);

        fovBuffer.Release();
        fovBuffer = null;
    }

    public void Cleanup()
    {
        if (ViewGridBuffer)
        {
            ViewGridBuffer.Release();
            ViewGridBuffer = null;
        }
        if (fovBuffer != null)
        {
            fovBuffer.Dispose();
            fovBuffer = null;
        }
    }

    private ComputeShader fovBuildShader;

    private ComputeBuffer fovBuffer;

    private int fovBuildKernel;
}
