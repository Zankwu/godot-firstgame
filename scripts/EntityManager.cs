using Godot;
using System;
using System.Collections.Generic;


public partial class EntityManager : Node
{
    public static EntityManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal]
    public delegate void SpawnCollectiblesEventHandler(int type, int state,
   Vector2 collectiblePosition, Vector2 direction,float height);

    [Signal]
    public delegate void ClearCollectibleEventHandler();

}
