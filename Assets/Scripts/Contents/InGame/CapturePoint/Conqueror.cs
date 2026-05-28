using System;
using System.Collections.Generic;
using UnityEngine;

// Manages two Activaters (Red / Blue).
// Activates an Activater only when that team has >= 2 requests.
// Freezes both when both teams meet the threshold (standoff).
public class Conqueror : MonoBehaviour
{
    public event Action<ECaptureState> OnCapture;

    [SerializeField] private float mIncreaseRate = 10f;
    [SerializeField] private float mDecreaseRate = 5f;
    [SerializeField] private float mHoldDecayDelay = 3f;

    public float GetRedGauge() => mRedActivater?.Gauge ?? 0f;
    public float GetBlueGauge() => mBlueActivater?.Gauge ?? 0f;

    public void AddRequest(int requesterId, ECaptureState team)
    {
        if (team == ECaptureState.Red)
            mRedRequests.Add(requesterId);
        else if (team == ECaptureState.Blue)
            mBlueRequests.Add(requesterId);
    }

    public void RemoveRequest(int requesterId)
    {
        mRedRequests.Remove(requesterId);
        mBlueRequests.Remove(requesterId);
    }

    public void Tick(float dt)
    {
        bool redEnough = mRedRequests.Count >= 1;
        bool blueEnough = mBlueRequests.Count >= 1;

        if (redEnough && blueEnough) return; // standoff — freeze both

        mRedActivater.SetRequest(redEnough);
        mBlueActivater.SetRequest(blueEnough);
        mRedActivater.Tick(dt);
        mBlueActivater.Tick(dt);
    }

    public void Clear()
    {
        mRedRequests.Clear();
        mBlueRequests.Clear();
        mRedActivater.Clear();
        mBlueActivater.Clear();
    }

    private void Awake()
    {
        mRedActivater = new Activater(mIncreaseRate, mDecreaseRate, mHoldDecayDelay);
        mBlueActivater = new Activater(mIncreaseRate, mDecreaseRate, mHoldDecayDelay);

        mRedActivater.OnComplete += () =>
            OnCapture?.Invoke(ECaptureState.Red);
        mBlueActivater.OnComplete += () => 
            OnCapture?.Invoke(ECaptureState.Blue);
    }

    private Activater mRedActivater;
    private Activater mBlueActivater;
    private readonly HashSet<int> mRedRequests = new HashSet<int>();
    private readonly HashSet<int> mBlueRequests = new HashSet<int>();
}
