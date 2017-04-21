using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class PNJ : MonoBehaviour{
    public bool isVulnerable = false;
    public GraphOwner owner;


    void Kill() {
        owner.SendEvent("Kill");
    }

    void Assomer()
    {
        owner.SendEvent("Assomer");
    }

}
