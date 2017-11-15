using UniRx;
using UniRx.Triggers;
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

	int points = 0;

	void Start()
	{
		var time = GetTime();
		var randomService = new RandomFloat(new System.Random());
		var collisions = GetCollisions();

		//Create the Hazard Spawner
		var hazardSpawner = new HazardSpawner(
			time,
			spawnInterval,
			randomService,
			minHazardHeight,
			maxHazardHeight,
			initialPosition,
			hazardSpeed
		);

		//Initialize Bird
		bird.Setup(time, collisions);
		
		//When a collision event occurs, the hazard spawner stop spawning hazards
		collisions.Subscribe(collisionEvent => hazardSpawner.Stop());
		
		//Every time the HazardSpawner spawns a hazar, create a hazard view
		hazardSpawner.Hazards.Subscribe(hazard => CreateHazard(hazard, collisions));
	}

	// Factories
	void CreateHazard(Hazard hazard, IObservable<Unit> collisions)
	{
		//Instantiate and intiliaze a new Hazard GameObject using a prefab
		var hazardGameObject = Instantiate(prefabHazard);
		
		hazardGameObject.Setup(hazard, collisions);
		
		hazard.Positions
			.TakeWhile(positions => positions > 0f)
			.Last()
			.Subscribe(last => pointsView.Points = ++points);
	}

	IObservable<float> GetTime()
	{
		//Transform fixed update calls into time
		return Observable.EveryFixedUpdate().Select(t => Time.deltaTime);
	}

	IObservable<Unit> GetCollisions()
	{
		//Transform bird onEnterCollision events into just an empty event
		return birdCollisions.OnTriggerEnter2DAsObservable().Select(collider => Unit.Default);
	}
}
