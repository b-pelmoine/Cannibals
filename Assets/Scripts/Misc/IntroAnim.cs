using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroAnim : MonoBehaviour {

    private RawImage img;
    public GameObject skipIndication;

    [Header("Anim related")]
    public MovieTexture movie;

    bool playing = false;

    void Start()
    {
        skipIndication.SetActive(false);
        img.texture = movie as MovieTexture;
    }

    public void PlayAnimation()
    {
        playing = true;
        movie.Play();
        StartCoroutine(SkipIntro());
    }

    IEnumerator SkipIntro()
    {
        while (!Input.GetKeyDown("joystick button 0") || movie.isPlaying) { yield return new WaitForEndOfFrame(); }
        if (movie.isPlaying)
        {
            skipIndication.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }
        while (!Input.GetKeyDown("joystick button 0") || movie.isPlaying) { yield return new WaitForEndOfFrame(); }
        SceneController.Instance.LoadScene(1);
    }
}
