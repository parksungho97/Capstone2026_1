using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualController : NetworkBehaviour
{
    [SerializeField] private GameObject helmet;
    public override void Spawned()
    {
        base.Spawned();
        characterHealth = GetComponent<CharacterHealth>();
        Debug.Assert(characterHealth);

        Debug.Assert(helmet);
    }

    private void Update()
    {
        if (characterHealth == null)
            return;

        if (characterHealth.CurrentArmor > 0)
            helmet.SetActive(true);
        else
            helmet.SetActive(false);
    }

    private CharacterHealth characterHealth;
}