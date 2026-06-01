using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayerTeam : byte
{
    Red,
    Blue,
}
// 1. INetworkStruct 상속 추가: "이 구조체는 메모리 크기가 고정된 네트워크용 데이터다!"
public struct PlayerRoomContext : INetworkStruct
{
    public NetworkString<_16> name;
    public EPlayerTeam team;
    public bool bReady;
}

public class RoomController : NetworkBehaviour
{
    public void Initalize(uint maxRedPlayerCount, uint maxBluePlayerCount)
    {
        this.maxRedPlayerCount = maxRedPlayerCount;
        this.maxBluePlayerCount = maxBluePlayerCount;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RemovePlayerRPC(int playerId)
    {
        if (PlayerContexts.TryGet(playerId, out PlayerRoomContext foundContext))
        {
            // Remove the player context and adjust team counts
            PlayerContexts.Remove(playerId);

            if (foundContext.team == EPlayerTeam.Red)
            {
                if (RedPlayerCount > 0)
                    RedPlayerCount -= 1;
            }
            else
            {
                if (BluePlayerCount > 0)
                    BluePlayerCount -= 1;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddPlayerRPC(int playerId, NetworkString<_16> playerName)
    {
        // 이미 입장한 플레이어라면 중복 추가 방지
        if (PlayerContexts.ContainsKey(playerId))
            return;

        // 방 최대 정원을 초과하면 추가하지 않음
        if (PlayerContexts.Count >= maxRedPlayerCount + maxBluePlayerCount)
            return;

        EPlayerTeam assignedTeam;

        // Blue 팀을 우선 배정
        // 인원수가 같거나 Blue가 더 적으면 Blue 입장
        if (BluePlayerCount < maxBluePlayerCount &&
            (BluePlayerCount <= RedPlayerCount || RedPlayerCount >= maxRedPlayerCount))
        {
            assignedTeam = EPlayerTeam.Blue;
            BluePlayerCount += 1;
        }
        // Blue가 더 많으면 Red 입장
        else if (RedPlayerCount < maxRedPlayerCount)
        {
            assignedTeam = EPlayerTeam.Red;
            RedPlayerCount += 1;
        }
        else
        {
            Debug.LogWarning("[RoomController] 입장 가능한 팀이 없습니다.");
            return;
        }

        PlayerRoomContext newContext = new PlayerRoomContext
        {
            team = assignedTeam,
            name = playerName,
            bReady = false
        };

        PlayerContexts.Add(playerId, newContext);

        Debug.Log(
            $"[RoomController] 자동 팀 배정 완료 / " +
            $"PlayerId: {playerId}, Team: {assignedTeam}, " +
            $"Blue: {BluePlayerCount}, Red: {RedPlayerCount}"
        );
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ReadyRPC(int playerId, bool bReady)
    {
        if (PlayerContexts.TryGet(playerId, out PlayerRoomContext newContext))
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
        if (PlayerContexts.TryGet(playerId, out PlayerRoomContext foundContext))
        {
            if (foundContext.team == team)
                return;

            if (foundContext.team == EPlayerTeam.Red)
            {
                if (BluePlayerCount < maxBluePlayerCount)
                {
                    foundContext.team = EPlayerTeam.Blue;
                    RedPlayerCount -= 1;
                    BluePlayerCount += 1;
                    PlayerContexts.Set(playerId, foundContext);
                }
            }
            else
            {
                if (RedPlayerCount < maxRedPlayerCount)
                {
                    foundContext.team = EPlayerTeam.Red;
                    RedPlayerCount += 1;
                    BluePlayerCount -= 1;
                    PlayerContexts.Set(playerId, foundContext);
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void StartGameRpc(int sceneIndex)
    {
        foreach (var context in PlayerContexts)
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

    private void OnPlayerContextsChanged()
    {
        List<PlayerRoomContext> redPlayers = new List<PlayerRoomContext>();
        List<PlayerRoomContext> bluePlayers = new List<PlayerRoomContext>();

        foreach (var context in PlayerContexts)
        {
            if (context.Value.team == EPlayerTeam.Red)
                redPlayers.Add(context.Value);
            else
                bluePlayers.Add(context.Value);
        }

        ActionPlayerContextChanged?.Invoke(redPlayers, bluePlayers);
    }

    public Action<List<PlayerRoomContext>, List<PlayerRoomContext>> ActionPlayerContextChanged;

    [Networked]
    [Capacity(8)]
    [OnChangedRender(nameof(OnPlayerContextsChanged))]
    public NetworkDictionary<int, PlayerRoomContext> PlayerContexts => default;


    [Networked]
    private uint RedPlayerCount { get; set; }
    [Networked]
    private uint BluePlayerCount { get; set; }

    private uint maxRedPlayerCount = 0;
    private uint maxBluePlayerCount = 0;
}