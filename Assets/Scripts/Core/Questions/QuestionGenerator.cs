using System.Collections.Generic;
using UnityEngine;

public static class QuestionGenerator
{
    //レベルに応じた問題が作られる
    public static Fraction CreateTargetNumber(int level)
    {
        switch (level)
        {
            case 1://整数
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:Random.Range(1,10))
                });
            case 2://√a 1~10
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:ChooseRootNumber(1,10),isRotted:true)
                });
            case 3://√a-b
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:ChooseRootNumber(1,10),isRotted:true),
                    new Monomial(number:-Random.Range(1,4)),
                });
            case 4://a√b
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(Coefficient:Random.Range(2,5),number:ChooseRootNumber(1,10),isRotted:true)
                });
            case 5://√a+√b
                return new Fraction(numerator: new List<Monomial>
                {
                    new Monomial(number:ChooseRootNumber(1,10),isRotted:true),
                    new Monomial(number:ChooseRootNumber(1,10),isRotted:true),
                });
            case 6: // 1 ÷ √a
                return new Fraction(
                    numerator: new List<Monomial>
                    {
                        new Monomial(number:1),
                    },
                    denominator: new List<Monomial>
                    {
                        new Monomial(number:ChooseRootNumber(2,6),isRotted:true),
                    }
                );
            case 7:// √a 11~30
                return new Fraction(
                    numerator: new List<Monomial>
                    {
                        new Monomial(number:ChooseRootNumber(11,30),isRotted:true),
                    }
                );
            case 8:// √a+√b ÷ c
                return new Fraction(
                    numerator: new List<Monomial>
                    {
                        new Monomial(number:ChooseRootNumber(1,15),isRotted:true),
                        new Monomial(number:ChooseRootNumber(1,15),isRotted:true),
                    },
                    denominator: new List<Monomial>
                    {
                        new Monomial(number:Random.Range(2,6),isRotted:false),
                    }
                );

        }

        throw new System.ArgumentOutOfRangeException(
            nameof(level),
            level,
            "問題生成が定義されていない難易度です。"
        );
    }

    static List<int> rootInNumbers = new List<int> { 2, 3, 5, 6, 7, 10, 11, 13, 14, 15, 17, 19, 21, 22, 23, 26, 29, 30, 31, 33, 34, 35, 37, 38, 39 };
    static int ChooseRootNumber(int min, int max)
    {
        List<int> numbers = new();

        foreach (int number in rootInNumbers)
        {
            if (min <= number && number <= max) numbers.Add(number);

        }
        if (numbers.Count == 0)
        {
            throw new System.InvalidOperationException(
                $"根号内の候補がありません。範囲: {min}～{max}"
            );
        }

        return numbers[Random.Range(0, numbers.Count)];
    }
}