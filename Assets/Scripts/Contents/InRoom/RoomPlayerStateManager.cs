using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayerStateManager : NetworkBehaviour
{
    public void Initalize(RoomController roomController, int playerId, string playerName
        , bool bReady)
    {
        this.roomController = roomController;
        this.playerId = playerId;
        this.playerName = playerName; 
        this.bReady = bReady;
    }

    public void ReadyToggle()
    {
        bReady = !bReady;
        roomController.ReadyRPC(playerId, bReady);
    }
    //public void ChangeTeam(EPlayerTeam team)
    //{
    //    if (bReady == false)
    //        roomController.ChangeTeamRPC(playerId, team);
    //}
    public void Exit()
    {
        Debug.Log($"Player {playerId} Exit");
        roomController.RemovePlayerRPC(playerId);
    }

    private RoomController roomController;
    private int playerId;
    private string playerName;
    private bool bReady;

    public override void Spawned()
    {
        base.Spawned();
        Debug.Assert(roomController);

        roomController.AddPlayerRPC(playerId, playerName);
        roomController.ReadyRPC(playerId, bReady);
    }
}
