using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    public void MeleeAttack(int id, AttackDelay delay)
    {
        MeleeAttackData data = AttackManager.Instance.Get<MeleeAttackData>(id);
        if (data == null) return;

        delay.SetDelay(data.ActivationTime + data.Duration);
        _ = SpawnMeleeAsync(data);
    }

    public void RangedAttack(int id, AttackDelay delay)
    {
        RangedAttackData data = AttackManager.Instance.Get<RangedAttackData>(id);
        if (data == null) return;

        delay.SetDelay(data.Cooldown);
        _ = SpawnRangedAsync(data);
    }

    private async Task SpawnMeleeAsync(MeleeAttackData data)
    {
        if (data.Prefab == null) { Debug.LogError("MeleeAttackData: Prefab이 등록되지 않았습니다."); return; }
        await Runner.SpawnAsync(
            data.Prefab,
            transform.position + transform.rotation * data.SpawnOffset,
            transform.rotation,
            onBeforeSpawned: (runner, obj) =>
            {
                MeleeAttackInstance instance = obj.gameObject.AddComponent<MeleeAttackInstance>();
                instance.Init(obj, Object, data);
            }
        );
    }

    private async Task SpawnRangedAsync(RangedAttackData data)
    {
        if (data.Prefab == null) { Debug.LogError("RangedAttackData: Prefab이 등록되지 않았습니다."); return; }
        await Runner.SpawnAsync(
            data.Prefab,
            transform.position + transform.rotation * data.SpawnOffset,
            transform.rotation,
            onBeforeSpawned: (runner, obj) =>
            {
                ProjectileInstance instance = obj.gameObject.AddComponent<ProjectileInstance>();
                instance.Init(obj, Object, data);
            }
        );
    }

}
