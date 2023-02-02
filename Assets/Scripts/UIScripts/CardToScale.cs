using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardToScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UIManager ui;
    Image img;

    void Start()
    {
        ui = Game.instance.ui;
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.ShowBigCard(img.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.HideBigCard();
    }
}
