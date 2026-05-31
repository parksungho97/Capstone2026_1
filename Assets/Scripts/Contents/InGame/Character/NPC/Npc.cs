using Fusion;
using UnityEngine;

public class Npc : NetworkBehaviour
{
    [SerializeField] private ItemDropper itemDropper;

    private VoicePlayer voicePlayer;
    private NpcMove npcMove;

    [Networked] public bool bMoving { get; private set; }

    public override void Spawned()
    {
        base.Spawned();

        Debug.Assert(itemDropper);

        voicePlayer = GetComponent<VoicePlayer>();
        npcMove = GetComponent<NpcMove>();

        Debug.Assert(voicePlayer);
        Debug.Assert(npcMove);
    }

    public void DestroyNpc()
    {
        if (!Object.HasStateAuthority) return;

        itemDropper.Drop();
        Runner.Despawn(Object);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority)
            bMoving = npcMove.IsMoving;
    }
    public override void Render()
    {
        base.Render();

        if (voicePlayer == null)
            return;

        if (bMoving)
            voicePlayer.StartFootSteps();
        else
            voicePlayer.StopFootSteps();
    }
}