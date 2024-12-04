using Godot;
using System;

public partial class Enemy2 : CharacterBody2D
{
    [Export]
    public int MaxHealth = 30;
    private int _currentHealth;
    [Export]
    public int Speed = 60;

    [Export]
    public int Damage = 10;

    [Export]
    public int Direction = -1;

    [Export]
    public float AttackCooldown = 0.3f; // Tiempo en segundos entre ataques

    private RayCast2D RayCastLeft;
    private RayCast2D RayCastRight;
    private AnimatedSprite2D Sprite;

    private bool canAttack = true; // Indica si el enemigo puede atacar
    private Timer attackTimer;

    private ProgressBar _healthBar;
    private Timer hideHealthBarTimer;

    private AnimatedSprite2D _animatedSprite2D;

    private bool _isDead = false;


    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _currentHealth = MaxHealth;
        _healthBar = GetNode<ProgressBar>("ProgressBar");
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = MaxHealth;
        _healthBar.Visible = false;

        hideHealthBarTimer = new Timer();
        hideHealthBarTimer.OneShot = true;
        hideHealthBarTimer.Timeout += HideHealthBar;
        AddChild(hideHealthBarTimer);

        RayCastLeft = GetNode<RayCast2D>("RayCastLeft");
        RayCastRight = GetNode<RayCast2D>("RayCastRight");
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        attackTimer = new Timer();
        AddChild(attackTimer);
        attackTimer.OneShot = true;
        attackTimer.Timeout += () => canAttack = true; // Permite atacar nuevamente al terminar el tiempo
    }

    public override void _Process(double delta)
    {
        if (_isDead)
        {
            _animatedSprite2D.Play("die");
        }
        else
        {
            _animatedSprite2D.Play("run");
            if (RayCastLeft.IsColliding())
            {
                var collider = RayCastLeft.GetCollider();
                if (collider is Player player && canAttack)
                {
                    Vector2 knockbackDirection = (player.GlobalPosition - GlobalPosition).Normalized();
                    player.TakeDamage(Damage, knockbackDirection);
                    canAttack = false;
                    attackTimer.Start(AttackCooldown);
                }
                else
                {
                    Direction = 1;
                    Sprite.FlipH = true;
                }
            }

            if (RayCastRight.IsColliding())
            {
                var collider = RayCastRight.GetCollider();
                if (collider is Player player && canAttack)
                {
                    Vector2 knockbackDirection = (player.GlobalPosition - GlobalPosition).Normalized();
                    player.TakeDamage(Damage, knockbackDirection);
                    canAttack = false;
                    attackTimer.Start(AttackCooldown);
                }
                else
                {
                    Direction = -1;
                    Sprite.FlipH = false;
                }
            }
            Position += new Vector2((float)(Direction * Speed * delta), 0);
        }

    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _healthBar.Value = _currentHealth;
        _healthBar.Visible = true;
        hideHealthBarTimer.Stop();
        hideHealthBarTimer.Start(2.0f);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _animatedSprite2D.Play("die");
        GetTree().CreateTimer(2.0f).Timeout += () =>
        {
            QueueFree();
        };
    }

    private void HideHealthBar()
    {
        _healthBar.Visible = false;
    }
}
