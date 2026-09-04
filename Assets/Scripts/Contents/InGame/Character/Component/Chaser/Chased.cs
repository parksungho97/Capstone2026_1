using System;
using UnityEngine;

public class Chased : MonoBehaviour
{
    public event Action OnBecameInactive;

    private bool isActive = true;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value) return;
            isActive = value;
            if (!isActive)
                OnBecameInactive?.Invoke();
        }
    }
}
