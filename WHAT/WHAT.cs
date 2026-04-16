using OnixRuntime.Plugin;
using WHAT.Utility;

namespace WHAT {
	public class WHAT : OnixPluginBase {
		public static WHAT Instance { get; private set; } = null!;
		public static WHATConfig Config { get; private set; } = null!;
		
		private static CoreSession? Session;

		public WHAT(OnixPluginInitInfo initInfo) : base(initInfo) {
			Instance = this;
			base.DisablingShouldUnloadPlugin = false;
			
			#if DEBUG
			//base.WaitForDebuggerToBeAttached();
			#endif
		}

		protected override void OnLoaded() {
			Config = new WHATConfig(PluginDisplayModule, true);
			
			AssetHelper.AssetsPath = PluginAssetsPath + "\\";
			Session = new CoreSession(Config);
		}

		protected override void OnEnabled() { }

		protected override void OnDisabled() { }

		protected override void OnUnloaded() {
			Session?.Dispose();
		}
	}
}