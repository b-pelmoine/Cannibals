using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : CannibalObject, IDropable {

    [SerializeField]
    Rigidbody m_rigidbody;

    [SerializeField]
    Collider m_collider;

    [SerializeField]
    Vector3 dropDirection = new Vector3(0, 1, 0);
   
    public void Throw(float force , Vector3 normalizedDirection)
    {
        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;
        // m_rigidbody.AddForce(force*(normalizedDirection+ dropDirection).normalized, ForceMode.Impulse);
    }

    public override void Take(Transform newParent)
    {
        base.Take(newParent);

        m_rigidbody.isKinematic = true;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_collider.enabled = false;
    }

    public override void Release()
    {
        base.Release();
        m_rigidbody.isKinematic = true;
        m_collider.enabled = true;
    }

}
