using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannibalScriptTest : MonoBehaviour {


    public Cannibal cannibal;

    public void Kill()
    {
        cannibal.Kill();
    }

    public void Resuscitate()
    {
        cannibal.Resurrect();
    }

}
