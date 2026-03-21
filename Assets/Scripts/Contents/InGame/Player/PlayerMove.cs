using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public bool Move(Vector3 moveDegree)
    {
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

    private float moveSpeed = 3.0f;
    private float turnSpeed = 15.0f;
}
