﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderReceiverMono : MonoBehaviour
{
    public UnitIdentifierMono unit;

    private SphereCollider m_Col;

    private void Start()
    {
        unit = transform.root.GetComponent<UnitIdentifierMono>(); 
        m_Col = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(unit.UnitRef.IsAttacking)
        {
            if(other.CompareTag("DamageableCollider")) // We hit the correct collider
            {
                UnitIdentifierMono otherUnitRef = other.transform.root.GetComponent<UnitIdentifierMono>();
                if(otherUnitRef == null) return;
                if(otherUnitRef.UnitID == unit.UnitID) return; // Don't want to be hitting ourselves

                otherUnitRef.UnitRef.SpawnSmoke(transform.position);
                otherUnitRef.UnitRef.DecreaseHealthBy(unit.UnitRef.GetDamage());
                AudioManager.Instance.PlaySFX(DataManager.Data.ActualHitGroup.GetRandomClip());
                AudioManager.Instance.PlaySFX(DataManager.Data.ActualGruntGroup.GetRandomClip());
            }
        }
    }
}
