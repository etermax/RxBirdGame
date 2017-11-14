using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

public class HazardSpawnerTest
{
    const float TimeDelta = 1 / 60f;
    const float SpawnHazardsEvery = 0.25f;
    const int RandomSeed = 0;
    const int HazardsCount = 4;
    const float MinHazardHeight = -10f;
    const float MaxHazardHeight = 10f;
    const float TimeToCreateHazards = SpawnHazardsEvery * HazardsCount;
    const float HazardInitialPosition = 0f;
    const float HazardsSpeed = -10;

    IObservable<float> time;
    HazardSpawner hazardSpawner;
    List<Hazard> spawnedHazards;

    [UnityTest]
    public IEnumerator EveryTSecondsAHazardSpawns()
    {
        GivenTime(TimeToCreateHazards);
        GivenAHazardSpawner();

        yield return WhenTimePasses(TimeToCreateHazards);

        ThenAHazardHasSpawned(spawnedHazards);
    }

    [UnityTest]
    public IEnumerator HazardsSpawnWithRandomHeight()
    {
        GivenTime(TimeToCreateHazards);
        GivenAHazardSpawner();

        yield return WhenTimePasses(TimeToCreateHazards);

        ThenHazardsHaveExpectedHeights(spawnedHazards);
    }

    [UnityTest]
    public IEnumerator HazardsStopSpawning()
    {
        GivenTime(TimeToCreateHazards);
        GivenAHazardSpawner();

        yield return WhenSpawnerIsStoppedTimePasses(TimeToCreateHazards);

        ThenHazardsDoNotSpawn();
    }

    void GivenTime(float seconds)
    {
        time = Observable.EveryUpdate()
            .Scan(0f, (accum, current) => accum + TimeDelta)
            .TakeWhile(totalTime => totalTime < seconds)
            .Select(totalTime => TimeDelta);
    }

    void GivenAHazardSpawner()
    {
        var randomService = new RandomFloat(new Random(RandomSeed));
        hazardSpawner = new HazardSpawner(
            time,
            SpawnHazardsEvery,
            randomService,
            MinHazardHeight,
            MaxHazardHeight,
            HazardInitialPosition,
            HazardsSpeed
        );
    }

    WaitForSeconds WhenTimePasses(float seconds)
    {
        spawnedHazards = new List<Hazard>();
        hazardSpawner.Hazards.Subscribe(hazard => spawnedHazards.Add(hazard));
        return new WaitForSeconds(seconds);
    }

    IEnumerator WhenSpawnerIsStoppedTimePasses(float seconds)
    {
        hazardSpawner.Stop();
        yield return WhenTimePasses(seconds);
    }

    void ThenAHazardHasSpawned(List<Hazard> spawnedHazards)
    {
        Assert.AreEqual(HazardsCount, spawnedHazards.Count);
    }

    void ThenHazardsHaveExpectedHeights(List<Hazard> spawnedHazards)
    {
        var randomService = new RandomFloat(new Random(RandomSeed));

        foreach (var hazard in spawnedHazards) {
            var randomHeight = randomService.Next(MinHazardHeight, MaxHazardHeight);
            AssertAreApproximatelyEqual(hazard.Height, randomHeight, 0.01f);
        }
    }

    void ThenHazardsDoNotSpawn()
    {
        Assert.IsEmpty(spawnedHazards);
    }

    void AssertAreApproximatelyEqual(float expected, float got, float delta)
    {
        if (!Approximately(expected, got, delta))
            Assert.Fail("Expected: " + expected + " got: " + got + " with delta: " + delta);
    }

    static bool Approximately(float a, float b, float delta)
    {
        return Mathf.Abs(a - b) < Mathf.Abs(delta);
    }
}
