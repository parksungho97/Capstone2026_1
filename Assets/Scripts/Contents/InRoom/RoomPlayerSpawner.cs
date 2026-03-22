using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayerSpawner : NetworkBehaviour
{
    public void Initalize(RoomController roomController, int playerId, string playerName)
    {
        this.roomController = roomController;
        this.playerId = playerId;
        this.playerName = playerName; 
    }
    private RoomController roomController;
    private int playerId;
    private string playerName;

    public override void Spawned()
    {
        base.Spawned();
        Debug.Assert(roomController);

        roomController.AddPlayerRPC(playerId, playerName);
    }
}
