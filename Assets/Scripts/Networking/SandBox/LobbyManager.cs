using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public Action<List<SessionInfo>> ActionRoomChange;

    public async void Initalize()
    {
        Network.MyNetworkRoot.Instance.Runner.AddCallbacks(this);

        // 3. 광장 접속 (비동기 대기). 
        // 이 순간부터 포톤 서버가 이 클라이언트에게 방 목록을 쏴주기 시작합니다.
        StartGameResult result = await Network.MyNetworkRoot.Instance.Runner.JoinSessionLobby(SessionLobby.Shared);

        Debug.Assert(result.Ok);

        Debug.Log("[로비] 접속 성공! 이제 서버가 방 목록을 갱신해 주기를 대기합니다.");
    }

    public async void JoinOrCreateRoom(string roomName, int sceneIndex)
    {
        Debug.Log($"[로비] '{roomName}' 방으로 진입을 시도합니다...");

        var sceneManager = Network.MyNetworkRoot.Instance.SceneManagerDefault;

        // StartGameArgs는 C++의 접속 설정 Struct라고 보시면 됩니다.
        var result = await Network.MyNetworkRoot.Instance.Runner.StartGame(new StartGameArgs
        {
            GameMode = Fusion.GameMode.Shared,       // 포톤 서버가 방을 관리하는 모드
            SessionName = roomName,           // 입장하거나 생성할 방의 고유 이름
            Scene = SceneRef.FromIndex(sceneIndex),
            SceneManager = sceneManager
        });

        if (result.Ok)
            Debug.Log($"[로비] '{roomName}' 진입 성공! 이제부터 NB 동기화가 시작됩니다.");
        else
            Debug.LogError($"[로비] 진입 실패: {result.ShutdownReason}");
    }

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

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        ActionRoomChange(sessionList);
    }
}
