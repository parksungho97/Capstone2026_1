using Fusion;
using System;
using System.Collections.Generic;
using Fusion.Sockets;
using UnityEngine;

// 방(Room) 씬에서만 존재하며 런너의 기능을 추상화하는 매니저
public class RoomSession : NetworkBehaviour, INetworkRunnerCallbacks
{
    public Action<int> ActionPlayerExit;
    public Action ActionRoomDestroy;
    public Action ActionSceneLoadStart;

    public override void Spawned()
    {
        Runner.AddCallbacks(this);
    }

    private void OnDestroy()
    {
        if (Runner != null)
            Runner.RemoveCallbacks(this);
    }

    // 1. 내가 방장(Host)인지 확인
    public bool IsHost => Runner != null && Runner.IsSharedModeMasterClient;

    public int LocalPlayerId => Runner.LocalPlayer.PlayerId;

    // 방 나가기 — NetworkRoot에 위임하여 Runner를 파괴하지 않고 세션만 종료 후 씬 전환
    public async void LeaveRoom(int sceneIndex)
    {
        await Network.NetworkRoot.Instance.LeaveToScene(sceneIndex);
    }
    public void ChangeRoomState(string roomState)
    {
        Debug.Assert(roomState == "Room" || roomState == "InGame");
        Debug.Assert(Runner.SessionInfo.Properties.ContainsKey("RoomState"));

        var newProperties = new Dictionary<string, SessionProperty>();

        newProperties["RoomState"] = roomState;

        Runner.SessionInfo.UpdateCustomProperties(newProperties);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // runner 파라미터를 사용. this.Runner는 Despawn 중에 null이 될 수 있음.
        if (runner == null || runner.SessionInfo == null)
            return;

        if (!runner.SessionInfo.Properties.TryGetValue("MasterClientId", out SessionProperty masterClientIdProp))
            return;

        int masterClientId = masterClientIdProp;
        if (player.PlayerId == masterClientId)
            ActionRoomDestroy?.Invoke();
        else
            ActionPlayerExit?.Invoke(player.PlayerId);
    }

    // --- INetworkRunnerCallbacks implementation ---
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { ActionSceneLoadStart?.Invoke(); }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }
}
