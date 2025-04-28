using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;
using UnityUtility.Recorders;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public const int GAMES_TO_PLAY = 10000;

    public Player player;

    [Button(nameof(StartGames))]
    [SerializeField] private string m_referencePlayerJsonPath; 
    
    [SerializeField] private Transform m_cardDisplayParent;
    [SerializeField] private GameObject m_prefabCardDisplay;


    [NonSerialized] private Player referencePlayer;

    [NonSerialized] public int gamesToPlay = GAMES_TO_PLAY;
    [NonSerialized] public bool gameOver;

    [NonSerialized] public int winRate;

    [NonSerialized] public Player currentPlayer, otherPlayer;

    private void StartGames()
    {
        Recorder recorder = new Recorder();

        recorder.AddEvent("Init");

        LoadReferencePlayer();

        gameOver = false;
        winRate = 0;
        gamesToPlay = GAMES_TO_PLAY;

        ResetGame();

        recorder.AddEvent("Game loop");

        while (!gameOver)
        {
            if (PlayTurn())
            {
                ResetGame();
            }
        }
        recorder.LogAllEvents();
    }

    private void Awake()
    {

        ResetGame();
    }

    private void LoadReferencePlayer()
    {
        TextAsset json = Resources.Load<TextAsset>(m_referencePlayerJsonPath);
        referencePlayer = Player.FromJson(json.text);
    }

    public bool PlayTurn()
    {
        currentPlayer.StartTurn();
        currentPlayer.PlayCards();
        int damage = currentPlayer.Attack();
        //Debug.Log($"Patate : {damage}");
        otherPlayer.TakeDamage(damage);

        if (otherPlayer.currentHealth <= 0)
        {
            Win(currentPlayer);
            return true;
        }

        (currentPlayer, otherPlayer) = (otherPlayer, currentPlayer);
        return false;
    }

    public void Win(Player winner)
    {
        //Debug.Log($"{player.currentHealth} - {referencePlayer.currentHealth}");
        if (winner == player)
        {
            player.Win(true);
            winRate++;
        }
        else
            player.Win(false);
        //Debug.Log(winner == player ? "Player 1" : "Player 2");
    }

    private void ResetGame()
    {
        if (--gamesToPlay == 0)
        {
            gameOver = true;
            Debug.Log((float)winRate / GAMES_TO_PLAY);
            DisplayAllCards(AssetList.GetAllPossibleCards().cards);
        }

        player.Reset(true);
        referencePlayer.Reset(false);


        (currentPlayer, otherPlayer) = Random.value > 0.5f ? (player, referencePlayer) : (referencePlayer, player);
    }

    private void DisplayAllCards(List<Card> cards)
    {
        int childCount = m_cardDisplayParent.childCount;
        for (int loop = 0; loop < childCount; loop++)
        {
            m_cardDisplayParent.GetChild(0).gameObject.Destroy();
        }
        
        cards.Sort((x, y) => y.score.CompareTo(x.score));
        
        foreach (Card card in cards)
        {
            DisplayCard displayCard = Instantiate(m_prefabCardDisplay, m_cardDisplayParent).GetComponent<DisplayCard>();
            displayCard.Init(card);
        }
    }
}
