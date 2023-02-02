using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public UIManager ui;

    [SerializeField] List<CardScriptableObject> cardsSO = new List<CardScriptableObject>(); //Liste des cartes du jeu

    private List<Player> players = new List<Player>(); //Liste des joueurs
    public Pile gamePile = new Pile(); //Pile du jeu

    public List<Dice> diceObjects = new(); //Liste des dès

    int currentTurnPlayerID;
    bool canBuy;

    int dicesNumber;

    //Events - Tous les events relatifs au déroulement des tours
    public delegate void OnTurnStart(int turnPlayerID);
    public event OnTurnStart onTurnStart; //Quand un tour commence

    public delegate void OnDiceRoll(int[] rolls, int playerID);
    public event OnDiceRoll onDiceRoll; //Quand les dés sont lancés
    Coroutine diceCoroutine;

    public delegate void OnCardBuy(Card card, int left);
    public event OnCardBuy onCardBuy; //Quand une carte est achetée

    public delegate void OnTurnEnd(int turnPlayerID);
    public event OnTurnEnd onTurnEnd; //Quand un tour se fini

    public delegate void OnBuyStart(int turnPlayerID);
    public event OnBuyStart onBuyStart; //Quand la phase d'achat commence (IA)

    //Callbacks
    int turnStartCallbacks = 0;
    int diceRollCallbacks = 0;
    int cardBuyCallbacks = 0; //Int permettant de récupérer des callbacks (monuments / certaines cartes)...
    int turnEndCallbacks = 0; //...qui eux-mêmes permettent d'attendre que tout soit fini avant la suite

    //Singleton
    public static Game instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); //Singleton

        //Création des joueurs (Humain contre IA)
        Player p = new Player("Joueur", 0, false);
        players.Add(p);

        IA ia = new IA("Ordinateur", 1, true);
        players.Add(ia);
    }

    IEnumerator Start() //Méthode Start en IEnumerator permet de "WaitForSeconds"
    {
        ui.InitPile(cardsSO); //Initialise la pile de cartes

        currentTurnPlayerID = 0;

        yield return new WaitForSeconds(0.8f); //Attends les rideaux

        foreach (Player P in players) //Donne les cartes de base aux joueurs
        {
            P.AddCard(new Card(P.PlayerID, cardsSO[0]));
            ui.GiveCardToPlayer(P.PlayerID, cardsSO[0]);
            gamePile.RemoveCard(0);

            P.AddCard(new Card(P.PlayerID, cardsSO[2]));
            ui.GiveCardToPlayer(P.PlayerID, cardsSO[2]);
            gamePile.RemoveCard(2);
        }
        
        StartCoroutine(StartTurn()); //Lance la partie
    }

    #region Methode
    public IEnumerator StartTurn() //Gère le début de chaque tour
    {
        ui.ShowTurnChangeSequence(currentTurnPlayerID);
        yield return new WaitForSeconds(1.7f); //PopUp de début de tour

        dicesNumber = 1;
        onTurnStart?.Invoke(currentTurnPlayerID); //Invoque l'event onTurnStart

        int listeners = 0;
        if (onTurnStart?.GetInvocationList() != null) listeners = onTurnStart.GetInvocationList().Length;

        if (listeners <= 0)
            RollDice(dicesNumber); //Fait directement la suite car aucune méthode n'attend l'event
        else
            StartCoroutine(WaitForTurnStartEvent(listeners)); //Lance une coroutine qui attend que tous les "Listeners" aient terminé pour continuer
    }

    IEnumerator WaitForTurnStartEvent(int callbacksNb)
    {
        while (turnStartCallbacks < callbacksNb)
        {
            yield return null;
        } //Attend que toutes les méthodes abonnées à l'event aient bien renvoyé un callback

        turnStartCallbacks = 0;

        RollDice(dicesNumber);
    }
    public void ChangeDiceNumbers(int diceNb) //Permet de changer le nombre de dés (Utile pour la gare)
    {
        dicesNumber = diceNb;
    } 

    public void RollDice(int diceNumber) //Lance le ou les dè(s)
    {
        int rollsSum = 0; //Somme des lancés
        int[] rolls = new int[diceNumber]; //Tableau contenant les valeurs des lancés (Parc -> attend un double)
        
        //Lance chaque dè
        int maxIndex = -1;
        for (int i = 0; i < diceNumber; i++)
        {
            maxIndex++;

            if(i < diceObjects.Count)
                rolls[i] = diceObjects[i].Roll();
            else
                rolls[i] = diceObjects[0].Roll(); //Si on demande plus de dès qu'ils n'en existent dans la scène alors on relance le premier

            rollsSum += rolls[i];
        }

        //Cache les dès qui n'ont pas été lancé
        if (maxIndex < diceObjects.Count - 1)
        {
            for(int i = maxIndex; i < diceObjects.Count; i++)
            {
                diceObjects[i].HideDice();
            }
        }

        int listeners = 0;

        if (onDiceRoll?.GetInvocationList() != null) listeners = onDiceRoll.GetInvocationList().Length;

        diceCoroutine = StartCoroutine(WaitForRollDiceEvent(listeners, rollsSum, rolls)); //Attend en fonction du nombre de méthodes abonnées à l'event
    }

    IEnumerator WaitForRollDiceEvent(int callbacksNb, int rollsSum, int[] rolls)
    {
        yield return new WaitForSeconds(2f);
        onDiceRoll?.Invoke(rolls, currentTurnPlayerID); //Appel l'event "onDiceRoll"

        while (diceRollCallbacks < callbacksNb) //Attend qu'il y ait assez de callbacks pour continuer
        {
            yield return null;
        }

        diceRollCallbacks = 0;

        CheckForCardsActivation(rollsSum); //Active les cartes
    }
    public void RerollDice() //Relance les dès (Tour radio)
    {
        if (diceCoroutine != null) StopCoroutine(diceCoroutine);

        diceRollCallbacks = 0;

        RollDice(dicesNumber);
    }

    int playerDoneActivation = 0;
    public void CheckForCardsActivation(int rollsSum) //Demande à chaque joueur de vérifier quelles cartes s'activent ou pas
    {
        foreach (Player p in players)
        {
            p.CheckForCards(rollsSum);
        }

        StartCoroutine(StartBuySequence()); //Attend que tous les joueurs aient fini
    }
    public void SetPlayerDone()
    {
        playerDoneActivation++;
    }

    public IEnumerator StartBuySequence()
    {
        while (playerDoneActivation < players.Count) //Attend que tous les joueurs aient fini
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        if (!players[currentTurnPlayerID].isIA) ui.OpenPilePanel(); //La séquence d'achat commence donc on ouvre la panneau d'achat si c'est au joueur de jouer

        playerDoneActivation = 0;
        canBuy = true; //Permet de lancer la séquence d'achat
    
        onBuyStart?.Invoke(currentTurnPlayerID); //Appel l'event "onBuyStart"
    }

    public void BuyCard(int cardID)
    {
        if (!canBuy) return; //Si on n'est pas en séquence d'achat on arrête

        Player player = players[currentTurnPlayerID];
        PileCards pile = gamePile.GetCard(cardID);

        if (pile.cardSO.Color == CardColor.Purple && player.HasCard(cardsSO[cardID])) return; //Si la carte est violette et qu'elle est déjà dans notre main alors stop
        if (pile.cardSO.Cost > player.coins || pile.nb <= 0) return; //Si on ne peut pas acheter la carte alors stop
        
        //Sinon on achête la carte
        Card card = new Card(currentTurnPlayerID, pile.cardSO);

        card.PlayerID = currentTurnPlayerID;
        player.AddCard(card);
        gamePile.RemoveCard(cardID);

        player.ChangeCoins(-card.values.Cost);

        ui.GiveCardToPlayer(currentTurnPlayerID, pile.cardSO); //Animation de la carte en jeu

        canBuy = false;
        onCardBuy?.Invoke(card, pile.nb); //Appel l'event "onCardBuy"

        StartCoroutine(EndTurn()); //Appel la fin du tour
    }

    public void BuyMonument(int monumentID)
    {
        if (!canBuy) return; //Si on n'est pas en séquence d'achat on arrête

        Player player = players[currentTurnPlayerID];
        Monument monument = player.monuments[monumentID];

        if (monument.isActive) return; //Si le monument est déjà actif stop
        if (monument.Cost > player.coins) return; //Si on n'a pas assez alors stop

        //Sinon on achête le monument
        player.ChangeCoins(-monument.Cost);
        monument.Buy();

        ui.BuyMonument(currentTurnPlayerID, monumentID); //Animation ingame

        canBuy = false;

        //Vérifie si le joueur actuel a gagné ou pas
        int mCount = 0;
        player.monuments.ForEach(x => {
            if (x.isActive) mCount++;
        });

        //Le joueur actuel a gagné
        if (mCount == player.monuments.Count)
        {
            if(player.isIA)
                ui.LaunchLoosePanel();
            else
                ui.LaunchWinPanel();
        }
        else //La partie continue
            StartCoroutine(EndTurn()); //Fin du tour
    }

    public void DontBuy() //Si on décide de ne pas acheter
    {
        if (!canBuy) return;

        canBuy = false;

        StartCoroutine(EndTurn()); //Appel la fin du tour
    }

    public IEnumerator EndTurn() //Fin du tour
    {
        if(!players[currentTurnPlayerID].isIA) ui.HidePilePanel(); //Si on est un joueur on cache le panneau d'achat

        yield return new WaitForSeconds(0.5f);

        currentTurnPlayerID++;
        if (currentTurnPlayerID >= players.Count) currentTurnPlayerID = 0; //Change le tour

        onTurnEnd?.Invoke(currentTurnPlayerID); //Appel l'event "onTurnEnd"

        StartCoroutine(StartTurn()); //Relance un tour
    }

    public Player GetPlayer(int index) //Renvoie le joueurs correspondant à l'index
    {
        return players[index];
    }

    public List<Player> GetAllPlayers() //Renvoie tous les joueurs
    {
        return players;
    }

    public int GetCurrentPlayerID() //Renvoie l'id du joueur actuel
    {
        return currentTurnPlayerID;
    }

    public void SetCurrentPlayerID(int value) //Change l'id du joueur actuel (Parc d'attraction)
    {
        currentTurnPlayerID = value;
    }

    public List<CardScriptableObject> GetSOList() //Renvoie la liste des scriptables objects des cartes
    {
        return cardsSO;
    }    
    public CardScriptableObject GetSO(int index) //Renvoie le scriptable object correspondant à l'index
    {
        return cardsSO[index];
    }

    public int GetIndexSO(CardScriptableObject card) //Renvoie l'index du scriptable object de carte
    {
        return cardsSO.IndexOf(card);
    }    
    public int GetCardCost(int index) //Renvoie le coût de la carte correspondant à l'index
    {
        return cardsSO[index].Cost;
    }

    public void SetCallback(CallbackTypes type) //Permet d'envoyer une réponse quand une méthode se termine après un event
    {
        switch (type)
        {
            case (CallbackTypes.TurnStart):
                turnStartCallbacks++;
                break;
            case (CallbackTypes.DiceRoll):
                diceRollCallbacks++;
                break;
            case (CallbackTypes.CardBuy):
                cardBuyCallbacks++;
                break;
            case (CallbackTypes.TurnEnd):
                turnEndCallbacks++;
                break;
        }
    }
    #endregion
}

public enum CallbackTypes //Enum des types de callbacks
{
    TurnStart,
    DiceRoll,
    CardBuy,
    TurnEnd
}