using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntryPoint : NetworkBehaviour
{
    [SerializeField] private RoomController roomController;
    [SerializeField] private RoomSession roomSession;
    [SerializeField] private int gameSceneIndex;
    [SerializeField] private int lobbySceneIndex;
    [SerializeField] private RoomPlayerStateManager roomPlayerStateManager;
    [SerializeField] private RoomTeamDashboardUI roomTeamDashboardUI;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button redTeamButton;
    [SerializeField] private Button blueTeamButton;

    public override void Spawned()
    {
        base.Spawned();

        Debug.Assert(roomController);
        Debug.Assert(roomSession);
        Debug.Assert(roomPlayerStateManager);
        Debug.Assert(roomTeamDashboardUI);
        Debug.Assert(readyButton);
        Debug.Assert(startButton);
        Debug.Assert(exitButton);

        roomSession.ActionPlayerExit += (int exitPlayerId) =>
        {
            roomController.RemovePlayerRPC(exitPlayerId);
        };
        roomSession.ActionRoomDestroy += () =>
        {
            roomSession.LeaveRoom(lobbySceneIndex);
        };

        roomController.Initalize(4, 4);
        roomPlayerStateManager.Initalize(roomController, roomSession.LocalPlayerId
            , roomSession.LocalPlayerId.ToString(), roomSession.IsHost);
        roomTeamDashboardUI.AddRedPlayer("sdf");
        roomTeamDashboardUI.AddRedPlayer("sdf1231");

        startButton.onClick.AddListener(() =>
        {
            roomSession.ChangeRoomState("InGame");
            roomController.StartGameRpc(gameSceneIndex);
        });
        readyButton.onClick.AddListener(() =>
        {
            roomPlayerStateManager.ReadyToggle();
        });
        exitButton.onClick.AddListener(() =>
        {
            roomPlayerStateManager.Exit();
            roomSession.LeaveRoom(lobbySceneIndex);
        });
        redTeamButton.onClick.AddListener(() =>
        {
            roomController.ChangeTeamRPC(roomSession.LocalPlayerId, EPlayerTeam.Red);
        });
        blueTeamButton.onClick.AddListener(() =>
        {
            roomController.ChangeTeamRPC(roomSession.LocalPlayerId, EPlayerTeam.Blue);
        });

        if (roomSession.IsHost)
        {
            startButton.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(false);
            readyButton.gameObject.SetActive(true);
        }
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
            roomController.ChangeTeamRPC(roomSession.LocalPlayerId, EPlayerTeam.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            roomController.ChangeTeamRPC(roomSession.LocalPlayerId, EPlayerTeam.Blue);


        else if (Input.GetKeyDown(KeyCode.Space))
            roomController.Log();
    }
}
