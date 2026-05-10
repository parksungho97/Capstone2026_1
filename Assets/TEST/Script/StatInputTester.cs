using UnityEngine;

public class StatInputTester : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Test Amount")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private int recoverAmount = 10;

    private void Update()
    {
        if (playerHealth == null)
        {
            PlayerHealth[] all = FindObjectsOfType<PlayerHealth>();

            foreach (var ph in all)
            {
                if (ph != null && ph.Object != null && ph.Object.IsValid)
                {
                    playerHealth = ph;
                    Debug.Log("[INPUT] Spawn된 PlayerHealth 자동 연결 완료");
                    break;
                }
            }

            if (playerHealth == null)
                return;
        }

        if (playerHealth.Object == null || !playerHealth.Object.IsValid)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[INPUT] T");
            playerHealth.ServeHP(damageAmount);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("[INPUT] Y");
            playerHealth.RecoverStat(recoverAmount);
        }
    }
}