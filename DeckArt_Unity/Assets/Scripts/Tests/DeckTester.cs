using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public class DeckTester : MonoBehaviour
{
    [Button(nameof(TestDeckBuilder))]
    [SerializeField] private float m_test;

    private void TestDeckBuilder()
    {
        AssetList assetList = AssetList.CreateAllPossibleCards();
        Debug.Log(assetList.cards.Count);

        Debug.Log("Cards : ");
        assetList.cards.ForEach(card => { Debug.Log($"- {card.name}"); });
    }
}
