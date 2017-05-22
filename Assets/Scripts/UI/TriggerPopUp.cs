using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPopUp : MonoBehaviour {
    public string param = "Show";
	public void setBool(bool state)
    {
        GetComponent<Animator>().SetBool(param, state);
    }
}
