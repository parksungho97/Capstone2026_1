using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class RoomListItem : MonoBehaviour
    {
        //[SerializeField] private Button selectButton = null;   // 루트 버튼
        //[SerializeField] private TMP_Text roomNameText = null;
        //[SerializeField] private TMP_Text countText = null;    // "1/6" 같은 표시(선택)

        //[SerializeField] private Image highlight = null;        // 선택 강조용(선택)

        //private LobbyUIController _lobby;
        //private Room _room;

        //public int RoomId => _room.roomId;

        //public void Bind(LobbyUIController lobby, Room room)
        //{
        //    _lobby = lobby;
        //    _room = room;

        //    if (roomNameText) roomNameText.text = room.roomName.ToString();
        //    if (countText) countText.text = $"{room.playerCount}/{room.maxPlayers}";

        //    if (selectButton)
        //    {
        //        selectButton.onClick.RemoveAllListeners();
        //        selectButton.onClick.AddListener(() => _lobby.SelectRoom(_room));
        //    }

        //    SetSelected(false);
        //}

        //public void SetSelected(bool selected)
        //{
        //    if (highlight) highlight.enabled = selected;
        //}
    }
}