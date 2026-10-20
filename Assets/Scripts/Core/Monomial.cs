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
        if (Coefficient != 1)
        {
            if (Coefficient == -1) result += "-";
            else result = Coefficient.ToString();
        }
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

    public float ToLength()
    {
        float result = 0;
        if (Coefficient > 0) result += Coefficient.ToString().Length;
        if (Coefficient < 0) result += Coefficient.ToString().Length - 1;
        if (IsRooted) result += 0.5f;
        result += Number.ToString().Length;
        return result;
    }
}