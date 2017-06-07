using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroAnim : MonoBehaviour {

    public GameObject skipIndication;

    [Header("Anim related")]
    public VideoClip movie;
    public VideoPlayer player;

    public CanvasGroup menu;
    public CanvasGroup cinematic;

    public RawImage videoDisplayer;

    bool playing = false;

    void Start()
    {
        AkSoundEngine.PostEvent("menu_amb", gameObject);

        playing = false;
        player.playOnAwake = false;
        player.source = VideoSource.VideoClip;
        player.clip = movie;
        player.Prepare();
        player.aspectRatio = VideoAspectRatio.FitOutside;
        skipIndication.SetActive(false);
        cinematic.alpha = 0;

        StartCoroutine(PrepareVideo(
            () =>
            {
                videoDisplayer.texture = player.texture;
                player.frame = 1;
                cinematic.alpha = 1;
            }
            ));
    }

    public void PlayAnimation()
    {
        if (!playing)
        {
            playing = true;
            AkSoundEngine.StopAll();
            StartCoroutine(PrepareVideo(
                () =>
                {
                    StartCoroutine(FadeCanvas(menu, 1, 0, .5f, () => {
                        AkSoundEngine.PostEvent("cinematic", Camera.main.gameObject);
                        player.Play();
                        StartCoroutine(SkipIntro());
                    }));
                }
                ));
        }
    }

    IEnumerator PrepareVideo(System.Action callback)
    {
        WaitForSeconds waitTime = new WaitForSeconds(.1f);
        while (!player.isPrepared)
        {
            yield return waitTime;
        }
        callback();
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float alphaSrc, float alphaDst, float transitDuration, System.Action callback = null)
    {
        float alpha = alphaSrc;
        float elpased = 0f;
        while (alpha != alphaDst)
        {
            elpased += Time.deltaTime;
            alpha = Mathf.Lerp(alphaSrc, alphaDst, elpased / transitDuration);
            canvas.alpha = alpha;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (callback != null)
            callback();
    }

    IEnumerator SkipIntro()
    {
        bool wantsToSkip = false;
        while (!Input.GetKeyDown("joystick button 0") && player.isPlaying) {
            yield return new WaitForFixedUpdate(); }
        if (player.isPlaying)
        {
            wantsToSkip = true;
            skipIndication.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }
        while (!Input.GetKeyDown("joystick button 0") && player.isPlaying) { yield return new WaitForFixedUpdate(); }

        if (!wantsToSkip)
        {
            skipIndication.SetActive(true);
            while (!Input.GetKeyDown("joystick button 0")) { yield return new WaitForFixedUpdate(); }
        }

        SceneController.Instance.LoadScene(1);
    }
}
