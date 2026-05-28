using UnityEngine;

public class PlayerCoordination : MonoBehaviour
{
    [SerializeField] private GameObject helmet;
    private void Start()
    {
        Debug.Assert(helmet);

        characterHealth = GetComponent<CharacterHealth>();
        Debug.Assert(characterHealth);
        equipmentSlot = GetComponent<EquipmentSlot>();
        Debug.Assert(equipmentSlot);
    }

    private void Update()
    {
        if(characterHealth.CurrentArmor == 0)
            helmet.SetActive(false);
        else
            helmet.SetActive(true);
    }

    private CharacterHealth characterHealth;
    private EquipmentSlot equipmentSlot;
}
