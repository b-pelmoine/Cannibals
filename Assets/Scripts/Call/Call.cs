using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Classe pour l'appeau
/// </summary>
public class Call : MonoBehaviour {

    [SerializeField]
    AudioSource m_audioSource;


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
