using UnityEngine;
using NodeCanvas.StateMachines;
using System.Collections.Generic;

public class Cannibal : MonoBehaviour {

    public static List<Cannibal> cannibals = new List<Cannibal>();

    [SerializeField]
    FSMOwner m_stateMachine;

    [Space(5)]
    public RewiredInput m_rewiredInput;

    [Space(20)]
    public Cannibal_Movement m_cannibalMovement;

    public Cannibal_Skill m_cannibalSkill;

    public Cannibal_Appearence m_cannibalAppearence;


    void Awake()
    {
        cannibals.Add(this);
    }

    void OnDestroy()
    {
        cannibals.Remove(this);
    }

    public static Vector3 BarycentricCannibalPosition()
    {
        Vector3 position = Vector3.zero;

        foreach(Cannibal c in cannibals)
        {
            position += c.m_cannibalMovement.CharacterControllerEx.CharacterTransform.position;
        }

        position /= cannibals.Count;

        return position;
    }


    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public bool Resurrect() { return CurrentState().Resurrect(); }


    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be killed in the current state</returns>
    public bool Kill() {
        return CurrentState().Kill(); }



    public bool IsDead()
    {
        return CurrentState().IsDead();
    }


    ICannibal_State CurrentState()
    {
        FSMState currentSate = m_stateMachine.behaviour.currentState;

        while (currentSate is NestedFSMState)
        {
            currentSate = ((NestedFSMState)currentSate).nestedFSM.currentState;
        }

        return (ICannibal_State)currentSate;
    }
}
