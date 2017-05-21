using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestsManager : MonoBehaviour {

    public GameObject phaseTwoData;
    private Transform posPOne, posPTwo, posCorpse;

    public Texture splashScreen;

    public ParticleSystem endPhase;

    [SerializeField]
    Cannibal[] cannibals;

    bool endGame = false;

    bool firstScreen = true;

    public GameObject[] cannibalsPositions;
    public GameObject corpse;

    Dictionary<string, bool> cannibalsInTheEnd;
    [Space]
    bool targetReachedTheExit;

    [System.Serializable]
    public enum GamePhase
    {
        PHASE_ONE,
        PHASE_TWO
    }

    public GameObject AISetPhaseOne;
    public GameObject AISetPhaseTwo;

    public GamePhase phase;

    public bool colliderEnabled;
    public Terrain terrainUsingTrees;

    private static PlaytestsManager instance = null;

    // Game Instance Singleton
    public static PlaytestsManager Instance
    {
        get
        {
            return instance;
        }
    }

    bool loading;

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        loading = false;

        targetReachedTheExit = false;
        cannibalsInTheEnd = new Dictionary<string, bool>();

        terrainUsingTrees = GameObject.Find("TerrainPlaytests").GetComponent<Terrain>();

        AISetPhaseOne = terrainUsingTrees.transform.Find("AI_01").gameObject;
        AISetPhaseTwo = terrainUsingTrees.transform.Find("AI_02").gameObject;

        corpse = terrainUsingTrees.transform.Find("Corpse").gameObject;
        endPhase = GameObject.Find("endPS").GetComponent<ParticleSystem>();
        phaseTwoData = terrainUsingTrees.transform.Find("Playtest_DataPhase2").gameObject;

        cannibals = GameObject.FindObjectsOfType<Cannibal>();
        cannibalsPositions[0] = cannibals[0].transform.Find("NotStatic").gameObject;
        cannibalsPositions[1] = cannibals[1].transform.Find("NotStatic").gameObject;

        //init both cannibals
        for (int i = 0; i < 2; ++i)
        {
            RewiredInput cIn = cannibals[i].GetComponent<RewiredInput>();
            cannibalsInTheEnd[cIn.playerInputID + cIn.number] = false;
        }

        if(phase != GamePhase.PHASE_TWO)
            phase = GamePhase.PHASE_ONE;

        posPOne = phaseTwoData.transform.Find("pOnePhase2").transform;
        posPTwo = phaseTwoData.transform.Find("pTwoPhase2").transform;
        posCorpse = phaseTwoData.transform.Find("CorpsePhase2").transform;

        if (phaseTwoData.activeInHierarchy)
            phaseTwoData.SetActive(false);


        /*switch (phase)
        {
            case GamePhase.PHASE_ONE: AISetPhaseOne.SetActive(true); AISetPhaseTwo.SetActive(false) ;break;
            case GamePhase.PHASE_TWO: AISetPhaseOne.SetActive(false); AISetPhaseTwo.SetActive(true) ; break;
        }*/

        //corpse.transform.position = GameObject.Find("startCorpse").transform.position;

        //if (phase == GamePhase.PHASE_TWO) LoadPhaseTwo();
    }

    /*void LoadPhaseTwo()
    {
        if (!phaseTwoData.activeInHierarchy)
            phaseTwoData.SetActive(true);

        AISetPhaseOne.SetActive(false); AISetPhaseTwo.SetActive(true);

        cannibalsPositions[0].transform.position = posPOne.position;
        cannibalsPositions[1].transform.position = posPTwo.position;
        corpse.transform.position = posCorpse.position;

        Camera.main.transform.position = cannibalsPositions[0].transform.position;

        loading = false;

        if(phase == GamePhase.PHASE_ONE)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Playtest_Infiltration");

        if (phase != GamePhase.PHASE_TWO)
        {
            phase = GamePhase.PHASE_TWO;
        }

        targetReachedTheExit = false;
    }*/

    // c = colliders / l= load / r= restart
    void Update() {

        if(firstScreen)
        {
            if(Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0) || Time.time > 5f)
                firstScreen = false;
            return;
        }

        if (!terrainUsingTrees) Start();

        if(loading) return;
        
        if (!BothCannibalsAreDead())
        {
            if (targetReachedTheExit)
            {
                loading = true;
                if (!endPhase.isPlaying)
                    endPhase.Play();
                StartCoroutine(startNextPhase());
                if(phase == GamePhase.PHASE_TWO)
                {
                    endGame = true;
                }
            }
        }
        else
        {
            //both dead
            loading = true;
            StartCoroutine(reloadLevel());
        }

        //magic buttons
        if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene("Playtest_Infiltration");
    }

    IEnumerator reloadLevel()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Playtest_Infiltration");
    }
    IEnumerator startNextPhase()
    {
        yield return new WaitForSeconds(3f);
        reloadLevel();
    }

    void OnGUI()
    {
        if(firstScreen)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splashScreen, ScaleMode.ScaleToFit);
        }
        /*
        string colliderState = (colliderEnabled) ? "ON" : "OFF";
        string phaseStr = phase.ToString();
        GUI.Label(new Rect(10, 10, 100, 20), colliderState);
        GUI.Label(new Rect(Screen.width -110, 10, 100, 20), phaseStr);
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        if (endGame) GUI.Label(new Rect(Screen.width/2 -100, Screen.height/2 -10, 200, 20), "It's meal time !", style);
        */
    }

    bool BothCannibalsAreDead()
    {
        return cannibals[0].IsDead() && cannibals[1].IsDead();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(corpse))
        {
            targetReachedTheExit = true;
        }
        RewiredInput cIN = other.GetComponent<RewiredInput>();
        if (cIN != null) cannibalsInTheEnd[cIN.playerInputID + cIN.number] = true;
    }

    void OnTriggerExit(Collider other)
    {
        RewiredInput cIN = other.GetComponent<RewiredInput>();
        if (cIN != null) cannibalsInTheEnd[cIN.playerInputID + cIN.number] = false;
    }
}
