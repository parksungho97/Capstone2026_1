using Fusion;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private Activater activater;

    private void Start()
    {
        Debug.Log("GameScene");

        Debug.Assert(activater);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            activater.StartActivateRpc(ERequestType.Red);
        else if (Input.GetKeyDown(KeyCode.W))
            activater.StartActivateRpc(ERequestType.Blue);
        else if (Input.GetKeyDown(KeyCode.E))
            activater.StopActivateRpc(ERequestType.Red);
        else if (Input.GetKeyDown(KeyCode.R))
            activater.StopActivateRpc(ERequestType.Blue);
    }
}
