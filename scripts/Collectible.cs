using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class Collectible : Area2D
{

    public const float GRAVITY = 600.0f;

    public float speed = 0.0f;

    public float height = 0.0f;

    public float heightSpeed = 0.0f;
    [Export]
    public float knockDown = 0.0f;


    public State currentState = State.fall;
    public enum State
    {
        fall, grounded, fly
    }

    public AnimationPlayer collectiblePlayer;

    public Sprite2D collectibleSprite2D;
    protected Dictionary<State, string> stateAnima = new()
    {
        {State.fall , "fall"},
        {State.grounded , "grounded"},
        {State.fly , "fly"},
    };

    public override void _Ready()
    {
        base._Ready();
        collectiblePlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        collectibleSprite2D = GetNode<Sprite2D>("CollectibleSprite2D");
        heightSpeed = knockDown;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleFall(delta);
        collectibleSprite2D.Position = Vector2.Up * height;
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        collectiblePlayer.Play(stateAnima[currentState]);
    }


    private void HandleFall(double delta)
    {
        if (currentState == State.fall)
        {
            height += (float)(heightSpeed * delta);

            if (height < 0)
            {
                height = 0;
                currentState = State.grounded;
            }
            else
            {
                heightSpeed -= (float)(GRAVITY * delta);
            }
        }
        GD.Print($"height: {height}");
    }
}

