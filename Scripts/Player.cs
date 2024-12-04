using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export]
    public int MaxHealth = 100;
    private int currentHealth;

    [Export]
    public int Damage = 10;

    [Export]
    public float Speed = 200.0f;
    [Export]
    public float JumpVelocity = -400.0f;
    [Export]
    public float KnockbackForce = 300.0f;
    [Export]
    public float KnockbackDuration = 0.2f;

    private bool isKnockedBack = false;
    private Timer knockbackTimer;

    public AnimatedSprite2D animatedSprite2D;
    public ProgressBar healthBar;

    private bool isDead = false;
    private Timer hideHealthBarTimer;

    // Ataque
    [Export]
    public float AttackCooldown = 0.3f;  // Tiempo de recarga del ataque
    private bool canAttack = true;
    private Timer attackCooldownTimer;

    private Area2D attackArea;
    private bool isAttacking = false;

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

        attackCooldownTimer = new Timer();
        AddChild(attackCooldownTimer);
        attackCooldownTimer.OneShot = true;
        attackCooldownTimer.Timeout += ResetAttackCooldown;

        attackArea = GetNode<Area2D>("AttackArea");
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;
        healthBar.Value = currentHealth;
        healthBar.Visible = true;
        hideHealthBarTimer.Stop();
        hideHealthBarTimer.Start(2.0f);

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
        GetTree().CreateTimer(2.0f).Timeout += () =>
        {
            GameManager.Instance.reset_money();
            GetTree().ReloadCurrentScene();
        };
    }

    private void OnKnockbackEnd()
    {
        isKnockedBack = false;
    }

    private void ResetAttackCooldown()
    {
        canAttack = true;
    }

    private void PerformAttack()
    {
        if (!canAttack || isDead) return;

        isAttacking = true;
        canAttack = false;
        attackCooldownTimer.Start(AttackCooldown);


        animatedSprite2D.Play("attack");

        var enemies = attackArea.GetOverlappingBodies();
        GD.Print(enemies);
        foreach (var enemy in enemies)
        {
            if (enemy is Enemy enemyInstance)
            {
                //Vector2 knockbackDirection = (enemy.GlobalPosition - GlobalPosition).Normalized();
                enemyInstance.TakeDamage(Damage);
            }
        }

        GetTree().CreateTimer(0.4f).Timeout += () =>
        {
            isAttacking = false;
        };
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
            if (!isAttacking)
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

        // Realizar ataque
        if (Input.IsActionJustPressed("attack") && !isAttacking)
        {
            PerformAttack();
        }
    }

    private void HideHealthBar()
    {
        healthBar.Visible = false;
    }
}
