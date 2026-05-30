using Fusion;
using UnityEngine;

public class Npc : NetworkBehaviour
{
    [SerializeField] private ItemDropper itemDropper;

    private VoicePlayer voicePlayer;
    private NpcMove npcMove;

    [Networked] public bool bMoving { get; private set; }

    // ⚡ [수정] 네트워크 오브젝트는 Start 대신 Spawned에서 컴포넌트를 들고 와야 
    // 생성 타이밍 꼬임으로 인한 널 에러(Null Error)를 완벽하게 예방합니다.
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

        //if (voicePlayer != null && bMoving)
        //    voicePlayer.PlayFootStep();
    }
    private void Update()
    {
        //if (voicePlayer != null && bMoving)
        //    voicePlayer.PlayFootStep();
    }
}