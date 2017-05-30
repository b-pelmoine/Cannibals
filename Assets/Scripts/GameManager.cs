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
        state = prevState = GameState.MENU;
    }

    void Update()
    {
        if(sceneController)
        {
            if (state == GameState.MENU)
                state = (GameState)sceneController.state;
            else
            {
                if (Input.GetKeyDown(KeyCode.R)) sceneController.LoadScene(1);
            }
            if(state != prevState)
            {
                switch(state)
                {
                    case GameState.PHASE_ONE: OnPhaseOneStart(); break;
                    case GameState.PHASE_TWO: OnPhaseTwoStart(); break;
                }
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
}
