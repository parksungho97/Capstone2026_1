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

    [Header("Team Colors")]

    // 흰색에 가까운 연한 시멘트톤 + 약간 투명
    [SerializeField] private Color neutralColor = new Color(0.93f, 0.93f, 0.93f, 0.85f);

    // 부드러운 연핑크 레드톤 + 약간 투명
    [SerializeField] private Color redTeamColor = new Color(1f, 0.72f, 0.72f, 0.9f);

    // 부드러운 스카이블루톤 + 약간 투명
    [SerializeField] private Color blueTeamColor = new Color(0.72f, 0.86f, 1f, 0.9f);

    /// <summary>
    /// 점령 게이지 상태 반영
    /// </summary>
    public void SetGauge(float progress, float maxProgress, EGuageColor team)
    {
        if (gaugeFillImage == null) return;

        float clampedProgress = Mathf.Clamp01(progress / maxProgress);

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

    /// <summary>
    /// 팀별 색상 반환
    /// </summary>
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