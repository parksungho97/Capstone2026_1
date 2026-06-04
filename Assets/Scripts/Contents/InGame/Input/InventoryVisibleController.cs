using Fusion;
using UnityEngine;

public class InventoryVisibleController : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    public bool bUIOpen { get; private set; }

    private void Start()
    {
        Debug.Assert(inventoryUI, "InventoryUI가 연결되지 않았습니다.");

        inventoryUI.Visible(false);
        bUIOpen = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            bUIOpen = !bUIOpen;
            inventoryUI.Visible(bUIOpen);
        }
    }
}