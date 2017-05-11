using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannibal_Resurrect : Cannibal_State {


    /// <summary>
    /// Nothing to do if we try to kill a cannibal that resurrect
    /// </summary>
    /// <returns></returns>
    public override bool Kill()
    {
        return false;
    }


}
