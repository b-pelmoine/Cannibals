using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Classe pour l'appeau
/// </summary>
public class BasicCall : CannibalObject, ICall {

    [SerializeField]
    AudioSource m_audioSource;

    [SerializeField]
    Collider m_collider;

#if UNITY_EDITOR
    public bool debug = true;
    bool used = false;
    float timer = 0;
#endif


    public void Use()
    {
        m_audioSource.Play();

#if UNITY_EDITOR
        if(debug)
        {
            used = true;
            timer = Time.time + 5f;
        }
#endif
    }

    public override void BeTaken(Transform newParent)
    {
        base.BeTaken(newParent);
        m_collider.enabled = false;
    }

    public override void Exchange(CannibalObject with)
    {
        base.Exchange(with);
        m_collider.enabled = true;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (used)
        {
            Handles.Label(this.transform.position + new Vector3(0, 1, 0), "APPEAU");
           
            if(Time.time > timer)
                 used = false;
        }
    }
#endif

}
