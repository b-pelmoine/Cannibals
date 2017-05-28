using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChienAnimEvent : MonoBehaviour {
    public AI.AIChien chien;

	void FootSteps()
    {
        //AkSoundEngine.PostEvent("dog_steps", chien.gameObject);
    }

    void Bark()
    {
        AkSoundEngine.PostEvent("dog_bark", chien.gameObject);
    }

    void Sniff()
    {
        AkSoundEngine.PostEvent("dog_sniff", chien.gameObject);
    }

    void Idle()
    {
        AkSoundEngine.PostEvent("dog_idle", chien.gameObject);
    }

    void Eat()
    {
        AkSoundEngine.PostEvent("dog_eat", chien.gameObject);
    }
}
