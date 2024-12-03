using Godot;
using System;

public partial class GameManager : Node
{
    [Export]
    public int Money = 0;

    public static GameManager Instance;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public void add_money(int money)
    {
        Money += money;
        GD.Print(Money);
    }
    public void reset_money()
    {
        Money = 0;
    }
}
