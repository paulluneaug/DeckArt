using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Hand
{
    public List<Card> cards = new();

    private Card GetHighestCard(ref List<Card> cardsToPickFrom)
    {
        Card highestCard = cards[0];

        foreach (Card card in cards)
        {
            if(card.cost > highestCard.cost)
                highestCard = card;
        }

        cardsToPickFrom.Remove(highestCard);
        return highestCard;
    }

    public Card GetCardToPlay(int maxMana)
    {
        List<Card> cardsCopy = new List<Card>(cards);
        
        Card highestCard = GetHighestCard(ref cardsCopy);

        while (cardsCopy.Count > 0 || highestCard.cost > maxMana)
        {
            highestCard = GetHighestCard(ref cardsCopy);
        }

        if (cardsCopy.Count == 0)
        {
            return null;
        }

        return highestCard;
    }

    public void AddCard(Card cardToAdd)
    {
        cards.Add(cardToAdd);
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
