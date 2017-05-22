using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SceneState
{
    InGame,
    InMenu
}

public class SceneController : MonoBehaviour {

    public SceneState state { get
        {
            return (currentScene == MenuScene.name) ? SceneState.InMenu : SceneState.InGame;
        }
    }

    private Object thisLock = new Object();

    public static SceneController Instance;

    public Object GameScene;
    public Object MenuScene;

    private bool Loading;
    private string currentScene;

    private CanvasGroup LoadingCanvas;
    private Text hint;

    [Header("Parameters")]
    [Range(0.1f, 2f)]
    public float fadeOutDuration;
    [Range(0.1f, 2f)]
    public float fadeInDuration;

    public List<string> hints;

    void Awake () {
		if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
	}

    void Start()
    {
        LoadingCanvas = transform.Find("transitionUI").GetComponent<CanvasGroup>();
        hint = LoadingCanvas.gameObject.transform.Find("Canvas").Find("Hint").GetComponent<Text>();
        LoadingCanvas.alpha = 1f;
        hint.text = "";
        StartCoroutine(FadeCanvas(1f, 0f, .5f));
        Loading = false;
        currentScene = SceneManager.GetActiveScene().name;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
            LoadScene(GameScene);
        if (Input.GetKeyDown(KeyCode.M))
            LoadScene(MenuScene);
    }

    public void LoadScene(Object Scene)
    {
        hint.text = getRandomHint();
        string sceneToLoad = Scene.name;
        if(!Loading)
        {
            Loading = true;
            lock (thisLock)
            {
                StartCoroutine(FadeCanvas(0, 1, fadeInDuration,
                () =>
                {
                    //loading Scene
                    StartCoroutine(PlayLoadingInfo(SceneManager.LoadSceneAsync(sceneToLoad),
                    () =>
                    {
                        StartCoroutine(FadeCanvas(1, 0, fadeOutDuration,
                        () =>
                        {
                            StartCoroutine(OnFinishTransition());
                        }
                        ));
                    }));
                }
                ));
            }
        }
    }

    string getRandomHint()
    {
        System.Random rnd = new System.Random();
        return hints[rnd.Next(0,hints.Count-1)];
    }

    IEnumerator PlayLoadingInfo(AsyncOperation ope, System.Action callback)
    {
        while(!ope.isDone)
        {
            //do something during transition
            yield return new WaitForSeconds(Time.deltaTime);
        } 
        callback();
    }

    IEnumerator FadeCanvas(float alphaSrc, float alphaDst, float transitDuration, System.Action callback = null)
    {
        float alpha = alphaSrc;
        float elpased = 0f;
        while (alpha != alphaDst)
        {
            elpased += Time.deltaTime;
            alpha = Mathf.Lerp(alphaSrc, alphaDst, elpased / transitDuration);
            LoadingCanvas.alpha = alpha;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (callback != null)
            callback();
    }

    IEnumerator OnFinishTransition()
    {
        Loading = false;
        currentScene = SceneManager.GetActiveScene().name;
        yield return new WaitForSeconds(0);
    }
}
