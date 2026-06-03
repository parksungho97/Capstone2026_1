using System.Collections;
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

        PlayerAttack = GetComponent<CharacterAttack>();
        Debug.Assert(PlayerAttack);

        extraWeaponSlot = GetComponent<ExtraWeaponSlot>();
        Debug.Assert(extraWeaponSlot);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasInputAuthority)
        {
            bool hasMovedThisTick = false;

            if (!bInputDisabled && GetInput<NetworkInputData>(out NetworkInputData data))
            {
                Vector3 moveDegree = Vector3.zero;

                if (data.buttons.IsSet(EInputButton.W)) moveDegree += new Vector3(0.0f, 0.0f, 1f);
                if (data.buttons.IsSet(EInputButton.S)) moveDegree += new Vector3(0.0f, 0.0f, -1f);
                if (data.buttons.IsSet(EInputButton.A)) moveDegree += new Vector3(-1f, 0.0f, 0.0f);
                if (data.buttons.IsSet(EInputButton.D)) moveDegree += new Vector3(1f, 0.0f, 0.0f);

                if (moveDegree != Vector3.zero)
                {
                    move.Move(moveDegree);
                    hasMovedThisTick = true;
                }

                if (data.buttons.WasPressed(previousButtons, EInputButton.Space))
                {
                    if (captureInteractor.IsInsideCaptureZone)
                    {
                        captureInteractor.TryStartCapture();
                        move.SetMovePossible(false);
                        PlayerAttack.bPossibleAttack = false;
                        bActivate = true;
                    }
                }
                if (data.buttons.WasReleased(previousButtons, EInputButton.Space))
                {
                    captureInteractor.TryStopCapture();
                    move.SetMovePossible(true);
                    PlayerAttack.bPossibleAttack = true;
                    bActivate = false;
                }
                if (data.buttons.WasPressed(previousButtons, EInputButton.Q)) extraWeaponSlot.Swap();
                if (data.buttons.WasPressed(previousButtons, EInputButton.Attack))
                {
                    if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                        PlayerAttack.Attack();
                }
                if (data.buttons.WasPressed(previousButtons, EInputButton.Z)) itemCollector.AcquireOne();

                if (cameraController != null && cameraController.GetMouseWorldPosition(data.mousePosition, out Vector3 mouseWorldPosition))
                {
                    Vector3 lookDir = mouseWorldPosition - transform.position;
                    lookDir.y = 0f;
                    if (lookDir.sqrMagnitude > 0.0001f)
                        move.RotateTo(lookDir.normalized);
                }

                previousButtons = data.buttons;
            }

            bMove = hasMovedThisTick;
            if (captureInteractor.IsCapturing)
                bMove = false;
        }

        move.MoveUpdate(Runner.DeltaTime);
    }

    private Movement move;
    private CameraController cameraController;
    private CapturePointInteracter captureInteractor;
    public CharacterAttack PlayerAttack { get; private set; }
    private ExtraWeaponSlot extraWeaponSlot;

    [Networked] private NetworkButtons previousButtons { get; set; }
    [Networked] public bool bMove { get; set; }

    public bool bInputDisabled { get; set; } = false;

    public void DisableInputForSeconds(float duration)
    {
        if (!Object.HasInputAuthority) return;
        StartCoroutine(DisableInputCoroutine(duration));
    }

    private IEnumerator DisableInputCoroutine(float duration)
    {
        bInputDisabled = true;
        yield return new WaitForSeconds(duration);
        bInputDisabled = false;
    }

    private bool bActivate = false;
}