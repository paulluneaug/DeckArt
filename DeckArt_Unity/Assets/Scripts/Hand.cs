using System;
using UnityEngine;
using System.Collections.Generic;
using UnityUtility.Extensions;

[Serializable]
public class Hand
{
    [NonSerialized] public List<Card> cards = new();
    [NonSerialized] public List<Card> playedCards = new();

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
        cards.Sort(CardsComparisons.CostComparer);

        //Debug.LogError($"{cards.EnumerableToString()}");

    }

    public void RemoveCard(Card cardToRemove)
    {
        cards.Remove(cardToRemove);
        playedCards.Add(cardToRemove);
    }

    public void Reset()
    {
        cards.Clear();
        playedCards.Clear();
    }

    public void RewardPlayedCards(float scoreFactor)
    {
        playedCards.ForEach(card => card.score += scoreFactor);
    }

    public float AverageCost()
    {
        int sum = 0;

        foreach (Card card in cards)
        {
            sum += card.cost;
        }

        return sum / (float)cards.Count;
    }
}
