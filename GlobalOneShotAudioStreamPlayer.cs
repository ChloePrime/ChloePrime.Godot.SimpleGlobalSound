#nullable enable
using Godot;

namespace ChloePrime.Godot.SimpleGlobalSound;

[GlobalClass]
public partial class GlobalOneShotAudioStreamPlayer : Node
{
    /// <summary>
    /// <para>The <see cref="Godot.AudioStream"/> resource to be played. Setting this property stops all currently playing sounds. If left empty, the <see cref="Godot.AudioStreamPlayer"/> does not work.</para>
    /// </summary>
    [Export]
    public AudioStream? Stream { get; set; }

    /// <summary>
    /// <para>Volume of sound, in decibels. This is an offset of the <see cref="Godot.AudioStreamPlayer.Stream"/>'s volume.</para>
    /// <para><b>Note:</b> To convert between decibel and linear energy (like most volume sliders do), use <see cref="Godot.AudioStreamPlayer.VolumeLinear"/>, or <c>@GlobalScope.db_to_linear</c> and <c>@GlobalScope.linear_to_db</c>.</para>
    /// </summary>
    [Export]
    public float VolumeDb { get; set; }

    /// <summary>
    /// <para>The audio's pitch and tempo, as a multiplier of the <see cref="Godot.AudioStreamPlayer.Stream"/>'s sample rate. A value of <c>2.0</c> doubles the audio's pitch, while a value of <c>0.5</c> halves the pitch.</para>
    /// </summary>
    [Export]
    public float PitchScale { get; set; } = 1;

    /// <summary>
    /// <para>The target bus name. All sounds from this node will be playing on this bus.</para>
    /// <para><b>Note:</b> At runtime, if no bus with the given name exists, all sounds will fall back on <c>"Master"</c>. See also <see cref="Godot.AudioServer.GetBusName(int)"/>.</para>
    /// </summary>
    [Export]
    public StringName? Bus { get; set; } = null;

    /// <summary>
    /// <para>Plays a sound from the beginning, The played sound will not get stopped when this node is freed.</para>
    /// </summary>
    public void Play()
    {
        Stream.Play(VolumeDb, PitchScale, Bus);
    }
}