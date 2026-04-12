using UnityEngine;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private bool isOpen = false;

    private void Start()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI가 연결되지 않았습니다.");
            return;
        }

        inventoryUI.Visible(false);
        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryUI가 연결되지 않았습니다.");
            return;
        }

        isOpen = !isOpen;
        inventoryUI.Visible(isOpen);
    }
}