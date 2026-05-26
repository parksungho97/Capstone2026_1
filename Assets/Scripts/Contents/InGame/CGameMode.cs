using Fusion;
using System;
using UnityEngine;

public enum EResultType : byte
{
    Red,
    Blue,
    Draw
}

public class CGameMode : NetworkBehaviour
{
    public Action<EResultType> ActionGameEnded;
    public void Initialize(NetworkTimerClock networkTimerClock)
    {
        this.networkTimerClock = networkTimerClock;

        ActionGameEnded += (resultType) =>
        {
            Debug.Log($"Game Ended with result: {resultType}");
        };
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (bGameEnded == true)
            return;

        // TODO: query jjhCapturePointController array to determine per-team capture counts
        // and end the game when all points are captured.
        if (networkTimerClock.IsExpired())
            EndGame(0, 0);
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

    private NetworkTimerClock networkTimerClock;
    private bool bGameEnded = false;

}

