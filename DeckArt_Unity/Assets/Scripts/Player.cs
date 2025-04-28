using System;
using Unity.VisualScripting;

[Serializable]
public class Player
{
    public int maxHealth, currentHealth; 
    public int manaMax, currentMana;
    public int cardCountStart;
    
    public Hand hand;
    public Board board;

    public Player()
    {
        Reset();
    }

    public void StartTurn()
    {
        manaMax++;
        currentMana = manaMax;
        //hand.AddCard(deck.draw())
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
            //hand.AddCard(deck.draw())
        }
    }
}