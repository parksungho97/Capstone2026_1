using Fusion;
using Network;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryPoint : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CapturePointController[] capturePointControllers;
    [SerializeField] private ActivaterUIController[] activaterUIControllers;
    [SerializeField] private Image[] activateProgressImages;

    public override async void Spawned()
    {
        base.Spawned();
        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(playerController);
        Debug.Assert(activaterUIControllers.Count() == capturePointControllers.Count() && activaterUIControllers.Count() == activateProgressImages.Count());

        var newPlayer = await Runner.SpawnAsync(player, position: Vector3.zero,
        rotation: Quaternion.identity,
        inputAuthority: Runner.LocalPlayer);
        playerInstance = newPlayer;

        cameraController.SetTarget(newPlayer.transform);

        var newPlayerController = await Runner.SpawnAsync(playerController, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: Runner.LocalPlayer);
        newPlayerController.GetComponent<PlayerController>().Initalize(newPlayer.gameObject, cameraController);

        AudioListener audioListener = newPlayer.GetComponent<AudioListener>();
        Debug.Assert(audioListener);
        audioListener.enabled = true;

        for (int i = 0; i < activaterUIControllers.Count(); ++i)
            activaterUIControllers[i].Initalize(capturePointControllers[i].Activater, activateProgressImages[i]);
    }

    private void Update()
    {
        if (playerInstance == null)
            return;

        ObjectId objectId = new ObjectId(playerInstance.gameObject);
        if (Input.GetKeyDown(KeyCode.Q))
            capturePointControllers[0].Activater.StartActivateRpc(objectId, ERequestType.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            capturePointControllers[0].Activater.StartActivateRpc(objectId, ERequestType.Blue);
        else if (Input.GetKeyDown(KeyCode.E))
            capturePointControllers[0].Activater.StopActivateRpc(objectId);
    }

    private NetworkObject playerInstance = null;
}
