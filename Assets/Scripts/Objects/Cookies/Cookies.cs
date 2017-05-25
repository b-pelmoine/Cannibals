using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cookies : CannibalObject
{
    [SerializeField]
    public Rigidbody m_rigidbody;

    [SerializeField]
    Collider m_collider;

    public override void Take(Cannibal c, Transform newParent)
    {
        base.Take(c, newParent);

        m_rigidbody.isKinematic = true;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_collider.isTrigger = true;
    }

    public override void Release()
    {
        base.Release();
        m_rigidbody.isKinematic = true;
        m_collider.isTrigger = false;
    }
}
