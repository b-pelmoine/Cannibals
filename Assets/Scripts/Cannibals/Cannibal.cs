using UnityEngine;
using NodeCanvas.StateMachines;

public class Cannibal : MonoBehaviour {

    [SerializeField]
    FSMOwner m_stateMachine;

    [Space(5)]
    public RewiredInput m_rewiredInput;

    [Space(20)]
    public Cannibal_Movement m_cannibalMovement;

    public Cannibal_Skill m_cannibalSkill;

    public Cannibal_Appearence m_cannibalAppearence;

    /// <summary>
    /// Knock out the cannibal.
    /// </summary>
    /// <returns>false if the cannibal can't be knock out for the moment</returns>
    public bool KnockOut() { return ((Cannibal_State)m_stateMachine.behaviour.currentState).KnockOut(); }

    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public bool Resuscitate() { return ((Cannibal_State)m_stateMachine.behaviour.currentState).Resuscitate(); }


    /// <summary>
    /// The cannibal will loose his object
    /// </summary>
    /// <returns>false if the cannibal has no object or don't want to loose it</returns>
    public bool LooseCannibalObject() { return ((Cannibal_State)m_stateMachine.behaviour.currentState).LooseCannibalObject(); }


    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be kill in the current state</returns>
    public bool Kill() { return ((Cannibal_State)m_stateMachine.behaviour.currentState).Kill(); }
}
