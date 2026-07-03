using Godot;
using System;
using static Character;

public partial class EnemyData : Resource
{
    [Export]
    public CharacterType characterType;
    [Export]
    public Vector2 GlobalPosition;
    [Export]
    public float height;
    [Export]
    public State state;
    [Export]
    public int door_index;

    public EnemySlot enemySlot;

    public const float HEIGHT_FALL = 50;
    public EnemyData(float temp_height, State temp_state, 
    CharacterType type = CharacterType.PUNK, Vector2? positon = null, int assigned_door_index = -1)
    {
        door_index = assigned_door_index;
        characterType = type;
        GlobalPosition = positon ?? Vector2.Zero;
        if (GlobalPosition.Y < 0)
        {
            height = HEIGHT_FALL;
            state = State.Drop;
            GlobalPosition = (Vector2)(positon + Vector2.Down * height);

        }
        else
        {
            height = temp_height;
            state = State.idle;
        }
    }
}
