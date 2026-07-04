using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    [Export]
    public AudioStreamPlayer[] SFXStreams;

    public static SoundManager instance{get;set;}

    public override void _Ready()
    {
        instance = this;
    }

    public enum SFX
    {
        CLICK,EATFOOD,GOGOGO,GRUNT,GUNSHOT,HIT1,HIT2,KNIFEHIT,SWOOOO
    }

    public void Play(SFX sfx)
    {
        SFXStreams[(int)sfx].Play();
    }
}
