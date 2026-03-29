using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField] private float angle;
    [SerializeField] private float radius;

    public float Angle => angle;
    public float Radius => radius;
}
