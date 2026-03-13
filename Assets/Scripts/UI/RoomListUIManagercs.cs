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

    private void Start()
    {
        // 테스트용 데이터
        AddRoom("방이름1", 1, 4);
        AddRoom("방이름2", 2, 4);
        AddRoom("방이름3", 4, 4);
    }
}