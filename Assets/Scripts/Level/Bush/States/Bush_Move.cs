using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush_Move : Bush_State
{

    public override bool Move()
    {
        return false;
    }

    public override bool IsMoving()
    {
        return true;
    }
}
