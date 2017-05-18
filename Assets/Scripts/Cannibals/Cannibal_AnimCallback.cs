using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_AnimCallback : MonoBehaviour {
    public GameObject targetObject;
	void FootSteps()
    {
        AkSoundEngine.PostEvent("cannibals_run", targetObject);
    }
}
