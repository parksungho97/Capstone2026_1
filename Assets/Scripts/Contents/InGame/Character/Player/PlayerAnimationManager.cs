using Fusion;
using jjh;
using System;
using UnityEngine;

public class PlayerAnimationManager : NetworkBehaviour
{
    [Serializable]
    public class WeaponTriggerEntry
    {
        public int weaponId;
        public string attackTrigger;
    }

    [Header("Components")]
    private Animator animator;
    private PlayerController playerController;
    private EquipmentSlot equipmentSlot;
    private CharacterAttack characterAttack;
    private Movement movement;

    [Header("Animator Parameters")]
    [SerializeField] private string paramIdle = "Idle";
    [SerializeField] private string paramMove = "Move";

    [Header("Weapon Trigger Map")]
    [SerializeField] private WeaponTriggerEntry[] weaponTriggerMap;

    public override void Spawned()
    {
        base.Spawned();

        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        equipmentSlot = GetComponent<EquipmentSlot>();
        characterAttack = GetComponent<CharacterAttack>();

        Debug.Assert(animator);
        Debug.Assert(playerController);
        Debug.Assert(equipmentSlot);
        Debug.Assert(characterAttack);

        characterAttack.ActionAttackStart += () =>
        {

        };
    }

    private void Update()
    {
        if (animator == null)
            return;

        string currentTrigger = playerController.bMove ? paramMove : paramIdle;

        animator.SetBool(prevTrigger, false);
        animator.SetBool(currentTrigger, true);

        prevTrigger = currentTrigger;
    }

    public bool TryGetAttackTrigger(out string trigger)
    {
        int id = equipmentSlot.WeaponId;
        foreach (WeaponTriggerEntry entry in weaponTriggerMap)
        {
            if (entry.weaponId == id)
            {
                trigger = entry.attackTrigger;
                return true;
            }
        }
        trigger = string.Empty;
        return false;
    }

    private string prevTrigger;
}
