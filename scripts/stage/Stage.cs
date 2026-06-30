using Godot;
using System;

public partial class Stage : Node2D
{

	[Export]
	public Node2D Containers;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
    {
        HandlerOrphanActor();
    }

    private void HandlerOrphanActor()
    {
        foreach (Node2D child in Containers.GetChildren())
        {

            StageManager.Instance.EmitSignal(StageManager.SignalName.OrphanActor, child);

        }
    }

}
