using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BirdBehaviour : MonoBehaviour
{
	public float gravity = 10;
	public float jumpBoost = 50f;

	public void Setup(IObservable<float> time, IObservable<Unit> collisions)
	{
		var bird = new Bird(transform.position.y, -gravity, jumpBoost);

		// Transform time into bird positions
		time.Select(n => bird.Update(Time.deltaTime))
			// Get bird position events and actually move the bird
			.Subscribe(state => ChangePosition(state));

		// From every frame, get only the frames where the mouse button was pressed down
		Observable.EveryUpdate()
			.Where(e => Input.GetMouseButtonDown(0))
			// Get the mouse events and make the bird jump every time
			.Subscribe(e => bird.Jump());

		// Get collision events and make the bird react to them 
		collisions.Subscribe(c => bird.Collide());
	}

	void ChangePosition(State state)
	{
		transform.position = new Vector3(transform.position.x, state.position, transform.position.z);
	}
}
