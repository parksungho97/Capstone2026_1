using TMPro;
using UnityEngine;

public class RoomItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;

    public void SetRoomInfo(string roomName, int currentPlayers, int maxPlayers)
    {
        roomNameText.text = roomName;
        playerCountText.text = currentPlayers + "/" + maxPlayers;
    }
}