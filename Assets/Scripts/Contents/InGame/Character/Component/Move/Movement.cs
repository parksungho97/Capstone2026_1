using UnityEngine;

public class Movement
{
    public Movement(GameObject owner, float moveSpeed = 3.0f, float turnSpeed = 15.0f)
    {
        this.owner = owner;
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
        bMovePossible = true;
    }
    public bool Move(Vector3 moveDegree)
    {
        if (bMovePossible == false)
            return false;

        if(moveDegree == Vector3.zero) 
            return false;

        owner.transform.position += moveDegree * moveSpeed;

        return true;
    }

    public bool RotateTo(Vector3 lookDir, float turnCoeiff)
    {
        if (lookDir.sqrMagnitude <= 0.0001f) 
            return false;

        Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
        owner.transform.rotation = Quaternion.Slerp(
            owner.transform.rotation,
            targetRot,
            turnSpeed * turnCoeiff
        );

        return true;
    }

    private GameObject owner;
    private float moveSpeed = 3.0f;
    private float turnSpeed = 15.0f;
    public bool bMovePossible { get; set; }
}
