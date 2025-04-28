using System;
using System.Collections.Generic;
using UnityUtility.Extensions;

[Serializable]
public class Deck
{
    public const int DECK_SIZE = 30;

    public List<Card> cards;

    public Deck()
    {
        cards = new List<Card>(DECK_SIZE);

        AssetList allCards = AssetList.CreateAllPossibleCards();

        allCards.cards.Shuffle();
        for (int i = 0; i < DECK_SIZE; i++)
        {
            cards.Add(allCards.cards[i]);
        }
    }

    public Card Draw()
    {
        Card drawnCard = cards[^1];
        cards.RemoveAt(cards.Count - 1);
        return drawnCard;
    }
}
