using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum EInputButton
{
    W = 0,
    S = 1,
    A = 2,
    D = 3,
    Space = 4,
    I = 5,
    End,
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons; // 점프, 공격 등 버튼 상태 저장용
    public Vector3 mousePosition;
}

public class InputManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public override void Spawned()
    {
        base.Spawned();
        Runner.AddCallbacks(this);

    }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.buttons.Set(EInputButton.W, true);
        if (Input.GetKey(KeyCode.S))
            data.buttons.Set(EInputButton.S, true);
        if (Input.GetKey(KeyCode.A))
            data.buttons.Set(EInputButton.A, true);
        if (Input.GetKey(KeyCode.D))
            data.buttons.Set(EInputButton.D, true);
        if (Input.GetKey(KeyCode.Space))
            data.buttons.Set(EInputButton.Space, true);
        if (Input.GetKey(KeyCode.I))
            data.buttons.Set(EInputButton.I, true);
        data.mousePosition = Input.mousePosition;

        input.Set(data); // Fusion 시스템에 입력을 전달!

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

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {

    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
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
