using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private NetworkTimerClock clock;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Update()
    {
        float totalRemainingSeconds = clock.GetRemainingTime();

        int minutes = Mathf.FloorToInt(totalRemainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalRemainingSeconds % 60f);

        timeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
}
