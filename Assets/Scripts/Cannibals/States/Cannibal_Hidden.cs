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
        m_cannibal.m_cannibalMovement.GroundMove(new Vector2(m_cannibal.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.m_rewiredInput.m_playerInput.GetAxis("FrontMove")));

        Vector3 orientationDirection = m_cannibal.m_cannibalMovement.CharacterControllerEx.velocity;
        orientationDirection.y = 0;
        m_cannibal.m_cannibalAppearence.Orientate(orientationDirection);
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
