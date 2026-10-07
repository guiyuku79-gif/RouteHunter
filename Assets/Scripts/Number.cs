using System.Collections.Generic;
using UnityEngine;

public class Monomial
{
    public int Coefficient { get; private set; }
    public int Number { get; private set; }
    public bool IsRooted { get; private set; }

    public Monomial(int Coefficient = 1, int number = 1, bool isRotted = false)
    {
        this.Coefficient = Coefficient;
        this.Number = number;
        this.IsRooted = isRotted;
    }

    public string MonomialToString()
    {
        string result = "";
        if (Coefficient != 1) result = Coefficient.ToString();
        if (IsRooted) result += "√";
        result += Number.ToString();
        return result;
    }

    public float ToFloat()
    {
        float result = Number;
        if (IsRooted) result = Mathf.Sqrt(result);
        return Coefficient * result;
    }
}

public class Fraction
{
    public List<Monomial> Denominator { get; private set; } //分母
    public List<Monomial> Numerator { get; private set; } //分子
    public Fraction(List<Monomial> numerator, List<Monomial> denominator = null)
    {
        if (denominator == null) this.Denominator = new List<Monomial> { new Monomial() };
        else this.Denominator = denominator;

        this.Numerator = numerator;
    }

    public string FractionToString()
    {
        string result = "(";
        for (int i = 0; i < Numerator.Count; i++)
        {
            result += Numerator[i].MonomialToString();
            result += " ";
            if (i < Numerator.Count - 1 && Numerator[i + 1].Coefficient > 0) result += "+";
        }
        result += ")/(";
        for (int i = 0; i < Denominator.Count; i++)
        {
            result += Denominator[i].MonomialToString();
            result += " ";
            if (i < Denominator.Count - 1 && Denominator[i + 1].Coefficient > 0) result += "+";
        }
        result += ")";

        return result;
    }
    public float ToFloat()
    {
        float DenominatorSum = 0;
        foreach (Monomial monomial in Denominator) DenominatorSum += monomial.ToFloat();
        float numeratorSum = 0;
        foreach (Monomial monomial in Numerator) numeratorSum += monomial.ToFloat();
        return numeratorSum / DenominatorSum;
    }
}

public static class Question
{
    //レベルに応じた問題が作られる
    public static Fraction CreateTargetNumber(int level)
    {
        switch (level)
        {
            case 1:
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:Random.Range(1,10))
                });
            case 2:
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:Random.Range(1,10),isRotted:true)
                });
        }

        return new Fraction(numerator: new List<Monomial>{});
    }
}