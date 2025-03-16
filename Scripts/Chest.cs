using Godot;
using System;

public partial class Chest : StaticBody2D
{
	private AnimatedSprite2D _animatedSprite;
	private bool _isOpened = false;
	private Area2D _openArea;
	private bool _playerNear = false;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		_openArea = GetNode<Area2D>("OpenArea");
	}

	public override void _Process(double delta)
	{
		if (_playerNear && Input.IsActionJustPressed("open_chest") && !_isOpened)
		{
			OpenChest();
		}
	}

	private void OpenChest()
	{
		if (!_isOpened)
		{
			_animatedSprite.Play("open");
			_isOpened = true;
			GameManager.Instance.GameWon();
			
			GetTree().CreateTimer(1.5f).Timeout += () => {
				GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
			};
		}
	}

	public void _on_open_area_body_entered(Node body)
	{
		if (body is Player)
		{
			_playerNear = true;
		}
	}

	public void _on_open_area_body_exited(Node body)
	{
		if (body is Player)
		{
			_playerNear = false;
		}
	}
}
