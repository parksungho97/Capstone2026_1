using System;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static event Action<Player> OnNetworkSpawned;

    [SerializeField] private int defaultWeaponId = 0;

    public override void Spawned()
    {
        base.Spawned();

        equipmentComponent = GetComponent<EquipmentSlot>();
        Debug.Assert(equipmentComponent);

        Debug.Assert(EquipmentManager.Instance.TryGet(defaultWeaponId, out EquipmentData weaponData));
        Weapon pipe = weaponData.Generate() as Weapon;
        if (pipe != null)
            equipmentComponent.SetWeapon(pipe);
        else
            Debug.Log("DefaultWeapon Is Null");

        magazine = GetComponent<Magazine>();
        magazine.SetAttackInstance(1);

        playerController = GetComponent<PlayerController>();
        Debug.Assert(playerController);

        voicePlayer = GetComponent<VoicePlayer>();
        Debug.Assert(voicePlayer);

        animtor = GetComponent<Animator>();
        Debug.Assert(animtor);

        OnNetworkSpawned?.Invoke(this);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        bMove = playerController.bMove;
    }

    public override void Render()
    {
        base.Render();
        if (bMove)
        {
            voicePlayer.PlayFootStep();
           // animtor.SetTrigger("Walk");
        }
    }

    private EquipmentSlot equipmentComponent;
    private Magazine magazine;
    private PlayerController playerController;
    private VoicePlayer voicePlayer;
    private Animator animtor;

    [Networked] private bool bMove { get; set; }
}
