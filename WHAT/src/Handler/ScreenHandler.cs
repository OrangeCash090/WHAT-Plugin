using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using OnixRuntime.Api.Rendering;
using OnixRuntime.Api.Utils;
using WHAT.Utility;

namespace WHAT.Handler;

public class ScreenHandler {
	private readonly WHATConfig Config;
	
	private float _fadeTimer;
	private float _waitTimer;
	
	private bool _texturesCached;
	private bool _shotUploaded;

	private readonly TexturePath _backgroundPath = TexturePath.Assets("Images/background.png");
	private readonly TexturePath _overlayPath = TexturePath.Assets("Images/overlay.png");
	private readonly TexturePath _ssPath = TexturePath.Game("screenshot.png");
	
	public bool BackgroundVisible { get; private set; }
	
	public const float TextScale = 7f;
	public const float TextYOffset = 100f;

	public ScreenHandler(WHATConfig config) {
		Config = config;
		Onix.Events.Rendering.ShouldHideScreen += OnShouldHideScreen;
	}

	public void ShowScreen() {
		BackgroundVisible = true;
	}

	public void Reset() {
		_shotUploaded = false;
		_fadeTimer = 0f;
		_waitTimer = 0f;
		BackgroundVisible = false;
	}

	public void OnHudRenderGame(RendererGame gfx, float delta) {
		if (!_texturesCached) {
			gfx.RenderTexture(new Rect(Vec2.Zero, Vec2.One), _backgroundPath);
			gfx.RenderTexture(new Rect(Vec2.Zero, Vec2.One), _overlayPath);
			_texturesCached = true;
		}
		
		if (!BackgroundVisible) return;
		
		if (Config.TakeScreenshot) {
			if (!_shotUploaded) {
				_waitTimer += delta;

				if (_waitTimer >= 0.05f + Config.ScreenshotDelayTime) {
					gfx.UploadTexture(_ssPath, TakeScreenshot());
					_shotUploaded = true;
				}
			}

			if (_shotUploaded) {
				if (Config.FadeInTime == 0) {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _ssPath);

					if (Config.UseOriginalImage) {
						gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _backgroundPath);
					} else {
						gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _overlayPath);
						gfx.RenderText(new Vec2((gfx.Width / 2f) - (gfx.MeasureText(Config.BottomText.Text, TextScale).X / 2f), (gfx.Height / 2f) + TextYOffset), ColorF.White, Config.BottomText.Text, TextScale);
					}
				} else {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _ssPath, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f));

					if (Config.UseOriginalImage) {
						gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _backgroundPath, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f));
					} else {
						gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _overlayPath, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f));
						gfx.RenderText(new Vec2((gfx.Width / 2f) - (gfx.MeasureText(Config.BottomText.Text, TextScale).X / 2f), (gfx.Height / 2f) + TextYOffset), new ColorF(1, 1, 1, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f)), Config.BottomText.Text, TextScale);
					}
				}

				_fadeTimer += delta;
			}
		} else {
			if (Config.FadeInTime == 0) {
				if (Config.UseOriginalImage) {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _backgroundPath);
				} else {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _overlayPath);
					gfx.RenderText(new Vec2((gfx.Width / 2f) - (gfx.MeasureText(Config.BottomText.Text, TextScale).X / 2f), (gfx.Height / 2f) + TextYOffset), ColorF.White, Config.BottomText.Text, TextScale);
				}
			} else {
				if (Config.UseOriginalImage) {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _backgroundPath, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f));
				} else {
					gfx.RenderTexture(new Rect(Vec2.Zero, new Vec2(gfx.Width, gfx.Height)), _overlayPath, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f));
					gfx.RenderText(new Vec2((gfx.Width / 2f) - (gfx.MeasureText(Config.BottomText.Text, TextScale).X / 2f), (gfx.Height / 2f) + TextYOffset), new ColorF(1, 1, 1, Math.Clamp(_fadeTimer * Config.FadeInTime, 0f, 1f)), Config.BottomText.Text, TextScale);
				}
			}
			
			_fadeTimer += delta;
		}
	}

	private bool OnShouldHideScreen(string screenName) {
		return screenName == "hud_screen" && BackgroundVisible;
	}

	private RawImageData TakeScreenshot() {
		return ImageUtils.ImageToRaw(WindowFuncs.CaptureWindow("Minecraft.Windows.exe"));
	}

	public void Dispose() {
		Onix.Events.Rendering.ShouldHideScreen -= OnShouldHideScreen;
	}
}