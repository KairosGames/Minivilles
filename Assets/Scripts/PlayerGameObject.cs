using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerGameObject : MonoBehaviour
{
    [HideInInspector] public List<CardGameObject> cards = new List<CardGameObject>();
    [SerializeField] List<CardGameObject> monuments = new List<CardGameObject>();

    public void AddCard(CardGameObject co)
    {
        cards.Add(co);
    }

    public void RemoveCard(int id)
    {
        Destroy(cards[id].gameObject);
        cards.RemoveAt(id);

    }

    public int GetCardNumber()
    {
        int numberOfTypes = cards.Select(x => x.cardSo).Distinct().Count();

        return numberOfTypes;
    }

    public void ActivateCard(int index)
    {
        cards[index].ShowActivation();
    }

    public CardGameObject GetMonument(int id)
    {
        return monuments[id];
    }
}
