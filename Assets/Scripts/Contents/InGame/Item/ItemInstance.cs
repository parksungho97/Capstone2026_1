using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private int itemId;
    [SerializeField] private int itemCount;

    public int ItemId {  get { return itemId; } }
    public int Count { get { return itemCount; } }
}
