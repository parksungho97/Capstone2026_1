using UnityEngine;

public class UpperBodyAttackRotationFix : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationNetworkState animationState;

    [Header("Rotation Fix")]
    [SerializeField] private HumanBodyBones upperBodyBone = HumanBodyBones.Chest;
    [SerializeField] private Vector3 localEulerOffset = new Vector3(0f, -70f, 0f);
    [SerializeField] private int upperBodyLayerIndex = 1;

    private Transform upperBodyTransform;
    private Transform neckTransform;
    private Transform headTransform;

    private Quaternion neckOriginalRotation;
    private Quaternion headOriginalRotation;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animationState == null)
            animationState = GetComponent<PlayerAnimationNetworkState>();

        upperBodyTransform = animator.GetBoneTransform(upperBodyBone);
        neckTransform = animator.GetBoneTransform(HumanBodyBones.Neck);
        headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void LateUpdate()
    {
        if (animator == null || animationState == null || upperBodyTransform == null)
            return;

        WeaponType weapon = GetWeaponType(animationState.CurrentWeaponId);

        // Pipe는 회전 보정 제외
        if (weapon == WeaponType.Pipe)
            return;

        float layerWeight = animator.GetLayerWeight(upperBodyLayerIndex);

        if (layerWeight <= 0.01f)
            return;

        if (neckTransform != null)
            neckOriginalRotation = neckTransform.rotation;

        if (headTransform != null)
            headOriginalRotation = headTransform.rotation;

        Quaternion targetRotation =
    transform.rotation * Quaternion.Euler(localEulerOffset);

        upperBodyTransform.rotation = Quaternion.Slerp(
            upperBodyTransform.rotation,
            targetRotation,
            layerWeight
        );

        if (neckTransform != null)
            neckTransform.rotation = neckOriginalRotation;

        if (headTransform != null)
            headTransform.rotation = headOriginalRotation;
    }

    private enum WeaponType
    {
        Pipe = 0,
        Pistol = 1,
        ShotGun = 3
    }

    private WeaponType GetWeaponType(int weaponId)
    {
        switch (weaponId)
        {
            case 0:
                return WeaponType.Pipe;

            case 1:
                return WeaponType.Pistol;

            case 3:
                return WeaponType.ShotGun;

            default:
                return WeaponType.Pipe;
        }
    }
}