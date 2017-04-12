using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class BushMoveAction : ActionTask<ColliderKeeper> {


    protected override void OnUpdate()
    {
        if(agent.gameObjects != null)
        {
            foreach(GameObject g in agent.gameObjects)
            {
                Bush b = g.GetComponent<Bush>();

                if(b)
                {
                    b.Move();
                }
            }
        }

        EndAction();
    }

}
