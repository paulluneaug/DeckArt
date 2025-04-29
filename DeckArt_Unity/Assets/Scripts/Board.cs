using System.Collections.Generic;

public class Board
{
    public List<Card> cards = new();
    
    public int GetAttack(int index)
    {
        int sum = 0;
        for (int loop = index; loop < cards.Count; loop++)
        {
            sum += cards[loop].attack;
        }
        
        return sum;
    }

    public List<Card> GetProvocCards()
    {
        List<Card> cardsProvoc = new List<Card>();
        foreach (Card card in cards)
        {
            if(card.HasCompetence(AssetList.Competences.Provoc))
                cardsProvoc.Add(card);
        }

        return cardsProvoc;
    }

    public void EndAttack()
    {
        cards.RemoveAll(x => x.currentDefense <= 0);
        cards.ForEach(x => x.EndTurn());
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
