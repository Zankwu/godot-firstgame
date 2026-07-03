using Godot;
using System;

public partial class DamageManager : Node
{
    public static DamageManager Instance { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal]
    public delegate void HealthChangeEventHandler(int temp);

}
