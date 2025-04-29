using NUnit.Framework.Internal.Commands;
using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Recorders;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public const int GAMES_TO_PLAY = 500;

    public Player player;

    [Button(nameof(StartGames))]
    [SerializeField] private string m_referencePlayerJsonPath;

    [NonSerialized] private Player referencePlayer;

    [NonSerialized] public int gamesToPlay = GAMES_TO_PLAY;
    [NonSerialized] public bool gameOver;

    [NonSerialized] public int winRate;

    [NonSerialized] public Player currentPlayer, otherPlayer;

    private void StartGames()
    {
        Recorder recorder = new Recorder();

        recorder.AddEvent("Init");

        Metrics.GetInstance().WriteShit("shit");

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

        winRate += winner == player ? 1 : 0;
        //Debug.Log(winner == player ? "Player 1" : "Player 2");
    }

    private void ResetGame()
    {
        if (--gamesToPlay == 0)
        {
            gameOver = true;
            Debug.Log((float)winRate / GAMES_TO_PLAY);
        }

        player.Reset(true);
        referencePlayer.Reset(false);


        (currentPlayer, otherPlayer) = Random.value > 0.5f ? (player, referencePlayer) : (referencePlayer, player);
    }
}
