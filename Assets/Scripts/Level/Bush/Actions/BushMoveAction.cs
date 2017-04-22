using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class BushMoveAction : ActionTask {

    public BBParameter<List<Bush>> bushes;

    protected override void OnUpdate()
    {
        foreach(Bush b in bushes.value)
        {
            b.Move();
        }

        EndAction();
    }

}
