using UnityEngine;

public class PlayerAnimationSelector : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationNetworkState animationState;

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
            animationState = GetComponent<PlayerAnimationNetworkState>();

        animator.SetLayerWeight(upperBodyLayerIndex, 0f);
    }

    private void Update()
    {
        if (animator == null || animationState == null)
            return;

        Vector3 moveDir = animationState.MoveDirection;
        Vector3 aimDir = animationState.AimDirection;
        WeaponType weapon = GetWeaponType(animationState.CurrentWeaponId);

        string moveAnim = SelectMoveAnimation(moveDir, aimDir);
        string nextMoveAnimName = "Pipe_" + moveAnim;

        if (currentAnimName != nextMoveAnimName)
        {
            animator.CrossFade(nextMoveAnimName, crossFadeTime, 0);
            currentAnimName = nextMoveAnimName;
        }

        // 공격 시작: 공격 애니메이션만 재생
        if (animationState.IsAttacking && !wasAttacking)
        {
            isWeaponAimActive = false;
            isAimFadingOut = false;
            weaponAimTimer = 0f;
            aimFadeTimer = 0f;

            animator.ResetTrigger("PipeAttack");
            animator.ResetTrigger("PistolAttack");
            animator.ResetTrigger("ShotGunAttack");

            switch (weapon)
            {
                case WeaponType.Pipe:
                    animator.CrossFadeInFixedTime(
                        "UpperBodyAttack.Pipe_Attack",
                        0.05f,
                        upperBodyLayerIndex,
                        0f
                    );
                    break;

                case WeaponType.Pistol:
                    animator.CrossFadeInFixedTime(
                        "UpperBodyAttack.Pistol_Attack",
                        0.05f,
                        upperBodyLayerIndex,
                        0f
                    );
                    break;

                case WeaponType.ShotGun:
                    animator.CrossFadeInFixedTime(
                        "UpperBodyAttack.ShotGun_Attack",
                        0.05f,
                        upperBodyLayerIndex,
                        0f
                    );
                    break;
            }
        }

        // 공격 종료 순간: Pistol / ShotGun만 Weapon_Aim 시작
        if (wasAttacking && !animationState.IsAttacking)
        {
            if (weapon == WeaponType.Pistol || weapon == WeaponType.ShotGun)
            {
                isWeaponAimActive = true;
                isAimFadingOut = false;
                weaponAimTimer = 0f;
                aimFadeTimer = 0f;

                animator.CrossFadeInFixedTime(
                    "UpperBodyAttack.Weapon_Aim",
                    0.08f,
                    upperBodyLayerIndex,
                    0f
                );
            }
        }

        // Weapon_Aim 유지 시간 계산
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

    private WeaponType GetWeaponType(int weaponId)
    {
        switch (weaponId)
        {
            case 0:
                return WeaponType.Pipe;

            case 1:
                return WeaponType.Pistol;

            case 2:
                return WeaponType.ShotGun;

            default:
                return WeaponType.Pipe;
        }
    }
}