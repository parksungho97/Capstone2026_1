using Fusion;
using Network;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private Activater activater;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private MyPlayerController playerController;
    [SerializeField] private InputManager inputManager;

    private async void Start()
    {
        Debug.Log("GameScene");

        Debug.Assert(activater);
        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(playerController);

        var newPlayer = await MyNetworkRoot.Instance.Runner.SpawnAsync(player, position: Vector3.zero,
        rotation: Quaternion.identity,
        inputAuthority: MyNetworkRoot.Instance.Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);

        var newPlayerController = await MyNetworkRoot.Instance.Runner.SpawnAsync(playerController, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: MyNetworkRoot.Instance.Runner.LocalPlayer);
        newPlayerController.GetComponent<MyPlayerController>().Initalize(newPlayer.gameObject, cameraController);

        AudioListener audioListener = newPlayer.GetComponent<AudioListener>();
        Debug.Assert(audioListener);
        audioListener.enabled = true;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //    activater.StartActivateRpc(ERequestType.Red);
        //else if (Input.GetKeyDown(KeyCode.W))
        //    activater.StartActivateRpc(ERequestType.Blue);
        //else if (Input.GetKeyDown(KeyCode.E))
        //    activater.StopActivateRpc(ERequestType.Red);
        //else if (Input.GetKeyDown(KeyCode.R))
        //    activater.StopActivateRpc(ERequestType.Blue);
    }
}
