using OnixRuntime.Api;
using OnixRuntime.Api.Entities;
using OnixRuntime.Api.Inputs;
using OnixRuntime.Api.Items;

namespace WHAT.Handler;

public class InputHandler {
	public bool UsingTelescope { get; private set; }

	public InputHandler() {
		Onix.Events.Player.UseItem += OnUseItem;
		Onix.Events.Player.StopUsingItem += OnStopUsingItem;
		Onix.Events.Input.InputHud += OnInputHud;
	}

	private bool OnInputHud(InputKey key, bool isDown) {
		if (isDown && UsingTelescope) {
			UsingTelescope = false;
		}

		return false;
	}

	private bool OnUseItem(LocalPlayer player, ItemStack stack) {
		if (player == Onix.LocalPlayer && stack.Item != null && stack.Item.Name == "spyglass") {
			UsingTelescope = true;
		}

		return false;
	}

	private bool OnStopUsingItem(LocalPlayer player) {
		if (player == Onix.LocalPlayer && UsingTelescope) {
			UsingTelescope = false;
		}

		return false;
	}

	public void Dispose() {
		Onix.Events.Player.UseItem -= OnUseItem;
		Onix.Events.Player.StopUsingItem -= OnStopUsingItem;
	}
}