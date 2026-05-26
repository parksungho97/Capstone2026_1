using UnityEngine;
using UnityEngine.UI;

public class StatVisibleUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillImage;

    [Header("Optional Off State")]
    [SerializeField] private Graphic[] offStateTargets;
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.gray;

    public void VisibleState(int currentState, int maxState)
    {
        if (fillImage == null) return;

        if (maxState <= 0)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        float ratio = Mathf.Clamp01((float)currentState / maxState);
        Debug.Log(ratio);
        fillImage.fillAmount = ratio;
    }

    public void VisibleOffState()
    {
        if (fillImage != null)
            fillImage.fillAmount = 0f;

        if (offStateTargets == null) return;

        for (int i = 0; i < offStateTargets.Length; i++)
        {
            if (offStateTargets[i] != null)
                offStateTargets[i].color = offColor;
        }
    }

    public void VisibleOnState()
    {
        if (offStateTargets == null) return;

        for (int i = 0; i < offStateTargets.Length; i++)
        {
            if (offStateTargets[i] != null)
                offStateTargets[i].color = onColor;
        }
    }
}
