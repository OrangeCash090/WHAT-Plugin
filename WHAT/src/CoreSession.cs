using OnixRuntime.Api;
using OnixRuntime.Api.Rendering;
using WHAT.Handler;

namespace WHAT;

public class CoreSession {
	private readonly WHATConfig Config;

	private readonly InputHandler Input;
	private readonly AudioHandler Audio;
	private readonly ScreenHandler Screen;

	private bool _lastUsed;
	private bool _mainEnded;
	private bool _musEnded;
	private bool _imgEnded;
	
	private float _mainTimer;

	public CoreSession(WHATConfig config) {
		Config = config;

		Input = new InputHandler();
		Audio = new AudioHandler(Config);
		Screen = new ScreenHandler(Config);
		
		Onix.Events.Common.HudRenderGame += OnHudRenderGame;
	}

	private void OnHudRenderGame(RendererGame gfx, float delta) {
		Screen.OnHudRenderGame(gfx, delta);
		
		if (_lastUsed != Input.UsingTelescope) {
			if (Audio.SongPlaying) {
				Screen.Reset();
				Audio.StopSong();
				
				_mainTimer = 0f;
				
				_mainEnded = false;
				_musEnded = false;
				_imgEnded = false;
			}
		}

		if (Input.UsingTelescope) {
			if (_mainEnded) {
				if (Input.UsingTelescope && !_musEnded) {
					if (_mainTimer >= Config.MusicDelayTime) {
						Audio.PlaySong();
						_musEnded = true;
					}
				}

				if (Input.UsingTelescope && !_imgEnded) {
					if (_mainTimer >= Config.ImageDelayTime) {
						Screen.ShowScreen();
						_imgEnded = true;
					}
				}
			} else {
				if (_mainTimer >= Config.MainDelayTime) {
					_mainEnded = true;
				}
			}
			
			_mainTimer += delta;
		}

		_lastUsed = Input.UsingTelescope;
	}

	public void Dispose() {
		Onix.Events.Common.HudRenderGame -= OnHudRenderGame;
		
		Input.Dispose();
		Audio.Dispose();
		Screen.Dispose();
	}
}