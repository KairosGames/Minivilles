using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Card
{
    public int PlayerID;

    public int coinsBonus;

    public CardScriptableObject values;

    public CardEvent cardEvent;

    bool finished;
    
    public Card(int ID, CardScriptableObject _values)
    {
        PlayerID = ID;
        values = _values;

        switch (values.EventType)
        {
            case (CardEventType.CoinsFromBank):
                cardEvent = new CoinsFromBankEvent(_values.cfb_CoinsNumber, _values.ForEachBuilding, _values.ForEachType);
                break;
            case (CardEventType.CoinsFromOther):
                cardEvent = new CoinsFromOtherEvent(_values.cfo_CoinsNumber, _values.AllPlayers, _values.CurrentPlayer);
                break;
            case (CardEventType.Trade):
                cardEvent = new TradeEvent(_values.ForbiddenType);
                break;
        }
    }

    public bool UseCard(int diceRoll)
    {
        if (values.Color == CardColor.Green || values.Color == CardColor.Purple)
            if (Game.instance.GetCurrentPlayerID() != PlayerID) return false;

        if (values.Color == CardColor.Red)
            if (Game.instance.GetCurrentPlayerID() == PlayerID) return false;

        if (diceRoll >= values.minValue && diceRoll <= values.maxValue)
        {
            cardEvent.Activate(Game.instance.GetPlayer(PlayerID), this);
            return true;
        }

        return false;
    }

    public void SetFinished()
    {
        finished = true;
    }

    public bool GetFinished()
    {
        if (finished == true)
        {
            finished = false;
            return true;
        }

        return false;
    }
}
