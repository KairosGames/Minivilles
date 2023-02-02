using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Radio : Monument
{
    bool usedThisTurn;

    public Radio(string name, string desc, int cost, Player player) : base(name, desc, cost, player) { }

    public override void Buy()
    {
        base.Buy();

        Game.instance.onDiceRoll += CheckForActivation;
        Game.instance.onTurnEnd += TurnEnd;
    }

    public void CheckForActivation(int[] rolls, int id)
    {
        if (id != player.PlayerID || usedThisTurn)
        {
            Game.instance.SetCallback(CallbackTypes.DiceRoll);
            return;
        }
        if (!player.isIA)
        {
            Game game = Game.instance;

            UIManager.PopUpCallback callback = Reroll;

            game.ui.ShowChoosePopUp("Tour Radio", "Voulez-vous relancer les dés ?", callback);
        }
        else
        {
            IA ia = (IA)player;
            int sum = rolls.Sum();
            Reroll(ia.EstimateResult(sum));
        }
    }

    public void Reroll(bool isValid)
    {
        if (isValid)
        {
            usedThisTurn = true;
            Game.instance.RerollDice();
        }

        Game.instance.SetCallback(CallbackTypes.DiceRoll);
    }

    public void TurnEnd(int id)
    {
        usedThisTurn = false;
    }

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    public override void Destroy()
    {
        Game.instance.onDiceRoll -= CheckForActivation;
        Game.instance.onTurnEnd -= TurnEnd;
    }
}
