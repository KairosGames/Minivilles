using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class Pile
{
    public List<PileCards> cards;

    public Pile()
    {
        cards = new();
    }

    public void AddCard(CardScriptableObject cardSO, int cardNumber)
    {
        cards.Add(new PileCards(cardSO, cardNumber));
    }

    public PileCards GetCard(int index)
    {
        return cards[index];
    }

    public void RemoveCard(int index)
    {
        cards[index].ChangeNumber(-1);
    }

    public void ShowAllCard()
    {

    }
}

[Serializable]
public class PileCards
{
    public CardScriptableObject cardSO;
    public int nb;

    public PileCards(CardScriptableObject so, int n)
    {
        cardSO = so;
        nb = n;
    }

    public void ChangeNumber(int nb)
    {
        this.nb += nb;
    }
}