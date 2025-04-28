using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player, player2;
    
    [NonSerialized] public Player currentPlayer, otherPlayer;


    private void Awake()
    {
        Reset();
    }

    private void Update()
    {
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
        Debug.Log(winner == player ? "Player 1" : "Player 2");
        Reset();
    }

    private void Reset()
    {
        player.Reset();
        player2.Reset();
        
        currentPlayer = player;
        otherPlayer = player2;
    }
}
