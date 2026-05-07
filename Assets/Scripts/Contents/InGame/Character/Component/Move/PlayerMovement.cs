using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    private void Start()
    {
        movement = new Movement(gameObject, moveSpeed, turnSpeed);
        bMove = false;
    }

    public void Move(Vector3 moveDegree)
    {
        bMove = movement.Move(moveDegree);
    }

    public void RotateTo(Vector3 lookDir, float coeff)
    {
        movement.RotateTo(lookDir, coeff);
    }

    public void SetMovePossible(bool bMovePossible)
    {
        movement.bMovePossible = bMovePossible;
    }

    public void MoveEnd() { bMove = false; }

    private Movement movement;
    public bool bMove { get; private set;  }
}
