using UnityEngine;
using UnityUtility.Singletons;

[CreateAssetMenu(fileName = nameof(ScoreFactorsHolder), menuName = "Scriptable Object/" + nameof(ScoreFactorsHolder))]
public class ScoreFactorsHolder : ScriptableSingleton<ScoreFactorsHolder>
{
    public float WinningCardRewardFactor => m_winningCardRewardFactor;
    public float LosingCardRewardFactor => m_losingCardRewardFactor;
    public float PlayedCardRewardFactor => m_playedCardRewardFactor;


    [SerializeField] private float m_winningCardRewardFactor;
    [SerializeField] private float m_losingCardRewardFactor;
    [SerializeField] private float m_playedCardRewardFactor;
}
