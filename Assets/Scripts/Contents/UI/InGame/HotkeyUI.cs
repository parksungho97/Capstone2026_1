using System;
using System.Collections.Generic;
using UnityEngine;

public class HotKeyUI : MonoBehaviour
{
    [SerializeField] private List<HotKeyItemUI> hotKeyItemUIList = new List<HotKeyItemUI>();

    public Action<KeyCode> ActionHotKeyClicked;

    public void Initialize(int keyCount, KeyCode[] keyCodes)
    {
        for (int i = 0; i < hotKeyItemUIList.Count; i++)
        {
            if (hotKeyItemUIList[i] == null)
            {
                Debug.LogError($"[HotKeyUI] hotKeyItemUIList[{i}]가 비어있음");
                continue;
            }

            hotKeyItemUIList[i].Initialize();
            hotKeyItemUIList[i].ActionClicked -= OnClickHotKey;
            hotKeyItemUIList[i].ActionClicked += OnClickHotKey;
        }
    }

    public void SetDescription(KeyCode keyCode, string name, Sprite sprite)
    {
        HotKeyItemUI target = FindHotKeyItem(keyCode);

        if (target == null)
        {
            Debug.LogWarning($"[HotKeyUI] 해당 KeyCode를 찾을 수 없습니다: {keyCode}");
            return;
        }

        target.SetDescription(name, sprite);
        Debug.Log($"[HotKeyUI] 핫키 아이템 설정 / key: {keyCode}, item: {name}");
    }

    public void RemoveHotKeyUI(KeyCode keyCode)
    {
        HotKeyItemUI target = FindHotKeyItem(keyCode);

        if (target == null)
        {
            Debug.LogWarning($"[HotKeyUI] 해당 KeyCode를 찾을 수 없습니다: {keyCode}");
            return;
        }

        target.Clear();
        Debug.Log($"[HotKeyUI] 핫키 아이템 제거 / key: {keyCode}");
    }

    private HotKeyItemUI FindHotKeyItem(KeyCode keyCode)
    {
        for (int i = 0; i < hotKeyItemUIList.Count; i++)
        {
            if (hotKeyItemUIList[i] == null) continue;

            if (hotKeyItemUIList[i].GetKeyCode() == keyCode)
            {
                return hotKeyItemUIList[i];
            }
        }

        return null;
    }

    private void OnClickHotKey(KeyCode keyCode)
    {
        ActionHotKeyClicked?.Invoke(keyCode);
    }
}