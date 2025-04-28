using System;
using UnityEngine;
using System.Collections.Generic;
using UnityUtility.Extensions;

[Serializable]
public class Hand
{
    public List<Card> cards = new();

    public Card GetCardToPlay(int maxMana)
    {
        Card card = cards.FindLast(x => x.cost <= maxMana);
        return card;
    }

    public void AddCard(Card cardToAdd)
    {
        //int manaCost = cardToAdd.cost;
        //int index = cards.FindLastIndex(x => x.cost <= manaCost);
        //if (index == -1)
        //{
        //    cards.Add(cardToAdd);
        //    return;
        //}
        //cards.Insert(index, cardToAdd);

        cards.Add(cardToAdd);
        cards.Sort((card0, card1) => card0.cost.CompareTo(card1.cost));

        //Debug.LogError($"{cards.EnumerableToString()}");

    }

    public void RemoveCard(Card cardToRemove)
    {
        cards.Remove(cardToRemove);
    }

    public void Reset()
    {
        cards.Clear();
    }
}
