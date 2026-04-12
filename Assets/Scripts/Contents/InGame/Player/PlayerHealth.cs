using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Default Stat")]
    [SerializeField] private int defaultMaxHP = 100;
    [SerializeField] private int defaultMaxArmor = 50;

    [Networked] public int MaxHP { get; private set; }
    [Networked] public int CurrentHP { get; private set; }

    [Networked] public int MaxArmor { get; private set; }
    [Networked] public int CurrentArmor { get; private set; }

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
    /// 체력만 회복
    /// </summary>
    public void AddHP(int amount)
    {
        if (!Object.HasStateAuthority) return;
        if (amount <= 0) return;

        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    /// <summary>
    /// 방어구만 회복
    /// </summary>
    public void AddArmor(int amount)
    {
        if (!Object.HasStateAuthority) return;
        if (amount <= 0) return;

        CurrentArmor = Mathf.Min(CurrentArmor + amount, MaxArmor);
    }

    /// <summary>
    /// 데미지 처리
    /// 방어구 먼저 감소 후 남는 데미지를 HP에 적용
    /// </summary>
    public void ServeHP(int damage)
    {
        Debug.Log($"[ServeHP] 호출됨 / Authority: {Object.HasStateAuthority}");

        if (!Object.HasStateAuthority) return;
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

        Debug.Log($"[ServeHP] 결과 → HP:{CurrentHP}, Armor:{CurrentArmor}");
    }

    /// <summary>
    /// 회복 처리
    /// HP 먼저 회복하고 남는 회복량이 있으면 Armor 회복
    /// </summary>
    public void RecoverStat(int amount)
    {
        Debug.Log($"[RecoverStat] 호출됨 / Authority: {Object.HasStateAuthority}");

        if (!Object.HasStateAuthority) return;
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

        Debug.Log($"[RecoverStat] 결과 → HP:{CurrentHP}, Armor:{CurrentArmor}");
    }

    /// <summary>
    /// 체력/방어구 초기화
    /// </summary>
    public void ResetStat()
    {
        if (!Object.HasStateAuthority) return;

        CurrentHP = MaxHP;
        CurrentArmor = MaxArmor;
    }
}