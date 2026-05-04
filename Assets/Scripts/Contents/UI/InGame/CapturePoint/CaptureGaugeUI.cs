using UnityEngine;
using UnityEngine.UI;

public enum EGuageColor : byte
{
    Neutral,
    Red,
    Blue
}
public class CaptureGaugeUI : MonoBehaviour
{
    [Header("Gauge Image")]
    [SerializeField] private Image gaugeFillImage;

    private Color neutralColor = Color.white;
    private Color redTeamColor = Color.red;
    private Color blueTeamColor = Color.blue;

    /// <summary>
    /// 점령 게이지 상태 반영
    /// progress는 0~1
    /// </summary>
    public void SetGauge(float progress, float maxProgress, EGuageColor team)
    {
        float clampedProgress = progress / maxProgress;
        gaugeFillImage.fillAmount = clampedProgress;

        gaugeFillImage.color = GetTeamColor(team);
    }

    /// <summary>
    /// 게이지만 변경
    /// </summary>
    private void SetProgress(float progress)
    {
        if (gaugeFillImage == null) return;

        gaugeFillImage.fillAmount = Mathf.Clamp01(progress);
    }

    /// <summary>
    /// 팀 색상만 변경
    /// </summary>
    private void SetTeamColor(EGuageColor team)
    {
        if (gaugeFillImage == null) return;

        gaugeFillImage.color = GetTeamColor(team);
    }

    private Color GetTeamColor(EGuageColor team)
    {
        switch (team)
        {
            case EGuageColor.Red:
                return redTeamColor;

            case EGuageColor.Blue:
                return blueTeamColor;

            default:
                return neutralColor;
        }
    }
}
