using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Classe pour l'appeau
/// </summary>
public class BasicCall : CannibalObject, ICall {

    public static event Action<BasicCall> OnBasicCallUsed;

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
        if(!AkSoundEngine.GetIsGameObjectActive(gameObject))
            AkSoundEngine.PostEvent("duck", gameObject);

        if (OnBasicCallUsed != null)
            OnBasicCallUsed(this);

#if UNITY_EDITOR
        if (debug)
        {
            //Debug.Log("Appeau");
        }
#endif
    }

    public override void Take(Cannibal c, Transform newParent)
    {
        base.Take(c, newParent);
        m_collider.enabled = false;
    }


    public override void Release()
    {
        base.Release();
        m_collider.enabled = true;
    }

}
