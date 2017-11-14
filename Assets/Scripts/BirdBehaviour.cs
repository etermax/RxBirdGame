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

		time.Select(n => bird.Update(Time.deltaTime))
			.Subscribe(state => ChangePosition(state));

		Observable.EveryUpdate()
			.Where(e => Input.GetMouseButtonDown(0))
			.Subscribe(e => bird.Jump());

		collisions.Subscribe(c => bird.Collide());
	}

	void ChangePosition(State state)
	{
		transform.position = new Vector3(transform.position.x, state.position, transform.position.z);
	}
}
