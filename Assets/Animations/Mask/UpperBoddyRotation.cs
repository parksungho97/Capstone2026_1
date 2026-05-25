using UnityEngine;

public class UpperBodyAttackRotationFix : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAnimationState animationState;

    [Header("Rotation Fix")]
    [SerializeField] private HumanBodyBones upperBodyBone = HumanBodyBones.Chest;
    [SerializeField] private Vector3 localEulerOffset = new Vector3(0f, 90f, 0f);
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
            animationState = GetComponent<PlayerAnimationState>();

        upperBodyTransform = animator.GetBoneTransform(upperBodyBone);
        neckTransform = animator.GetBoneTransform(HumanBodyBones.Neck);
        headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void LateUpdate()
    {
        if (animator == null || animationState == null || upperBodyTransform == null)
            return;

        if (animationState.CurrentWeapon == 0)
            return;

        float layerWeight = animator.GetLayerWeight(upperBodyLayerIndex);

        if (layerWeight <= 0.01f)
            return;

        if (neckTransform != null)
            neckOriginalRotation = neckTransform.rotation;

        if (headTransform != null)
            headOriginalRotation = headTransform.rotation;

        Quaternion offsetRotation = Quaternion.Euler(localEulerOffset);

        Quaternion blendedOffset = Quaternion.Slerp(
            Quaternion.identity,
            offsetRotation,
            layerWeight
        );

        upperBodyTransform.localRotation =
            upperBodyTransform.localRotation * blendedOffset;

        if (neckTransform != null)
            neckTransform.rotation = neckOriginalRotation;

        if (headTransform != null)
            headTransform.rotation = headOriginalRotation;
    }
}