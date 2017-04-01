using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steak : CannibalObject, IDropable {

    [SerializeField]
    Rigidbody m_rigidbody;

    public void Throw(Vector3 force)
    {
        m_rigidbody.isKinematic = false;
        m_rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public override void BeTaken(Transform newParent)
    {
        base.BeTaken(newParent);

        m_rigidbody.isKinematic = true;
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
    }

}
