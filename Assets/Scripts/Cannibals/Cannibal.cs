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
    /// Knock out the cannibal.
    /// </summary>
    /// <returns>false if the cannibal can't be knock out for the moment</returns>
    public bool KnockOut() { return CurrentState().KnockOut(); }

    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public bool Resurrect() { return CurrentState().Resurrect(); }


    /// <summary>
    /// The cannibal will loose his object
    /// </summary>
    /// <returns>false if the cannibal has no object or don't want to loose it</returns>
    public bool LooseCannibalObject() { return CurrentState().LooseCannibalObject(); }


    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be killed in the current state</returns>
    public bool Kill() {
        return CurrentState().Kill(); }


    /// <summary>
    /// Revive the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be revived in the current state</returns>
    public bool Revive() { return CurrentState().Revive(); }

    public bool IsDead()
    {
        return CurrentState().IsDead();
    }


    Cannibal_State CurrentState()
    {
        FSMState currentSate = m_stateMachine.behaviour.currentState;

        while (currentSate is NestedFSMState)
        {
            currentSate = ((NestedFSMState)currentSate).nestedFSM.currentState;
        }

        return (Cannibal_State)currentSate;
    }
}
