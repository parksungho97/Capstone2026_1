using ParrelSync;
using UnityEngine;

namespace Network
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private RoomManager mRoomManager = null;
        [SerializeField] private RoomController mRoomController = null;
        [SerializeField] private Spawner mSpawner = null;
        [SerializeField] private CapturePoint mCapturePoint = null;
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
            Debug.Assert(mSpawner);
            Debug.Assert(mCapturePoint);
            Debug.Assert(mCapturePoint.GetComponent<Activater>());

            mRoomController.ActionRoomMemberInfoChanged += (RoomMemberInfo roomMemberInfo) =>
            {
                Debug.Log($"PlayerId: {roomMemberInfo.playerId}, Status: {roomMemberInfo.readyStatus}");
            };

            GameContextManager.Instance.BindNetworkDisconnectEvent(mNetworkRoot);

            // 여기서는 테스트를 위해
            // 원래는 방에 사람이 다 모이면 Start를 하는 순간 그 방에 있는 멤버들의 PlayerId를 전부 GCM에게 주면 됌
            GameContextManager.Instance.JoinToGame(NetworkRoot.GetLocalClientId());
            GameContextManager.Instance.JoinToGame(NetworkRoot.GetLocalClientId() + 1);

            mSpawner.InitializeFromGameContextManager(GameContextManager.Instance);

            // StartClient하자마자 Rpc함수 쓰면 안됌
            // mRoomController.BindMemberServerRpc();
        }
        private float _logTimer;
        private void Update()
        {
            var activater = mCapturePoint.GetComponent<Activater>();

            if (Input.GetKeyDown(KeyCode.Q)) activater.RegistActivateServerRpc(ERequestType.Red);
            else if (Input.GetKeyDown(KeyCode.W)) activater.UnregistActivateServerRpc(ERequestType.Red);
            else if (Input.GetKeyDown(KeyCode.E)) activater.RegistActivateServerRpc(ERequestType.Blue);
            else if (Input.GetKeyDown(KeyCode.R)) activater.UnregistActivateServerRpc(ERequestType.Blue);

            _logTimer += Time.deltaTime;
            if (_logTimer >= 0.25f)
            {
                _logTimer = 0f;
                Debug.Log($"Red: {activater.GetRedProgress()} / Blue: {activater.GetBlueProgress()}");
            }

            activater.TryActivateCapturePoint(mCapturePoint);
        
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    Transform t = mSpawner.GetMappingSpawnPosition(NetworkRoot.GetLocalClientId());
            //    Debug.Log(t.position);
            //}
            //else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Transform t = mSpawner.GetMappingSpawnPosition(NetworkRoot.GetLocalClientId() + 1);
            //    Debug.Log(t.position);
            //}
            //if (Input.GetKeyDown(KeyCode.Q))
            //    mRoomController.BindMemberServerRpc();

            //else if (Input.GetKeyDown(KeyCode.W))
            //    mRoomController.ReleaseMemberServerRpc();

            //else if (Input.GetKeyDown(KeyCode.E))
            //    mRoomController.ToggleReadyServerRpc();

            //else if (Input.GetKeyDown(KeyCode.R))
            //    Debug.Log($"IsReadyToStart: {mRoomController.IsReadyToStart()}");

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


