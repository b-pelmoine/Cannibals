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

}
