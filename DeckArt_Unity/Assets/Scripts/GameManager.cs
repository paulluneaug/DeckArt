using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class GameManager : MonoBehaviour
{
    public Player player;

    [Button(nameof(LoadPlayer))]
    [SerializeField] private string m_referencePlayerJsonPath;

    [NonSerialized] private Player player2;
    
    [NonSerialized]public int turnToPlay;
    [NonSerialized]public bool gameOver;

    [NonSerialized] public int winRate;

    [NonSerialized] public Player currentPlayer, otherPlayer;


    private void Awake()
    {

        LoadPlayer();
        Reset();
    }

    private void LoadPlayer()
    {
        TextAsset json = Resources.Load<TextAsset>(m_referencePlayerJsonPath);
        player2 = Player.FromJson(json.text);
        player2.deck.ForEach(card => { Debug.Log(card); });
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

        if (otherPlayer.currentHealth <= 0)
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
