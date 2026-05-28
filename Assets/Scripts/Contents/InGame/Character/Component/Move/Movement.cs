using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    public bool bMovePossible { get; set; } = true;

    public Vector3 MoveDirection => mPendingMove;
    public Vector3 ViewDirection => mPendingLookDir;

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

    public void MoveUpdate()
    {
        if (bMovePossible && mPendingMove != Vector3.zero)
            transform.position += mPendingMove * moveSpeed * Time.deltaTime;
        mPendingMove = Vector3.zero;

        if (bMovePossible && mPendingLookDir.sqrMagnitude > 0.0001f)
        {
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