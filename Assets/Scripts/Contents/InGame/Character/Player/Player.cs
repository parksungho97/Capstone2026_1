using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int defaultWeaponId = 0;
    private void Start()
    {
        equipmentComponent = GetComponent<EquipmentComponent>();
        Debug.Assert(equipmentComponent);

        Debug.Assert(EquipmentManager.Instance.TryGet(defaultWeaponId, out EquipmentData weaponData));
        Weapon pipe = weaponData.Generate() as Weapon;
        if (pipe != null)
            equipmentComponent.SetWeapon(pipe);
        else
            Debug.Log("DefaultWeapon Is Null");

        magazine = GetComponent<Magazine>();
        magazine.SetAttackInstance(1);
    }

    private EquipmentComponent equipmentComponent;
    private Magazine magazine;
}
