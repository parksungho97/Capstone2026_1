using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private int sceneIndex;
        [SerializeField] private LobbyManager lobbyManager;
        private void Start()
        {
            Debug.Assert(lobbyManager);
            Debug.Assert(MyNetworkRoot.Instance.Runner);

            lobbyManager.ActionRoomChange += (List<SessionInfo> sessionInfos) =>
            {
                Debug.Log($"[로비] 방 목록 동기화됨. 현재 활성화된 방 개수: {sessionInfos.Count}");

                foreach (SessionInfo session in sessionInfos)
                {
                    Debug.Log($"- 방 이름: {session.Name} | 인원: {session.PlayerCount}/{session.MaxPlayers} | 입장가능?: {session.IsOpen}");
                }
            };

            lobbyManager.Initalize();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                lobbyManager.JoinOrCreateRoom("Room1", sceneIndex);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                lobbyManager.JoinOrCreateRoom("Room2", sceneIndex);
            }
        }
    }
}