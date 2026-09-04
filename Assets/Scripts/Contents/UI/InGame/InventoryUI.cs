using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;
    [SerializeField] private List<InventoryItemUI> slotUIList = new List<InventoryItemUI>();

    public Action<int> ActionSlotClicked;
    public Action<int> ActionSlotDoubleClicked;
    public Action ActionDisabled;
    public Action OnBecameVisible;

    private void Awake()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            int index = i;

            if (slotUIList[i] == null)
            {
                Debug.LogWarning($"[InventoryUI] slotUIList[{i}]�� ����ֽ��ϴ�.");
                continue;
            }

            slotUIList[i].Initialize(index);
            slotUIList[i].ActionClicked -= OnClickSlot;
            slotUIList[i].ActionClicked += OnClickSlot;
            slotUIList[i].ActionDoubleClicked -= OnDoubleClickSlot;
            slotUIList[i].ActionDoubleClicked += OnDoubleClickSlot;
        }
    }

    private void OnDisable()
    {
        ActionDisabled?.Invoke();
    }

    public int AddItemUI(string name, Sprite sprite, int count)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (slotUIList[i].HasItem()) continue;

            slotUIList[i].SetItem(name, sprite, count);
            return i;
        }

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

        if (visible)
        {
            RefreshAllSlots();
            OnBecameVisible?.Invoke();
        }
    }

    public void RefreshAllSlots()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (!IsValidIndex(i)) continue;

            InventoryItemUI slot = slotUIList[i];

            if (slot.HasItem())
            {
                slot.SetItem(
                    slot.GetItemName(),
                    slot.GetItemSprite(),
                    slot.GetItemCount()
                );
            }
            else
            {
                slot.Clear();
            }
        }
    }

    private void OnClickSlot(int index)
    {
        ActionSlotClicked?.Invoke(index);
    }

    private void OnDoubleClickSlot(int index)
    {
        ActionSlotDoubleClicked?.Invoke(index);
    }

    private bool IsValidIndex(int index)
    {
        if (index < 0 || index >= slotUIList.Count)
        {
            Debug.LogWarning($"[InventoryUI] �߸��� �ε���: {index}");
            return false;
        }

        if (slotUIList[index] == null)
        {
            Debug.LogWarning($"[InventoryUI] slotUIList[{index}]�� ����ֽ��ϴ�.");
            return false;
        }

        return true;
    }

    public void SwapItemUI(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        (string name, Sprite sprite, int count, bool hasItem) tempA = (
            slotUIList[indexA].GetItemName(),
            slotUIList[indexA].GetItemSprite(),
            slotUIList[indexA].GetItemCount(),
            slotUIList[indexA].HasItem()
        );

        if (slotUIList[indexB].HasItem())
        {
            slotUIList[indexA].SetItem(
                slotUIList[indexB].GetItemName(),
                slotUIList[indexB].GetItemSprite(),
                slotUIList[indexB].GetItemCount()
            );
        }
        else
        {
            slotUIList[indexA].Clear();
        }

        if (tempA.hasItem)
        {
            slotUIList[indexB].SetItem(tempA.name, tempA.sprite, tempA.count);
        }
        else
        {
            slotUIList[indexB].Clear();
        }

        RefreshAllSlots();
    }
}