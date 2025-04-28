using System;
using System.Collections.Generic;

[Serializable]
public class AssetList
{
    private static AssetList instance;
    public List<Card> cards;

    public AssetList()
    {
        cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }


    public static AssetList CreateAllPossibleCards()
    {
        if (instance == null)
        {
            instance = new AssetList();

            for (int attack = 0; attack <= Card.MAX_COST * 3; attack++)
            {
                for (int defence = 1; defence <= Card.MAX_COST * 3; defence++)
                {
                    if (Card.ComputeCardCost(attack, defence) <= Card.MAX_COST)
                    {
                        instance.AddCard(new Card($"Card_{attack}_{defence}", attack, defence));
                    }
                }
            }
        }
        return instance;
    }
}
