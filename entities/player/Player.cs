using Godot;
using System;

namespace MultiWindows.entities.player
{
	public class Player : BaseGravityEntity
	{
		public Player()
		{
			Speed = 200;
			GravitySpeed = 200;
		}

		protected void GetInput()
		{
			Velocity = new Vector2((Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left")) * Speed, Velocity.y);
		}

		public override void _PhysicsProcess(float delta)
		{
			GetInput();
			base._PhysicsProcess(delta);
			Animate();
		}

		protected void Animate()
		{

		}
	}
}
