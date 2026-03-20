using Fusion;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private bool faceMoveDirection = true;
    [SerializeField] private float turnSpeed = 15f;

    [Header("Animator Params")]
    [SerializeField] private string moveBoolParam = "IsMove";
    [SerializeField] private string jTriggerParam = "Anim_J";
    [SerializeField] private string kTriggerParam = "Anim_K";
    [SerializeField] private string lTriggerParam = "Anim_L";

    private Animator animator;
    private Camera mainCamera;
    private CameraFollow cameraFollow;

    // NGO의 NetworkVariable -> Fusion의 [Networked] 속성
    // OnChangedRender를 통해 값이 바뀔 때마다 OnMoveChanged가 호출됩니다.
    [Networked, OnChangedRender(nameof(OnMoveChanged))]
    public bool IsMoveNet { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // OnNetworkSpawn -> Spawned
    public override void Spawned()
    {
        animator.SetBool(moveBoolParam, IsMoveNet);

        // NGO의 IsOwner -> Fusion에서는 플레이어 입력 권한인 HasInputAuthority를 주로 사용합니다.
        if (HasInputAuthority)
        {
            mainCamera = Camera.main;

            if (mainCamera != null)
            {
                cameraFollow = mainCamera.GetComponentInParent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTarget(transform);
                }
            }
        }
    }

    // 값이 변경될 때 실행될 콜백 함수 (파라미터 없이 작성)
    private void OnMoveChanged()
    {
        animator.SetBool(moveBoolParam, IsMoveNet);
    }

    private void Update()
    {
        // 내 캐릭터인지 확인
        if (!HasInputAuthority) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.W)) z += 1f;

        Vector3 input = new Vector3(x, 0f, z);
        bool hasInput = input.sqrMagnitude > 0.0001f;

        // 로컬 애니메이션 즉시 적용
        animator.SetBool(moveBoolParam, hasInput);
        Rpc_SetMoveState(hasInput);

        if (hasInput && mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.z + camRight * input.x).normalized;

            Rpc_Move(moveDir);
        }

        HandleMouseLook();

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger(jTriggerParam);
            Rpc_PlayAction(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger(kTriggerParam);
            Rpc_PlayAction(1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger(lTriggerParam);
            Rpc_PlayAction(2);
        }
    }

    private void HandleMouseLook()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Rpc_Rotate(lookDir.normalized);
            }
        }
    }

    // InputAuthority(클라이언트)가 StateAuthority(방장/서버)에게 실행을 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_SetMoveState(bool isMoving)
    {
        IsMoveNet = isMoving;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_Move(Vector3 dir)
    {
        // 네트워크 RPC 내부에서는 Time.deltaTime 대신 Runner.DeltaTime을 사용하는 것이 안전합니다.
        transform.position += dir * moveSpeed * Runner.DeltaTime;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_Rotate(Vector3 lookDir)
    {
        if (lookDir.sqrMagnitude <= 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Runner.DeltaTime
        );
    }

    // 기존의 ServerRpc -> ClientRpc 구조를 합쳐서 
    // 클라이언트가 "모든 사람(All)"에게 직접 쏘도록 최적화했습니다.
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void Rpc_PlayAction(int actionIndex)
    {
        // RPC를 쏜 본인(내 캐릭터)은 Update문에서 이미 트리거를 작동시켰으므로 무시합니다.
        if (HasInputAuthority) return;

        switch (actionIndex)
        {
            case 0:
                animator.SetTrigger(jTriggerParam);
                break;
            case 1:
                animator.SetTrigger(kTriggerParam);
                break;
            case 2:
                animator.SetTrigger(lTriggerParam);
                break;
        }
    }
}