using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color selectedColor = new Color(1f, 0.75f, 0.35f, 0.85f);

    public void SetRoomInfo(string roomSession, string roomName, int currentPlayers, int maxPlayers)
    {
        roomNameText.text = roomName;
        playerCountText.text = currentPlayers + "/" + maxPlayers;

        RoomSession = roomSession;
        RoomName = roomName;
        CurrentPlayers = currentPlayers;
        MaxPlayers = maxPlayers;

        DeselectUI();
    }

    public void SelectUI()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = selectedColor;
        }

        Debug.Log($"Selected: {RoomName}");
    }

    public void DeselectUI()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }

    public string RoomName { get; private set; }
    public string RoomSession { get; private set; }
    public int CurrentPlayers { get; private set; }
    public int MaxPlayers { get; private set; }
}