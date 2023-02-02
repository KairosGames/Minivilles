using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Park : Monument
{
    bool activeForNextTurn;

    public Park(string name, string desc, int cost, Player player) : base(name, desc, cost, player) { }

    public override void Buy()
    {
        base.Buy();

        Game.instance.onDiceRoll += CheckForActivation;
        Game.instance.onTurnEnd += SetTurn;
    }

    public void CheckForActivation(int[] rolls, int id)
    {
        if (id != player.PlayerID)
        {
            Game.instance.SetCallback(CallbackTypes.DiceRoll);
            return;
        }
        int lastRoll = 0;

        for (int i = 0; i < rolls.Length; i++)
        {
            if (lastRoll == rolls[i])
            {
                activeForNextTurn = true;
                Game.instance.SetCallback(CallbackTypes.DiceRoll);
                return;
            }

            lastRoll = rolls[i];
        }
        Game.instance.SetCallback(CallbackTypes.DiceRoll);
    }

    public void SetTurn(int id)
    {
        if (activeForNextTurn)
        {
            Game.instance.SetCurrentPlayerID(player.PlayerID);
            activeForNextTurn = false;
        }
    }

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    public override void Destroy()
    {
        Game.instance.onDiceRoll -= CheckForActivation;
        Game.instance.onTurnEnd -= SetTurn;
    }
}
