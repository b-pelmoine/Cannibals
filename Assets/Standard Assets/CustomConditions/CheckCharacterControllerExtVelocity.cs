using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using EquilibreGames;

public class CheckCharacterControllerExtVelocity : ConditionTask<CharacterControllerExt> {

    public BBParameter<float> minVelocity;

    protected override bool OnCheck()
    {
        return agent.velocity.magnitude >= minVelocity.value;
    }
}
