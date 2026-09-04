using Fusion;
using UnityEngine;

public class PlayerAnimationState : NetworkBehaviour
{
    [Networked] public Vector3 MoveDirection { get; private set; }
    [Networked] public Vector3 AimDirection { get; private set; }
    [Networked] public int CurrentWeapon { get; private set; }
    [Networked] public NetworkBool IsAttacking { get; private set; }

    [Networked] private TickTimer AttackTimer { get; set; }

    [Header("Attack Duration")]
    [SerializeField] private float pipeAttackDuration = 1.0f;
    [SerializeField] private float pistolAttackDuration = 0.7f;
    [SerializeField] private float shotGunAttackDuration = 1.2f;

    public void SetMoveDirection(Vector3 direction)
    {
        if (Object.HasStateAuthority == false)
            return;

        MoveDirection = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : Vector3.zero;
    }

    public void SetAimDirection(Vector3 direction)
    {
        if (Object.HasStateAuthority == false)
            return;

        AimDirection = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : transform.forward;
    }

    public void SetWeapon(int weaponIndex)
    {
        if (Object.HasStateAuthority == false)
            return;

        CurrentWeapon = weaponIndex;
    }

    public void NextWeapon()
    {
        if (Object.HasStateAuthority == false)
            return;

        CurrentWeapon = (CurrentWeapon + 1) % 3;
    }

    public void StartAttack(NetworkRunner runner)
    {
        if (Object.HasStateAuthority == false)
            return;

        if (IsAttacking)
            return;

        IsAttacking = true;
        AttackTimer = TickTimer.CreateFromSeconds(runner, GetAttackDuration());
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority == false)
            return;

        if (IsAttacking && AttackTimer.Expired(runner: Runner))
            IsAttacking = false;
    }

    private float GetAttackDuration()
    {
        switch (CurrentWeapon)
        {
            case 0:
                return pipeAttackDuration;

            case 1:
                return pistolAttackDuration;

            case 2:
                return shotGunAttackDuration;

            default:
                return pipeAttackDuration;
        }
    }
}