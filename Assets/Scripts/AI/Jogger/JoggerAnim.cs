using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoggerAnim : MonoBehaviour {
    public GameObject obj;

	public void FootSteps()
    {
        AkSoundEngine.PostEvent("jogger_steps", obj);
    }
}
