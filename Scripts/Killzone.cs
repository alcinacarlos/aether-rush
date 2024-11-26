using Godot;
using System;

public partial class Killzone : Area2D
{
	private Timer timer;
	public void _on_body_entered(Node body){
		timer = GetNode<Timer>("Timer");
		Engine.TimeScale = 0.5;
		body.GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();

		timer.Start();
	}

	public void _on_timer_timeout(){
		Engine.TimeScale = 1;
		GetTree().ReloadCurrentScene();
	}
}
