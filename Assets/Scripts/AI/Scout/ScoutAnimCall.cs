using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAnimCall : MonoBehaviour {
    public AI.Scout scout;
    public AI.ScoutSimple scoutS;

	void Call()
    {
        if(scout)
            scout.AnimCall();
    }

    void FootSteps()
    {
        if(scout)
            AkSoundEngine.PostEvent("scout_steps", scout.gameObject);
        else
            AkSoundEngine.PostEvent("scout_steps", scoutS.gameObject);
    }
}
