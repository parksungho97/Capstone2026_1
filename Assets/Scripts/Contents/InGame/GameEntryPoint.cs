using Fusion;
using Network;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CapturePointController[] capturePointControllers;
    [SerializeField] private ActivaterUIController[] activaterUIControllers;
    [SerializeField] private Image[] activateProgressImages;

    private async void Start()
    {
        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(playerController);
        Debug.Assert(activaterUIControllers.Count() == capturePointControllers.Count() && activaterUIControllers.Count() == activateProgressImages.Count());

        var newPlayer = await MyNetworkRoot.Instance.Runner.SpawnAsync(player, position: Vector3.zero,
        rotation: Quaternion.identity,
        inputAuthority: MyNetworkRoot.Instance.Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);

        var newPlayerController = await MyNetworkRoot.Instance.Runner.SpawnAsync(playerController, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: MyNetworkRoot.Instance.Runner.LocalPlayer);
        newPlayerController.GetComponent<PlayerController>().Initalize(newPlayer.gameObject, cameraController);

        AudioListener audioListener = newPlayer.GetComponent<AudioListener>();
        Debug.Assert(audioListener);
        audioListener.enabled = true;

        for(int i = 0;i< activaterUIControllers.Count();++i)
            activaterUIControllers[i].Initalize(capturePointControllers[i].Activater, activateProgressImages[i]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            capturePointControllers[0].Activater.StartActivateRpc(ERequestType.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            capturePointControllers[0].Activater.StartActivateRpc(ERequestType.Blue);
        else if (Input.GetKeyDown(KeyCode.E))
            capturePointControllers[0].Activater.StopActivateRpc(ERequestType.Red);
        else if (Input.GetKeyDown(KeyCode.R))
            capturePointControllers[0].Activater.StopActivateRpc(ERequestType.Blue);
    }
}
