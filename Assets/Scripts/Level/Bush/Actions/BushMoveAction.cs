using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class BushMoveAction : ActionTask {

    public BBParameter<List<Bush>> bushes;
    public float duration = 1f;

    public bool moveOneTime = false;

    protected override void OnExecute()
    {
        base.OnExecute();

        foreach (Bush b in bushes.value)
        {
            b.Move(duration);
        }

        if (moveOneTime)
            EndAction();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        foreach (Bush b in bushes.value)
        {
            b.Move(duration);
        }
    }


}
