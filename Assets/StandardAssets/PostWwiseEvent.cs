using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEvent : MonoBehaviour {

    public string eventName;


    public void PostEvent()
    {
        AkSoundEngine.PostEvent(eventName, gameObject);
    }


}
