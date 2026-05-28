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

    // 추가: UI에서 남은 시간을 계산하기 위한 값
    private float respawnEndTime = 0f;
    private float currentRespawnDelay = 0f;

    // 추가: UI에서 현재 리스폰 중인지 확인할 수 있게 함
    public bool IsRespawning => isRespawning;

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

    /*
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
    }*/
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        ResolveReferences();

        if (playerHealth == null)
            return;

        if (!isRespawning && playerHealth.CurrentHP <= 0)
        {
            StartCoroutine(RespawnRoutine());
        }
    }


    /*
    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        float respawnDelay = baseRespawnDelay + deathCount * extraDelayPerDeath;

        // 추가: UI 카운트다운 계산용
        currentRespawnDelay = respawnDelay;
        respawnEndTime = Time.time + respawnDelay;

        Debug.Log($"[Respawn] 리스폰 대기 시간: {respawnDelay}초");

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = respawnManager.GetRandomSafeRespawnPosition();

        transform.position = respawnPosition;

        playerHealth.RPC_ResetStat();

        deathCount++;
        isRespawning = false;

        // 추가: 리스폰 완료 후 값 초기화
        respawnEndTime = 0f;
        currentRespawnDelay = 0f;

        Debug.Log($"[Respawn] 리스폰 완료 / Death Count: {deathCount}");
    }*/
    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        float respawnDelay = baseRespawnDelay + deathCount * extraDelayPerDeath;

        currentRespawnDelay = respawnDelay;
        respawnEndTime = Time.time + respawnDelay;

        Debug.Log($"[Respawn] 리스폰 대기 시간: {respawnDelay}초");

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = transform.position;

        if (respawnManager != null)
        {
            respawnPosition = respawnManager.GetRandomSafeRespawnPosition();
        }
        else
        {
            Debug.LogWarning("[Respawn] RespawnManager를 찾지 못해서 현재 위치에서 부활합니다.");
        }

        transform.position = respawnPosition;

        playerHealth.ResetStat();

        deathCount++;
        isRespawning = false;

        respawnEndTime = 0f;
        currentRespawnDelay = 0f;

        Debug.Log($"[Respawn] 리스폰 완료 / Death Count: {deathCount}");
    }


    // 추가: UI 숫자 카운트다운에 사용
    public int GetRemainingRespawnSeconds()
    {
        if (!isRespawning)
            return 0;

        float remainTime = respawnEndTime - Time.time;
        return Mathf.CeilToInt(Mathf.Max(remainTime, 0f));
    }

    // 추가: 원형 게이지 fillAmount에 사용 가능
    public float GetRespawnProgress01()
    {
        if (!isRespawning)
            return 0f;

        if (currentRespawnDelay <= 0f)
            return 0f;

        float remainTime = Mathf.Max(respawnEndTime - Time.time, 0f);
        return 1f - Mathf.Clamp01(remainTime / currentRespawnDelay);
    }
}