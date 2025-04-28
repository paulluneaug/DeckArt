using System.Collections.Generic;

public class Board
{
    List<Card> cards = new();
    public int GetAttack()
    {
        int sum = 0;
        foreach (Card card in cards)
        {
            sum += card.atk;
        }
        return sum;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void Reset()
    {
        cards.Clear();
    }
}
