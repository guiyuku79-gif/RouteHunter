using System.Collections.Generic;

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

    public (string numerator, string denominator) FractionToString()
    {
        string result1 = "";
        for (int i = 0; i < Numerator.Count; i++)
        {
            result1 += Numerator[i].MonomialToString();
            //if (i < Numerator.Count - 1) result1 += " ";
            if (i < Numerator.Count - 1 && Numerator[i + 1].Coefficient > 0 &&
                Numerator[i + 1].Number > 0) result1 += "+";
        }
        string result2 = "";
        for (int i = 0; i < Denominator.Count; i++)
        {
            result2 += Denominator[i].MonomialToString();
            //if (i < Denominator.Count - 1) result2 += " ";
            if (i < Denominator.Count - 1 && Denominator[i + 1].Coefficient > 0 &&
                Denominator[i + 1].Number > 0) result2 += "+";
        }

        return (result1, result2);
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