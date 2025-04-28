using System;
using System.Collections.Generic;
using TMPro;
using UnityUtility.Extensions;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [Header("Card Display")]
    [SerializeField] private Transform m_cardDisplayParent;
    [SerializeField] private GameObject m_prefabCardDisplay;

    [NonSerialized] private List<Vector3> points;
    
    public void DisplayAllCards(List<Card> cards)
    {
        int childCount = m_cardDisplayParent.childCount;
        for (int loop = 0; loop < childCount; loop++)
        {
            m_cardDisplayParent.GetChild(0).gameObject.Destroy();
        }
        
        cards.Sort((x, y) => y.score.CompareTo(x.score));
        
        foreach (Card card in cards)
        {
            DisplayCard displayCard = Instantiate(m_prefabCardDisplay, m_cardDisplayParent).GetComponent<DisplayCard>();
            displayCard.Init(card);
        }
    }

    public void CreateGraph(List<Card> cards)
    {
        points = new List<Vector3>();

        foreach (Card card in cards)
        {
            points.Add(new Vector3(card.cost, card.score/100.0f, 0));
        }
    }

    private void OnDrawGizmos()
    {
        points.Sort((first, second) => first.x.CompareTo(second.x));

        float previousX = -1;
        float baseRadius = .02f;
        Dictionary<Vector3, float> spheres = new();
        
        Gizmos.color = Color.blue;
        for (int loop = 0; loop < points.Count; loop++)
        {
            Vector3 point = points[loop];
            
            if (!spheres.TryAdd(point, baseRadius))
            {
                spheres[point] += 0.01f;
            }
            
            if (Mathf.Approximately(previousX, point.x))
            {
                Gizmos.DrawLine(points[loop-1]/5, points[loop]/5);
            }
            
            previousX = point.x;
        }

        Gizmos.color = Color.red;
        foreach (KeyValuePair<Vector3, float> sphere in spheres)
        {
            Gizmos.DrawSphere(sphere.Key/5, sphere.Value);
        }
    }
}
