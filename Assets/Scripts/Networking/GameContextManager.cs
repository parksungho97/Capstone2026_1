using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Network
{
    public struct PlayerInfo
    {
        public ulong playerId;
    }
    public class GameContextManager : NetworkBehaviour
    {
        public static GameContextManager Instance { get; private set; }

        // 외부에서는 이 리스트를 읽기만 가능하게 노출
        public IReadOnlyList<PlayerInfo> PlayerInfos => mPlayerInfos;

        // PlayerId를 올려놓음.
        public void JoinToGame(ulong playerId)
        {
            Debug.Assert(mPlayerInfos.Exists(playerInfo => playerInfo.playerId == playerId) == false);

            mPlayerInfos.Add(new PlayerInfo { playerId = playerId });
            Debug.Log($"[GameContext] Player {playerId} Joined.");
        }

        // 웬만해서는 불리면 안되는 함수, network비정상 종료시 호출
        public void TerminateInGame(ulong playerId)
        {
            Debug.Assert(mPlayerInfos.Exists(playerInfo => playerInfo.playerId == playerId));

            mPlayerInfos.RemoveAll(p => p.playerId == playerId);
            Debug.Log($"[GameContext] Player {playerId} Terminated.");
        }


        // 외부(EntryPoint 등)에서 네트워크 매니저와 이 클래스를 연결해주는 함수
        // GCM은 NetworkVariable을 사용하지 않기 때문에 누군가 연결이 끊기면 수동으로 업데이트 해줘야함.
        public void BindNetworkDisconnectEvent(NetworkRoot networkRoot)
        {
            networkRoot.RegistClientDisconnected(TerminateInGame);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private List<PlayerInfo> mPlayerInfos = new();
    }
}

