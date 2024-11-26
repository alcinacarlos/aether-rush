using Godot;
using System;

public partial class Enemy : Area2D
{
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

    public override void _Ready()
    {
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
