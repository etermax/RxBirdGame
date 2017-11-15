using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{
	public BirdBehaviour bird;
	public HazardBehaviour prefabHazard;
	public ObservableTrigger2DTrigger birdCollisions;
	public PointsView pointsView;
	public float spawnInterval = 4f;
	public float minHazardHeight = -3f;
	public float maxHazardHeight = 3f;
	public float hazardSpeed = -2f;
	public float initialPosition = 10f;

	void Start()
	{
		var time = GetTime();
		var randomService = new RandomFloat(new System.Random());
		var collisions = GetCollisions();

		var hazardSpawner = new HazardSpawner(
			time,
			spawnInterval,
			randomService,
			minHazardHeight,
			maxHazardHeight,
			initialPosition,
			hazardSpeed
		);

		bird.Setup(time, collisions);
		collisions.Subscribe(collisionEvent => hazardSpawner.Stop());
		hazardSpawner.Hazards.Subscribe(hazard => CreateHazard(hazard, collisions));
	}

	// Factories
	void CreateHazard(Hazard hazard, IObservable<Unit> collisions)
	{
		var hazardGameObject = Instantiate(prefabHazard);
		hazardGameObject.Setup(hazard, collisions);
	}

	IObservable<float> GetTime()
	{
		return Observable.EveryFixedUpdate().Select(t => Time.deltaTime);
	}

	IObservable<Unit> GetCollisions()
	{
		return birdCollisions.OnTriggerEnter2DAsObservable().Select(collider => Unit.Default);
	}
}
