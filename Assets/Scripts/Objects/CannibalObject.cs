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
    /// Exchange the CannibalObject c with the seconde CannibalObject
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="with"></param>
    public static void Exchange(CannibalObject c, CannibalObject with)
    {
        Vector3 memoryPosition = c.m_transform.position;
        c.m_transform.position = with.m_transform.position;
        with.m_transform.position = memoryPosition;

        Transform memoryParent = c.m_transform.parent;
        c.m_transform.SetParent(with.m_transform.parent);
        with.m_transform.SetParent(memoryParent);

        Quaternion memoryRotation = c.m_transform.rotation;
        c.m_transform.rotation = with.m_transform.rotation;
        with.m_transform.rotation = memoryRotation;

    }

}
