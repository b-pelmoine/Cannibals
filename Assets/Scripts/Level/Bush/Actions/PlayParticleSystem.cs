using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class PlayParticleSystem : ActionTask {

    public BBParameter<ParticleSystem> leaves;

    protected override void OnUpdate()
    {
        leaves.value.Play();

        EndAction();
    }
}
