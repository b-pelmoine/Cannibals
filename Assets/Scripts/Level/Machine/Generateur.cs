using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Generateur : MonoBehaviour, IActivable {
    public IconDisplayer icon;
    public bool On = true;
    public bool electric = false;
    public Machine machine;
    private Animator animator;

    public GameObject sparkle;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        machine.GenerateurSwitch(On);
        if (On)
        {
            animator.Play("ToOn");
            AkSoundEngine.PostEvent("generator", gameObject);
        }
    }

	public void Switch()
    {
        if (On)
        {
            animator.Play("ToIdle");
            On = false;
            AkSoundEngine.StopAll(gameObject);
        }
        else
        {
            animator.Play("ToOn");
            On = true;
            AkSoundEngine.PostEvent("generator", gameObject);
        }
        machine.GenerateurSwitch(On);
    }

    public bool IsActivable(CannibalObject cannibal)
    {
        if (electric)
            return false;
        else
            return true;
    }

    public void Activate(CannibalObject cannibal)
    {
        if(cannibal!=null && cannibal is Bottle)
        {
            electric = true;
            if(sparkle != null) Instantiate(sparkle,transform);
            cannibal.linkedCannibal.LooseCannibalObject();
            cannibal.gameObject.SetActive(false);
            if (On)
                Switch();
        }
        else
        {
            Switch();
        }
    }

    public void ShowIcon()
    {
        icon.Show();
    }
}
