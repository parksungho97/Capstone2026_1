using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    private void Start()
    {
        bMove = true;
    }
    public bool Move(Vector3 moveDegree)
    {
        if (bMove == false)
            return false;

        if(moveDegree == Vector3.zero) 
            return false;

        transform.position += moveDegree * moveSpeed;

        return true;
    }

    public bool RotateTo(Vector3 lookDir, float turnCoeiff)
    {
        if (lookDir.sqrMagnitude <= 0.0001f) 
            return false;

        Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * turnCoeiff
        );

        return true;
    }

    public bool bMove { get; set; }
}
