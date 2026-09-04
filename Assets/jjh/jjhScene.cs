using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class jjhScene : NetworkBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private NetworkObject player;
    [SerializeField] private InputManager inputManager;

    public override async void Spawned()
    {
        base.Spawned();
        Debug.Log("GameScene");

        Debug.Assert(cameraController);
        Debug.Assert(player);


        var newPlayer = await Runner.SpawnAsync(player,
            position: new Vector3(0.0f, 0.0f, 0.0f),
            inputAuthority: Runner.LocalPlayer);

        cameraController.SetTarget(newPlayer.transform);
        newPlayer.GetComponent<PlayerController>().Initalize(cameraController);

        newPlayer.GetComponent<Rigidbody>().useGravity = false;
    }
}