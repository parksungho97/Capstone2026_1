using UnityEngine;
using UnityEngine.Rendering;

public class LightingFOV : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private GameObject player;
    [SerializeField] private CameraController cameraController;

    [Header("ExGrid")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Camera fovCamera;

    [SerializeField] private Renderer[] obstacleRenderers;

    private CommandBuffer commandBuffer;
    private ComputeBuffer computeBuffer;
    private Material mat;

    private RenderTexture fovDepthTexture;

    private Shader stencilShader;
    private Material stencilMat;
    private RenderTexture obstacleMaskTexture;

    private void Start()
    {
        //////////////////////////////////////////////////////////////////////////////////
        // 반드시 FOVCamera가 플레이어의 FOV를 정확히 따라가도록 설정 (위치, 회전, FOV 모두)//
        //////////////////////////////////////////////////////////////////////////////////
        
        cameraController.SetTarget(player.transform);

        // 카메라가 깊이 텍스처를 생성하도록 설정 (월드 좌표 복원 필수)
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;

        // 1. 커맨드 버퍼 및 컴퓨트 버퍼 초기화
        commandBuffer = new CommandBuffer { name = "ViewRenderPass" };

        mat = new Material(Shader.Find("Hidden/ViewRender"));

        fovDepthTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.Depth);
        fovDepthTexture.name = "FovDepthTexture";

        // fovCamera 설정
        // 반드시 fov랑 fovCamera설정 같게해줘야됌
        fovCamera.targetTexture = fovDepthTexture;
        // 렌더링 최적화: Depth만 그리므로 불필요한 연산 끄기
        fovCamera.enabled = false;
        fovCamera.cullingMask = LayerMask.GetMask("Obstacle");



        stencilShader = Shader.Find("Hidden/ObstacleMask");
        stencilMat = new Material(stencilShader);

        obstacleMaskTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.R8)
        {
            name = "ObstacleMaskTexture",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point,
        };
        obstacleMaskTexture.Create();

        // 2. 카메라 이벤트에 커맨드 버퍼 등록 (한 번만 등록)
        mainCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
    }

    private void Update()
    {
        fovCamera.Render();
        
        // 1. 셰이더 파라미터 업데이트
        mat.SetVector("_PlayerPos", player.transform.position);
        mat.SetVector("_PlayerForward", player.transform.forward);
        mat.SetFloat("_ViewAngle", fovCamera.fieldOfView); // 부채꼴 전체 각도 (예: 60도)
        mat.SetFloat("_MaxDist", fovCamera.farClipPlane);   // 시야 거리
        mat.SetColor("_FogColor", Color.black);
        mat.SetFloat("_FogOpacity", 0.8f);
        mat.SetTexture("_FovDepthTex", fovDepthTexture);
        Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(mainCamera.projectionMatrix, true);

        Matrix4x4 invVP = (gpuProj * mainCamera.worldToCameraMatrix).inverse;
        mat.SetMatrix("_InvVP", invVP);
        Matrix4x4 fovViewProj = GL.GetGPUProjectionMatrix(fovCamera.projectionMatrix, true) * fovCamera.worldToCameraMatrix;
        mat.SetMatrix("_FovViewProj", fovViewProj);

        // Todo: 특정텍스쳐에 화면에 있는 모든놈들 깊이 고려해서 전부 다시 그리는데 Layer가 Obstacle인 것들은 스텐실을 1로 그리기

        // 렌더타겟을 임시 텍스쳐에 복사
        // 현재 렌더타겟을 clear
        // 장애물들만 다시 그림. 이때 장애물들만 r값을 1로.
        // 렌더타겟을 임시 텍스쳐로 설정
        // 장애물을 그린 텍스쳐를 셰이더에서 받아서, 픽셀마다 장애물이 있는지 체크해서, 있으면 안보이는걸로 처리

        commandBuffer.Clear();

        int tempRT = Shader.PropertyToID("_TempViewRender");
        int depthRT = Shader.PropertyToID("_TempDepth");

        commandBuffer.GetTemporaryRT(tempRT, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
        commandBuffer.GetTemporaryRT(depthRT, -1, -1, 24, FilterMode.Point, RenderTextureFormat.Depth);

        // [1] 현재 화면 임시 텍스처에 복사
        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, tempRT);

        // [2] depth도 복사 (ZTest용)
        commandBuffer.CopyTexture(BuiltinRenderTextureType.Depth, depthRT);

        // [3] obstacleMaskTexture 클리어 후 장애물만 R=1로 그리기
        //     depthRT 붙여서 ZTest LEqual → 벽 뒤 장애물은 기각
        commandBuffer.SetRenderTarget(
            new RenderTargetIdentifier(obstacleMaskTexture),
            new RenderTargetIdentifier(depthRT));
        commandBuffer.ClearRenderTarget(false, true, Color.clear);

        foreach (var r in obstacleRenderers)
            if (r != null) commandBuffer.DrawRenderer(r, stencilMat);

        // [4] 마스크를 셰이더에 넘기고 후처리 적용
        commandBuffer.SetGlobalTexture("_ObstacleMask", obstacleMaskTexture);
        commandBuffer.Blit(tempRT, BuiltinRenderTextureType.CameraTarget, mat);

        commandBuffer.ReleaseTemporaryRT(tempRT);
        commandBuffer.ReleaseTemporaryRT(depthRT);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지: 버퍼 해제 및 커맨드 버퍼 제거
        if (computeBuffer != null) computeBuffer.Release();

        if (mainCamera != null && commandBuffer != null)
        {
            mainCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
        }
        obstacleMaskTexture?.Release();   // ← 추가
        fovDepthTexture?.Release();       // ← 이것도 누락돼 있었음
    }
}
