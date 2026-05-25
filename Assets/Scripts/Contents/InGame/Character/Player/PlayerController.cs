using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private ItemCollector itemCollector;

    public void Initalize(CameraController cameraController)
    {
        this.cameraController = cameraController;

        move = GetComponent<Movement>();
        Debug.Assert(move);

        captureInteractor = GetComponent<CapturePointInteracter>();
        Debug.Assert(captureInteractor);

        EPlayerTeam playerTeam = TeamInfo.Instance.GetTeam(Runner.LocalPlayer.PlayerId);
        captureInteractor.SetTeam(playerTeam == EPlayerTeam.Red ? ECaptureState.Red : ECaptureState.Blue);

        Debug.Assert(itemCollector);

        playerAttack = GetComponent<CharacterAttack>();
        Debug.Assert(playerAttack);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        Vector3 moveDegree = Vector3.zero;

        if (GetInput<NetworkInputData>(out NetworkInputData data))
        {
            if (data.buttons.IsSet(EInputButton.W))
                move.Move(new Vector3(0.0f, 0.0f, 1f));
            if (data.buttons.IsSet(EInputButton.S))
                move.Move(new Vector3(0.0f, 0.0f, -1f));
            if (data.buttons.IsSet(EInputButton.A))
                move.Move(new Vector3(-1f, 0.0f, 0.0f));
            if (data.buttons.IsSet(EInputButton.D))
                move.Move(new Vector3(1f, 0.0f, 0.0f));

            if (data.buttons.WasPressed(previousButtons, EInputButton.Space))
                captureInteractor.TryStartCapture();

            if (data.buttons.WasReleased(previousButtons, EInputButton.Space))
                captureInteractor.TryStopCapture();

            //if (data.buttons.WasPressed(previousButtons, EInputButton.Q))
            //{
            //    animationState.NextWeapon();
            //}

            if (data.buttons.WasPressed(previousButtons, EInputButton.Attack))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    return;
                playerAttack.Attack();
            }

            if (data.buttons.WasPressed(previousButtons, EInputButton.Z))
            {
                itemCollector.AcquireOne();
            }

            if (cameraController.GetMouseWorldPosition(data.mousePosition, out Vector3 mouseWorldPosition))
            {
                Vector3 lookDir = mouseWorldPosition - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.0001f)
                {
                    Vector3 aimDir = lookDir.normalized;
                    move.RotateTo(aimDir);
                }
            }

            previousButtons = data.buttons;
        }

    }

    private Movement move;
    private CameraController cameraController;
    private CapturePointInteracter captureInteractor;
    private CharacterAttack playerAttack;

    [Networked] private NetworkButtons previousButtons { get; set; }
}