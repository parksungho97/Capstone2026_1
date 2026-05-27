using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private PlayerAnimationNetworkState animationState;

    [Header("Weapon Objects")]
    [SerializeField] private GameObject pipeObject;
    [SerializeField] private GameObject pistolObject;
    [SerializeField] private GameObject shotgunObject;

    private int lastWeaponId = -999;

    private void Awake()
    {
        if (animationState == null)
            animationState = GetComponent<PlayerAnimationNetworkState>();
    }

    private void Update()
    {
        if (animationState == null)
            return;

        int weaponId = animationState.CurrentWeaponId;

        if (weaponId == lastWeaponId)
            return;

        lastWeaponId = weaponId;

        if (pipeObject != null)
            pipeObject.SetActive(weaponId == 0);

        if (pistolObject != null)
            pistolObject.SetActive(weaponId == 1);

        if (shotgunObject != null)
            shotgunObject.SetActive(weaponId == 2);
    }
}