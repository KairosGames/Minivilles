using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    string stringToShow;

    [Header("Random time (sec) for writing")]
    [SerializeField] float min;
    [SerializeField] float max;

    byte counter = 0;
    internal bool isTextCoroutineSwitcherOn;
    bool isAllTextLoaded;

    void Awake()
    {
        stringToShow = textMesh.text.ToString();
        textMesh.text = "";
        isTextCoroutineSwitcherOn = false;
        isAllTextLoaded = false;
    }

    private void OnEnable()
    {
        isTextCoroutineSwitcherOn = true;
    }


    void Update()
    {
        if (isTextCoroutineSwitcherOn)
        {
            StartCoroutine(LoadText());
            isTextCoroutineSwitcherOn = false;
        }

    }

    IEnumerator LoadText()
    {
        if (stringToShow.Length < 1)
        {
            isAllTextLoaded = true;
        }
        else
        {
            while (!isAllTextLoaded)
            {
                yield return new WaitForSeconds(Random.Range(min,max));

                if (counter < stringToShow.Length)
                {
                    textMesh.text += stringToShow[counter];
                    counter++;
                }
                else
                {
                    isAllTextLoaded = true;
                    counter = 0;
                }
            }
        }
    }
}
