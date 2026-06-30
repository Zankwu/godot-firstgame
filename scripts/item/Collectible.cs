using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class Collectible : Area2D
{

    public const float GRAVITY = 600.0f;
    [Export]
    public float speed = 0.0f;
    public Vector2 direction = Vector2.Zero;

    public Vector2 velocity = Vector2.Zero;

    [Export]
    public float damage;

    public Area2D damageEmitter;
    public float height = 0.0f;
    public float heightTemp = 0.0f;

    public float heightSpeed = 0.0f;
    [Export]
    public float knockDown = 0.0f;
    [Export]
    public bool auto_destroyed;

    public State currentState = State.fall;
    public enum State
    {
        fall, grounded, fly
    }
    [Export]
    public Type currentType;

    public enum Type
    {
        knife,
        gun,
        food,
        nothing

    }
    public AnimationPlayer collectiblePlayer;

    public Sprite2D collectibleSprite2D;

    public Area2D collectibleArea2D;

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
        // collectibleArea2D = GetNode<Area2D>("Collectible");
        damageEmitter = GetNode<Area2D>("DamageEmitter");
        this.BodyEntered += ClearCollectible;
        damageEmitter.AreaEntered += onDamage;
        damageEmitter.Position = Vector2.Up * height;
        heightSpeed = knockDown;
        if (direction != Vector2.Zero)
        {
            collectibleSprite2D.Scale = new Vector2(direction.X, collectibleSprite2D.Scale.Y);
        }

    }

    private void ClearCollectible(Node2D body)
    {
        // EntityManager.Instance.EmitSignal(EntityManager.SignalName.CollectibleCollected, this);
        QueueFree();
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleFall(delta);

        collectibleSprite2D.Position = Vector2.Up * height;
        Position += direction * speed * (float)delta;
        HandleAnimation();

    }

    private void HandleAnimation()
    {
        collectiblePlayer.Play(stateAnima[currentState]);
    }


    private void HandleFall(double delta)
    {
        var modulate = Modulate;

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
            if (auto_destroyed == true)
                modulate.A *= (float)delta * height;
            if (modulate.A <= 0)
            {
                QueueFree();
            }



        }


    }

    public void onDamage(Area2D area2D)
    {

        area2D.EmitSignal(DamageReceiver.SignalName.DamageCompleted, damage, direction, (int)DamageReceiver.HitType.KNOCKDOWN);
        QueueFree();
    }
}

