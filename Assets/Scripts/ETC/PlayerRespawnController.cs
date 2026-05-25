using System.Collections;
using Fusion;
using UnityEngine;

public class PlayerRespawnController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterHealth playerHealth;
    [SerializeField] private RespawnManager respawnManager;

    [Header("Respawn Time")]
    [SerializeField] private float baseRespawnDelay = 12f;
    [SerializeField] private float extraDelayPerDeath = 4f;

    private int deathCount = 0;
    private bool isRespawning = false;

    public override void Spawned()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<CharacterHealth>();
        }

        if (respawnManager == null)
        {
            respawnManager = FindObjectOfType<RespawnManager>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        ResolveReferences();

        if (playerHealth == null)
            return;

        if (respawnManager == null)
            return;

        if (!isRespawning && playerHealth.CurrentHP <= 0)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        float respawnDelay = baseRespawnDelay + deathCount * extraDelayPerDeath;

        Debug.Log($"[Respawn] 리스폰 대기 시간: {respawnDelay}초");

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = respawnManager.GetRandomSafeRespawnPosition();

        transform.position = respawnPosition;

        playerHealth.RPC_ResetStat();

        deathCount++;
        isRespawning = false;

        Debug.Log($"[Respawn] 리스폰 완료 / Death Count: {deathCount}");
    }
}