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
    [SerializeField] private CapturePointController[] capturePointControllers;

    public Action<EResultType> ActionGameEnded;

    public void Initialize(NetworkTimerClock networkTimerClock)
    {
        this.networkTimerClock = networkTimerClock;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Object.HasStateAuthority || bGameEnded)
            return;

        if (networkTimerClock.IsExpired())
        {
            int redCount = 0, blueCount = 0;
            foreach (var cp in capturePointControllers)
            {
                if (cp == null) continue;
                ECaptureState state = cp.CapturePoint.GetCaptureState();
                if (state == ECaptureState.Red) redCount++;
                else if (state == ECaptureState.Blue) blueCount++;
            }
            EndGame(redCount, blueCount);
        }
    }

    private void EndGame(int redCount, int blueCount)
    {
        bGameEnded = true;

        EResultType result;
        if (redCount > blueCount)       result = EResultType.Red;
        else if (blueCount > redCount)  result = EResultType.Blue;
        else                            result = EResultType.Draw;

        Debug.Log($"[CGameMode] Timer expired — Red: {redCount}, Blue: {blueCount}, Result: {result}");
        RPC_NotifyGameEnded(result);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyGameEnded(EResultType result)
    {
        ActionGameEnded?.Invoke(result);
    }

    private NetworkTimerClock networkTimerClock;
    private bool bGameEnded = false;
}
