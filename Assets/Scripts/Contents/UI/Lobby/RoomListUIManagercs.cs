using UnityEngine;

public class RoomListUIManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject roomItemPrefab;

    public void AddRoom(string roomName, int currentPlayers, int maxPlayers)
    {
        GameObject roomItemObject = Instantiate(roomItemPrefab, contentParent);
        RoomItemUI roomItemUI = roomItemObject.GetComponent<RoomItemUI>();

        if (roomItemUI != null)
        {
            roomItemUI.SetRoomInfo(roomName, currentPlayers, maxPlayers);
        }
    }

    public void ClearRooms()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }
}