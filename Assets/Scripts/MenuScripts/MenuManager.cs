using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    public static sbyte difficulty = 0;

    [Header("Visual")]
    [SerializeField] Image title;
    [SerializeField] Image sunburst;
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;
    [SerializeField] Image[] curtains;

    [Header("Audio")]
    [SerializeField] AudioSource mscMenuTheme;
    [SerializeField] AudioSource sndCityMood;
    [SerializeField] AudioSource sndOnClickButton;
    [SerializeField] AudioSource sndLaunch;

    [Header("Events")]
    [SerializeField] UnityEvent curtainsAction;

    byte step;

    float timer;

    bool areCurtainsOpened;


    //==================================================================================================
   
    void Start()
    {
        timer = 0;
        step = 0;

        title.transform.localScale = new Vector2(0, 0);
        sunburst.transform.localScale = new Vector2(0, 0);
        panel1.transform.localScale = new Vector2(0, 0);
        panel2.transform.localScale = new Vector2(0, 0);

        curtains[0].rectTransform.localScale = Vector3.one;
        curtains[1].rectTransform.localScale = Vector3.one;

        areCurtainsOpened = false;
    }

    //==================================================================================================

    void Update()
    {
        if (step != 4 && step != 6) { timer += Time.deltaTime; }

        if ( timer >= 0.5f && step == 0)
        {
            curtainsAction.Invoke();
            step++;
            timer = 0;
        }

        if(timer>= 0.3f && step == 1)
        {
            mscMenuTheme.Play();
            LeanTween.scale(title.gameObject, new Vector2(1.0f, 1.0f), 2.0f).setEase(LeanTweenType.easeOutElastic);
            step++;
            timer = 0;
        }

        if (timer >= 0.3f && step == 2)
        {
            LeanTween.scale(sunburst.gameObject, new Vector2(1.0f, 1.0f), 2.0f).setEase(LeanTweenType.easeOutElastic);
            step++;
            timer = 0;
        }

        if(timer>= 0.6f && step == 3)
        {
            LeanTween.scale(panel1, new Vector2(1.0f, 1.0f), 1.0f).setEase(LeanTweenType.easeOutElastic);
            step++;
            timer = 0;
        }

        if(timer >= 0.1f && step == 5)
        {
            LeanTween.scale(panel2, new Vector2(1.0f, 1.0f), 1.0f).setEase(LeanTweenType.easeOutElastic);
            step++;
            timer = 0;
        }

        if (timer >= 0.4 && step ==7)
        {
            mscMenuTheme.volume -= Time.deltaTime/2;
            sndCityMood.volume -= Time.deltaTime / 4;
            if (mscMenuTheme.volume <= 0f)
            {
                SceneManager.LoadScene(1);
            }
        }

        if(step == 10 && timer>= 0.4f)
        {
            mscMenuTheme.volume -= Time.deltaTime/2;
            sndCityMood.volume -= Time.deltaTime/3;
            if (mscMenuTheme.volume <= 0f)
            {
                Application.Quit();
            }
        }
    }

    //==================================================================================================
   
    public void CurtainsAction()
    {
        if (!areCurtainsOpened)
        {
            LeanTween.scaleY(curtains[0].gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scaleY(curtains[1].gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutSine);
            areCurtainsOpened = true;
        }
        else
        {
            LeanTween.scaleY(curtains[0].gameObject, 1, 0.3f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scaleY(curtains[1].gameObject, 1, 0.3f).setEase(LeanTweenType.easeOutSine);
        }
    }

    public void PlayButton()
    {
        sndOnClickButton.Play();
        LeanTween.scale(panel1, new Vector2(0,0),0.3f).setEase(LeanTweenType.easeOutSine);
        step++;
    }

    public void ExitButton()
    {
        sndOnClickButton.Play();
        CurtainsAction();
        step = 10;
    }

    public void SetEasyDifficultyButton()
    {
        sndOnClickButton.Play();
        CurtainsAction();
        difficulty = 1;
        step++;
    }

    public void SetHardDifficultyButton()
    {
        sndOnClickButton.Play();
        CurtainsAction();
        difficulty = 0;
        step++;
    }
}
