using System;
using System.Collections.Generic;
using System.Globalization;
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

    [Title("Generated player")]
    [Button(nameof(GenerateRandomPlayer))]
    [SerializeField] private string m_generatedPlayerJsonPath;

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

        Metrics metrics = Metrics.GetInstance();
        metrics.ClearMetrics();
        metrics.WriteData("Games per iteration", m_gamesToPlayPerIterations.ToString());
        metrics.WriteData("Iteration Count", m_iterationCount.ToString());

        for (int i = 0; i < m_iterationCount; i++)
        {
            float winRate = PlayIteration();

            metrics.WriteData("Average Cost", player.averageCost.ToString("R", CultureInfo.InvariantCulture));
            metrics.WriteData("Average Attack", player.averageAtk.ToString("R", CultureInfo.InvariantCulture));
            metrics.WriteData("Average Defense", player.averageDef.ToString("R", CultureInfo.InvariantCulture));
            metrics.WriteData("Average Game Duration", m_averageTurnThisIteration.ToString("R", CultureInfo.InvariantCulture));

            for (int loop = 0; loop < 1 << Enum.GetValues(typeof(AssetList.Competences)).Length; loop++)
            {
                AssetList.Competences competence = (AssetList.Competences)loop;
                metrics.WriteData(competence.ToString(), player.averageCompetences[(int)competence].ToString());
            }
            

            FinishIteration(winRate, i);
        }

        metrics.FlushAll();
        
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
        Metrics.GetInstance().WriteData("WinRate", winRate.ToString("R", CultureInfo.InvariantCulture));
        Metrics.GetInstance().WriteData("Best WinRate", previousWinRate.ToString("R", CultureInfo.InvariantCulture));


        player.IterateOnDeck(m_decreaseCardsModified ? MathUf.CeilToInt((1.0f - ((float)iteration / m_iterationCount)) * m_modifiedCardsPerIteration) : m_modifiedCardsPerIteration);
    }

    private bool PlayTurn()
    {
        currentPlayer.StartTurn();
        currentPlayer.PlayCards();
        int attackingCardLeft = otherPlayer.Block(currentPlayer.board);
        int damage = currentPlayer.Attack(attackingCardLeft);
        otherPlayer.TakeDamage(damage);
        currentPlayer.EndAttack();
        otherPlayer.EndAttack();

        m_averageTurnThisIteration++;

        if (otherPlayer.currentHealth <= 0 || otherPlayer.deck.Count <= 0)
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

        AssetList.GetAllPossibleCards().cards.ForEach(card => { card.ResetScore(); });
        player.deck.ForEach(card => { card.ResetScore(); });
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


        (currentPlayer, otherPlayer) = gamesToPlay % 2 == 0 ? (player, referencePlayer) : (referencePlayer, player);
    }

    private void GenerateRandomPlayer()
    {
        Player generatedPlayer = new Player();
        generatedPlayer.Reset(true);

        File.WriteAllText(m_generatedPlayerJsonPath, generatedPlayer.ToJson());
    }
}
