using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum WeaponAmmoType
{
    Infinite,
    Limited
}

public class WeaponSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text ammoText;

    [Header("Default Weapon")]
    [SerializeField] private string defaultWeaponName = "¼èÆÄÀÌÇÁ";
    [SerializeField] private Sprite defaultWeaponIcon;

    private string currentWeaponName;
    private Sprite currentWeaponIcon;

    private WeaponAmmoType currentAmmoType;
    private int currentAmmo;
    private int maxAmmo;

    private void Awake()
    {
        SetWeapon(defaultWeaponName, defaultWeaponIcon, WeaponAmmoType.Infinite, 0, 0);
    }

    public void SetWeapon(
        string weaponName,
        Sprite weaponIconSprite,
        WeaponAmmoType ammoType,
        int currentAmmoCount,
        int maxAmmoCount
    )
    {
        currentWeaponName = weaponName;
        currentWeaponIcon = weaponIconSprite;
        currentAmmoType = ammoType;
        currentAmmo = currentAmmoCount;
        maxAmmo = maxAmmoCount;

        RefreshUI();

        Debug.Log($"[WeaponSlotUI] ¹«±â ÀåÂø: {currentWeaponName}");
    }

    public void UpdateAmmo(int currentAmmoCount, int maxAmmoCount)
    {
        currentAmmo = currentAmmoCount;
        maxAmmo = maxAmmoCount;

        RefreshAmmoText();
    }

    private void RefreshUI()
    {
        if (weaponIcon != null)
        {
            weaponIcon.sprite = currentWeaponIcon;
            weaponIcon.enabled = currentWeaponIcon != null;
        }

        RefreshAmmoText();
    }

    private void RefreshAmmoText()
    {
        if (ammoText == null) return;

        if (currentAmmoType == WeaponAmmoType.Infinite)
        {
            ammoText.text = "¡Ä";
        }
        else
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }

    public string GetCurrentWeaponName()
    {
        return currentWeaponName;
    }

    public Sprite GetCurrentWeaponIcon()
    {
        return currentWeaponIcon;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
}