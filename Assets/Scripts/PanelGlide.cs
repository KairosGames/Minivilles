using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGlide : MonoBehaviour
{
    [HideInInspector] public bool isPileOpen = false;

    private void Start()
    {
        isPileOpen = false;
    }
    public void PileGlide()
    {
        RectTransform parent = transform.parent.GetComponent<RectTransform>();

        isPileOpen = !isPileOpen;
        if (isPileOpen)
            LeanTween.move(parent, new Vector3(-467, 0, 0), 0.2f);
        else
            LeanTween.move(parent, new Vector3(-24, 0, 0), 0.2f);
    }
}
