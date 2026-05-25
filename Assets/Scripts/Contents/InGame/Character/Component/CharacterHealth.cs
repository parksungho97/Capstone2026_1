using Fusion;
using UnityEngine;

public class CharacterHealth : NetworkBehaviour
{
    [Header("Default Stat")]
    [SerializeField] private int defaultMaxHP = 100;
    [SerializeField] private int defaultMaxArmor = 50;

    [Networked] public int MaxHP { get; private set; }
    [Networked] public int CurrentHP { get; private set; }

    [Networked] public int MaxArmor { get; private set; }
    [Networked] public int CurrentArmor { get; private set; }

    [Networked] private float recoveryRemaining { get; set; }
    [Networked] private float recoveryRate { get; set; }
    [Networked] private float recoveryAccumulator { get; set; }

    public override void Spawned()
    {
        Debug.Log($"[PlayerHealth] Spawned / Authority: {Object.HasStateAuthority}");

        if (Object.HasStateAuthority)
        {
            MaxHP = defaultMaxHP;
            CurrentHP = defaultMaxHP;

            MaxArmor = defaultMaxArmor;
            CurrentArmor = defaultMaxArmor;

            Debug.Log($"[PlayerHealth] 초기화 완료 HP:{CurrentHP}/{MaxHP}, Armor:{CurrentArmor}/{MaxArmor}");
        }
    }
    /// <summary>
    /// 체력만 회복 (RPC 개조)
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddHP(int amount)
    {
        // 🛡️ 포톤 퓨전 규칙: RpcTargets.StateAuthority 덕분에 이 내부 로직은 무조건 서버에서만 실행됩니다.
        if (amount <= 0) return;

        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    /// <summary>
    /// 방어구만 회복 (RPC 개조)
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddArmor(int amount)
    {
        if (amount <= 0) return;

        CurrentArmor = Mathf.Min(CurrentArmor + amount, MaxArmor);
    }

    /// <summary>
    /// 데미지 처리 (RPC 개조)
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ServeHP(int damage)
    {
        Debug.Log($"[ServeHP RPC] 서버에서 실행됨 / damage: {damage}");

        if (damage <= 0) return;

        int remainingDamage = damage;

        // 1. 방어구 먼저 감소
        if (CurrentArmor > 0)
        {
            int armorDamage = Mathf.Min(CurrentArmor, remainingDamage);
            CurrentArmor -= armorDamage;
            remainingDamage -= armorDamage;
        }

        // 2. 남은 데미지를 HP에 적용
        if (remainingDamage > 0)
        {
            CurrentHP = Mathf.Max(CurrentHP - remainingDamage, 0);
        }

        Debug.Log($"[ServeHP RPC] 결과 → HP:{CurrentHP}, Armor:{CurrentArmor}");
    }

    /// <summary>
    /// 회복 처리 (RPC 개조)
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RecoverStat(int amount)
    {
        Debug.Log($"[RecoverStat RPC] 서버에서 실행됨 / amount: {amount}");

        if (amount <= 0) return;

        int remainingRecovery = amount;

        // 1. HP 먼저 회복
        if (CurrentHP < MaxHP)
        {
            int hpNeed = MaxHP - CurrentHP;
            int hpRecovery = Mathf.Min(hpNeed, remainingRecovery);
            CurrentHP += hpRecovery;
            remainingRecovery -= hpRecovery;
        }

        // 2. 남은 양이 있으면 Armor 회복
        if (remainingRecovery > 0 && CurrentArmor < MaxArmor)
        {
            int armorNeed = MaxArmor - CurrentArmor;
            int armorRecovery = Mathf.Min(armorNeed, remainingRecovery);
            CurrentArmor += armorRecovery;
        }

        Debug.Log($"[RecoverStat RPC] 결과 → HP:{CurrentHP}, Armor:{CurrentArmor}");
    }

    /// <summary>
    /// 체력/방어구 초기화 (RPC 개조)
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetStat()
    {
        CurrentHP = MaxHP;
        CurrentArmor = MaxArmor;
    }

    /// <summary>
    /// duration초에 걸쳐 amount만큼 HP/Armor를 회복
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RecoverOverTime(int amount, float duration)
    {
        if (amount <= 0 || duration <= 0f) return;
        recoveryRemaining = amount;
        recoveryRate = amount / duration;
        recoveryAccumulator = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || recoveryRemaining <= 0f) return;

        float delta = Mathf.Min(recoveryRate * Runner.DeltaTime, recoveryRemaining);
        recoveryAccumulator += delta;
        recoveryRemaining -= delta;

        int whole = Mathf.FloorToInt(recoveryAccumulator);
        if (whole <= 0) return;

        recoveryAccumulator -= whole;

        int remaining = whole;
        if (CurrentHP < MaxHP)
        {
            int gain = Mathf.Min(MaxHP - CurrentHP, remaining);
            CurrentHP += gain;
            remaining -= gain;
        }
        if (remaining > 0 && CurrentArmor < MaxArmor)
        {
            CurrentArmor = Mathf.Min(CurrentArmor + remaining, MaxArmor);
        }
    }
}