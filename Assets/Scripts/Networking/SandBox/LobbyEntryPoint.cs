using ParrelSync;
using UnityEngine;

namespace Network
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private RoomManager mRoomManager = null;
        [SerializeField] private RoomController mRoomController = null;
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

            Debug.Assert(mRoomManager);
            Debug.Assert(mRoomController);

            mRoomController.ActionRoomMemberInfoChanged += (RoomMemberInfo roomMemberInfo) =>
            {
                Debug.Log($"PlayerId: {roomMemberInfo.playerId}, Status: {roomMemberInfo.readyStatus}");
            };

            // StartClient하자마자 Rpc함수 쓰면 안됌
            // mRoomController.BindMemberServerRpc();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                mRoomController.BindMemberServerRpc();

            else if (Input.GetKeyDown(KeyCode.W))
                mRoomController.ReleaseMemberServerRpc();

            else if (Input.GetKeyDown(KeyCode.E))
                mRoomController.ToggleReadyServerRpc();

            else if (Input.GetKeyDown(KeyCode.R))
                Debug.Log($"IsReadyToStart: {mRoomController.IsReadyToStart()}");

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


