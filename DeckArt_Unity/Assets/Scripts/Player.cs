using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityUtility.Extensions;

[Serializable]
public class Player
{
    public const int DECK_SIZE = 30;
    
    public List<Card> deck;
    public string name;
    
    [NonSerialized] public int maxHealth, currentHealth; 
    [NonSerialized] public int manaMax, currentMana;
    [NonSerialized] public int cardCountStart = 5;
    
    [NonSerialized] public Hand hand;
    [NonSerialized] public Board board;
    
    
    public Player()
    {
        hand = new Hand();
        board = new Board();


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
        while (cardToPlay != null)
        {
            hand.RemoveCard(cardToPlay);
            board.AddCard(cardToPlay);
            currentMana -= cardToPlay.cost;

            cardToPlay = hand.GetCardToPlay(currentMana);
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

    public static Player FromJson(string json)
    {
        return JsonUtility.FromJson<Player>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    
    #region Deck
    public void CreateDeck()
    {
        deck = new List<Card>(DECK_SIZE);

        AssetList allCards = AssetList.GetAllPossibleCards();

        allCards.cards.Shuffle();
        for (int i = 0; i < DECK_SIZE; i++)
        {
            deck.Add(allCards.cards[i]);
        }
    }

    public Card Draw()
    {
        Card drawnCard = deck[^1];
        deck.RemoveAt(deck.Count - 1);
        return drawnCard;
    }
    

    #endregion
}