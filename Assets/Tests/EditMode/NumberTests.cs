using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NumberTests
{
    [Test]
    public void RootedMonomial_EvaluatesCoefficientTimesRoot()
    {
        var term = new Monomial(
            Coefficient: 3,
            number: 2,
            isRotted: true
        );

        Assert.That(
            term.ToFloat(),
            Is.EqualTo(3f * Mathf.Sqrt(2f)).Within(0.0001f)
        );

        Assert.That(term.MonomialToString(), Is.EqualTo("3√2"));
    }

    [Test]
    public void Fraction_WithoutDenominator_UsesOne()
    {
        var fraction = new Fraction(
            new List<Monomial>
            {
                new Monomial(number: 5)
            }
        );

        Assert.That(fraction.ToFloat(), Is.EqualTo(5f));

        var text = fraction.FractionToString();

        Assert.That(text.numerator, Is.EqualTo("5"));
        Assert.That(text.denominator, Is.EqualTo("1"));
    }

    [Test]
    public void Fraction_EvaluatesSumDividedByDenominator()
    {
        var fraction = new Fraction(
            numerator: new List<Monomial>
            {
                new Monomial(number: 2, isRotted: true),
                new Monomial(number: 3, isRotted: true)
            },
            denominator: new List<Monomial>
            {
                new Monomial(number: 2)
            }
        );

        float expected = (Mathf.Sqrt(2f) + Mathf.Sqrt(3f)) / 2f;

        Assert.That(
            fraction.ToFloat(),
            Is.EqualTo(expected).Within(0.0001f)
        );

        var text = fraction.FractionToString();

        Assert.That(text.numerator, Is.EqualTo("√2+√3"));
        Assert.That(text.denominator, Is.EqualTo("2"));
    }
}