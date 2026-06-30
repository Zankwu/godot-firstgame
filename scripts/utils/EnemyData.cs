using Godot;
using System;
using static Character;

public partial class EnemyData : Resource
{
    [Export]
    public CharacterType characterType;
    [Export]
    public Vector2 GlobalPosition;

    public EnemyData(CharacterType type = CharacterType.PUNK ,Vector2? positon = null)
    {
        characterType = type;
        GlobalPosition = positon ?? Vector2.Zero;
    }
}
