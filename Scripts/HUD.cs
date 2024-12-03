using Godot;
using System;


public partial class HUD : CanvasLayer
{
    private Label _coinCountLabel;
    private AnimatedSprite2D _coinSprite;

    private int _currentCoins = 0;

    public override void _Process(double delta)
    {
        UpdateCoinCount();
    }

    public override void _Ready()
    {
        _coinCountLabel = GetNode<Label>("CoinCountLabel");
        _coinSprite = GetNode<AnimatedSprite2D>("CoinSprite");

        UpdateCoinCount();
        _coinSprite.Play("idle");
        GD.Print(GameManager.Instance);

    }

    public void UpdateCoinCount()
    {
        var newCoinCount = GameManager.Instance.Money;
        {
            if (newCoinCount > _currentCoins)
            {
                _coinSprite.Play("collect");
            }

            _currentCoins = newCoinCount;
            _coinCountLabel.Text = _currentCoins.ToString();
        }
    }
}

