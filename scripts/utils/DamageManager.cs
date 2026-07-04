using Godot;
using System;

public partial class DamageManager : Node
{
    public static DamageManager Instance { get; set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    [Signal]
    public delegate void HealthChangeEventHandler(Character.CharacterType type,int currentHealth,int max_health);

}
