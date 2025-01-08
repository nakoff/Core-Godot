using Godot;

namespace LooksLike.Framework.Services;

public interface IAudioService
{
    void PlaySound(AudioStream audioStream, Node3D? target = null, bool loop = false);
    void PlaySound(AudioStreamPlayer3D audioStreamPlayer, AudioStream audioStream, bool loop = false);
}

