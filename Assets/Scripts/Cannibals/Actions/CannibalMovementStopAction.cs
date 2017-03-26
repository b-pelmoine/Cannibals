using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class CannibalMovementStopAction : ActionTask<Cannibal> {

    protected override void OnExecute()
    {
        agent.CannibalMovement.Stop();
        EndAction();
    }


}
