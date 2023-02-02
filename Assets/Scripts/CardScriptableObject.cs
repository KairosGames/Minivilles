using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObjects/Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public Sprite texture;

    public CardColor Color;
    public BuildingType BuildingType;

    public string Name;
    public string Desc;

    public int Cost;
    public int minValue, maxValue;

    [Header("Events")]
    public CardEventType EventType;

    [Header("Coins From Other")]
    public int cfo_CoinsNumber;

    [Tooltip("Si False et CurrentPlayer False alors le joueur doit choisir sa cible.")]
    public bool AllPlayers;
    [Tooltip("Si False et AllPlayers False alors le joueur doit choisir sa cible.")]
    public bool CurrentPlayer;

    [Header("Coins From Bank")]
    public int cfb_CoinsNumber;

    [Tooltip("Si False et CurrentPlayer False alors le joueur doit choisir sa cible.")]
    public bool ForEachBuilding;
    [Tooltip("Si False et AllPlayers False alors le joueur doit choisir sa cible.")]
    public BuildingType ForEachType;

    [Header("Trade")]
    public BuildingType ForbiddenType;
}

public enum CardColor
{
    Green,
    Blue,
    Red,
    Purple
}

public enum BuildingType
{
    Farm,
    Animals,
    Food,
    Factory,
    Engineering,
    Restaurant,
    Fruits,
    Tower
}

public enum CardEventType
{
    Trade,
    CoinsFromOther,
    CoinsFromBank
}
