using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int MaxHealth = 100;

	private int currentHealth;

	[Export]
	public float Speed = 200.0f;

	[Export]
	public float JumpVelocity = -400.0f;

	[Export]
	public float KnockbackForce = 300.0f;

	[Export]
	public float KnockbackDuration = 0.2f; // Duración del empuje en segundos

	private bool isKnockedBack = false; // Indica si el jugador está en knockback
	private Timer knockbackTimer;

	public AnimatedSprite2D animatedSprite2D;
	public ProgressBar healthBar;

	private bool isDead = false;

	private Timer hideHealthBarTimer;

	public override void _Ready()
	{
		currentHealth = MaxHealth;
		healthBar = GetNode<ProgressBar>("ProgressBar");
		healthBar.MaxValue = MaxHealth;
		healthBar.Value = MaxHealth;
		healthBar.Visible = false;
		hideHealthBarTimer = new Timer();
    	hideHealthBarTimer.OneShot = true;
    	hideHealthBarTimer.Timeout += HideHealthBar;
    	AddChild(hideHealthBarTimer);

		knockbackTimer = new Timer();
		AddChild(knockbackTimer);
		knockbackTimer.OneShot = true;
		knockbackTimer.Timeout += OnKnockbackEnd;
	}

	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		currentHealth -= damage;
		healthBar.Value = currentHealth;
    	healthBar.Visible = true;
		hideHealthBarTimer.Stop();
    	hideHealthBarTimer.Start(2.0f);

		// Aplicar el empuje
		isKnockedBack = true;
		Velocity = knockbackDirection * KnockbackForce;
		knockbackTimer.Start(KnockbackDuration);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		isDead = true;
		animatedSprite2D.Play("die");
		GetTree().CreateTimer(2.5f).Timeout += () =>
		{
			GameManager.Instance.reset_money();
			GetTree().ReloadCurrentScene();
		};
	}


	private void OnKnockbackEnd()
	{
		isKnockedBack = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead)
		{
			Velocity = Vector2.Zero;
			return;
		}
		if (isKnockedBack)
		{
			Velocity += GetGravity() * (float)delta;
			MoveAndSlide();
			return;
		}

		Vector2 velocity = Velocity;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

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
		}
		else
		{
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

	private void HideHealthBar()
	{
		healthBar.Visible = false;
	}
}
