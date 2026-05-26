using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Image progressImage;

    private PlayerRespawnController targetRespawnController;

    public void Initialize(PlayerRespawnController controller)
    {
        targetRespawnController = controller;

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    private void Update()
    {
        if (targetRespawnController == null)
        {
            if (rootPanel != null && rootPanel.activeSelf)
                rootPanel.SetActive(false);

            return;
        }

        if (targetRespawnController.IsRespawning)
        {
            if (rootPanel != null && !rootPanel.activeSelf)
                rootPanel.SetActive(true);

            int remainSeconds = targetRespawnController.GetRemainingRespawnSeconds();

            if (countdownText != null)
                countdownText.text = remainSeconds.ToString();

            if (progressImage != null)
                progressImage.fillAmount = targetRespawnController.GetRespawnProgress01();
        }
        else
        {
            if (rootPanel != null && rootPanel.activeSelf)
                rootPanel.SetActive(false);
        }
    }
}
