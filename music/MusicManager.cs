using Godot;
using System;
using System.Collections.Generic;

public partial class MusicManager : Node
{
    [Export]
    public AudioStreamPlayer music_stream_player;


    public enum Music
    {
        STAGE01
    }
    public static MusicManager incetance { get; set; }

    
    public AudioStream autoplayed_music = null;
    public override void _EnterTree()
    {
        incetance = this;
    }
    public override void _Ready()
    {

        if(autoplayed_music!=null){
            music_stream_player.Stream = autoplayed_music;
            music_stream_player.Play();
        }


    }




    public Dictionary<Music, AudioStream> musicMenu = new()
    {
        {Music.STAGE01,GD.Load<AudioStream>("res://assets/music/stage-01.mp3")}
    };

    public void Play(Music music)
    {
        if (music_stream_player.IsNodeReady())
        {
            music_stream_player.Stream = musicMenu[music];
            music_stream_player.Play();
            
        }
        else
        {
            autoplayed_music = musicMenu[music];
        }


    }
}
