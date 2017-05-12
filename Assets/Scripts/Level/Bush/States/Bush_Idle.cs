using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush_Idle : Bush_State
{
    public override bool Move()
    {
        FSM.SendEvent("Move");
        return true;
    }

    public override bool IsMoving()
    {
        return false;
    }
}
