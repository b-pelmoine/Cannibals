using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MENU,
    PHASE_ONE,
    PHASE_TWO
}

public class GameManager : MonoBehaviour {
    SceneController sceneController;

    public static GameManager Instance;

    private Cannibal[] cannibals;

    GameState state, prevState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        sceneController = GameObject.FindObjectOfType<SceneController>();
        if (sceneController.currentScene == 0)
            state = prevState = GameState.MENU;
        switch(state)
        {
            case GameState.PHASE_ONE: OnPhaseOneStart(); break;
            case GameState.PHASE_TWO: OnPhaseTwoStart(); break;
        }
    }

    void Update()
    {
        if(sceneController)
        {
            bool inMenu = state == GameState.MENU;
            if (inMenu)
                state = (GameState)sceneController.state;
            if(state != prevState)
            {
                switch(state)
                {
                    case GameState.PHASE_ONE: OnPhaseOneStart(); break;
                    case GameState.PHASE_TWO: OnPhaseTwoStart(); break;
                }
            }
            if (!inMenu)
            {
                if (Input.GetKeyDown(KeyCode.R) || AreBothCannibalsDead()) StartCoroutine(reloadCurrentPhase());
            }
            prevState = state;
        }
        switch(state)
        {
            case GameState.PHASE_ONE: PhaseOneUpdate(); break;
            case GameState.PHASE_TWO: PhaseTwoUpdate(); break;
        }
    }

    void OnPhaseOneStart()
    {
        cannibals = GameObject.FindObjectsOfType<Cannibal>();
    }

    void PhaseOneUpdate()
    {
        
    }

    void OnPhaseTwoStart()
    {

    }

    void PhaseTwoUpdate()
    {

    }

    bool AreBothCannibalsDead()
    {
        if (state == GameState.MENU || cannibals.Length == 0) return false;
        bool dead = true;
        foreach(var c in cannibals)
        {
            if (!c.IsDead()) dead = false;
        }
        return dead;
    }

    IEnumerator reloadCurrentPhase()
    {
        yield return new WaitForSeconds(3f);
        sceneController.LoadScene(1);
    }
}
