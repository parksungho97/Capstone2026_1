using TMPro;
using UnityEngine;

public class RoomItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;

    public void SetRoomInfo(string roomSession, string roomName, int currentPlayers, int maxPlayers)
    {
        roomNameText.text = roomName;
        playerCountText.text = currentPlayers + "/" + maxPlayers;

        RoomSession = roomSession;
        RoomName = roomName;
        CurrentPlayers = currentPlayers;
        MaxPlayers = maxPlayers;
    }

    public void SelectUI()
    {
        // Todo: 뭐 색상변경 또는 애니메이션 효과 등등
        Debug.Log("Selected");
    }

    public string RoomName { get; private set; }
    public string RoomSession { get; private set; }
    public int CurrentPlayers { get; private set; }
    public int MaxPlayers { get; private set; }
}