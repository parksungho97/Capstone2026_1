using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public void Initalize(GameObject player, CameraController cameraController)
    {
        this.player = player;
        this.cameraController = cameraController;

        move = player.GetComponent<PlayerMove>();
        Debug.Assert(move);

        stateManager = player.GetComponent<PlayerStateManager>();
        Debug.Assert(stateManager);

        captureInteractor = player.GetComponent<CaptureInteractor>();
        Debug.Assert(captureInteractor);
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

        bool bMove = false;
        if (GetInput<NetworkInputData>(out NetworkInputData data))
        {
            Vector3 moveDegree = Vector3.zero;
            if (data.buttons.IsSet(EInputButton.W))
                moveDegree += new Vector3(0.0f, 0.0f, 1f);
            if (data.buttons.IsSet(EInputButton.S))
                moveDegree += new Vector3(0.0f, 0.0f, -1f);
            if (data.buttons.IsSet(EInputButton.A))
                moveDegree += new Vector3(-1f, 0.0f, 0.0f);
            if (data.buttons.IsSet(EInputButton.D))
                moveDegree += new Vector3(1f, 0.0f, 0.0f);

            moveDegree = moveDegree * Runner.DeltaTime;
            bMove = move.Move(moveDegree);


            EPlayerTeam playerTeam = TeamInfo.Instance.GetTeam(Runner.LocalPlayer.PlayerId);
            ERequestType requestType = playerTeam == EPlayerTeam.Red ? ERequestType.Red : ERequestType.Blue;

            if (data.buttons.WasPressed(previousButtons, EInputButton.Space))
            {
                move.bMove = false;
                captureInteractor.TryStartActivateCapturePoint(requestType);
            }
            if (data.buttons.WasReleased(previousButtons, EInputButton.Space))
            {
                move.bMove = true;
                captureInteractor.TryStopActivateCapturePoint(requestType);
            }

            previousButtons = data.buttons;

            if (cameraController.GetMouseWorldPosition(data.mousePosition, out Vector3 mouseWorldPosition))
            {
                Vector3 lookDir = mouseWorldPosition - player.transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.0001f)
                    move.RotateTo(lookDir.normalized, Runner.DeltaTime);
            }
        }
        stateManager.Move = bMove;
    }

    private GameObject player;
    private PlayerMove move;
    private PlayerStateManager stateManager;
    private CameraController cameraController;
    private CaptureInteractor captureInteractor;

    [Networked] private NetworkButtons previousButtons { get; set; }
}
