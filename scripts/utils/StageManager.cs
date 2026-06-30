using System;
using Godot;

public partial class StageManager : Node
{
    public static StageManager Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }
    
    [Signal]
    public delegate void CheckPointStartEventHandler();

    [Signal]
    public delegate void CheckPointEndEventHandler();
}