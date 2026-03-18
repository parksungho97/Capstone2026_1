using Fusion;
using Network;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayerTeam : byte
{
    Red,
    Blue,
}

// 1. INetworkStruct 상속 추가: "이 구조체는 메모리 크기가 고정된 네트워크용 데이터다!"
public struct MyPlayerContext : INetworkStruct
{
    public NetworkString<_16> name;
    public EPlayerTeam team;
    public bool bReady;
}

public class MyRoomController : NetworkBehaviour
{
    public Action ActionSpawned;

    public void Initalize(uint maxRedPlayerCount, uint maxBluePlayerCount)
    {
        this.maxRedPlayerCount = maxRedPlayerCount;
        this.maxBluePlayerCount = maxBluePlayerCount;
    }

    [Networked]
    public NetworkDictionary<int, MyPlayerContext> PlayerContexts => default;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddPlayerRPC(int playerId, NetworkString<_16> playerName)
    {
        if (PlayerContexts.ContainsKey(playerId))
            return;

        if (PlayerContexts.Count >= maxRedPlayerCount + maxBluePlayerCount)
            return;

        EPlayerTeam team = EPlayerTeam.Red;
        if (redPlayerCount >= maxRedPlayerCount)
        {
            team = EPlayerTeam.Blue;
            bluePlayerCount += 1;
        }
        else
        {
            team = EPlayerTeam.Red;
            redPlayerCount += 1;
        }

        MyPlayerContext newContext = new MyPlayerContext
        {
            team = team,
            name = playerName,
            bReady = false
        };

        PlayerContexts.Add(playerId, newContext);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReadyRPC(int playerId, bool bReady)
    {
        if(PlayerContexts.TryGet(playerId, out MyPlayerContext newContext))
        {
            if (newContext.bReady == bReady)
                return;

            newContext.bReady = bReady;
            PlayerContexts.Set(playerId, newContext);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ChangeTeamRPC(int playerId, EPlayerTeam team)
    {
        if (PlayerContexts.TryGet(playerId, out MyPlayerContext foundContext))
        {
            if (foundContext.team == team)
                return;

            if (foundContext.team == EPlayerTeam.Red)
            {
                if (bluePlayerCount < maxBluePlayerCount)
                {
                    foundContext.team = EPlayerTeam.Blue;
                    redPlayerCount -= 1;
                    bluePlayerCount += 1;
                    PlayerContexts.Set(playerId, foundContext);
                }
            }
            else
            {
                if (redPlayerCount < maxRedPlayerCount)
                {
                    foundContext.team = EPlayerTeam.Red;
                    redPlayerCount += 1;
                    bluePlayerCount -= 1;
                    PlayerContexts.Set(playerId, foundContext);
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void StartGameRpc(int sceneIndex)
    {
        foreach(var context in PlayerContexts)
        {
            if (context.Value.bReady == false)
                return;
        }

        Runner.LoadScene(SceneRef.FromIndex(sceneIndex), LoadSceneMode.Single);
    }

    public void Log()
    {
        foreach (var a in PlayerContexts)
        {
            Debug.Log($"name: {a.Value.name}, team: {a.Value.team}, ready: {a.Value.bReady}");
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        ActionSpawned();
    }

    private uint maxRedPlayerCount = 0;
    private uint maxBluePlayerCount = 0;

    private uint redPlayerCount = 0;
    private uint bluePlayerCount = 0;
}