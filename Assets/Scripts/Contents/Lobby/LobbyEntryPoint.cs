using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntryPoint : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private int maxPlayerCount = 8;
    [SerializeField] private RoomListUIManager roomListUIManager;
    [SerializeField] private CreateRoomPopupUI createRoomPopupUI;
    [SerializeField] private Button joinRoomButton;

    private void Awake()
    {
        Debug.Assert(lobbyManager);
        Debug.Assert(roomListUIManager);
        Debug.Assert(createRoomPopupUI);
        Debug.Assert(joinRoomButton);

        lobbyManager.ActionRoomChange += (List<CRoomInfo> roomInfos) =>
        {
            roomListUIManager.ClearRooms();
            foreach (CRoomInfo roomInfo in roomInfos)
            {
                roomListUIManager.AddRoom(roomInfo.roomSession, roomInfo.roomName, roomInfo.playerCount, roomInfo.maxPlayerCount);
            }
        };

        roomListUIManager.ActionRoomSelected += (RoomItemUI roomItemUI) =>
        {
            selectedRoom = roomItemUI;
            roomItemUI.SelectUI();
        };

        createRoomPopupUI.OnCreateRoomRequested += (string roomName) =>
        {
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogWarning("[로비] 방 이름이 유효하지 않습니다. 방 이름을 입력해주세요.");
                return;
            }
            lobbyManager.CreateAndJoinRoom(roomName, maxPlayerCount, sceneIndex);
        };

        joinRoomButton.onClick.AddListener(() =>
        {
            if (selectedRoom == null)
                return;
            lobbyManager.JoinRoom(selectedRoom.RoomSession);
        });
    }

    private void Start()
    {
        Debug.Assert(Network.NetworkRoot.Instance != null, "[LobbyEntryPoint] NetworkRoot가 없습니다. 로비 씬에 NetworkRoot 오브젝트를 배치하세요.");
    }

    private RoomItemUI selectedRoom;

}
