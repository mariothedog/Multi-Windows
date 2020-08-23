using Godot;

namespace MultiWindows.entities
{
	public abstract class BaseGravityEntity : BaseEntity
	{
		public int GravitySpeed { get; protected set; }

		public override void _PhysicsProcess(float delta)
		{
			ApplyGravity(delta);
			base._PhysicsProcess(delta);
		}

		private void ApplyGravity(float delta)
		{
			Velocity += new Vector2(0, GravitySpeed * delta);
		}
	}
}
