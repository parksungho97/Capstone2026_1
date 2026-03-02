using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        private void Start()
        {
            var nm = NetworkManager.Singleton;

            if (nm == null)
            {
                Debug.LogError("NetworkManager 없음");
                return;
            }

            // 이미 시작했으면 무시
            if (nm.IsClient || nm.IsServer)
                return;

#if UNITY_EDITOR
            // ParrelSync 클론이면 Client
            if (ParrelSync.ClonesManager.IsClone())
            {
                Debug.Log("CLIENT 시작");
                nm.StartClient();
            }
            else
            {
                Debug.Log("HOST 시작");
                nm.StartHost();
            }
#else
            nm.StartHost();
#endif
        }
    }
}