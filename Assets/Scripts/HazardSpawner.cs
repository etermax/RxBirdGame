using System;
using System.Collections.Generic;
using UniRx;

public class HazardSpawner
{
	public UniRx.IObservable<Hazard> Hazards { get; private set; }

	bool spawning;

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
		Func<Hazard> hazardFactory =
			() => new Hazard(randomService.Next(minHazardHeight, maxHazardHeight), initialPosition, hazardSpeed, time);

		spawning = true;

		Hazards = time
			.Buffer(Span(spawnInterval))
			.StartWith(FirstSpawnEvent())
			.Where(_ => spawning)
			.Select(l => hazardFactory());
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
}