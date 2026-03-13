using UnityEngine;

public class RoomEntryPoint : MonoBehaviour
{
    [SerializeField] private MyRoomController roomController;
    [SerializeField] private Network.RoomSession roomSession;
    [SerializeField] private int nextSceneIndex;
    private void Start()
    {
        roomController.Initalize(4, 4);
        roomController.ActionSpawned += () =>
        {
            roomController.AddPlayerRPC(Network.MyNetworkRoot.Instance.GetLocalPlayerId(), Network.MyNetworkRoot.Instance.GetLocalPlayerId().ToString());
        };
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
                roomController.StartGameRPC(nextSceneIndex);
        }
    }
}
