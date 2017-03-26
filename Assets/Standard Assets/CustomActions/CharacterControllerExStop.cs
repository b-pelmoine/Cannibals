using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using EquilibreGames;

[Category("CharacterControllerEx")]
public class CharacterControllerExStop : ActionTask<CharacterControllerExt> {

    protected override void OnExecute()
    {
        agent.velocity.x = 0;
        agent.velocity.y = 0;
        agent.velocity.z = 0;
        agent.acceleration.x = 0;
        agent.acceleration.y = 0;
        agent.acceleration.z = 0;
    }


}
