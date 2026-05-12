using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button button;

    private string itemName;
    private Sprite itemSprite;
    private int itemCount;
    private int slotIndex;
    private bool hasItem;

    public Action<int> ActionClicked;

    public void Initialize(int index)
    {
        Debug.Log($"[InventoryItemUI] Initialize 호출됨 / index: {index}");

        slotIndex = index;
        Clear();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickSlot);

            Debug.Log($"[InventoryItemUI] Button onClick 연결 성공 / index: {slotIndex}");
        }
        else
        {
            Debug.LogError($"[InventoryItemUI] button NULL / index: {slotIndex}");
        }
    }

    public void SetItem(string name, Sprite sprite, int count)
    {
        itemName = name;
        itemSprite = sprite;
        itemCount = count;
        hasItem = true;

        if (itemIcon != null)
        {
            itemIcon.sprite = sprite;
            itemIcon.enabled = sprite != null;
        }

        if (countText != null)
        {
            countText.text = count >= 1 ? $"x{count}" : "";
        }

        Debug.Log($"[InventoryItemUI] 아이템 설정됨 / index: {slotIndex}, item: {itemName}, count: {itemCount}");
    }

    public void UpdateItem(string name, Sprite sprite, int count)
    {
        SetItem(name, sprite, count);
    }

    public void Clear()
    {
        itemName = string.Empty;
        itemSprite = null;
        itemCount = 0;
        hasItem = false;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (countText != null)
        {
            countText.text = "";
        }
    }

    public bool HasItem()
    {
        return hasItem;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public Sprite GetItemSprite()
    {
        return itemSprite;
    }

    public int GetItemCount()
    {
        return itemCount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[InventoryItemUI] OnPointerClick 감지됨 / index: {slotIndex}");

        // Button.onClick이 안 잡히는지 확인하기 위한 직접 호출
        OnClickSlot();
    }

    private void OnClickSlot()
    {
        Debug.Log($"[InventoryItemUI] OnClickSlot 호출됨 / index: {slotIndex}");

        if (!hasItem)
        {
            Debug.LogWarning($"[InventoryItemUI] 슬롯 {slotIndex} 아이템 없음");
            return;
        }

        Debug.Log($"[InventoryItemUI] ActionClicked Invoke 호출 / index: {slotIndex}");

        if (ActionClicked == null)
        {
            Debug.LogWarning($"[InventoryItemUI] ActionClicked 구독자 없음 / index: {slotIndex}");
        }

        ActionClicked?.Invoke(slotIndex);
    }
}