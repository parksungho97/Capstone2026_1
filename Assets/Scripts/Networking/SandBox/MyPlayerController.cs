using Fusion;
using UnityEngine;

public class MyPlayerController : NetworkBehaviour
{
    public void Initalize(GameObject player, CameraController cameraController)
    {
        this.player = player;
        this.cameraController = cameraController;

        move = player.GetComponent<PlayerMove>();
        Debug.Assert(move);

        stateManager = player.GetComponent<PlayerStateManager>();
        Debug.Assert(stateManager);
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
                moveDegree = new Vector3(0.0f, 0.0f, 1f);
            if (data.buttons.IsSet(EInputButton.S))
                moveDegree = new Vector3(0.0f, 0.0f, -1f);
            if (data.buttons.IsSet(EInputButton.A))
                moveDegree = new Vector3(-1f, 0.0f, 0.0f);
            if (data.buttons.IsSet(EInputButton.D))
                moveDegree = new Vector3(1f, 0.0f, 0.0f);

            moveDegree = moveDegree * Runner.DeltaTime;
            bMove = move.Move(moveDegree);
        }

        stateManager.Move = bMove;

        if (cameraController.GetMouseWorldPosition(data.mousePosition, out Vector3 mouseWorldPosition))
        {
            Vector3 lookDir = mouseWorldPosition - player.transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
                move.RotateTo(lookDir.normalized, Runner.DeltaTime);
        }
    }

    private GameObject player;
    private PlayerMove move;
    private PlayerStateManager stateManager;
    private CameraController cameraController;
}
