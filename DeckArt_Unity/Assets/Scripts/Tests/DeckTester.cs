using UnityEngine;
using UnityUtility.CustomAttributes;

public class DeckTester : MonoBehaviour
{
    [Button(nameof(TestDeckBuilder))]
    [SerializeField] private string m_deckJson;

    private void TestDeckBuilder()
    {
        AssetList assetList = AssetList.GetAllPossibleCards();
        Debug.Log(assetList.cards.Count);

        Debug.Log("Cards : ");
        assetList.cards.ForEach(card => { Debug.Log($"- {card.name}"); });

        Player player = new Player();
        Debug.Log("Deck : ");
        player.deck.ForEach(card => { Debug.Log($"- {card.name}"); });

        Player jsonPlayer = Player.FromJson(m_deckJson);
        Debug.Log("Deck json : ");
        jsonPlayer.deck.ForEach(card => { Debug.Log($"- {card.name}"); });

    }
}
