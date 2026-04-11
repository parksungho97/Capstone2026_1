using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;
    [SerializeField] private List<InventoryItemUI> slotUIList = new List<InventoryItemUI>();

    public Action<int> ActionSlotClicked;

    private void Awake()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            int index = i;
            slotUIList[i].Initialize(index);
            slotUIList[i].ActionClicked += OnClickSlot;
        }
    }

    public int AddItemUI(string name, Sprite sprite, int count)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (slotUIList[i].HasItem()) continue;

            slotUIList[i].SetItem(name, sprite, count);
            return i;
        }

        Debug.LogWarning("[InventoryUI] ºó ½½·ÔÀÌ ¾ø½À´Ï´Ù.");
        return -1;
    }

    public void UpdateItemUI(int index, string name, Sprite sprite, int count)
    {
        if (!IsValidIndex(index)) return;
        slotUIList[index].UpdateItem(name, sprite, count);
    }

    public void RemoveItem(int index)
    {
        if (!IsValidIndex(index)) return;
        slotUIList[index].Clear();
    }

    public void Visible(bool visible)
    {
        if (rootObject != null)
            rootObject.SetActive(visible);
        else
            gameObject.SetActive(visible);
    }

    private void OnClickSlot(int index)
    {
        ActionSlotClicked?.Invoke(index);
    }

    private bool IsValidIndex(int index)
    {
        if (index < 0 || index >= slotUIList.Count)
        {
            Debug.LogWarning($"[InventoryUI] Àß¸øµÈ ÀÎµ¦½º: {index}");
            return false;
        }

        return true;
    }
}