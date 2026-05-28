using UnityEngine;

public class PlayerAnimationSelector : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationState animationState;

    public enum WeaponType
    {
        Pipe = 0,
        Pistol = 1,
        ShotGun = 2
    }

    [SerializeField] private float crossFadeTime = 0.2f;
    [SerializeField] private int upperBodyLayerIndex = 1;

    [Header("Weapon Aim")]
    [SerializeField] private float aimHoldTime = 2.0f;
    [SerializeField] private float aimExitFadeTime = 1.5f;

    private bool wasAttacking = false;

    private bool isWeaponAimActive = false;
    private float weaponAimTimer = 0f;

    private bool isAimFadingOut = false;
    private float aimFadeTimer = 0f;

    private string currentAnimName = "";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animationState == null)
            animationState = GetComponent<PlayerAnimationState>();

        animator.SetLayerWeight(upperBodyLayerIndex, 0f);
    }

    private void Update()
    {
        if (animator == null || animationState == null)
            return;

        Vector3 moveDir = animationState.MoveDirection;
        Vector3 aimDir = animationState.AimDirection;
        WeaponType weapon = (WeaponType)animationState.CurrentWeapon;

        string moveAnim = SelectMoveAnimation(moveDir, aimDir);
        string nextMoveAnimName = "Pipe_" + moveAnim;

        if (currentAnimName != nextMoveAnimName)
        {
            animator.CrossFade(nextMoveAnimName, crossFadeTime, 0);
            currentAnimName = nextMoveAnimName;
        }

        if (animationState.IsAttacking && !wasAttacking)
        {
            isWeaponAimActive = true;
            isAimFadingOut = false;
            weaponAimTimer = 0f;
            aimFadeTimer = 0f;

            switch (weapon)
            {
                case WeaponType.Pipe:
                    animator.SetTrigger("PipeAttack");
                    break;

                case WeaponType.Pistol:
                    animator.SetTrigger("PistolAttack");
                    break;

                case WeaponType.ShotGun:
                    animator.SetTrigger("ShotGunAttack");
                    break;
            }
        }

        if (isWeaponAimActive && !animationState.IsAttacking)
        {
            weaponAimTimer += Time.deltaTime;

            if (weaponAimTimer >= aimHoldTime)
            {
                isWeaponAimActive = false;
                isAimFadingOut = true;
                aimFadeTimer = 0f;
            }
        }

        float upperBodyWeight = 0f;

        if (animationState.IsAttacking || isWeaponAimActive)
        {
            upperBodyWeight = 1f;
        }
        else if (isAimFadingOut)
        {
            aimFadeTimer += Time.deltaTime;

            float t = Mathf.Clamp01(aimFadeTimer / aimExitFadeTime);
            upperBodyWeight = Mathf.Lerp(1f, 0f, t);

            if (t >= 1f)
                isAimFadingOut = false;
        }

        animator.SetLayerWeight(upperBodyLayerIndex, upperBodyWeight);

        wasAttacking = animationState.IsAttacking;
    }

    private string SelectMoveAnimation(Vector3 moveDir, Vector3 aimDir)
    {
        if (moveDir.sqrMagnitude < 0.001f)
            return "Idle";

        if (aimDir.sqrMagnitude < 0.001f)
            aimDir = transform.forward;

        Vector3 rightDir = Vector3.Cross(Vector3.up, aimDir).normalized;

        float forwardValue = Vector3.Dot(moveDir.normalized, aimDir.normalized);
        float rightValue = Vector3.Dot(moveDir.normalized, rightDir);

        if (Mathf.Abs(forwardValue) >= Mathf.Abs(rightValue))
            return forwardValue >= 0f ? "Forward" : "Back";

        return rightValue >= 0f ? "Right" : "Left";
    }
}