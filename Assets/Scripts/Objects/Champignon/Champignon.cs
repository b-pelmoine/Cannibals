using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champignon : CannibalObject, IDropable
{
    [SerializeField]
    Rigidbody m_rigidbody;

    [SerializeField]
    Collider m_collider;

    [SerializeField]
    Vector3 dropDirection = new Vector3(0, 1, 0);


    public enum Type
    {
        Champibon,
        Champoison
    }
    public Type type;


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
        m_rigidbody.isKinematic = false;
        m_collider.isTrigger = false;
    }

    public void Throw(float force, Vector3 normalizedDirection)
    {
        Release();
    }

}
