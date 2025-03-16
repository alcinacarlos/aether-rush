using Godot;
using System;

public partial class Menu : Control
{
	private Button playButton;
	private LineEdit inputField;

	public override void _Ready()
	{
		playButton = GetNode<Button>("VBoxContainer/Play");
		inputField = GetNode<LineEdit>("TextInput");

		playButton.Disabled = true;
		inputField.TextChanged += OnTextChanged;
	}

	private void OnTextChanged(string newText)
	{
		playButton.Disabled = string.IsNullOrWhiteSpace(newText);
	}

	private void _on_play_pressed()
	{
		if (!playButton.Disabled)
		{
			GameManager.Instance.SetPlayerName(inputField.Text);

			GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
		}
	}

	private void _on_options_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/options.tscn");
	}

	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}
}
