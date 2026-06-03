using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    private VoicePlayer voicePlayer;
    public Action ActionAttack;

    private void Start()
    {
        voicePlayer = GetComponent<VoicePlayer>();
    }

    public void MeleeAttack(int id, AttackDelay delay)
    {
        MeleeAttackData data = AttackManager.Instance.Get<MeleeAttackData>(id);
        if (data == null) return;

        delay.SetDelay(data.ActivationTime + data.Duration);
        Vector3 spawnPos = transform.position + transform.rotation * data.SpawnOffset;
        RPC_OnAttackCast(data.AttackVfxId, data.CastSoundId, spawnPos, transform.rotation);
        _ = SpawnMeleeAsync(data);
    }

    public void RangedAttack(int id, AttackDelay delay)
    {
        RangedAttackData data = AttackManager.Instance.Get<RangedAttackData>(id);
        if (data == null) return;

        delay.SetDelay(data.Cooldown);
        Vector3 spawnPos = transform.position + transform.rotation * data.SpawnOffset;
        RPC_OnAttackCast(data.AttackVfxId, data.CastSoundId, spawnPos, transform.rotation);
        _ = SpawnRangedAsync(data);
    }

    public void ShotgunAttack(int id, AttackDelay delay)
    {
        ShotgunAttackData data = AttackManager.Instance.Get<ShotgunAttackData>(id);
        if (data == null) return;

        delay.SetDelay(data.ActivationTime + data.Duration);
        Vector3 spawnPos = transform.position + transform.rotation * data.SpawnOffset;
        RPC_OnAttackCast(data.AttackVfxId, data.CastSoundId, spawnPos, transform.rotation);
        _ = SpawnShotgunAsync(data);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnAttackCast(int vfxId, int soundId, Vector3 position, Quaternion rotation)
    {
        if (vfxId >= 0)
            VFXManager.Instance.Spawn(vfxId, position, rotation);
        if (soundId >= 0)
            voicePlayer?.PlayAttackClip(soundId);
        ActionAttack?.Invoke();
    }

    private async Task SpawnShotgunAsync(ShotgunAttackData data)
    {
        if (data.Prefab == null) { Debug.LogError("ShotgunAttackData: Prefab이 등록되지 않았습니다."); return; }
        await Runner.SpawnAsync(
            data.Prefab,
            transform.position + transform.rotation * data.SpawnOffset,
            transform.rotation,
            onBeforeSpawned: (runner, obj) =>
            {
                ShotgunAttackInstance instance = obj.gameObject.AddComponent<ShotgunAttackInstance>();
                instance.Init(obj, Object, data);
            }
        );
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
