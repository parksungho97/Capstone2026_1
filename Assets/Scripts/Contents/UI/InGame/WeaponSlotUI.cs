using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponNameText;

    [Header("Default Weapon")]
    [SerializeField] private string defaultWeaponName = "¼èÆÄÀÌÇÁ";
    [SerializeField] private Sprite defaultWeaponIcon;

    private string currentWeaponName;
    private Sprite currentWeaponIcon;

    private void Awake()
    {
        SetWeapon(defaultWeaponName, defaultWeaponIcon);
    }

    public void SetWeapon(string weaponName, Sprite weaponIconSprite)
    {
        currentWeaponName = weaponName;
        currentWeaponIcon = weaponIconSprite;

        if (weaponIcon != null)
        {
            weaponIcon.sprite = currentWeaponIcon;
            weaponIcon.enabled = currentWeaponIcon != null;
        }

        if (weaponNameText != null)
        {
            weaponNameText.text = currentWeaponName;
        }

        Debug.Log($"[WeaponSlotUI] ¹«±â ½½·Ô °»½Å: {currentWeaponName}");
    }

    public string GetCurrentWeaponName()
    {
        return currentWeaponName;
    }

    public Sprite GetCurrentWeaponIcon()
    {
        return currentWeaponIcon;
    }
}