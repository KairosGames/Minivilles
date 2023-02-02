using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinsFromOtherEvent : CardEvent
{
    public int coinsNumber;

    bool allPlayers;
    bool currentPlayer;

    public CoinsFromOtherEvent(int _coinsNumber, bool _allPlayers, bool _currentPlayer)
    {
        coinsNumber = _coinsNumber;
        allPlayers = _allPlayers;
        currentPlayer = _currentPlayer;
    }

    public override void Activate(Player player, Card card)
    {
        Game game = Game.instance;

        if (currentPlayer)
        {
            Player p2 = game.GetPlayer(game.GetCurrentPlayerID());

            if(p2.coins >= coinsNumber)
            {
                p2.ChangeCoins(-coinsNumber);
                player.ChangeCoins(coinsNumber);
            }
            else
            {
                p2.ChangeCoins(-p2.coins);
                player.ChangeCoins(p2.coins);
            }

            card.SetFinished();
        }
        else if (allPlayers)
        {
            List<Player> players = game.GetAllPlayers();

            foreach(Player p in players)
            {
                if(p != player)
                {
                    if(p.coins >= coinsNumber)
                    {
                        p.ChangeCoins(-coinsNumber);
                        player.ChangeCoins(coinsNumber);
                    }
                    else
                    {
                        p.ChangeCoins(-p.coins);
                        player.ChangeCoins(p.coins);
                    }
                }
            }

            card.SetFinished();
        }
        else
        {
            if (!player.isIA)
            {
            UIManager.PopUpSelectCallback callback = TargettingCallback;

            game.ui.ShowSelectPopUp("Selection", player.PlayerID, callback);
            game.StartCoroutine(WaitForTarget(player,card));
            }
            else
            {
                IA ia = (IA)player;
                Player target = ia.CheckWealthiestPlayer();
                ActionResult(player, target, card);
            }
        }
    }

    int target = -1;
    IEnumerator WaitForTarget(Player player, Card card)
    {
        Game game = Game.instance;

        while(target == -1)
        {
            yield return null;
        }

        Player p2 = game.GetPlayer(target);
        ActionResult(player, p2, card);

        target = -1;
    }

    public void TargettingCallback(int id)
    {
        target = id;
    }

    public void ActionResult(Player player,Player target, Card card)
    {
        if (target.coins >= coinsNumber)
        {
            target.ChangeCoins(-coinsNumber);
            player.ChangeCoins(coinsNumber);
        }
        else
        {
            target.ChangeCoins(-target.coins);
            player.ChangeCoins(target.coins);
        }

        card.SetFinished();
    }
}
