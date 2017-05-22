using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Generateur : MonoBehaviour {
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
}
