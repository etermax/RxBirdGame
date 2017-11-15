using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BirdBehaviour : MonoBehaviour
{
	public float RotationFactor = 10f;
	public float gravity = 10;
	public float jumpBoost = 50f;

	public void Setup(IObservable<float> time, IObservable<Unit> collisions)
	{
		var bird = new Bird(time, transform.position.y, -gravity, jumpBoost);

		// Subscribe to bird positions and update transform position
		bird.VerticalPosition.Subscribe(position => ChangePosition(position));
		
		// Subscribe to bird velocities and update transform rotation
		bird.VerticalVelocity.Subscribe(velocity => ChangeOrientation(velocity));

		// From every frame, get only the frames where the mouse button was pressed down
		Observable.EveryUpdate()
			.Where(e => Input.GetMouseButtonDown(0))
			// Get the mouse events and make the bird jump every time
			.Subscribe(e => bird.Jump());

		// Get collision events and make the bird react to them 
		collisions.Subscribe(c => bird.Collide());
	}

	void ChangeOrientation(float velocity)
	{
		transform.rotation = Quaternion.Euler(0, 0, velocity*RotationFactor);
	}

	void ChangePosition(float position)
	{
		transform.position = new Vector3(transform.position.x, position, transform.position.z);
	}
}
