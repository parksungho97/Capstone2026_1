using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomEntryPoint : MonoBehaviour
{
    [SerializeField] private RoomController roomController;
    [SerializeField] private Network.RoomSession roomSession;
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private RoomPlayerSpawner roomPlayerSpawner;
    [SerializeField] private RoomTeamDashboardUI roomTeamDashboardUI;
    private void Start()
    {
        Debug.Assert(roomController);
        Debug.Assert(roomSession);
        Debug.Assert(roomPlayerSpawner);
        Debug.Assert(roomTeamDashboardUI);

        roomController.Initalize(4, 4);
        roomPlayerSpawner.Initalize(roomController, Network.MyNetworkRoot.Instance.GetLocalPlayerId(), Network.MyNetworkRoot.Instance.GetLocalPlayerId().ToString());
        roomTeamDashboardUI.AddRedPlayer("sdf");
        roomTeamDashboardUI.AddRedPlayer("sdf1231");
        //roomController.ActionPlayerContextChanged += (List<MyPlayerContext> redPlayers, List<MyPlayerContext> bluePlayers) =>
        //{
        //    roomTeamDashboardUI.ClearAll();
        //    foreach (MyPlayerContext redPlayerContext in redPlayers)
        //    {
        //        roomTeamDashboardUI.AddRedPlayer(redPlayerContext.name.ToString());
        //    }
        //    foreach (MyPlayerContext bluePlayerContext in bluePlayers)
        //    {
        //        roomTeamDashboardUI.AddRedPlayer(bluePlayerContext.name.ToString());
        //    }
        //};
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            roomController.ChangeTeamRPC(Network.MyNetworkRoot.Instance.GetLocalPlayerId(), EPlayerTeam.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            roomController.ChangeTeamRPC(Network.MyNetworkRoot.Instance.GetLocalPlayerId(), EPlayerTeam.Blue);

        else if (Input.GetKeyDown(KeyCode.E))
            roomController.ReadyRPC(Network.MyNetworkRoot.Instance.GetLocalPlayerId(), true);
        else if (Input.GetKeyDown(KeyCode.R))
            roomController.ReadyRPC(Network.MyNetworkRoot.Instance.GetLocalPlayerId(), false);

        else if (Input.GetKeyDown(KeyCode.Space))
            roomController.Log();
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (roomSession.IsHost)
            {
                roomSession.ChangeRoomState("InGame");
                roomController.StartGameRpc(nextSceneIndex);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
            roomSession.LeaveRoom(6);
    }
}
