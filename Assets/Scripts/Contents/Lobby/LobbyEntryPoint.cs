using System.Collections.Generic;
using UnityEngine;

public class LobbyEntryPoint : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private int maxPlayerCount = 8;

    private void Awake()
    {
        Debug.Assert(lobbyManager);

        lobbyManager.ActionRoomChange += (List<CRoomInfo> roomInfos) =>
        {
            Debug.Log($"[로비] 방 목록 동기화됨. 현재 활성화된 방 개수: {roomInfos.Count}");

            this.roomInfos = roomInfos;
            foreach (CRoomInfo roomInfo in roomInfos)
            {
                Debug.Log($"- 방 이름: {roomInfo.roomName} | 인원: {roomInfo.playerCount}/{roomInfo.maxPlayerCount} | 방 상태: {roomInfo.roomState} | 방 세션: {roomInfo.roomSession}");
            }
        };
    }

    private void Start()
    {
        Debug.Assert(Network.NetworkRoot.Instance != null, "[LobbyEntryPoint] NetworkRoot가 없습니다. 로비 씬에 NetworkRoot 오브젝트를 배치하세요.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            lobbyManager.CreateAndJoinRoom("Room1", maxPlayerCount, sceneIndex);
        else if (Input.GetKeyDown(KeyCode.W))
            lobbyManager.CreateAndJoinRoom("Room2", maxPlayerCount, sceneIndex);
        else if (Input.GetKeyDown(KeyCode.E))
            lobbyManager.JoinRoom(roomInfos[0].roomSession);
        else if (Input.GetKeyDown(KeyCode.R))
            lobbyManager.JoinRoom(roomInfos[1].roomSession);
    }

    private List<CRoomInfo> roomInfos;
}
