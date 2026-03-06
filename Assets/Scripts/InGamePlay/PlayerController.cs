using Unity.Netcode;
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

    private NetworkVariable<bool> isMoveNet =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        isMoveNet.OnValueChanged += OnMoveChanged;
        animator.SetBool(moveBoolParam, isMoveNet.Value);

        if (IsOwner)
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

    public override void OnNetworkDespawn()
    {
        isMoveNet.OnValueChanged -= OnMoveChanged;
    }

    private void OnMoveChanged(bool previousValue, bool newValue)
    {
        animator.SetBool(moveBoolParam, newValue);
    }

    private void Update()
    {
        if (!IsOwner) return;

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

        animator.SetBool(moveBoolParam, hasInput);
        SetMoveStateServerRpc(hasInput);

        if (hasInput && mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.z + camRight * input.x).normalized;

            MoveServerRpc(moveDir);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger(jTriggerParam);
            PlayActionServerRpc(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger(kTriggerParam);
            PlayActionServerRpc(1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger(lTriggerParam);
            PlayActionServerRpc(2);
        }
    }

    [ServerRpc]
    private void SetMoveStateServerRpc(bool isMoving)
    {
        isMoveNet.Value = isMoving;
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 dir)
    {
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (faceMoveDirection && dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }
    }

    [ServerRpc]
    private void PlayActionServerRpc(int actionIndex)
    {
        PlayActionClientRpc(actionIndex);
    }

    [ClientRpc]
    private void PlayActionClientRpc(int actionIndex)
    {
        if (IsOwner) return;

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