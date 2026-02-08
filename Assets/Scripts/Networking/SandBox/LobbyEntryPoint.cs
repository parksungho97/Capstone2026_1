using ParrelSync;
using Unity.Netcode;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Network
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        // [SerializeField] private RoomManager mRoomManager = null;
        private void Start()
        {
            mNetworkRoot = NetworkRoot.Instance;

            mNetworkRoot.Init("127.0.0.1", 7777);
            mNetworkRoot.RegistClientConnected(OnClientConnected);
            mNetworkRoot.RegistClientDisconnected(OnClientDisconnected);

            if (ClonesManager.IsClone())
            {
                // 복제본 에디터: 클라이언트로 시작
                Debug.Log("이곳은 클론 에디터입니다. Client로 접속합니다.");
                mNetworkRoot.StartClient();
            }
            else
            {
                // 원본 에디터: 호스트로 시작
                Debug.Log("이곳은 메인 에디터입니다. Host를 실행합니다.");
                mNetworkRoot.StartHost();
            }
        }

        private void OnDestroy()
        {
            mNetworkRoot.Shutdown();
        }
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"{clientId} connected");
        }

        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"{clientId} disConnected");
            // 여러가지 것들
        }

        private NetworkRoot mNetworkRoot = null;

    }
}


