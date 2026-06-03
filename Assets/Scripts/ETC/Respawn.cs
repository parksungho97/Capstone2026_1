using Fusion;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterHealth))]
public class Respawn : NetworkBehaviour
{
    [SerializeField] private float respawnDelay = 12f;
    [SerializeField] private float respawnDelayIncrement = 0f;
    private float currentRespawnDelay;
    private CharacterHealth health;

    [Networked] public bool bDead { get; private set; }
    public bool IsTimerActive { get; private set; }

    public Action ActionDead;
    public Action ActionRespawnComplete;

    private float timerEndTime;
    private float timerDuration;

    private void Start()
    {
        health = GetComponent<CharacterHealth>();
        Debug.Assert(health);
        currentRespawnDelay = respawnDelay;
    }

    private void Update()
    {
        if (!Object.HasStateAuthority) return;
        if (bDead) return;

        if (health.CurrentHP <= 0)
            Die();
    }

    public void Die()
    {
        if (bDead) return;

        bDead = true;
        ActionDead?.Invoke();
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        IsTimerActive = true;
        timerDuration = currentRespawnDelay;
        timerEndTime = Time.realtimeSinceStartup + currentRespawnDelay;

        yield return new WaitForSecondsRealtime(currentRespawnDelay);

        currentRespawnDelay += respawnDelayIncrement;
        IsTimerActive = false;
        bDead = false;
        ActionRespawnComplete?.Invoke();
    }

    public float GetRemainingSeconds()
    {
        if (!IsTimerActive) return 0f;
        return Mathf.Max(timerEndTime - Time.realtimeSinceStartup, 0f);
    }

    public float GetProgress01()
    {
        if (!IsTimerActive || timerDuration <= 0f) return 0f;
        return 1f - Mathf.Clamp01(GetRemainingSeconds() / timerDuration);
    }
}
