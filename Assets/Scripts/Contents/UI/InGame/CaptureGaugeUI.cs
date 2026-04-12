using UnityEngine;
using UnityEngine.UI;

public class CaptureGaugeUI : MonoBehaviour
{
    [Header("Gauge Image")]
    [SerializeField] private Image gaugeFillImage;

    [Header("Team Colors")]
    [SerializeField] private Color neutralColor = Color.white;
    [SerializeField] private Color redTeamColor = Color.red;
    [SerializeField] private Color blueTeamColor = Color.blue;

    /// <summary>
    /// 점령 게이지 상태 반영
    /// progress는 0~1
    /// </summary>
    public void SetGauge(float progress, TeamType team)
    {
        if (gaugeFillImage == null) return;

        gaugeFillImage.fillAmount = Mathf.Clamp01(progress);
        gaugeFillImage.color = GetTeamColor(team);
    }

    /// <summary>
    /// 게이지만 변경
    /// </summary>
    public void SetProgress(float progress)
    {
        if (gaugeFillImage == null) return;

        gaugeFillImage.fillAmount = Mathf.Clamp01(progress);
    }

    /// <summary>
    /// 팀 색상만 변경
    /// </summary>
    public void SetTeamColor(TeamType team)
    {
        if (gaugeFillImage == null) return;

        gaugeFillImage.color = GetTeamColor(team);
    }

    private Color GetTeamColor(TeamType team)
    {
        switch (team)
        {
            case TeamType.Red:
                return redTeamColor;

            case TeamType.Blue:
                return blueTeamColor;

            default:
                return neutralColor;
        }
    }
}
