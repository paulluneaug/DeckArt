using System.Collections.Generic;
using UnityEngine;

public static class CardsComparisons
{
    public static int ScoreComparer(Card card0, Card card1)
    {
        return card0.score.CompareTo(card1.score);
    }
    public static int InverseScoreComparer(Card card0, Card card1)
    {
        return -card0.score.CompareTo(card1.score);
    }
    public static int CostComparer(Card card0, Card card1)
    {
        return card0.cost.CompareTo(card1.cost);
    }
    public static int AttackComparer(Card card0, Card card1)
    {
        return card0.attack.CompareTo(card1.attack);
    }
    public static int DefenceComparer(Card card0, Card card1)
    {
        return card0.defense.CompareTo(card1.defense);
    }
}
