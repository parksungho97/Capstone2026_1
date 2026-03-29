using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// 외부에서 전달된 GameObject에 스텐실 셰이더를 세팅하고,
// CommandBuffer 가 씬 렌더 완료 후 스텐실=0 영역을 어둡게 처리.
public class FOVPostProcess : MonoBehaviour
{
    [SerializeField] private Color fogColor   = Color.black;
    [SerializeField] [Range(0f, 1f)] private float fogOpacity = 0.7f;

    private Material      _stencilMat;
    private Camera        _camera;
    private CommandBuffer _cb;
    private Material      _overlayMat;

    private void Awake()
    {
        Shader stencilShader = Shader.Find("jjh/FOVStencilMask");
        Debug.Assert(stencilShader != null, "[FOVPostProcess] ''jjh/FOVStencilMask'' 셰이더를 찾을 수 없습니다.");

        _stencilMat = new Material(stencilShader);
    }

    // 카메라를 받아 CommandBuffer를 등록. Start() 등에서 반드시 호출.
    public void Initialize(Camera cam)
    {
        _camera = cam;

        Shader s = Shader.Find("jjh/FOVOverlay");
        Debug.Assert(s != null, "[FOVPostProcess] ''jjh/FOVOverlay'' 셰이더를 찾을 수 없습니다.");

        _overlayMat = new Material(s);
        _overlayMat.SetColor("_Color",   fogColor);
        _overlayMat.SetFloat("_Opacity", fogOpacity);

        _cb = new CommandBuffer { name = "FOV Fog Overlay" };
        _cb.Blit((Texture)null, BuiltinRenderTextureType.CameraTarget, _overlayMat);
        _camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _cb);
    }

    // 외부 GameObject를 받아 스텐실 렌더러로 세팅하고 메쉬를 할당.
    public void SetFOVMesh(GameObject obj)
    {
        Debug.Assert(obj, "obj is null");

        MeshFilter mf = obj.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        Debug.Assert(mf != null, "AddFOVMesh 대상 GameObject에 MeshFilter가 없습니다.");
        Debug.Assert(meshRenderer != null, "AddFOVMesh 대상 GameObject에 MeshRenderer가 없습니다.");
        Debug.Assert(mf.sharedMesh);

        obj.SetActive(true);
        meshRenderer.sharedMaterial = _stencilMat;
    }

    private void OnDisable()
    {
        if (_camera != null && _cb != null)
            _camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, _cb);

        _cb?.Release();
        if (_overlayMat != null) Destroy(_overlayMat);
    }

    private void OnDestroy()
    {
        if (_stencilMat != null) Destroy(_stencilMat);
    }
}
