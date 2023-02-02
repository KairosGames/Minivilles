using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEvent
{
    public abstract void Activate(Player player, Card card);
}
