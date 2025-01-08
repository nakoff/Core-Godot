using LooksLike.Utils;
using Godot;
using System.Collections.Generic;
using LooksLike.Services;

namespace LooksLike.Framework.Services;

public class AudioService : ServiceBase, IAudioService, IUpdate
{
	private Node _parentDefault = null!;
	private List<ActiveAudioPlayer> activePlayers = new();

	private Logger _logger = Logger.GetLogger("AudioService", "#a9affa");

	public AudioService(Node parentDefault)
	{
		_parentDefault = parentDefault;
	}

	public void PlaySound(AudioStream audioStream, Node3D? target = null, bool loop = false) => PlaySound(CreateAudioPlayer(target), audioStream, loop);
	public void PlaySound(AudioStreamPlayer3D audioStreamPlayer, AudioStream audioStream, bool loop = false)
	{
		audioStreamPlayer.Stream = audioStream;

		if (audioStream is AudioStreamOggVorbis audioStreamOggVorbis)
			audioStreamOggVorbis.Loop = loop;
		else if (audioStream is AudioStreamWav audioStreamWav)
			audioStreamWav.LoopMode = loop ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled;
		else if (audioStream is AudioStreamMP3 audioStreamMp3)
			audioStreamMp3.Loop = loop;

		audioStreamPlayer.Play();
	}


	public void Update(float dt)
	{
		var removedPlayers = new List<ActiveAudioPlayer>();
		foreach (var ap in activePlayers)
		{
			if (!GodotObject.IsInstanceValid(ap.AudioPlayer))
			{
				removedPlayers.Add(ap);
				continue;
			}

			if (!ap.AudioPlayer.IsPlaying())
			{
				removedPlayers.Add(ap);
				ap.AudioPlayer.QueueFree();
				continue;
			}

			if (ap.TargetNode != null && !GodotObject.IsInstanceValid(ap.TargetNode))
			{
				_logger.Warn("Target node is invalid");
				continue;
			}

			if (ap.TargetNode != null)
				ap.AudioPlayer.GlobalTransform = ap.TargetNode.GlobalTransform;
		}

		foreach (var player in removedPlayers)
			activePlayers.Remove(player);
	}

	private AudioStreamPlayer3D CreateAudioPlayer(Node3D? target = null)
	{
		var audioPlayer = new AudioStreamPlayer3D();

		_parentDefault.AddChild(audioPlayer);
		activePlayers.Add(new ActiveAudioPlayer(audioPlayer, target));

		return audioPlayer;
	}

	private class ActiveAudioPlayer
	{
		public AudioStreamPlayer3D AudioPlayer = null!;
		public Node3D? TargetNode;

		public ActiveAudioPlayer(AudioStreamPlayer3D audioStreamPlayer, Node3D? targetNode)
		{
			AudioPlayer = audioStreamPlayer;
			TargetNode = targetNode;
		}
	}
}
