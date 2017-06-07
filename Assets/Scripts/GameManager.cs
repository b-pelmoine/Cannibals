using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GameState
{
    MENU,
    PHASE_ONE,
    PHASE_TWO,
    END_GAME
}

public class GameManager : MonoBehaviour {
    SceneController sceneController;

    public static GameManager Instance;

    private Cannibal[] cannibals;
    private AI.Mamie granny;
    private Corpse corpse;

    public GameState state, prevState;

    bool endCondition;
    bool Reload_CR_Running = false;

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

    public void setEndConditionState(bool state)
    {
        endCondition = state;
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
            bool inMenu = sceneController.currentScene == 0;
            if (inMenu)
                state = (GameState)sceneController.state;
            else
            {
                if (state == GameState.MENU) state = GameState.PHASE_ONE;
            }
            if(state != prevState)
            {
                switch(state)
                {
                    case GameState.PHASE_ONE: OnPhaseOneStart(); break;
                    case GameState.PHASE_TWO: OnPhaseTwoStart(); break;
                    case GameState.END_GAME: OnPhaseTwoEnd();break;
                }
            }
            if (!inMenu)
            {
                if (Input.GetKeyDown(KeyCode.R) || AreBothCannibalsDead())
                {
                    if(!Reload_CR_Running)
                        StartCoroutine(reloadCurrentPhase());
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
        cannibals = GameObject.FindObjectsOfType<Cannibal>();
        granny = GameObject.FindObjectOfType<AI.Mamie>();

        AI.Chasseur.alert = false;
        Reload_CR_Running = false;
    }

    void PhaseOneUpdate()
    {
        if(granny)
        {
            if (granny.isDead())
            {
                if (GameObject.FindObjectOfType<Corpse>())
                {
                    state = GameState.PHASE_TWO;
                }
            }
        }
        else
        {
            if (GameObject.FindObjectOfType<Corpse>())
            {
                state = GameState.PHASE_TWO;
            }
        }
    }

    void OnPhaseTwoStart()
    {
        corpse = GameObject.FindObjectOfType<Corpse>();
        AkSoundEngine.PostEvent("ambiance_return", gameObject);
        setEndConditionState(false);
    }

    void PhaseTwoUpdate()
    {
        if (endCondition)
            state = GameState.END_GAME;
    }

    void OnPhaseTwoEnd()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        sceneController.displayEndGameScreen(true);
        yield return new WaitForSeconds(.5f);
        while (!Input.GetKeyDown("joystick button 0")) { yield return new WaitForEndOfFrame(); }
        yield return new WaitForSeconds(.5f);
        sceneController.displayEndGameScreen(false);
        state = GameState.MENU;
        sceneController.LoadScene(0);
    }

    bool AreBothCannibalsDead()
    {
        if (state == GameState.MENU || cannibals.Length == 0) return false;
        bool dead = true;
        foreach(var c in cannibals)
        {
            if(c)
            {
                if (!c.IsDead()) dead = false;
            }
        }
        return dead;
    }

    IEnumerator reloadCurrentPhase()
    {
        Reload_CR_Running = true;
        yield return new WaitForSeconds(3f);
        sceneController.LoadScene(1);
    }
}
