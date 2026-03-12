using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntryPoint : MonoBehaviour
{
    [SerializeField] private MyRoomController roomController;
    [SerializeField] private RoomSession roomSession;
    private void Start()
    {
        roomController.Initalize(4, 4);
        roomController.ActionSpawned += () =>
        {
            roomController.AddPlayerRPC(MyNetworkRoot.Instance.GetLocalPlayerId(), MyNetworkRoot.Instance.GetLocalPlayerId().ToString());
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            roomController.ChangeTeamRPC(MyNetworkRoot.Instance.GetLocalPlayerId(), EPlayerTeam.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            roomController.ChangeTeamRPC(MyNetworkRoot.Instance.GetLocalPlayerId(), EPlayerTeam.Blue);
        else if (Input.GetKeyDown(KeyCode.E))
            roomController.Log();
        else if (Input.GetKeyDown(KeyCode.S))
        {
            roomController.StartGameRPC(8);
        }
    }
}
