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
    public void Initialize(CapturePointManager capturePointManager)
    {
        this.capturePointManager = capturePointManager;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (bGameEnded == true)
            return;

        if (capturePointManager.IsAllCaptured(out ECaptureState[] captureStates))
        {
            int redCount = 0;
            int blueCount = 0;
            foreach (ECaptureState captureState in captureStates)
            {
                if (captureState == ECaptureState.Red)
                    redCount++;
                else if (captureState == ECaptureState.Blue)
                    blueCount++;
            }

            bGameEnded = true;

            if (redCount > blueCount)
                ActionGameEnded?.Invoke(EResultType.Red);
            else if (blueCount > redCount)
                ActionGameEnded?.Invoke(EResultType.Blue);
            else
                ActionGameEnded?.Invoke(EResultType.Draw);
        }
    }

    private CapturePointManager capturePointManager;
    private bool bGameEnded = false;

}

