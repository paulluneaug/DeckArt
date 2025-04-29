using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility.Extensions;

using Random = UnityEngine.Random;

[Serializable]
public class Player
{
    public const int DECK_SIZE = 30;
    public const int MAX_HEALTH = 30;

    public List<Card> deck;
    public string name;

    [NonSerialized] public int currentHealth;
    [NonSerialized] public int manaMax, currentMana;
    [NonSerialized] public int cardCountStart = 5;

    [NonSerialized] public Hand hand;
    [NonSerialized] public Board board;

    [NonSerialized] public List<Card> storedDeck;
    [NonSerialized] public List<Card> bestDeck;

    public Player()
    {
        hand = new Hand();
        board = new Board();
    }

    public void Init()
    {
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
            cardToPlay.score += ScoreFactorsHolder.Instance.PlayedCardRewardFactor;
            hand.RemoveCard(cardToPlay);
            board.AddCard(cardToPlay);
            currentMana -= cardToPlay.cost;

            cardToPlay = hand.GetCardToPlay(currentMana);

        }
    }

    public void SaveDeck(bool asBest)
    {
        storedDeck = new List<Card>(deck);

        if (asBest)
        {
            bestDeck = new List<Card>(deck);
        }
    }

    public void RestoreDeck(bool restoreBest)
    {
        deck = new List<Card>(restoreBest ? bestDeck : storedDeck);
    }

    public int Attack()
    {
        return board.GetAttack();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Reset(bool recreateDeck)
    {
        currentHealth = MAX_HEALTH;
        manaMax = 0;
        hand.Reset();
        board.Reset();

        if (recreateDeck)
        {
            CreateDeck();
        }
        else
        {
            RestoreDeck(false);
        }
        deck.Shuffle();

        for (int loop = 0; loop < cardCountStart; loop++)
        {
            hand.AddCard(Draw());
        }
    }

    public static Player FromJson(string json)
    {
        Player player = JsonUtility.FromJson<Player>(json);

        player.deck.ForEach(card => card.ComputeCost());
        player.SaveDeck(true);
        return player;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    #region Deck
    public void CreateDeck()
    {

        AssetList allCards = AssetList.GetAllPossibleCards();

        deck = allCards.cards.ChooseWeighted(DECK_SIZE, (Card card) => card.score);

    }

    public Card Draw()
    {
        Card drawnCard = deck[^1];
        deck.RemoveAt(deck.Count - 1);
        return drawnCard;
    }

    public void RewardWinningCards(float scoreFactor)
    {
        hand.RewardPlayedCards(scoreFactor);
    }
    #endregion



    public void IterateOnDeck(int cardsToModify)
    {
        Debug.LogError(cardsToModify);
        RestoreDeck(true);
        deck.Sort(CardsComparisons.ScoreComparer);

        for (int i = 0; i < cardsToModify; i++)
        {
            Card card = deck[i];
            deck[i] = GetCardToAdd(deck);
            Debug.Log($"Replaced {card} ({card.score}) with {deck[i]}");
        }

        Debug.LogWarning("Best Cards : ");
        for (int i = 1; i <= 10; i++)
        {
            Debug.LogWarning($" - {deck[^i]}({deck[^i].score})");
        }

        SaveDeck(false);
    }

    private Card GetCardToAdd(List<Card> currentDeck)
    {
        Card card = null;

        AssetList allCards = AssetList.GetAllPossibleCards();

        while (card == null)
        {
            card = allCards.cards[Random.Range(0, allCards.cards.Count - 1)];
            if (currentDeck.Contains(card))
            {
                card = null;
                continue;
            }
        }
        return card;
    }
}

public static class EnumerableExtension
{
    public static List<T> ChooseWeighted<T>(this IList<T> values, int count, Func<T, float> weightSelector)
    {
        List<T> result = new List<T>(count);

        float weightSum = values.Sum(x => weightSelector(x));
        HashSet<int> selectedIndices = new HashSet<int>();
        for (int i = 0; i < count; i++)
        {
            int selectedIndex;
            do
            {
                selectedIndex = GetWeightedIndex(values, weightSelector, Random.Range(0, weightSum));
            } while (selectedIndices.Contains(selectedIndex));

            selectedIndices.Add(selectedIndex);
            result.Add(values[selectedIndex]);
        }

        return result;
    }

    private static int GetWeightedIndex<T>(IList<T> values, Func<T, float> weightSelector, float randomValue)
    {
        float sum = 0;
        for (int i = 0; i < values.Count; i++)
        {
            sum += weightSelector(values[i]);
            if (sum >= randomValue)
            {
                return i;
            }
        }
        return -1;
    }
}