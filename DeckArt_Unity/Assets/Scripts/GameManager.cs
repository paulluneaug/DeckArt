using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

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
        LoadReferencePlayer();

        gamesToPlay = GAMES_TO_PLAY;
        ResetGame();

        while (!gameOver)
        {
            if (PlayTurn())
            {
                ResetGame();
            }
        }
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
        otherPlayer.TakeDamage(currentPlayer.Attack());

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
        winRate += winner == player ? 0 : 1;
        Debug.Log(winner == player ? "Player 1" : "Player 2");
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

        currentPlayer = player;
        otherPlayer = referencePlayer;
    }
}
