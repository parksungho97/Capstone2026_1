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
    [SerializeField] private float attackFadeTime = 0.05f;

    private string currentAnimName = "";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animationState == null)
            animationState = GetComponent<PlayerAnimationState>();
    }

    private void Update()
    {
        if (animator == null || animationState == null)
            return;

        Vector3 moveDir = animationState.MoveDirection;
        Vector3 aimDir = animationState.AimDirection;
        WeaponType weapon = (WeaponType)animationState.CurrentWeapon;

        string nextAnimName;

        if (animationState.IsAttacking)
        {
            nextAnimName = GetAttackAnimationName(weapon, moveDir, aimDir);
        }
        else
        {
            string moveAnim = SelectMoveAnimation(moveDir, aimDir);
            nextAnimName = GetAnimationName(weapon, moveAnim);
        }

        if (currentAnimName == nextAnimName)
            return;

        float fadeTime = animationState.IsAttacking ? attackFadeTime : crossFadeTime;

        if (animationState.IsAttacking)
            animator.CrossFade(nextAnimName, fadeTime, 0, 0f);
        else
            animator.CrossFade(nextAnimName, fadeTime);

        currentAnimName = nextAnimName;
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

    private string GetAnimationName(WeaponType weapon, string moveAnim)
    {
        return weapon.ToString() + "_" + moveAnim;
    }

    private string GetAttackAnimationName(WeaponType weapon, Vector3 moveDir, Vector3 aimDir)
    {
        switch (weapon)
        {
            case WeaponType.Pipe:
                return "Pipe_Attack";

            case WeaponType.ShotGun:
                return "ShotGun_Attack";

            case WeaponType.Pistol:
                string moveAnim = SelectMoveAnimation(moveDir, aimDir);
                return GetAnimationName(weapon, moveAnim);

            default:
                return "Pipe_Idle";
        }
    }
}
