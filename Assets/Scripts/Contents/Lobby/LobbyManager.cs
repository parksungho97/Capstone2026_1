using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
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

    private uint mRoomNumber = 0;

    private void Start()
    {
        Debug.Assert(Network.NetworkRoot.Instance != null, "[LobbyManager] NetworkRoot가 존재하지 않습니다.");
        Network.NetworkRoot.Instance.Runner.AddCallbacks(this);
        JoinLobby();
    }

    private void OnDestroy()
    {
        if (Network.NetworkRoot.Instance != null && Network.NetworkRoot.Instance.Runner != null)
            Network.NetworkRoot.Instance.Runner.RemoveCallbacks(this);
    }

    private async void JoinLobby()
    {
        StartGameResult result = await Network.NetworkRoot.Instance.Runner.JoinSessionLobby(SessionLobby.Shared);
        if (result.Ok)
            Debug.Log("[로비] 접속 성공!");
        else
            Debug.LogError($"[로비] JoinSessionLobby 실패: {result.ShutdownReason}");
    }

    public async void CreateAndJoinRoom(string roomName, int maxPlayerCount, int sceneIndex)
    {
        NetworkRunner runner = Network.NetworkRoot.Instance.Runner;
        NetworkSceneManagerDefault sceneManager = Network.NetworkRoot.Instance.SceneManagerDefault;

        string sessionName = $"ROOM_{mRoomNumber++}_{Guid.NewGuid().ToString().Substring(0, 5)}";

        var customProps = new Dictionary<string, SessionProperty>
        {
            ["RoomSession"] = sessionName,
            ["DisplayName"] = roomName,
            ["RoomState"] = "Room",
            ["MasterClientId"] = -1
        };

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneIndex),
            SceneManager = sceneManager,
            PlayerCount = maxPlayerCount,
            SessionProperties = customProps
        });

        if (result.Ok)
        {
            int myId = runner.LocalPlayer.PlayerId;
            var newProps = new Dictionary<string, SessionProperty>(runner.SessionInfo.Properties)
            {
                ["MasterClientId"] = myId
            };
            runner.SessionInfo.UpdateCustomProperties(newProps);
            Debug.Log($"[로비] '{roomName}' 생성 성공! 방장 ID: {myId}");
        }
        else
            Debug.LogError($"[로비] 방 생성 실패: {result.ShutdownReason}");
    }

    public async void JoinRoom(string sessionName)
    {
        NetworkRunner runner = Network.NetworkRoot.Instance.Runner;
        NetworkSceneManagerDefault sceneManager = Network.NetworkRoot.Instance.SceneManagerDefault;

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            SceneManager = sceneManager
        });

        if (result.Ok)
            Debug.Log($"[로비] 세션 '{sessionName}' 입장 성공!");
        else
            Debug.LogError($"[로비] 방 입장 실패: {result.ShutdownReason}");
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        List<CRoomInfo> roomInfos = new List<CRoomInfo>();

        foreach (SessionInfo session in sessionList)
        {
            if (session.PlayerCount <= 0 || !session.IsOpen)
                continue;

            string propsRoomState = session.Properties["RoomState"];
            ERoomState roomState;

            if (propsRoomState == "Room")
                roomState = ERoomState.Room;
            else if (propsRoomState == "InGame")
                roomState = ERoomState.InGame;
            else
            {
                Debug.Assert(false, $"[LobbyManager] 알 수 없는 RoomState: {propsRoomState}");
                continue;
            }

            roomInfos.Add(new CRoomInfo
            {
                roomName = session.Properties["DisplayName"],
                playerCount = session.PlayerCount,
                maxPlayerCount = session.MaxPlayers,
                roomState = roomState,
                roomSession = session.Properties["RoomSession"]
            });
        }

        ActionRoomChange?.Invoke(roomInfos);
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
}
