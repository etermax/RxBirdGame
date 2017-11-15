using UniRx;

public class Bird
{
	float acceleration = 10f;
	float jumpBoost = 10f;
	bool dead = false;

	float verticalVelocity;
	float verticalPosition;
	
	public IObservable<float> VerticalPosition { get; private set; }
	
	public Bird(IObservable<float> time, float initialPosition, float acceleration, float jumpBoost)
	{
		verticalPosition = initialPosition;
		verticalVelocity = 0;
		this.acceleration = acceleration;
		this.jumpBoost = jumpBoost;

		VerticalPosition = time
			.Select(Update)
			.Select(state => state.position);

	}

	public State Update(float delta)
	{
		verticalVelocity += acceleration * delta;
		verticalPosition += verticalVelocity * delta;

		return State.Create(verticalPosition, verticalVelocity);
	}

	public void Jump()
	{
		if (!dead)
			verticalVelocity = jumpBoost;
	}

	public void Collide()
	{
		if (!dead)
		{
			verticalVelocity = 0;
			dead = true;
		}
	}
}
