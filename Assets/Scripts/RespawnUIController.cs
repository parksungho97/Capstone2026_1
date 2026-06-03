using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image progressImage;

    private Respawn respawn;

    public void Initialize(Respawn respawn)
    {
        this.respawn = respawn;

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    private void Update()
    {
        if (respawn == null)
        {
            if (rootPanel != null && rootPanel.activeSelf)
                rootPanel.SetActive(false);
            return;
        }

        if (respawn.IsTimerActive)
        {
            if (rootPanel != null && !rootPanel.activeSelf)
                rootPanel.SetActive(true);

            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(respawn.GetRemainingSeconds()).ToString();

            if (progressImage != null)
                progressImage.fillAmount = respawn.GetProgress01();
        }
        else
        {
            if (rootPanel != null && rootPanel.activeSelf)
                rootPanel.SetActive(false);
        }
    }
}
