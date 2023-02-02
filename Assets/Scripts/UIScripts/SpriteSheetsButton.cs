using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteSheetsButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite[] spriteSheet;
    Image imageButton;
    void Start()
    {
        imageButton = GetComponent<Image>();
        imageButton.sprite = spriteSheet[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageButton.sprite = spriteSheet[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageButton.sprite = spriteSheet[0];
    }
}
