using System;
using UniRx;

public class Hazard
{
    public float Height { get; private set; }
    public IObservable<float> Positions { get; private set; }
    public float InitialPosition { get; private set; }

    bool moving;

    public Hazard(float height, float initialPosition, float speed, IObservable<float> time)
    {
        Height = height;
        InitialPosition = initialPosition;
        moving = true;

        Positions = time
            .Where(delta => moving)
            .Scan(initialPosition, (position, deltaTime) => position + speed * deltaTime);
    }

    public void Stop()
    {
        moving = false;
    }
}