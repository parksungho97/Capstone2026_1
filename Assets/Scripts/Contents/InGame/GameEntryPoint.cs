using Fusion;
using Network;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameEntryPoint : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private CGameMode gameMode;
    [SerializeField] private ViewContext viewContext;
    [SerializeField] private NetworkObject voices;
    [SerializeField] private PlayerStatUIController playerStatUIController;

    //추가
    [SerializeField] private RespawnUIController respawnUIController;

    [Header("Spawn")]
    [SerializeField] private PlayerSpawnPointManager spawnPointManager;
    [SerializeField] private NetworkTimerClock timer;
    [SerializeField] private float minute = 15;

    [Header("Item")]
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private InventoryUIMapper inventoryUIMapper;
    [SerializeField] private WeaponSlotUIBinder weaponSlotUIBinder;

    [SerializeField] private ResultUIController resultUIController;

    [Networked] private int readyPlayerCount { get; set; }

    private PlayerController localPlayerController;

    [Networked, Capacity(8)]
    private NetworkDictionary<int, int> spawnAssignments => default;

    public override void Spawned()
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
        Debug.Assert(respawnUIController);

        gameMode.Initialize(timer);
        gameMode.ActionGameEnded += (EResultType resultType) =>
        {
            Debug.Log($"[CGameMode] Result: {resultType}");

            if (localPlayerController != null)
                localPlayerController.bInputDisabled = true;

            EPlayerTeam localTeam = TeamInfo.Instance.GetTeam(Runner.LocalPlayer.PlayerId);
            bool isVictory = (resultType == EResultType.Red  && localTeam == EPlayerTeam.Red)
                          || (resultType == EResultType.Blue && localTeam == EPlayerTeam.Blue);
            resultUIController.ShowResult(isVictory);

            Network.NetworkRoot.Instance.StartCoroutine(LeaveAfterDelay(5f));
        };

        RPC_ReportReady(Runner.LocalPlayer.PlayerId);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ReportReady(int clientId)
    {
        readyPlayerCount++;
        if (readyPlayerCount < TeamInfo.Instance.All.Count) return;

        var clientIds = TeamInfo.Instance.All.Keys.OrderBy(id => id).ToList();
        int[] indices = Enumerable.Range(0, clientIds.Count).ToArray();

        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        for (int i = 0; i < clientIds.Count; i++)
            spawnAssignments.Set(clientIds[i], indices[i]);

        RPC_StartGame();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartGame()
    {
        if (!spawnAssignments.TryGet(Runner.LocalPlayer.PlayerId, out int spawnIndex))
        {
            Debug.LogError($"[GameEntryPoint] No spawn assignment for player {Runner.LocalPlayer.PlayerId}");
            return;
        }

        SpawnPlayerAsync(spawnPointManager.GetSpawnPointByIndex(spawnIndex));

        if (Object.HasStateAuthority)
            StartCoroutine(WaitAndStartTimer());
    }

    private async void SpawnPlayerAsync(Transform spawnPoint)
    {
        var newPlayer = await Runner.SpawnAsync(player, position: spawnPoint.position,
            rotation: spawnPoint.rotation,
            inputAuthority: Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);
        viewContext.Initalize(newPlayer.gameObject);
        localPlayerController = newPlayer.GetComponent<PlayerController>();
        localPlayerController.Initalize(cameraController);

        InventoryController inventoryController = newPlayer.GetComponent<InventoryController>();
        Debug.Assert(inventoryController);
        inventoryUIMapper.LinkInventoryController(inventoryController);

        if (weaponSlotUIBinder != null)
            weaponSlotUIBinder.Link(newPlayer.GetComponent<EquipmentSlot>());

        CharacterHealth playerHealth = newPlayer.GetComponent<CharacterHealth>();
        playerStatUIController.Initialize(playerHealth);

        Respawn respawn = newPlayer.GetComponent<Respawn>();
        Debug.Assert(respawn);
        respawnUIController.Initialize(respawn);
    }

    private IEnumerator LeaveAfterDelay(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        _ = NetworkRoot.Instance.LeaveToLobby();
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

        yield return new WaitForSeconds(0.1f);

        timer.SetMinutes(minute);
    }
}
