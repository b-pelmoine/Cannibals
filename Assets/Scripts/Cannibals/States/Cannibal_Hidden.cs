using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Cannibal_Hidden : Cannibal_State {


#if UNITY_EDITOR
    public bool debug = true;
#endif


    protected override void OnEnter()
    {
        base.OnEnter();
#if UNITY_EDITOR
        if (debug)
            MonoManager.current.onGUI += OnGUI;
 #endif
    }


    protected override void OnUpdate()
    {
        base.OnUpdate();
        cannibal.value.m_cannibalMovement.GroundMove(new Vector2(cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));
    }


    protected override void OnExit()
    {
        base.OnExit();

#if UNITY_EDITOR
        if (debug)
            MonoManager.current.onGUI -= OnGUI;
#endif
    }


#if UNITY_EDITOR
    void OnGUI()
    {
      Handles.Label(cannibal.value.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position + new Vector3(-1, 1, 0), "Hidden");
    }
#endif
}
