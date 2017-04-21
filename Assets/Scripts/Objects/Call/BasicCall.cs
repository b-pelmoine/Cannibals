using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Classe pour l'appeau
/// </summary>
public class BasicCall : CannibalObject, ICall {

    public static event Action<BasicCall> BasicCallUsed;

    [SerializeField]
    AudioSource m_audioSource;

    [SerializeField]
    Collider m_collider;

#if UNITY_EDITOR
    public bool debug = true;
#endif


    public void Use()
    {
        m_audioSource.Play();

        if (BasicCallUsed != null)
            BasicCallUsed(this);

#if UNITY_EDITOR
        if (debug)
        {
            Debug.Log("Appeau");
        }
#endif
    }

    public override void Take(Transform newParent)
    {
        base.Take(newParent);
        m_collider.enabled = false;
    }


    public override void Release()
    {
        base.Release();
        m_collider.enabled = true;
    }

}
