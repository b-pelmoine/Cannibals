using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using EquilibreGames;

[Category("CharacterControllerEx")]
public class CharacterControllerExActiveGravity : ActionTask<CharacterControllerExt> {

    public bool activeGravity = true;

    protected override void OnExecute()
    {
        agent.gravity.isActive = activeGravity;
        EndAction();
    }
}
