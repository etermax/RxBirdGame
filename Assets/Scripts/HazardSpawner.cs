using System;
using System.Collections.Generic;
using UniRx;

public class HazardSpawner
{
    public UniRx.IObservable<Hazard> Hazards { get; private set; }

    bool spawning;
    RandomFloat randomService;
    float minHazardHeight;
	float maxHazardHeight;
    float initialPosition;
    float hazardSpeed;
    IObservable<float> time;

    public HazardSpawner(
        UniRx.IObservable<float> time,
        float spawnInterval,
        RandomFloat randomService,
        float minHazardHeight,
        float maxHazardHeight,
        float initialPosition,
        float hazardSpeed
    )
    {
        this.randomService = randomService;
		this.minHazardHeight = minHazardHeight;
		this.maxHazardHeight = maxHazardHeight;
        this.initialPosition = initialPosition;
        this.hazardSpeed = hazardSpeed;
        this.time = time;
        spawning = true;

		// Transform the time to emit events every 'spawnInterval' seconds 
        Hazards = time
            .Buffer(Span(spawnInterval))
			// Put an event at the beginning of the time-line
            .StartWith(FirstSpawnEvent())
			// Only spawn if 'spawning' is on
            .Where(_ => spawning)
			// transform the spawn events into hazards
            .Select(l => HazardFactory());
    }

    public void Stop()
    {
        spawning = false;
    }

    static List<float> FirstSpawnEvent()
    {
        return new List<float>();
    }

    TimeSpan Span(float spawnInterval)
    {
        return TimeSpan.FromSeconds(spawnInterval);
    }
    Hazard HazardFactory()
    {
        return new Hazard(randomService.Next(minHazardHeight, maxHazardHeight), initialPosition, hazardSpeed, time);
    }
}