using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomListUIManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject roomItemPrefab;

    private RoomItemUI selectedRoomItem;

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

        RoomItemUI roomItemUI = roomItemObject.GetComponent<RoomItemUI>();
        if (roomItemUI == null)
        {
            Debug.LogError("[RoomListUIManager] RoomItemUI 컴포넌트 없음");
            return;
        }

        Button button = roomItemObject.GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("[RoomListUIManager] Button 컴포넌트 없음");
            return;
        }

        roomItemUI.SetRoomInfo(roomSession, roomName, currentPlayers, maxPlayers);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RoomItemSelected(roomItemUI));
    }

    public void ClearRooms()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        selectedRoomItem = null;
    }

    private void RoomItemSelected(RoomItemUI roomItem)
    {
        if (selectedRoomItem != null && selectedRoomItem != roomItem)
        {
            selectedRoomItem.DeselectUI();
        }

        selectedRoomItem = roomItem;
        selectedRoomItem.SelectUI();

        Debug.Log($"선택된 방: {roomItem.RoomName}");

        ActionRoomSelected?.Invoke(roomItem);
    }
}