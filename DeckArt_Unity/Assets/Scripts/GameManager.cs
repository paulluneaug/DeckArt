using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.MathU;
using UnityUtility.Recorders;
using UnityUtility.Utils;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Button(nameof(StartIterations))]
    [Button(nameof(PlayOneIteration))]
    [SerializeField] public int m_gamesToPlayPerIterations = 500;
    [SerializeField] public int m_iterationCount = 5;
    [SerializeField] public int m_modifiedCardsPerIteration = 5;
    [SerializeField] public bool m_decreaseCardsModified = false;

    [Title("JSON Paths")]

    [SerializeField] private string m_saveJsonPathFormat;
    [SerializeField] private string m_playerJsonPath;
    [SerializeField] private string m_referencePlayerJsonPath;

    [SerializeField, HideInInspector] public int m_iterationsOnDeck = 0;
    [NonSerialized] private Player referencePlayer, player;

    [NonSerialized] public int gamesToPlay;
    [NonSerialized] public bool gameOver;

    [NonSerialized] public int winCount;
    [NonSerialized] public float previousWinRate;

    [NonSerialized] public Player currentPlayer, otherPlayer;

    [NonSerialized] private float m_averageTurnThisIteration;


    private void StartIterations()
    {
        Recorder recorder = new Recorder();

        recorder.AddEvent("Init");

        LoadPlayers();
        previousWinRate = 0.0f;
        
        Metrics.GetInstance().WriteData("Game / Iteration", m_gamesToPlayPerIterations.ToString());
        Metrics.GetInstance().WriteData("Iteration Nb", m_iterationCount.ToString());

        for (int i = 0; i < m_iterationCount; i++)
        {
            float winRate = PlayIteration();
            Metrics.GetInstance().WriteData("Average Cost", player.averageCost.ToString());
            Metrics.GetInstance().WriteData("Average Attack", player.averageAtk.ToString());
            Metrics.GetInstance().WriteData("Average Defense", player.averageDef.ToString());
            Metrics.GetInstance().WriteData("Average Turn This Iteration", m_averageTurnThisIteration.ToString());
            FinishIteration(winRate, i);
        }
        
        Metrics.GetInstance().FlushAll();
        
        SavePlayerDeck();
    }

    private void SavePlayerDeck()
    {
        string playerDeckJson = player.ToJson();
        string savePath = string.Format(m_saveJsonPathFormat, m_iterationsOnDeck);
        File.WriteAllText(savePath, playerDeckJson);

        m_iterationsOnDeck++;
    }

    private void LoadPlayers()
    {
        TextAsset referencePlayerJson = Resources.Load<TextAsset>(m_referencePlayerJsonPath);
        referencePlayer = Player.FromJson(referencePlayerJson.text);
        referencePlayer.Init();

        TextAsset playerJson = Resources.Load<TextAsset>(m_playerJsonPath);
        player = Player.FromJson(playerJson.text);
        player.Init();
    }

    private void PlayOneIteration()
    {
        LoadPlayers();
        float iterationWinRate = PlayIteration();
        Debug.LogError($"WIN RATE : {iterationWinRate}");
    }

    private float PlayIteration()
    {
        InitIteration();

        m_averageTurnThisIteration = 0;

        while (!gameOver)
        {
            if (PlayTurn())
            {
                ResetGame();
            }
        }

        m_averageTurnThisIteration /= m_gamesToPlayPerIterations;
        float winRate = (float)winCount / m_gamesToPlayPerIterations;
        return winRate;
    }

    private void FinishIteration(float winRate, int iteration)
    {
        if (winRate < previousWinRate)
        {
            player.RestoreDeck(true);
        }
        else
        {
            player.RestoreDeck(false);
            player.SaveDeck(true);
            previousWinRate = winRate;
        }
        Debug.LogError($"WIN RATE : {winRate} vs {previousWinRate}");
        Metrics.GetInstance().WriteData("WinRate", winRate.ToString());
        Metrics.GetInstance().WriteData("Best WinRate", previousWinRate.ToString());


        player.IterateOnDeck(m_decreaseCardsModified ? MathUf.CeilToInt((1.0f - ((float)iteration / m_iterationCount)) * m_modifiedCardsPerIteration) : m_modifiedCardsPerIteration);
    }

    private bool PlayTurn()
    {
        currentPlayer.StartTurn();
        currentPlayer.PlayCards();
        int damage = currentPlayer.Attack();
        //Debug.Log($"Patate : {damage}");
        otherPlayer.TakeDamage(damage);

        m_averageTurnThisIteration++;

        if (otherPlayer.currentHealth <= 0)
        {
            Win(currentPlayer, otherPlayer);
            return true;
        }

        (currentPlayer, otherPlayer) = (otherPlayer, currentPlayer);
        return false;
    }

    public void Win(Player winner, Player loser)
    {
        //Debug.Log($"{player.currentHealth} - {referencePlayer.currentHealth}");

        winCount += winner == player ? 1 : 0;
        winner.RewardWinningCards(ScoreFactorsHolder.Instance.WinningCardRewardFactor);
        loser.RewardWinningCards(ScoreFactorsHolder.Instance.LosingCardRewardFactor);
        //Debug.Log(winner == player ? "Player 1" : "Player 2");
    }

    private void InitIteration()
    {
        gameOver = false;
        winCount = 0;
        gamesToPlay = m_gamesToPlayPerIterations;

        player.deck.ForEach(card => { card.score = 10.0f; });
        ResetGame();
    }

    private void ResetGame()
    {
        if (--gamesToPlay == 0)
        {
            gameOver = true;
        }

        player.Reset(false);
        referencePlayer.Reset(false);


        (currentPlayer, otherPlayer) = Random.value > 0.5f ? (player, referencePlayer) : (referencePlayer, player);
    }
}
