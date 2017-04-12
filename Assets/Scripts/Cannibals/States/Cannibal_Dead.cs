using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Dead : Cannibal_State {



    /// <summary>
    /// Nothing to do if we try to kill a cannibal that is already dead
    /// </summary>
    /// <returns></returns>
    public override bool Kill()
    {
        return false;
    }

}
