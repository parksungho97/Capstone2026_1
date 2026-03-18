using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum ERoomState : byte
{
    Room,
    InGame,
}

public struct CRoomInfo
{
    public string roomSession;
    public string roomName;
    public int playerCount;
    public int maxPlayerCount;
    public ERoomState roomState;
}

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public Action<List<CRoomInfo>> ActionRoomChange;

    public async void Start()
    {
        Network.MyNetworkRoot.Instance.Runner.AddCallbacks(this);

        // 3. 광장 접속 (비동기 대기). 
        // 이 순간부터 포톤 서버가 이 클라이언트에게 방 목록을 쏴주기 시작합니다.
        StartGameResult result = await Network.MyNetworkRoot.Instance.Runner.JoinSessionLobby(SessionLobby.Shared);

        Debug.Assert(result.Ok);

        Debug.Log("[로비] 접속 성공! 이제 서버가 방 목록을 갱신해 주기를 대기합니다.");
    }

    public async void CreateAndJoinRoom(string roomName, int maxPlayerCount, int sceneIndex)
    {
        var sceneManager = Network.MyNetworkRoot.Instance.SceneManagerDefault;

        // C++의 고유 ID 생성 로직처럼, 세션 네임은 전역 고유값이 안전합니다.
        string sessionName = $"ROOM_{mRoomNumber++}_{Guid.NewGuid().ToString().Substring(0, 5)}";

        var customProps = new Dictionary<string, SessionProperty>();
        customProps["RoomSession"] = sessionName;
        customProps["DisplayName"] = roomName;
        customProps["RoomState"] = "Room";

        var result = await Network.MyNetworkRoot.Instance.Runner.StartGame(new StartGameArgs
        {
            GameMode = Fusion.GameMode.Shared,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneIndex),
            SceneManager = sceneManager,
            PlayerCount = maxPlayerCount,
            SessionProperties = customProps
        });

        if (result.Ok)
            Debug.Log($"[로비] '{roomName}' 생성 및 진입 성공!");
        else
            Debug.LogError($"[로비] 방 생성 실패: {result.ShutdownReason}");
    }

    public async void JoinRoom(string sessionName)
    {
        var sceneManager = Network.MyNetworkRoot.Instance.SceneManagerDefault;

        var result = await Network.MyNetworkRoot.Instance.Runner.StartGame(new StartGameArgs
        {
            GameMode = Fusion.GameMode.Shared, // 모드 명시
            SessionName = sessionName,
            SceneManager = sceneManager        // 씬 매니저 필수
        });

        if (!result.Ok)
        {
            Debug.LogError($"[로비] 방 입장 실패: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"[로비] 세션 '{sessionName}' 입장 성공!");
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        List<CRoomInfo> roomInfos = new List<CRoomInfo>();
        
        foreach (SessionInfo session in sessionList)
        {
            ERoomState roomState = ERoomState.Room;
            string propsRoomState = session.Properties["RoomState"];

            if (propsRoomState == "Room")
                roomState = ERoomState.Room;
            else if (propsRoomState == "InGame")
                roomState = ERoomState.InGame;
            else
                Debug.Assert(false);

            CRoomInfo roomInfo = new CRoomInfo
            {
                roomName = session.Properties["DisplayName"],
                playerCount = session.PlayerCount,
                maxPlayerCount = session.MaxPlayers,
                roomState = roomState,
                roomSession = session.Properties["RoomSession"]
            };

            roomInfos.Add(roomInfo);
        }

        ActionRoomChange(roomInfos);
    }

    private uint mRoomNumber = 0;

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
