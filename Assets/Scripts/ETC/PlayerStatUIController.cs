using UnityEngine;
using Fusion;

public class PlayerStatUIController : MonoBehaviour
{
    [Header("Stat Reference")]
    [SerializeField] private CharacterHealth playerHealth;

    [Header("UI Reference")]
    [SerializeField] private StatVisibleUI hpVisibleUI;
    [SerializeField] private StatVisibleUI armorVisibleUI;

    public void Initialize(CharacterHealth playerHealth)
    {
        this.playerHealth = playerHealth;
        Debug.Assert(this.playerHealth);
    }
    private void Update()
    {
        if (playerHealth == null)
        {
            CharacterHealth[] all = FindObjectsOfType<CharacterHealth>();

            foreach (var ph in all)
            {
                if (ph != null && ph.Object != null && ph.Object.IsValid)
                {
                    playerHealth = ph;
                    Debug.Log("[UI] Spawn된 PlayerHealth 자동 연결 완료");
                    break;
                }
            }

            if (playerHealth == null)
                return;
        }

        if (playerHealth.Object == null || !playerHealth.Object.IsValid)
            return;

        if (hpVisibleUI != null)
            hpVisibleUI.VisibleState(playerHealth.CurrentHP, playerHealth.MaxHP);

        if (armorVisibleUI != null)
            armorVisibleUI.VisibleState(playerHealth.CurrentArmor, playerHealth.MaxArmor);
    }
}