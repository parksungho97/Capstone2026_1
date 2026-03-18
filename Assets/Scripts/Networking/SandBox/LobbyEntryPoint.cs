using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class LobbyEntryPoint : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private int maxPlayerCount = 8;
    private void Start()
    {
        Debug.Assert(lobbyManager);
        Debug.Assert(Network.MyNetworkRoot.Instance.Runner);

        lobbyManager.ActionRoomChange += (List<CRoomInfo> roomInfos) =>
        {
            Debug.Log($"[로비] 방 목록 동기화됨. 현재 활성화된 방 개수: {roomInfos.Count}");

            this.roomInfos = roomInfos;
            foreach (CRoomInfo roomInfo in roomInfos)
            {
                Debug.Log($"- 방 이름: {roomInfo.roomName} | 인원: {roomInfo.playerCount}/{roomInfo.maxPlayerCount} | 방 상태?: {roomInfo.roomState}" +
                    $"| 방 세션: {roomInfo.roomSession}");
            }
        };
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lobbyManager.CreateAndJoinRoom("Room1", 8, sceneIndex);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            lobbyManager.CreateAndJoinRoom("Room2", 8, sceneIndex);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            lobbyManager.JoinRoom(roomInfos[0].roomSession);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            lobbyManager.JoinRoom(roomInfos[1].roomSession);
        }
    }
    private List<CRoomInfo> roomInfos;
}
