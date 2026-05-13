using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotKeyItemUI : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private Button button;

    private string itemName;
    private Sprite itemSprite;
    private bool hasItem;

    public Action<KeyCode> ActionClicked;

    public void Initialize()
    {
        Clear();

        if (keyText != null)
        {
            keyText.text = KeyCodeToDisplayText(keyCode);
        }
        else
        {
            Debug.LogWarning($"[HotKeyItemUI] keyText NULL / key: {keyCode}");
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickSlot);
        }
        else
        {
            Debug.LogError($"[HotKeyItemUI] button NULL / key: {keyCode}");
        }
    }

    public void SetDescription(string name, Sprite sprite)
    {
        itemName = name;
        itemSprite = sprite;
        hasItem = true;

        if (itemIcon != null)
        {
            itemIcon.sprite = sprite;
            itemIcon.enabled = sprite != null;
        }

        Debug.Log($"[HotKeyItemUI] 아이템 설정됨 / key: {keyCode}, item: {itemName}");
    }

    public void Clear()
    {
        itemName = string.Empty;
        itemSprite = null;
        hasItem = false;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
    }

    public KeyCode GetKeyCode()
    {
        return keyCode;
    }

    private void OnClickSlot()
    {
        ActionClicked?.Invoke(keyCode);
    }

    private string KeyCodeToDisplayText(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            default: return code.ToString();
        }
    }
}