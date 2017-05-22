using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mamie_AnimCallback : MonoBehaviour {
    public AI.Mamie mamie;
    //Callbacks des Animations
    void SonIdle()
    {
        AkSoundEngine.PostEvent("granny_idle", gameObject);
    }

    void Call()
    {
        mamie.Call();
    }
}
