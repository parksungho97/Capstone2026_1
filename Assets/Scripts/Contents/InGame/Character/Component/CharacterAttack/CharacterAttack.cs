using System;
using UnityEngine;

public class AttackDelay
{
    private float timer;
    public bool IsReady { get; set; } = true;

    public void SetDelay(float delay)
    {
        timer = delay;
        IsReady = delay <= 0f;
    }
    public float Timer { get { return timer; } }
    public bool IsAttackReady() => IsReady;

    public void Tick()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float dt)
    {
        if (timer > 0f)
        {
            timer -= dt;
            if (timer <= 0f)
            {
                timer = 0f;
                IsReady = true;
            }
        }
    }
}

public class AttackContext
{
    public AttackDelay AttackDelay { get; set; }
}

public class CharacterAttack : MonoBehaviour
{
    public Action ActionAttackStart;
    public Action ActionAttackEnd;

    private void Start()
    {
        equipmentComponent = GetComponent<EquipmentSlot>();
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
        if (!bPossibleAttack)
            return;

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
                if (RequiresConsumption())
                {
                    if (magazine == null || !magazine.TryConsume()) return;
                    attacker.RangedAttack(magazine.CurrentProjectileId, AttackContext.AttackDelay);
                }
                else
                {
                    attacker.RangedAttack(equipmentComponent.Weapon.AttackIds[0], AttackContext.AttackDelay);
                }
                break;
            case EAttackType.Shotgun:
                if (RequiresConsumption())
                {
                    if (magazine == null || !magazine.TryConsume()) return;
                    attacker.ShotgunAttack(magazine.CurrentProjectileId, AttackContext.AttackDelay);
                }
                else
                {
                    attacker.ShotgunAttack(equipmentComponent.Weapon.AttackIds[0], AttackContext.AttackDelay);
                }
                break;
        }

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

    private bool RequiresConsumption()
    {
        if (AttackConsumptionMapping.Instance == null) return false;
        foreach (int id in equipmentComponent.Weapon.AttackIds)
            if (AttackConsumptionMapping.Instance.TryGetConsumption(id, out _)) return true;
        return false;
    }

    private EquipmentSlot equipmentComponent;
    private Attacker attacker;
    private Magazine magazine;
    public AttackContext AttackContext { get; private set; }
    private bool bPrevAttackReady = true;

    public bool bPossibleAttack { get; set; } = true;
}
