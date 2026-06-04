using System;
using System.Collections.Generic;
using UnityEngine;

// Manages one Activater for uncapturing.
// Activates the Activater with even a single request.
public class Rebel : MonoBehaviour
{
    public event Action OnUncapture;

    [SerializeField] private float mIncreaseRate = 10f;
    [SerializeField] private float mDecreaseRate = 5f;
    [SerializeField] private float mHoldDecayDelay = 3f;
    [SerializeField] private int requestCount = 1;

    public float GetGauge() => mActivater?.Gauge ?? 0f;

    public void AddRequest(int requesterId)
    {
        mRequests.Add(requesterId);
    }

    public void RemoveRequest(int requesterId)
    {
        mRequests.Remove(requesterId);
    }

    public void Tick(float dt)
    {
        mActivater.SetRequest(mRequests.Count >= requestCount);
        mActivater.Tick(dt);
    }

    public void Clear()
    {
        mRequests.Clear();
        mActivater.Clear();
    }

    public void ClearRequest()
    {
        mRequests.Clear();
    }

    private void Awake()
    {
        mActivater = new Activater(mIncreaseRate, mDecreaseRate, mHoldDecayDelay, useCheckpoints: false);
        mActivater.OnComplete += () => OnUncapture?.Invoke();
    }

    private Activater mActivater;
    private readonly HashSet<int> mRequests = new HashSet<int>();
}
