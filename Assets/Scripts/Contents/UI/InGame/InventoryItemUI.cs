using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
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
        slotIndex = index;
        Clear();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickSlot);
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
            countText.text = count > 1 ? $"x{count}" : "";
        }
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

    private void OnClickSlot()
    {
        if (!hasItem) return;
        ActionClicked?.Invoke(slotIndex);
    }
}