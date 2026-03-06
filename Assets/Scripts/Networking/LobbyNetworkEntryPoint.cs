using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class LobbyNetworkEntryPoint : MonoBehaviour
    {
        [SerializeField] private RoomManager mRoomManager = null;
        [SerializeField] private RoomController mRoomController = null;

        private void Start()
        {
            var nm = NetworkManager.Singleton;

            if (nm == null)
            {
                Debug.LogError("[LobbyNetworkEntryPoint] NetworkManager.Singleton 없음");
                return;
            }

            if (!nm.IsClient && !nm.IsServer)
            {
                if (ClonesManager.IsClone())
                {
                    Debug.Log("[LobbyNetworkEntryPoint] 클론 에디터 → Client 시작");
                    nm.StartClient();
                }
                else
                {
                    Debug.Log("[LobbyNetworkEntryPoint] 메인 에디터 → Host 시작");
                    nm.StartHost();
                }
            }

            Debug.Assert(mRoomManager);
            Debug.Assert(mRoomController);
        }
    }
}