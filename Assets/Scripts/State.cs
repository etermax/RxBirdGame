public struct State
{
	public float position;
	public float velocity;

	public static State Create(float position, float velocity)
	{
		State state;
		state.position = position;
		state.velocity = velocity;

		return state;
	}
}

