using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public void Initalize(GameObject player, CameraController cameraController)
    {
        this.player = player;
        this.cameraController = cameraController;

        move = player.GetComponent<PlayerMovement>();
        Debug.Assert(move);

        captureInteractor = player.GetComponent<CaptureInteractor>();
        Debug.Assert(captureInteractor);

        animationState = player.GetComponent<PlayerAnimationState>();
        Debug.Assert(animationState);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Object.AssignInputAuthority(Runner.LocalPlayer);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority == false)
            return;

        Vector3 moveDegree = Vector3.zero;
        bool bMove = false;

        if (GetInput<NetworkInputData>(out NetworkInputData data))
        {
            if (data.buttons.IsSet(EInputButton.W))
                moveDegree += new Vector3(0.0f, 0.0f, 1f);
            if (data.buttons.IsSet(EInputButton.S))
                moveDegree += new Vector3(0.0f, 0.0f, -1f);
            if (data.buttons.IsSet(EInputButton.A))
                moveDegree += new Vector3(-1f, 0.0f, 0.0f);
            if (data.buttons.IsSet(EInputButton.D))
                moveDegree += new Vector3(1f, 0.0f, 0.0f);

            bool blockMoveAndRotate =
                animationState.IsAttacking &&
                animationState.CurrentWeapon != 1;

            animationState.SetMoveDirection(
                !blockMoveAndRotate && moveDegree.sqrMagnitude > 0.0001f
                    ? moveDegree.normalized
                    : Vector3.zero
            );

            if (!blockMoveAndRotate)
            {
                moveDegree = moveDegree * Runner.DeltaTime;
                move.Move(moveDegree);
                bMove = true;
            }
            else
            {
                bMove = false;
            }

            EPlayerTeam playerTeam = TeamInfo.Instance.GetTeam(Runner.LocalPlayer.PlayerId);
            ERequestType requestType = playerTeam == EPlayerTeam.Red ? ERequestType.Red : ERequestType.Blue;

            if (data.buttons.WasPressed(previousButtons, EInputButton.Space))
            {
                captureInteractor.TryStartActivateCapturePoint(requestType);
            }

            if (data.buttons.WasReleased(previousButtons, EInputButton.Space))
            {
                captureInteractor.TryStopActivateCapturePoint(requestType);
            }

            if (data.buttons.WasPressed(previousButtons, EInputButton.Q))
            {
                animationState.NextWeapon();
            }

            if (data.buttons.WasPressed(previousButtons, EInputButton.Attack))
            {
                animationState.StartAttack(Runner);
            }

            if (cameraController.GetMouseWorldPosition(data.mousePosition, out Vector3 mouseWorldPosition))
            {
                Vector3 lookDir = mouseWorldPosition - player.transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.0001f)
                {
                    Vector3 aimDir = lookDir.normalized;
                    animationState.SetAimDirection(aimDir);

                    if (!blockMoveAndRotate)
                    {
                        move.RotateTo(aimDir, Runner.DeltaTime);
                    }
                }
            }

            previousButtons = data.buttons;
        }

        if (!bMove)
            move.MoveEnd();
    }

    private GameObject player;
    private PlayerMovement move;
    private CameraController cameraController;
    private CaptureInteractor captureInteractor;
    private PlayerAnimationState animationState;

    [Networked] private NetworkButtons previousButtons { get; set; }
}