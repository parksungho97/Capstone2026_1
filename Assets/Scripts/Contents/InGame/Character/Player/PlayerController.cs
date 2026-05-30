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

        playerAttack = GetComponent<CharacterAttack>();
        Debug.Assert(playerAttack);

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
                    move.Move(moveDegree); // 대각선 이동 속도 균일을 위해 normalized 권장
                    hasMovedThisTick = true;
                }

                if (data.buttons.WasPressed(previousButtons, EInputButton.Space)) captureInteractor.TryStartCapture();
                if (data.buttons.WasReleased(previousButtons, EInputButton.Space)) captureInteractor.TryStopCapture();
                if (data.buttons.WasPressed(previousButtons, EInputButton.Q)) extraWeaponSlot.Swap();
                if (data.buttons.WasPressed(previousButtons, EInputButton.Attack))
                {
                    if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                        playerAttack.Attack();
                }
                if (data.buttons.WasPressed(previousButtons, EInputButton.Z)) itemCollector.AcquireOne();

                previousButtons = data.buttons;
            }

            bMove = hasMovedThisTick;
        }

        move.MoveUpdate();
    }

    private void Update()
    {
        if (!Object.HasInputAuthority) 
            return;

        if (cameraController.GetMouseWorldPosition(Input.mousePosition, out Vector3 mouseWorldPosition))
        {
            Vector3 lookDir = mouseWorldPosition - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Vector3 aimDir = lookDir.normalized;
                move.RotateTo(aimDir);
            }
        }
    }

    private Movement move;
    private CameraController cameraController;
    private CapturePointInteracter captureInteractor;
    private CharacterAttack playerAttack;
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
}