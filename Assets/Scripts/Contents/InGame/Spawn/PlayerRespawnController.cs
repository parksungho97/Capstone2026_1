using System.Collections;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RespawnManager respawnManager;

    private int deathCount = 0;
    private float baseDelay = 12f;
    private float extraDelay = 4f;

    private bool isDead = false;

    private void Update()
    {
        if (!isDead && playerHealth.CurrentHP <= 0)
        {
            isDead = true;
            StartCoroutine(RespawnRoutine());
        }
    }
    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (respawnManager == null)
            respawnManager = FindObjectOfType<RespawnManager>();
    }
    private IEnumerator RespawnRoutine()
    {
        float delay = baseDelay + deathCount * extraDelay;

        Debug.Log($"리스폰 대기 시간: {delay}초");

        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = respawnManager.GetSafePosition();

        transform.position = spawnPos;
        playerHealth.ResetStat();

        deathCount++;
        isDead = false;
    }
}