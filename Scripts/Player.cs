using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;
	[Export]
	public float JumpVelocity = -400.0f;

	[Export]
	public bool isDead = false;

	public AnimatedSprite2D animatedSprite2D;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}


		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");

		if (direction[0] > 0)
		{
			animatedSprite2D.FlipH = false;
		}
		else if (direction[0] < 0)
		{
			animatedSprite2D.FlipH = true;
		}

		if (IsOnFloor())
		{
			if (direction[0] == 0)
			{
				animatedSprite2D.Play("idle");
			}
			else
			{
				animatedSprite2D.Play("run");
			}
		}else {
			animatedSprite2D.Play("jump");
		}
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
