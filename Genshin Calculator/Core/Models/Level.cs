using System;

namespace Genshin_Calculator.Core.Models;

public class Level
{
    public Level(int value, bool isAscended)
    {
        this.Value = value;
        this.IsAscended = isAscended;
    }

    public int Value { get; }

    public bool IsAscended { get; }

    public static Level Parse(string input)
    {
        bool hasStar = input.Contains('★');
        int value = int.Parse(input.Replace("★", string.Empty));
        return new Level(value, hasStar);
    }

    public override bool Equals(object? obj)
    {
        return obj is Level other &&
               this.Value == other.Value &&
               this.IsAscended == other.IsAscended;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value, this.IsAscended);
    }

    public override string ToString()
        => this.IsAscended ? $"{this.Value}★" : this.Value.ToString();

    public int CompareTo(Level other)
    {
        int valueCompare = this.Value.CompareTo(other.Value);

        if (valueCompare != 0)
        {
            return valueCompare;
        }

        return this.IsAscended.CompareTo(other.IsAscended);
    }
}