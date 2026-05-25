using System;
using UnityEngine;

public class AttackDelay
{
    private float timer;
    public bool IsReady { get; set; } = true;

    public void SetDelay(float delay)
    {
        timer = delay;
        IsReady = false;
    }

    public bool IsAttackReady() => IsReady == true;

    public void Tick()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
                IsReady = true;
        }
    }
}

public class AttackContext
{
    public AttackDelay AttackDelay { get; set; }
}

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    public Action ActionAttackStart;
    public Action ActionAttackEnd;

    private void Start()
    {
        equipmentComponent = GetComponent<EquipmentComponent>();
        attacker = GetComponent<Attacker>();
        magazine = GetComponent<Magazine>();

        AttackContext = new AttackContext();
        AttackContext.AttackDelay = new AttackDelay();
    }

    public void SetEquip(int weaponId)
    {
        equipmentComponent.Equip(weaponId);
    }

    public void Attack()
    {
        if (equipmentComponent.Weapon == null)
            return;
        if (!AttackContext.AttackDelay.IsAttackReady())
            return;

        switch (equipmentComponent.Weapon.AttackType)
        {
            case EAttackType.Melee:
                attacker.MeleeAttack(equipmentComponent.Weapon.AttackIds[0], AttackContext.AttackDelay);
                break;
            case EAttackType.Ranged:
                if (magazine == null || !magazine.TryConsume())
                    return;

                attacker.RangedAttack(magazine.CurrentProjectileId, AttackContext.AttackDelay);
                break;
        }

        animator?.SetTrigger(attackTrigger);
        ActionAttackStart?.Invoke();
        bPrevAttackReady = false;
    }

    private void Update()
    {
        AttackContext.AttackDelay.Tick();

        if (AttackContext.AttackDelay.IsAttackReady() && bPrevAttackReady == false)
        {
            ActionAttackEnd?.Invoke();
            bPrevAttackReady = true;
        }
    }

    private EquipmentComponent equipmentComponent;
    private Attacker attacker;
    private Magazine magazine;
    public AttackContext AttackContext { get; private set; }
    private bool bPrevAttackReady = true;
}
