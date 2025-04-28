using UnityEngine;

public class DisplayCard : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI score, attack, defense;
    public void Init(Card card)
    {
        score.text = $"Score = {card.score}";
        attack.text = $"Attack = {card.attack}";
        defense.text = $"Defense = {card.defense}";
    }
}
