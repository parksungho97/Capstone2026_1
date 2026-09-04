using System;
using UnityEngine;

public enum jjhActivaterState
{
    Default,
    Increasing,
    Holding,
    Decreasing
}

// Blind to team context. Caller decides when to set request and when to tick.
public class Activater
{
    public event Action OnComplete;

    public jjhActivaterState State { get; private set; }
    public float Gauge { get; private set; }

    private readonly float mIncreaseRate;
    private readonly float mDecreaseRate;
    private readonly float mHoldDecayDelay;
    private readonly bool mUseCheckpoints;

    private float mHoldTimer;
    private float mDecayTarget;

    public Activater(float increaseRate, float decreaseRate, float holdDecayDelay, bool useCheckpoints = true)
    {
        mIncreaseRate = increaseRate;
        mDecreaseRate = decreaseRate;
        mHoldDecayDelay = holdDecayDelay;
        mUseCheckpoints = useCheckpoints;
    }

    // Call once per tick before Tick() to reflect current request presence.
    public void SetRequest(bool hasRequest)
    {
        switch (State)
        {
            case jjhActivaterState.Default:
                if (hasRequest) State = jjhActivaterState.Increasing;
                break;
            case jjhActivaterState.Increasing:
                if (!hasRequest) { mHoldTimer = 0f; State = jjhActivaterState.Holding; }
                break;
            case jjhActivaterState.Holding:
                if (hasRequest) State = jjhActivaterState.Increasing;
                break;
            case jjhActivaterState.Decreasing:
                if (hasRequest) State = jjhActivaterState.Increasing;
                break;
        }
    }

    public void Tick(float dt)
    {
        switch (State)
        {
            case jjhActivaterState.Increasing:
                Gauge = Mathf.Min(Gauge + mIncreaseRate * dt, 100f);
                if (Gauge >= 100f)
                {
                    State = jjhActivaterState.Default;
                    OnComplete?.Invoke();
                }
                break;

            case jjhActivaterState.Holding:
                mHoldTimer += dt;
                if (mHoldTimer >= mHoldDecayDelay)
                {
                    mDecayTarget = mUseCheckpoints ? ComputeDecayTarget(Gauge) : 0f;
                    State = jjhActivaterState.Decreasing;
                }
                break;

            case jjhActivaterState.Decreasing:
                Gauge -= mDecreaseRate * dt;
                if (Gauge <= mDecayTarget)
                {
                    Gauge = mDecayTarget;
                    State = jjhActivaterState.Default;
                }
                break;
        }
    }

    public void Clear()
    {
        State = jjhActivaterState.Default;
        Gauge = 0f;
        mHoldTimer = 0f;
        mDecayTarget = 0f;
    }

    // Returns the nearest lower checkpoint: 0, 33, 66, or 99.
    private static float ComputeDecayTarget(float gauge)
    {
        if (gauge > 99f) return 99f;
        if (gauge > 66f) return 66f;
        if (gauge > 33f) return 33f;
        return 0f;
    }
}
