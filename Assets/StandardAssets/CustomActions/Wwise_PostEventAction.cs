using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

public class Wwise_PostEventAction : ActionTask {

    [RequiredField]
    public BBParameter<GameObject> gameObject;
    public string eventName;

    protected override void OnExecute()
    {
        base.OnExecute();

        AkSoundEngine.PostEvent(eventName, gameObject.value);
        EndAction();
    }
}
