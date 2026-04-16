using OnixRuntime.Api.Audio;
using WHAT.Utility;

namespace WHAT.Handler;

public class AudioHandler(WHATConfig config) {
	private string _currentSong = "";
	private readonly AudioPlayer _player = new();

	public bool SongPlaying { get; private set; }
	
	public void PlaySong() {
		_player.Volume = config.SongVolume;
		SongPlaying = true;

		if (config.SongPath.Text is "song1.wav" or "song2.wav") {
			_player.Restart();
			_player.Play(AssetHelper.AssetsPath + "Songs/" + config.SongPath.Text);
			return;
		}
		
		if (_player.Status == AudioPlayerStatus.NoSource || _currentSong != config.SongPath.Text) {
			_player.Play(config.SongPath.Text);
			_currentSong = config.SongPath.Text;
		} else {
			_player.Restart();
		}
		
	}

	public void StopSong() {
		_player.Stop();
		SongPlaying = false;
	}

	public void Dispose() {
		_player.Dispose();
	}
}