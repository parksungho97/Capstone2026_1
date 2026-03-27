using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomListUIManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject roomItemPrefab;

    public Action<RoomItemUI> ActionRoomSelected;

    public void AddRoom(string roomSession, string roomName, int currentPlayers, int maxPlayers)
    {
        Debug.Log($"[RoomListUIManager] AddRoom 호출됨: {roomName}, {currentPlayers}/{maxPlayers}");

        if (contentParent == null)
        {
            Debug.LogError("[RoomListUIManager] contentParent가 연결되지 않았습니다.");
            return;
        }

        if (roomItemPrefab == null)
        {
            Debug.LogError("[RoomListUIManager] roomItemPrefab이 연결되지 않았습니다.");
            return;
        }

        GameObject roomItemObject = Instantiate(roomItemPrefab, contentParent);
        Debug.Log($"[RoomListUIManager] roomItem 생성됨: {roomItemObject.name}");

        RoomItemUI roomItemUI = roomItemObject.GetComponent<RoomItemUI>();
        roomItemUI.GetComponent<Button>().onClick.AddListener(() => RoomItemSelected(roomItemUI));

        if (roomItemUI != null)
        {
            roomItemUI.SetRoomInfo(roomSession, roomName, currentPlayers, maxPlayers);
            Debug.Log("[RoomListUIManager] SetRoomInfo 실행 완료");
        }
        else
        {
            Debug.LogError("[RoomListUIManager] roomItemPrefab에 RoomItemUI 컴포넌트가 없습니다.");
        }
    }

    public void ClearRooms()
    {
        if (contentParent == null)
        {
            Debug.LogError("[RoomListUIManager] contentParent가 연결되지 않았습니다.");
            return;
        }

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        Debug.Log("[RoomListUIManager] ClearRooms 완료");
    }

    private void RoomItemSelected(RoomItemUI roomItem)
    {
        ActionRoomSelected?.Invoke(roomItem);
    }
}