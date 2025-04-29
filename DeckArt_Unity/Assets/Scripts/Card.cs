using System;
using UnityUtility.MathU;

[Serializable]
public class Card
{
    public const int MAX_COST = 6;

    public string name;
    public int attack;
    public int defense;
    public AssetList.Competences competences;

    [NonSerialized] public int cost;
    [NonSerialized] public float score;
    [NonSerialized] public int currentDefense;

    public Card(int atk, int def, AssetList.Competences competences)
    {
        this.attack = atk;
        this.defense = def;
        this.competences = competences;
        Rename();

        EndTurn();
        cost = ComputeCardCost(atk, def, competences);
        ResetScore();
    }

    public void Rename()
    {
        name = $"Card_{attack}_{defense}_{competences.ToString()}";
    }

    public bool HasCompetence(AssetList.Competences competenceToTest)
    {
        return HasCompetence(this.competences, competenceToTest);
    }

    public void Block(Card card)
    {
        currentDefense -= card.attack;
        card.currentDefense -= attack;
    }

    public void EndTurn()
    {
        currentDefense = defense;
    }
    
    public override string ToString()
    {
        return name;
    }

    public override bool Equals(object obj)
    {
        if (obj is Card otherCard)
        {
            return otherCard.attack == attack && otherCard.defense == defense && otherCard.competences == competences;
        }
        return false;
    }

    public static int ComputeCardCost(int attack, int defense, AssetList.Competences competences)
    {
        int cost = (int)MathUf.Floor((attack + defense) / 2.0f);
        cost += HasCompetence(competences, AssetList.Competences.Provoc) ? 1 : 0;
        return cost;
    }
    
    public static bool HasCompetence(AssetList.Competences competences, AssetList.Competences competenceToTest)
    {
        return (competences & competenceToTest) != 0;
    }


    public void ComputeCost()
    {
        Rename();
        cost = ComputeCardCost(attack, defense, competences);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(attack, defense, competences);
    }

    public void ResetScore()
    {
        score = 10.0f;
    }
}
