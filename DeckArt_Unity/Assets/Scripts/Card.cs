using System;
using UnityUtility.MathU;

[Serializable]
public class Card
{
    public const int MAX_COST = 6;

    public string name;
    public int attack;
    public int defense;

    [NonSerialized] public int cost;

    public Card(string name, int atk, int def)
    {
        this.name = name;
        this.attack = atk;
        this.defense = def;

        cost = ComputeCardCost(atk, def);
    }

    public override string ToString()
    {
        return name;
    }

    public static int ComputeCardCost(int attack, int defense)
    {
        return (int)MathUf.Floor((attack + defense) / 2.0f);
    }

    public void ComputeCost()
    {
        cost = ComputeCardCost(attack, defense);
    }
}
