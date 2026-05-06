using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color readyColor = new Color(0.3f, 1f, 0.3f, 0.7f);

    public bool IsReady { get; private set; }

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
        SetReady(false);
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;

        if (backgroundImage != null)
        {
            backgroundImage.color = IsReady ? readyColor : normalColor;
        }

        Debug.Log($"{playerNameText.text} Ready 상태: {IsReady}");
    }

    public void ToggleReady()
    {
        SetReady(!IsReady);
    }
}