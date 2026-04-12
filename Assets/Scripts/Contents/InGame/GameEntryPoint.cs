using Fusion;
using Network;
using UnityEngine;

public class GameEntryPoint : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CapturePointManager capturePointManager;
    [SerializeField] private CGameMode gameMode;

    public override async void Spawned()
    {
        base.Spawned();
        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(playerController);
        // Debug.Assert(capturePointManager);
        // Debug.Assert(gameMode);
        
        // capturePointManager.Initialize();
        // gameMode.Initialize(capturePointManager);
        // gameMode.ActionGameEnded += (EResultType resultType) =>
        // {
        //     Debug.Log($"Game Ended! Result: {resultType}");
        // };

        var newPlayer = await Runner.SpawnAsync(player, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);

        var newPlayerController = await Runner.SpawnAsync(playerController, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: Runner.LocalPlayer);
        newPlayerController.GetComponent<PlayerController>().Initalize(newPlayer.gameObject, cameraController);

        AudioListener audioListener = newPlayer.GetComponent<AudioListener>();
        Debug.Assert(audioListener);
        audioListener.enabled = true;
    }
}

