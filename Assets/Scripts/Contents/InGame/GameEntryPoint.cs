using Fusion;
using Network;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Unity.VisualScripting;
using UnityEngine;

public class GameEntryPoint : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CapturePointManager capturePointManager;
    [SerializeField] private CGameMode gameMode;
    [SerializeField] private ViewContext viewContext;
    [SerializeField] private NetworkObject voices;
    [SerializeField] private PlayerStatUIController playerStatUIController;
    [SerializeField] private VoiceNPCStateManager voiceNPCStateManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private Inventory inventory;

    public override async void Spawned()
    {
        base.Spawned();
        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(playerController);
        Debug.Assert(capturePointManager);
        Debug.Assert(gameMode);
        Debug.Assert(viewContext);
        Debug.Assert(playerStatUIController);
        Debug.Assert(voiceNPCStateManager);

        gameMode.Initialize(capturePointManager);
        gameMode.ActionGameEnded += (EResultType resultType) =>
        {
            Debug.Log($"Game Ended! Result: {resultType}");
        };

        var newPlayer = await Runner.SpawnAsync(player, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);

        var newPlayerController = await Runner.SpawnAsync(playerController, position: Vector3.zero,
            rotation: Quaternion.identity,
            inputAuthority: Runner.LocalPlayer);
        newPlayerController.GetComponent<PlayerController>().Initalize(newPlayer.gameObject, cameraController);

        AudioListener audioListener = newPlayer.AddComponent<AudioListener>();
        audioListener.enabled = true;

        viewContext.Initalize(newPlayer.gameObject);
        
        PlayerHealth playerHealth = newPlayer.GetComponent<PlayerHealth>();
        playerStatUIController.Initialize(playerHealth);

        voiceNPCStateManager.AddTargetChaseState(newPlayer.gameObject);

        ItemCollector itemCollector = newPlayer.GetComponentInChildren<ItemCollector>();
        Debug.Assert(itemCollector != null, "ItemCollector가 없습니다.");
        itemCollector.Initialize(itemManager, inventory);
    }
}

