using Godot;
using System;

public partial class Enemy : Node2D
{
    [Export]
    public int Speed = 60;

	[Export]
	public int Direction = -1;

	public RayCast2D RayCastLeft;
	public RayCast2D RayCastRight;

	public AnimatedSprite2D Sprite;
    public override void _Process(double delta)
    {
		RayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		RayCastRight = GetNode<RayCast2D>("RayCastRight");
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if(RayCastLeft.IsColliding()){
			Direction = 1;
			Sprite.FlipH = true;
		}
		if(RayCastRight.IsColliding()){
			Direction = -1;
			Sprite.FlipH = false;
		}

        Position += new Vector2((float)(Direction * Speed * delta), 0);
    }
}

