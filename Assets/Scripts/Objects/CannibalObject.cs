using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CannibalObject : MonoBehaviour {

    [SerializeField]
    Transform m_transform;

    public Transform Transform
    {
        get { return m_transform; }
    }

    /// <summary>
    /// The object is taken by the newParent
    /// </summary>
    /// <param name="newParent"></param>
    public virtual void BeTaken(Transform newParent)
    {
        m_transform.SetParent(newParent);
    }

    /// <summary>
    /// Exchange the CannibalObject c with the seconde CannibalObject
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="with"></param>
    public virtual void Exchange(CannibalObject with)
    {
        Vector3 memoryPosition = m_transform.position;
        m_transform.position = with.m_transform.position;
        with.m_transform.position = memoryPosition;

        Transform memoryParent = m_transform.parent;
        m_transform.SetParent(with.m_transform.parent);
        with.m_transform.SetParent(memoryParent);

        Quaternion memoryRotation = m_transform.rotation;
        m_transform.rotation = with.m_transform.rotation;
        with.m_transform.rotation = memoryRotation;
    }

}
