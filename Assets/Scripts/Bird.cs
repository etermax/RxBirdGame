using System;

public class Bird
{
	float acceleration = 10f;
	float jumpBoost = 10f;
	bool dead = false;

	public float VerticalVelocity { get; private set; }
	public float VerticalPosition { get; private set; }

	public Bird(float initialPosition, float acceleration, float jumpBoost)
	{
		VerticalPosition = initialPosition;
		VerticalVelocity = 0;
		this.acceleration = acceleration;
		this.jumpBoost = jumpBoost;
	}

	public State Update(float delta)
	{
		VerticalVelocity += acceleration * delta;
		VerticalPosition += VerticalVelocity * delta;

		return State.Create(VerticalPosition, VerticalVelocity);
	}

	public void Jump()
	{
		if (!dead)
			VerticalVelocity = jumpBoost;
	}

	public void Collide()
	{
		if (!dead)
		{
			VerticalVelocity = 0;
			dead = true;
		}
	}
}
