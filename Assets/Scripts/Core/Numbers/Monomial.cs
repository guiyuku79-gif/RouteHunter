using UnityEngine;

public class Monomial
{
    public int Coefficient { get; private set; }
    public int Number { get; private set; }
    public bool IsRooted { get; private set; }
    public bool IsForthRooted { get; private set; }

    public Monomial(int Coefficient = 1, int number = 1, bool isRotted = false, bool isForthRooted = false)
    {
        this.Coefficient = Coefficient;
        this.Number = number;
        this.IsRooted = isRotted;
        this.IsForthRooted = isForthRooted;
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
        if (IsForthRooted) result += "f√";
        result += Number.ToString();
        return result;
    }

    public float ToFloat()
    {
        float result = Number;
        if (IsRooted) result = Mathf.Sqrt(result);
        if (IsForthRooted) result = Mathf.Sqrt(Mathf.Sqrt(result));
        return Coefficient * result;
    }

}