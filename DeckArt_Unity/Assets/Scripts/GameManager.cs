using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player, player2;
    
    public int turnToPlay;
    public bool gameOver;

    public int winRate;
    
    [NonSerialized] public Player currentPlayer, otherPlayer;


    private void Awake()
    {
        Reset();
    }

    private void Update()
    {
        if(gameOver)
            return;
        PlayTurn();
    }

    public void PlayTurn()
    {
        currentPlayer.StartTurn();
        currentPlayer.PlayCards();
        otherPlayer.TakeDamage(currentPlayer.Attack());
        
        if(otherPlayer.currentHealth <= 0)
            Win(currentPlayer);
        
        (currentPlayer, otherPlayer) = (otherPlayer, currentPlayer);
    }

    public void Win(Player winner)
    {
        winRate += winner == player ? 0 : 1;
        Debug.Log(winner == player ? "Player 1" : "Player 2");
        Reset();
    }

    private void Reset()
    {
        if (turnToPlay == 0)
        {
            gameOver = true;
            Debug.Log((float)winRate/turnToPlay);
        }

        player.Reset();
        player2.Reset();
        
        currentPlayer = player;
        otherPlayer = player2;
    }
}
