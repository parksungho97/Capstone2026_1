using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Movement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    public bool bMovePossible { get; set; } = true;
    public bool bRotatePossible {  get; set; } = true;
    public bool bAnyPossible {  get; set; } = true;

    [Networked] public Vector3 MoveDirection { get; private set; }
    [Networked] public Vector3 ViewDirection { get; private set; }

    private Vector3 mPendingMove;
    private Vector3 mPendingLookDir;
    private float mPendingTurnCoeff;

    public void Move(Vector3 degree)
    {
        mPendingMove += degree;
    }

    public void RotateTo(Vector3 lookDir, float coeff = 1.0f)
    {
        mPendingLookDir = lookDir;
        mPendingTurnCoeff = coeff;
    }

    public void SetMovePossible(bool possible)
    {
        bMovePossible = possible;
    }

    public void MoveUpdate(float dt)
    {
        if (bAnyPossible == false)
            return;

        if (bMovePossible && mPendingMove != Vector3.zero)
        {
            MoveDirection = mPendingMove.normalized;
            transform.position += mPendingMove * moveSpeed * dt;
        }
        else
        {
            MoveDirection = Vector3.zero;
        }

        mPendingMove = Vector3.zero;

        if (bRotatePossible && mPendingLookDir.sqrMagnitude > 0.0001f)
        {
            ViewDirection = mPendingLookDir.normalized;

            Quaternion targetRot = Quaternion.LookRotation(mPendingLookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * mPendingTurnCoeff * Time.deltaTime
            );
        }

        mPendingLookDir = Vector3.zero;
        mPendingTurnCoeff = 0f;
    }
}
