using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class BushMoveAction : ActionTask {

    public BBParameter<List<Bush>> bushes;
    public bool playOneTime = false;

    protected override void OnExecute()
    {
        base.OnExecute();

        foreach (Bush b in bushes.value)
        {
            b.Move();
        }

        if (playOneTime)
            EndAction();
    }

    protected override void OnUpdate()
    {
        foreach(Bush b in bushes.value)
        {
            b.Move();
        }
    }

}
