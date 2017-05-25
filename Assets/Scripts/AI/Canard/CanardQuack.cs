using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanardQuack : MonoBehaviour {
    public GameObject target;
	void Quack()
    {
        AkSoundEngine.PostEvent("duck", target);
    }
}
