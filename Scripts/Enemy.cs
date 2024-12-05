using Godot;
using System;

public partial class Enemy : CharacterBody2D
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
    public float AttackCooldown = 0.5f;

    private RayCast2D RayCastLeft;
    private RayCast2D RayCastRight;
    private AnimatedSprite2D Sprite;

    private bool canAttack = true;
    private Timer attackTimer;

    private ProgressBar _healthBar;
    private Timer hideHealthBarTimer;

    private AnimatedSprite2D _animatedSprite2D;

    private bool _isDead = false;

    private Vector2 _velocity;

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
        attackTimer.Timeout += () => canAttack = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            _animatedSprite2D.Play("die");
            return;
        }

        _animatedSprite2D.Play("run");

        if (!IsOnFloor())
        {
            _velocity += GetGravity() * (float)delta;
        }
        else
        {
            _velocity.Y = 0;
        }

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
            else if(collider is not CharacterBody2D)
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
            else if(collider is not CharacterBody2D)
            {
                Direction = -1;
                Sprite.FlipH = false;
            }
        }

        _velocity.X = Direction * Speed;
        Velocity = _velocity;

        MoveAndSlide();
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
    private void Attack(){
        
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
