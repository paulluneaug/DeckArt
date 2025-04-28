using System;
using Unity.VisualScripting;

[Serializable]
public class Player
{
    [NonSerialized] public int maxHealth, currentHealth; 
    [NonSerialized] public int manaMax, currentMana;
    [NonSerialized] public int cardCountStart = 5;
    
    [NonSerialized] public Hand hand;
    [NonSerialized] public Board board;
    [NonSerialized] public Deck deck;

    public string name;

    public Player()
    {
        Reset();
    }

    public void StartTurn()
    {
        manaMax++;
        currentMana = manaMax;
        hand.AddCard(deck.Draw());
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

        for (int loop = 0; loop < cardCountStart; loop++)
        {
            hand.AddCard(deck.Draw());
        }
    }
}