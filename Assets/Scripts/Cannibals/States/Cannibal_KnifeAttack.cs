using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_KnifeAttack : Cannibal_State
{
    public BBParameter<iAI> aiDetected;


    protected override void OnEnter()
    {
        aiDetected.value.Kill();
    }
}
