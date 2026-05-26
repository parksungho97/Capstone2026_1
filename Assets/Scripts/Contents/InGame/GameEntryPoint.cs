using Fusion;
using Network;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GameEntryPoint : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CGameMode gameMode;
    [SerializeField] private ViewContext viewContext;
    [SerializeField] private NetworkObject voices;
    [SerializeField] private PlayerStatUIController playerStatUIController;

    [Header("Spawn")]
    [SerializeField] private PlayerSpawnPointManager spawnPointManager;
    [SerializeField] private NetworkTimerClock timer;
    [SerializeField] private float minute = 15;

    [Header("Item")]
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private InventoryUIMapper inventoryUIMapper;

    public override async void Spawned()
    {
        base.Spawned();

        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);
        Debug.Assert(gameMode);
        Debug.Assert(viewContext);
        Debug.Assert(playerStatUIController);
        Debug.Assert(spawnPointManager);
        Debug.Assert(inventoryUIMapper);

        gameMode.Initialize(timer);
        gameMode.ActionGameEnded += (EResultType resultType) =>
        {
            Debug.Log($"Game Ended! Result: {resultType}");
        };
        
        Transform spawnPoint = spawnPointManager.GetRandomSpawnPoint();

        var newPlayer = await Runner.SpawnAsync(player, position: spawnPoint.position,
            rotation: spawnPoint.rotation,
            inputAuthority: Runner.LocalPlayer);
        cameraController.SetTarget(newPlayer.transform);
        newPlayer.GetComponent<PlayerController>().Initalize(cameraController);

        InventoryController inventoryController = newPlayer.GetComponent<InventoryController>();
        Debug.Assert(inventoryController);
        inventoryUIMapper.LinkInventoryController(inventoryController);


        AudioListener audioListener = newPlayer.AddComponent<AudioListener>();
        audioListener.enabled = true;

        viewContext.Initalize(newPlayer.gameObject);

        CharacterHealth playerHealth = newPlayer.GetComponent<CharacterHealth>();
        playerStatUIController.Initialize(playerHealth);

        if (Object.HasStateAuthority)
            StartCoroutine(WaitAndStartTimer());
    }

    private IEnumerator WaitAndStartTimer()
    {
        if (timer == null)
        {
            Debug.LogError("Timer 레퍼런스가 인스펙터에 할당되지 않았습니다!");
            yield break;
        }

        while (!timer.Object || !timer.Object.IsValid)
        {
            yield return null;
        }

        // 3. 아주 짧은 프레임 대기 (Fusion의 내부 상태 전파를 위해)
        yield return new WaitForSeconds(0.1f);

        timer.SetMinutes(minute);
    }
}