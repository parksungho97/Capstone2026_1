using UnityEngine;
using Fusion;

public class PlayerStatUIController : MonoBehaviour
{
    [Header("Stat Reference")]
    [SerializeField] private CharacterHealth playerHealth;

    [Header("UI Reference")]
    [SerializeField] private StatVisibleUI hpVisibleUI;
    [SerializeField] private StatVisibleUI armorVisibleUI;

    private void Start()
    {
        Debug.Assert(hpVisibleUI);
        Debug.Assert(armorVisibleUI);
    }

    public void Initialize(CharacterHealth playerHealth)
    {
        this.playerHealth = playerHealth;
        Debug.Assert(this.playerHealth);
    }

    private void Update()
    {
        if (playerHealth == null)
            return;

        hpVisibleUI.VisibleState(playerHealth.CurrentHP, playerHealth.MaxHP);

        armorVisibleUI.VisibleState(playerHealth.CurrentArmor, playerHealth.MaxArmor);
    }
}