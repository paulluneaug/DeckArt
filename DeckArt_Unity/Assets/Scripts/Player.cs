using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityUtility.Extensions;

[Serializable]
public class Player
{
    public const int DECK_SIZE = 30;
    
    public List<Card> cards;
    public string name;
    
    [NonSerialized] public int maxHealth, currentHealth; 
    [NonSerialized] public int manaMax, currentMana;
    [NonSerialized] public int cardCountStart = 5;
    
    [NonSerialized] public Hand hand;
    [NonSerialized] public Board board;
    
    
    public Player()
    {
        Reset();
    }

    public void StartTurn()
    {
        manaMax++;
        currentMana = manaMax;
        hand.AddCard(Draw());
    }

    public void PlayCards()
    {
        Card cardToPlay = hand.GetCardToPlay(currentMana);
        while (!cardToPlay.IsUnityNull())
        {
            hand.RemoveCard(cardToPlay);
            board.AddCard(cardToPlay);
            currentMana -= cardToPlay.cost;
        }
    }

    public int Attack()
    {
        return board.GetAttack();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        hand.Reset();
        board.Reset();
        
        CreateDeck();

        for (int loop = 0; loop < cardCountStart; loop++)
        {
            hand.AddCard(Draw());
        }
    }
    
    #region Deck
    public void CreateDeck()
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
    

    #endregion
}