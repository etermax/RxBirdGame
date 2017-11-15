using UnityEngine;
using UniRx;

public class HazardBehaviour : MonoBehaviour
{
	public void Setup(Hazard hazard, IObservable<Unit> collisions)
	{
		var height = hazard.Height;

		transform.position = new Vector3(hazard.InitialPosition, height, 0);

		// On every position event, move the hazard
		hazard.Positions.Subscribe(p => ChangePosition(p, height));

		// When a collision occurs, stop moving the hazard.
		collisions.Subscribe(c => hazard.Stop());
	}

	void ChangePosition(float p, float height)
	{
		transform.position = new Vector3(p, height, 0);
	}
}
