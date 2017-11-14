using System;

public class RandomFloat
{
    readonly Random random;

    public RandomFloat(Random random)
    {
        this.random = random;
    }

    public float Next(float minValue, float maxValue)
    {
        return minValue + (float) random.NextDouble() * (maxValue - minValue);
    }
}