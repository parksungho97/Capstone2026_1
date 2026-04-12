using UnityEngine;

public class CaptureGaugeTest : MonoBehaviour
{
    [SerializeField] private CaptureGaugeUI gaugeUI;

    private float progress = 0f;
    private TeamType currentTeam = TeamType.None;

    private void Update()
    {
        if (gaugeUI == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentTeam = TeamType.Red;
            progress += 0.1f;
            gaugeUI.SetGauge(progress, currentTeam);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentTeam = TeamType.Blue;
            progress += 0.1f;
            gaugeUI.SetGauge(progress, currentTeam);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            progress = 0f;
            currentTeam = TeamType.None;
            gaugeUI.SetGauge(progress, currentTeam);
        }
    }
}