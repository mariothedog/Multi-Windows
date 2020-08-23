using Godot;

namespace MultiWindows.entities
{
	public abstract class BaseEntity : KinematicBody2D
	{
		public Vector2 Velocity { get; protected set; }
		public int Speed { get; protected set; }

		public override void _PhysicsProcess(float delta)
		{
			Move(delta);
		}

		protected virtual void Move(float delta)
		{
			GD.Print("=Before MoveAndSlide= Position: ", Position, " - ", "Slide Count: ", GetSlideCount(), " - ", "Velocity: ", Velocity);

			Velocity = MoveAndSlide(Velocity, Vector2.Up);

			GD.Print("=After MoveAndSlide= Position: ", Position, " - ", "Slide Count: ", GetSlideCount(), " - ", "Velocity: ", Velocity);
			GD.Print("");
		}
	}
}
