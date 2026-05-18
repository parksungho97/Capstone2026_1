using Fusion;
using System;
using UnityEngine;

public enum EResultType : byte
{
    Red,
    Blue,
    Draw
}

// 이름 중복 방지 위해 CGameMode로 명명
public class CGameMode : NetworkBehaviour
{
    public Action<EResultType> ActionGameEnded;
    public void Initialize(CapturePointManager capturePointManager, NetworkTimerClock networkTimerClock)
    {
        this.capturePointManager = capturePointManager;
        this.networkTimerClock = networkTimerClock;

        ActionGameEnded+= (resultType) =>
        {
            Debug.Log($"Game Ended with result: {resultType}");
        };
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (bGameEnded == true)
            return;

        bool bAllCaptured = capturePointManager.IsAllCaptured(out ECaptureState[] captureStates);

        int redCount = 0;
        int blueCount = 0;
        foreach (ECaptureState captureState in captureStates)
        {
            if (captureState == ECaptureState.Red)
                redCount++;
            else if (captureState == ECaptureState.Blue)
                blueCount++;
        }

        if (networkTimerClock.IsExpired() || bAllCaptured)
            EndGame(redCount, blueCount);
    }

    private void EndGame(int redCount, int blueCount)
    {
        bGameEnded = true;

        if (redCount > blueCount)
            ActionGameEnded?.Invoke(EResultType.Red);
        else if (blueCount > redCount)
            ActionGameEnded?.Invoke(EResultType.Blue);
        else
            ActionGameEnded?.Invoke(EResultType.Draw);
    }

    private CapturePointManager capturePointManager;
    private NetworkTimerClock networkTimerClock;
    private bool bGameEnded = false;

}

