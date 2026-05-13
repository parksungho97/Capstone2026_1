using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;
    [SerializeField] private List<InventoryItemUI> slotUIList = new List<InventoryItemUI>();

    public Action<int> ActionSlotClicked;
    public Action ActionDisabled;

    private void Awake()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            int index = i;
            slotUIList[i].Initialize(index);
            slotUIList[i].ActionClicked += OnClickSlot;
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

        Debug.LogWarning("[InventoryUI] 빈 슬롯이 없습니다.");
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
            Debug.LogWarning($"[InventoryUI] 잘못된 인덱스: {index}");
            return false;
        }

        return true;
    }

    public void SwapItemUI(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        // 데이터 임시 저장
        (string name, Sprite sprite, int count, bool hasItem) tempA = (
            slotUIList[indexA].GetItemName(),
            slotUIList[indexA].GetItemSprite(),
            slotUIList[indexA].GetItemCount(),
            slotUIList[indexA].HasItem()
        );

        // A → B
        if (slotUIList[indexB].HasItem())
            slotUIList[indexA].SetItem(slotUIList[indexB].GetItemName(), slotUIList[indexB].GetItemSprite(), slotUIList[indexB].GetItemCount());
        else
            slotUIList[indexA].Clear();

        // B → A
        if (tempA.hasItem)
            slotUIList[indexB].SetItem(tempA.name, tempA.sprite, tempA.count);
        else
            slotUIList[indexB].Clear();
    }
}