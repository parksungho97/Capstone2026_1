using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : NetworkBehaviour
{
    [SerializeField] private NetworkTimerClock clock;
    [SerializeField] private TextMeshProUGUI timeText;
    private bool bRunning = false;
    public override void Spawned()
    {
        base.Spawned();
        bRunning = true;
    }

    private void Update()
    {
        if (bRunning == false)
            return;

        float totalRemainingSeconds = clock.GetRemainingTime();

        int minutes = Mathf.FloorToInt(totalRemainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalRemainingSeconds % 60f);

        timeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
}
