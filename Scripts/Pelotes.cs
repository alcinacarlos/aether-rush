using Godot;
using System;

public partial class Pelotes : Area2D
{
	[Export]
	public int Dinero;


	public void _on_body_entered(Node body){
		GameManager.Instance.add_money(Dinero);
		QueueFree();
	}

}