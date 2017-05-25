using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Generateur : MonoBehaviour, IActivable {
    public bool On = true;
    public Machine machine;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        machine.GenerateurSwitch(On);
        animator.Play("On");
    }

	public void Switch()
    {
        if (On)
        {
            animator.Play("Click");
            On = false;
        }
        else
        {
            animator.Play("On");
            On = true;
        }
        machine.GenerateurSwitch(On);
    }

    public bool IsActivable(CannibalObject cannibal)
    {
        return true;
    }

    public void Activate(CannibalObject cannibal)
    {
        Switch();
    }

    public void ShowIcon()
    {
        //Debug.Log("ok");
    }
}
