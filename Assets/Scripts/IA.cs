using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : Player
{
    public Strategie strat;
    int nMonument;
    int gareOwn;
    bool buyMonument;
    bool radioBuy;
    int coinDiff;
    sbyte difficulty;
    public IA(string name, int playerID, bool isIA) : base(name, playerID, isIA)
    {
        strat = CreateStrategie();
        Game.instance.onCardBuy += LowerStartegie;
        Game.instance.onBuyStart += ChoseCard;
        nMonument = 0;
        gareOwn = 0;
        radioBuy = false;
        coinDiff = coins;
        difficulty = MenuManager.difficulty;
    }

    public void ChoseCard(int playerId)
    {
        //Choisit la carte en fct de la priorité de la strategie definit
        Game game = Game.instance;

        if (coinDiff + 10 <= coins && nMonument == 1)
            radioBuy = true;
        coinDiff = coins;
        CheckMonumentToBuy();

        if (this.PlayerID != playerId) { return; }
        if (!buyMonument)
        {
            List<int> indexes = new List<int>();
            int priority = RollPriority();
            for (int i = 0; i < strat.priority.GetLength(1); i++)
                if (strat.priority[gareOwn, i] == priority)
                    indexes.Add(i);
            int chooseCard = UnityEngine.Random.Range(0, indexes.Count);
            int cost = game.GetCardCost(indexes[chooseCard]);

            if (cost <= this.coins)
            {
                Game.instance.StartCoroutine(ResolveAction(true, true, indexes[chooseCard]));

                game.BuyCard(indexes[chooseCard]);
            }

            else
            {
                game.DontBuy();
            }
        }
        else
        {
            int index = (nMonument % 2) + ((nMonument != 0 && nMonument !=3 )?2:0);

            if (coins >= monuments[index].Cost)
            {
                Game.instance.StartCoroutine(ResolveAction(true, false, index));
                buyMonument = false;
                nMonument++;
            }
            else
            {
                Game.instance.StartCoroutine(ResolveAction(false));
            }

        }
    }

    IEnumerator ResolveAction(bool buy, bool isCard = true, int id = 0)
    {
        Game game = Game.instance;

        yield return new WaitForSeconds(1f);

        if (buy)
        {
            if (isCard) game.BuyCard(id);
            else game.BuyMonument(id);
        }
        else
            game.DontBuy();
    }

    public int RollPriority()
    {
        int priority = -1;
        if(difficulty == 0)
        {
            int rand = UnityEngine.Random.Range(0, 20);
            if (rand < 15)
                priority = 1;
            else if (rand < 19 && rand >= 15)
                priority = 2;
            else if (rand == 19)
                priority = 3;
        }
        else
        {
            priority = UnityEngine.Random.Range(1, 4);
        }
        return priority;
    }

    public void LowerStartegie(Card card, int left)
    {
        int index = -1;
        if (left == 0)
        {
            index = Game.instance.GetIndexSO(card.values);
            if (strat.priority[gareOwn, index] <= 3)
            {
                int priority = strat.priority[gareOwn, index];
                for (int i = 0; i < strat.priority.GetLength(1); i++)
                {
                    if (strat.priority[gareOwn, i] >= priority)
                    {
                        strat.priority[gareOwn, i]--;
                    }
                }
            }
        }
        if (index < 0)
            return;
    }

    public bool EstimateResult(int roll)
    {
        int result = 0;
        int allPlayerResult = 0;
        foreach (Player P in Game.instance.GetAllPlayers())
        {
            foreach (Card C in P.cards)
            {
                if (C.values.minValue >= roll && C.values.maxValue <= roll)
                {
                    if (PlayerID == P.PlayerID)
                    {
                        if (C.values.Color == CardColor.Blue || C.values.Color == CardColor.Green || C.values.Color == CardColor.Purple)
                            result++;
                    }
                    else
                    {
                        if (C.values.Color == CardColor.Blue || C.values.Color == CardColor.Red)
                            allPlayerResult++;
                    }
                }
            }
        }
        allPlayerResult /= (Game.instance.GetAllPlayers().Count - 1);
        return result >= allPlayerResult ? false : true;
    }
    public Card CheckBestCard(List<Card> cards)
    {
        Card thisCard = null;
        int best = 10;
        foreach (Card C in cards)
        {
            if (strat.priority[0, Game.instance.GetIndexSO(C.values)] <= best)
            {
                best = Game.instance.GetIndexSO(C.values);
                thisCard = C;
            }
        }
        return thisCard;
    }
    public Card CheckWorthCard(List<Card> cards)
    {
        Card thisCard = null;
        int best = 10;
        foreach (Card C in cards)
        {
            if (strat.priority[0, Game.instance.GetIndexSO(C.values)] >= best)
            {
                best = Game.instance.GetIndexSO(C.values);
                thisCard = C;
            }
        }
        return thisCard;
    }
    public Player CheckWealthiestPlayer()
    {
        List<Player> players = Game.instance.GetAllPlayers();
        Player thisPlayer = null;
        foreach (Player P in players)
        {
            if (P.coins > thisPlayer.coins && P != this)
                thisPlayer = P;
        }
        return thisPlayer;
    }
    public void CheckMonumentToBuy()
    {
        switch (nMonument)
        {
            case 0:
                if (HasCard(Game.instance.GetSO(6)))
                {
                    buyMonument = true;
                    gareOwn = 1;
                }
                break;
            case 1:
                if (radioBuy)
                    buyMonument = true;
                break;
            case 2:
                if (coins > 10)
                    buyMonument = true;
                break;
            case 3:
                buyMonument = true;
                break;
            default:
                buyMonument = false;
                break;
        }
    }

    public Strategie CreateStrategie()
    {
        //Choisit une stratégie au hasard en fct de celle pre-définit
        //Tableau   (0 à 11 les 12 cartes de base
        //          (12 à 14 les 3 cartes violette)
        //          (15 à 18 les 4 monuments
        int rand = UnityEngine.Random.Range(0, 3);
        int[,] strategie;
        switch (rand)
        {
            case 0:
                //Strat Ferme + Fromagerie
                strategie = new int[2, 15] { { 8, 1, 4, 3, 3, 2, 2, 3, 5, 5, 6, 7, 0, 0, 0 }, { 8, 5, 8, 7, 7, 4, 1, 2, 3, 6, 7, 8, 10, 10, 10 } };
                break;
            case 1:
                strategie = new int[2, 15] { { 8, 1, 4, 3, 3, 2, 2, 3, 5, 5, 6, 7, 0, 0, 0 }, { 8, 5, 8, 7, 7, 4, 1, 2, 3, 6, 7, 8, 10, 10, 10 } };
                //Strat meuble
                break;
            case 2:
                strategie = new int[2, 15] { { 8, 1, 4, 3, 3, 2, 2, 3, 5, 5, 6, 7, 0, 0, 0 }, { 8, 5, 8, 7, 7, 4, 1, 2, 3, 6, 7, 8, 10, 10, 10 } };
                //Strat 1D6
                break;
            default:
                strategie = new int[2, 15] { { 8, 1, 4, 3, 3, 2, 2, 3, 5, 5, 6, 7, 0, 0, 0 }, { 8, 5, 8, 7, 7, 4, 1, 2, 3, 6, 7, 8, 10, 10, 10 } };
                break;
        }
        return new Strategie(strategie);
    }
    public struct Strategie
    {
        public int[,] priority;
        public Strategie(int[,] priority)
        {
            this.priority = priority;
        }
    }
}