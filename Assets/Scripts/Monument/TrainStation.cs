using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStation : Monument
{
    public TrainStation(string name, string desc, int cost, Player player) : base(name, desc, cost, player) { }

    public override void Buy()
    {
        base.Buy();

        Game.instance.onTurnStart += CheckForActivation;
    }

    public void CheckForActivation(int turnPlayerID)
    {
        if (turnPlayerID == player.PlayerID) Activate();
        else Game.instance.SetCallback(CallbackTypes.TurnStart);
    }

    public override void Activate()
    {
        if (!player.isIA)
        {
        Game game = Game.instance;

        UIManager.PopUpCallback callback = ChangeDiceNumber;

        game.ui.ShowChoosePopUp("Gare", "Voulez-vous lancer 2 dés ?", callback);
        }
        else
        {
            ChangeDiceNumber(true);
        }
    }

    public void ChangeDiceNumber(bool isValid)
    {
        if(isValid)
            Game.instance.ChangeDiceNumbers(2);

        Game.instance.SetCallback(CallbackTypes.TurnStart);
    }

    public override void Destroy()
    {
        Game.instance.onTurnStart -= CheckForActivation;
    }
}
