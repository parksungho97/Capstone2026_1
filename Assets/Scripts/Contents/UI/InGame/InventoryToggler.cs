using Fusion;
using UnityEngine;

public class InventoryToggler : NetworkBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    private bool isOpen = false;

    private void Start()
    {
        Debug.Assert(inventoryUI, "InventoryUI가 연결되지 않았습니다.");

        inventoryUI.Visible(false);
        isOpen = false;
    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Object.AssignInputAuthority(Runner.LocalPlayer);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput<NetworkInputData>(out NetworkInputData data))
        {
            if (data.buttons.WasPressed(previousButtons, EInputButton.I))
            {
                isOpen = !isOpen;
                inventoryUI.Visible(isOpen);
            }
            previousButtons = data.buttons;
        }
    }

    [Networked] private NetworkButtons previousButtons { get; set; }
}