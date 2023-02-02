using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string name;
    public int coins;
    public List<Card> cards = new List<Card>();
    public List<Monument> monuments = new List<Monument>();

    public int PlayerID;

    public bool isIA;

    public Player(string name, int playerID, bool isIA)
    {
        this.name = name;
        this.coins = 3;
        this.isIA = isIA;
        PlayerID = playerID;
        Game.instance.ui.RefreshCoins(coins, PlayerID);

        //Ajouter cartes de base
        monuments.Add(new TrainStation("Gare", "Vous pouvez lancer deux dès.", 4,this));
        monuments.Add(new Mall("Centre commercial", "Vos établissements de type Restaurant et Food rapportent une pièce de plus.", 10, this));
        monuments.Add(new Park("Parc d'attractions", "Si votre jet de dès est un double, rejouez un tour après celui-ci.", 16, this));
        monuments.Add(new Radio("Tour radio", "Une fois par tour, vous pouvez choisir de relancer vos dès.", 22,this));
    }

    public void ChangeCoins(int value)
    {
        coins += value;
        Game.instance.ui.RefreshCoins(coins, PlayerID, coins - value);
    }

    public void AddCard(Card cardToAdd)
    {
        cards.Add(cardToAdd);
    }

    public void CheckForCards(int diceRoll)
    {
        Game.instance.StartCoroutine(CardsActivationRoutine(diceRoll));
    }

    IEnumerator CardsActivationRoutine(int diceRoll)
    {
        int cardID = 0;

        foreach (Card card in cards.ToList())
        {
            if (card.UseCard(diceRoll))
            {
                Game.instance.ui.ShowCardActivation(PlayerID, cardID);

                while (!card.GetFinished())
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
            }

            cardID++;
        }

        Game.instance.SetPlayerDone();
    }

    public bool HasCardColor(CardColor color)
    {
        foreach(Card card in cards)
        {
            if (card.values.Color == color) return true;
        }

        return false;
    }

    public bool HasCard(CardScriptableObject cso)
    {
        foreach(Card card in cards)
        {
            if (card.values == cso) return true;
        }

        return false;
    }

}
