using Godot;
using System;

public partial class Stage : Node2D
{

    [Export]
    public Node2D Containers;

    [Export]
    public Node2D Doors;

    [Export]
    public Node2D checkPoints;

    public override void _Ready()
    {
        CallDeferred(nameof(HandlerOrphanActor));
    }

    public override void _Process(double delta)
    {
        
        
    }

    private void HandlerOrphanActor()
    {
        foreach (Node2D child in Containers.GetChildren())
        {

            StageManager.Instance.EmitSignal(StageManager.SignalName.OrphanActor, child);

        }

        for (int i = 0; i < Doors.GetChildCount(); i++)
        {
            Door door = Doors.GetChild(i) as Door;
            foreach (var temp_enemy in door.enemies)
            {
                temp_enemy.assigned_door_index = i;
            }
        }
        foreach (Node2D door in Doors.GetChildren())
        {

            StageManager.Instance.EmitSignal(StageManager.SignalName.OrphanActor, door);

        }
        foreach (CheckPoint checkPoint in checkPoints.GetChildren())
        {
            checkPoint.CreateEnemyData();
        }


    }

}
