using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] Color onExitTextColor;
    [SerializeField] Color onEnterTextColor;

    [SerializeField] AudioSource sndOnEnterButton;

    TextMeshProUGUI text;
    EventTrigger eventTrigger;
    EventTrigger.Entry entry1;
    EventTrigger.Entry entry2;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();

        transform.gameObject.AddComponent<EventTrigger>();
        eventTrigger = GetComponent<EventTrigger>();

        entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((eventData) => { OnPointerEnter(); });

        entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((eventData) => { OnPointerExit(); });

        eventTrigger.triggers.Add(entry1);
        eventTrigger.triggers.Add(entry2);
    }

    public void OnPointerEnter()
    {
        sndOnEnterButton.Play();
        text.color = onEnterTextColor;
    }

    public void OnPointerExit()
    {
        text.color = onExitTextColor;
    }
}
