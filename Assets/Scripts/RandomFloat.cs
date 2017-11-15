using System;

public class RandomFloat
{
    //We used System.Random so that we can predict the values in the unit tests initializing it with a test seed.
    //System.Random only generates int, so we implemented our own float Next().
    
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