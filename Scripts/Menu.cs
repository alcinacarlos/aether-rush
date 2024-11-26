using Godot;
using System;

public partial class Menu : Control
{
	private void _on_play_pressed(){
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}
	private void _on_options_pressed(){
		GetTree().ChangeSceneToFile("res://Scenes/options.tscn");
	}
	private void _on_exit_pressed(){
		GetTree().Quit();
	}
}
