using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Cannibal_KnifeAttack : Cannibal_State
{
    public BBParameter<iKillable> killable;


    protected override void OnEnter()
    {
        killable.value.Kill();
    }
}
