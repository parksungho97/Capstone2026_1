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

    [Networked] public NetworkBool IsDead { get; private set; }


    /*
        상태 변경은 권한 있는 쪽에서
        보통 Object.HasStateAuthority가 있는 쪽에서 HP/Armor/Timer를 바꾸는 게 맞다. 
        권한 없는 쪽이 바꾸면 예측값처럼 보일 수 있고 나중에 덮어씌워질 수 있다.
     */

    /*
     * 사용방법 *
     플레이어 오브젝트에 붙이고, 그 오브젝트에는 **NetworkObject**도 있어야 함.
     Fusion에서 [Networked] 프로퍼티는 NetworkObject에 붙은 NetworkBehaviour상태로 복제된다..
     */

    // Spawned() 이후에 접근하는 게 안전함
    // 네트워크 상태값은 Spawned()가 호출된 뒤부터 유효.
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            MaxHP = defaultMaxHP;
            CurrentHP = defaultMaxHP;

            MaxArmor = defaultMaxArmor;
            CurrentArmor = defaultMaxArmor;

            IsDead = false;
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    public void AddHP(int amount)
    {
        if (!Object.HasStateAuthority) return;
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    
    // 방어구 회복
    public void AddArmor(int amount)
    {
        if (!Object.HasStateAuthority) return;
        if (IsDead) return;
        if (amount <= 0) return;

        CurrentArmor = Mathf.Min(CurrentArmor + amount, MaxArmor);
    }

    // 데미지 처리
    // 방어구가 있으면 방어구를 먼저 깎고,
    // 남는 데미지만 체력에 적용
    public void ServeHP(int damage)
    {
        if (!Object.HasStateAuthority) return;
        if (IsDead) return;
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

        // 3. 사망 처리
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            IsDead = true;
        }
    }

    // 체력/방어구 완전 초기화
    public void ResetStat()
    {
        if (!Object.HasStateAuthority) return;

        CurrentHP = MaxHP;
        CurrentArmor = MaxArmor;
        IsDead = false;
    }
}