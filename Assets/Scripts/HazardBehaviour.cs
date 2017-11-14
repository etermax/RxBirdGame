using UnityEngine;
using UniRx;
using System;

public class HazardBehaviour : MonoBehaviour
{
	public void Setup(Hazard hazard, IObservable<Unit> collisions)
	{
		var height = hazard.Height;

		transform.position = new Vector3(hazard.InitialPosition, height, 0);
		hazard.Positions.Subscribe(p => ChangePosition(p, height));
		collisions.Subscribe(c => hazard.Stop());
	}

	void ChangePosition(float p, float height)
	{
		transform.position = new Vector3(p, height, 0);
	}
}
