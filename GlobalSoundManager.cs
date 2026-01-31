#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace ChloePrime.Godot.SimpleGlobalSound;

/// <summary>
/// Extension method designed to be called by other C# scripts.
/// </summary>
public static class GlobalSoundManagerExt
{
    /// <summary>
    /// <para>Plays a sound from the beginning.</para>
    /// </summary>
    public static void Play(this AudioStream? sound, float volumeDb = 0, float pitchScale = 1, StringName? bus = null)
    {
        GlobalSoundManager.PlayConfigured(sound, volumeDb, pitchScale, bus);
    }

    /// <summary>
    /// <para>Plays a sound from the beginning.</para>
    /// <para>Use <see cref="Play"/> unless you are using custom main loop.</para>
    /// </summary>
    public static void PlaySound(
        this Node node, AudioStream? sound, float volumeDb = 0, float pitchScale = 1, StringName? bus = null
    )
    {
        GlobalSoundManager.PlaySound(node, sound, volumeDb, pitchScale, bus);
    }
}

[GlobalClass]
public partial class GlobalSoundManager : Node
{
    public static readonly StringName MasterBusName = "Master";
    public const int MaxPooledPlayers = 64;
    
    /// <summary>
    /// <para>Plays a sound from the beginning, The played sound will not get stopped when this node is freed.</para>
    /// <para>This method is designed to be called by GDScript code. See <see cref="GlobalSoundManagerExt"/> for C# usage</para>
    /// </summary>
    public static void Play(AudioStream? sound)
    {
        // ReSharper disable once IntroduceOptionalParameters.Global
        PlayConfigured(sound, 0, 1, null);
    }

    /// <summary>
    /// <para>Plays a sound from the beginning, The played sound will not get stopped when this node is freed.</para>
    /// <para>This method is designed to be called by GDScript code. See <see cref="GlobalSoundManagerExt"/> for C# usage</para>
    /// </summary>
    public static void PlayConfigured(AudioStream? sound, float volumeDb, float pitchScale, StringName? bus)
    {
        (Engine.GetMainLoop() as SceneTree)?.Root.PlaySound(sound, volumeDb, pitchScale, bus);
    }
    
    /// <summary>
    /// <para>Plays a sound from the beginning.</para>
    /// <para>Use <see cref="PlayConfigured"/> unless you are using custom main loop.</para>
    /// <para>This method is designed to be called by GDScript code. See <see cref="GlobalSoundManagerExt"/> for C# usage</para>
    /// </summary>
    public static void PlaySound(Node node, AudioStream? sound, float volumeDb = 0, float pitchScale = 1, StringName? bus = null)
    {
        if (sound is null)
        {
            return;
        }

        var player = TryPurgeAndPop(out var p) ? p : NewPlayer();
        player.Stream = sound;
        player.VolumeDb = volumeDb;
        player.PitchScale = pitchScale;
        player.Bus = bus ?? MasterBusName;
        if (player.GetParent() is null)
        {
            node.GetTree().Root.AddChild(player);
        }
        player.Play();
    }

    private static AudioStreamPlayer NewPlayer()
    {
        var player = new AudioStreamPlayer();
        player.Finished += () =>
        {
            if (PlayerPool.Count >= MaxPooledPlayers)
            {
                player.QueueFree();
            }
            else
            {
                PlayerPool.Push(player);
            }
        };
        return player;
    }

    
    private static bool TryPurgeAndPop([NotNullWhen(true)] out AudioStreamPlayer? ret)
    {
        while (PlayerPool.TryPop(out var player))
        {
            // 清理跨场景时可能被释放的实例
            if (!IsInstanceValid(player))
            {
                continue;
            }

            ret = player;
            return true;
        }

        ret = null;
        return false;
    }

    private static readonly Stack<AudioStreamPlayer> PlayerPool = new();
}