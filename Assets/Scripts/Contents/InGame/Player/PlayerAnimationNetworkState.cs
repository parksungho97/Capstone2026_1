using Fusion;
using UnityEngine;

public class PlayerAnimationNetworkState : NetworkBehaviour
{
    //[Networked] public Vector3 MoveDirection { get; private set; }
    //[Networked] public Vector3 AimDirection { get; private set; }

    [Networked] public int CurrentWeaponId { get; private set; }
    [Networked] public NetworkBool IsAttacking { get; private set; }
    [Networked] public NetworkBool IsDead { get; private set; }

    public bool IsHit { get; private set; }

    [SerializeField] private HitComponent hitComponent;
    [SerializeField] private Movement movement;
    [SerializeField] private EquipmentSlot equipmentSlot;
    [SerializeField] private CharacterAttack characterAttack;
    [SerializeField] private Respawn respawn;

    public void SetMoveDirection(Vector3 direction)
    {
        //if (!Object.HasStateAuthority)
        //    return;

        //MoveDirection = direction.sqrMagnitude > 0.0001f
        //    ? direction.normalized
        //    : Vector3.zero;
    }

    private void Awake()
    {
        if (equipmentSlot == null)
            equipmentSlot = GetComponent<EquipmentSlot>();

        if (characterAttack == null)
            characterAttack = GetComponent<CharacterAttack>();

        if (hitComponent == null)
            hitComponent = GetComponentInChildren<HitComponent>();

        if (respawn == null)
            respawn = GetComponent<Respawn>();
    }

    public override void Spawned()
    {
        if (characterAttack != null)
        {
            characterAttack.ActionAttackStart += OnAttackStart;
            characterAttack.ActionAttackEnd += OnAttackEnd;
        }

        if (hitComponent != null)
        {
            hitComponent.ActionHitted += OnHit;
        }
    }

    public void SetAimDirection(Vector3 direction)
    {
        //if (!Object.HasStateAuthority)
        //    return;

        //AimDirection = direction.sqrMagnitude > 0.0001f
        //    ? direction.normalized
        //    : transform.forward;
    }

    public void SetCurrentWeaponId(int weaponId)
    {
        if (!Object.HasStateAuthority)
            return;

        CurrentWeaponId = weaponId;
    }

    public void SetAttacking(bool isAttacking)
    {
        if (!Object.HasStateAuthority)
            return;

        IsAttacking = isAttacking;
    }

    public void SetDead(bool isDead)
    {
        if (!Object.HasStateAuthority)
            return;

        IsDead = isDead;
    }

    private void OnAttackStart()
    {
        SetAttacking(true);
    }

    private void OnAttackEnd()
    {
        SetAttacking(false);
    }

    private void OnHit()
    {
        SetHit(true);
    }

    public void SetHit(bool isHit)
    {
        IsHit = isHit;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (characterAttack != null)
        {
            characterAttack.ActionAttackStart -= OnAttackStart;
            characterAttack.ActionAttackEnd -= OnAttackEnd;
        }

        if (hitComponent != null)
        {
            hitComponent.ActionHitted -= OnHit;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (equipmentSlot != null)
            SetCurrentWeaponId(equipmentSlot.WeaponId);

        if (respawn != null)
            SetDead(respawn.bDead);
    }
}