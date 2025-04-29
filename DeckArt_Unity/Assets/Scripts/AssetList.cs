using System;
using System.Collections.Generic;
using UnityUtility.MathU;

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


    public static AssetList GetAllPossibleCards()
    {
        if (instance == null)
        {
            instance = new AssetList();

            for (int attack = 0; attack <= Card.MAX_COST * 3; attack++)
            {
                for (int defence = 1; defence <= Card.MAX_COST * 3; defence++)
                {
                    for (int loop = 0; loop < MathF.Pow(2, Enum.GetValues(typeof(Competences)).Length); loop++)
                    {
                        Competences competences = (Competences)loop;
                        if (Card.ComputeCardCost(attack, defence, competences) <= Card.MAX_COST)
                        {
                            instance.AddCard(new Card(attack, defence, competences));
                        }
                    }
                }
            }
        }
        return instance;
    }
    
    [Flags]
    public enum Competences
    {
        Provoc = 1 << 0,
        //Deferlement = 1 << 1, // Piètinement
        //Distortion = 1 << 2, // Imblocable sauf par distortion
    }
}
