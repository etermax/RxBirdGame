using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

public class HazardTest
{
    const float TimeDelta = 1 / 60f;
    const float TimeToMoveHazards = 4f;
    const float Height = 0f;
    const float Position = 0f;
    const float Speed = 0f;

    IObservable<float> time;
    Hazard hazard;
    List<float> positions;

    [Test]
    public void HazardMovesLeft()
    {
        GivenTime(TimeToMoveHazards);
        GivenAHazard();

        WhenTimePasses();

        ThenHazardsHaveMovedLeftWithConstantSpeed();
    }

    [Test]
    public void HazardDoesntMoveWhenStopped()
    {
        GivenTime(TimeToMoveHazards);
        GivenAHazard();

        WhenHazardIsStopedAndTimePasses();

        ThenHazardDoesntMove();
    }

    void WhenHazardIsStopedAndTimePasses()
    {
        WhenAHazardIsStopped();
        WhenTimePasses();
    }

    void WhenAHazardIsStopped()
    {
        hazard.Stop();
    }

    void GivenTime(float seconds)
    {
        time = Observable.Repeat(TimeDelta)
            .Scan(0f, (accum, current) => accum + TimeDelta)
            .TakeWhile(totalTime => totalTime < seconds)
            .Select(totalTime => TimeDelta);
    }

    void GivenAHazard()
    {
        hazard = new Hazard(Height, Position, Speed, time);
    }

    void WhenTimePasses()
    {
        positions = new List<float>();
        hazard.Positions.Subscribe(position => positions.Add(position));
    }

    void ThenHazardsHaveMovedLeftWithConstantSpeed()
    {
        var expectedPositions = positions.Select(position => position + TimeDelta * Speed).ToArray();

        for (int i = 1; i < positions.Count; i++)
            AssertApproximatesPosition(positions[i], expectedPositions[i - 1], 0.01f);
    }

    void ThenHazardDoesntMove()
    {
        Assert.IsEmpty(positions);
    }

    static void AssertApproximatesPosition(float expected, float got, float delta)
    {
        if (!Approximately(expected, got, delta))
            Assert.Fail("Expected: " + expected + " Got: " + got);
    }

    static bool Approximately(float a, float b, float delta)
    {
        return Mathf.Abs(a - b) < Mathf.Abs(delta);
    }
}
