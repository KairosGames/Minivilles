using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [SerializeField] List<Image> imgs;
    [SerializeField] List<TextMeshProUGUI> texts;

    public void SetText(int id, string text)
    {
        texts[id].text = text; 
    }

    public void SetImage(int id, Sprite sprite)
    {
        imgs[id].sprite = sprite;
    }
}
