using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.OnixClient;
using WHAT.Utility;

namespace WHAT {
    public partial class WHATConfig : OnixModuleSettingRedirector {
	    [Category("Visual Config")]
	    
	    [MinMax(0f, 10f)]
	    [Value(4f)]
	    [Name("Fade In Time", "The time it takes for the image to fade in.")]
	    public partial float FadeInTime { get; set; }
	    
	    [Value(true)]
	    [Name("Take Screenshot", "Whether or not to take a screenshot.")]
	    public partial bool TakeScreenshot { get; set; }

	    [Value(true)]
	    [Name("Use Original Image", "If enabled, custom text is not allowed.")]
	    public partial bool UseOriginalImage { get; set; }

	    [Value("WHAT")]
	    [Name("Bottom Text", "The text on the bottom of the screen.")]
	    public partial OnixTextbox BottomText { get; set; }

	    [Category("Timing Config")]
	    
	    [MinMax(0f, 10f)]
	    [Value(0f)]
	    [Name("Main Delay Time", "The time it takes for everything to start.")]
	    public partial float MainDelayTime { get; set; }

	    [MinMax(0f, 10f)]
	    [Value(0f)]
	    [Name("Music Delay Time", "The time it takes for the music to start.")]
	    public partial float MusicDelayTime { get; set; }

	    [MinMax(0f, 10f)]
	    [Value(0f)]
	    [Name("Image Delay Time", "The time it takes for the overlay to appear.")]
	    public partial float ImageDelayTime { get; set; }

	    [MinMax(0f, 10f)]
	    [Value(0.5f)]
	    [Name("Screenshot Delay Time", "The time it takes for the screenshot.")]
	    public partial float ScreenshotDelayTime { get; set; }

	    [Category("Audio Config")]
	    
	    [MinMax(0f, 1f)]
	    [Value(1f)]
	    [Name("Song Volume", "The volume of the song.")]
	    public partial float SongVolume { get; set; }

	    [Button(nameof(SelectSongFunc), "Open")]
	    [Name("Select Song", "Click this button to select a song to play.")]
	    public partial OnixSetting.SettingChangedDelegate SelectSong { get; set; }

	    [Value("song2.wav")]
	    [Name("Song Path", "The path to the song file. (song1.wav = earthbound, song2.wav = scatman)")]
	    public partial OnixTextbox SongPath { get; set; }

	    public void SelectSongFunc() {
		    string? romPath = NativeFileDialog.ShowAudioFilePicker();

		    if (!string.IsNullOrEmpty(romPath) && File.Exists(romPath)) {
			    SongPath.Text = romPath;
		    }
	    }
    }
}