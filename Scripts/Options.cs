using Godot;
using System;

public partial class Options : Control
{
	private void _on_back_pressed(){
		GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
	}
}
