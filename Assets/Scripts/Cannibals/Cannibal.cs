using UnityEngine;
using NodeCanvas.StateMachines;

public class Cannibal : MonoBehaviour {

    [SerializeField]
    FSMOwner m_stateMachine;

    [Space(5)]
    public RewiredInput m_rewiredInput;

    [Space(20)]
    [SerializeField]
    Cannibal_Movement m_cannibalMovement;

    public Cannibal_Movement CannibalMovement
    {
        get { return m_cannibalMovement; }
    }

    public Call m_call;

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
