using Godot;

namespace MultiWindows.windows
{
	public class Windows : Node2D
	{
		public static SelectedWindow SelectedWindow;

		public override void _Ready()
		{
			foreach (BaseWindow window in GetChildren())
			{
				window.Connect("input_event", this, nameof(OnWindowInputEvent), new Godot.Collections.Array() { window });
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			if (SelectedWindow is SelectedWindow)
			{
				Vector2 targetPos = GetLocalMousePosition() - SelectedWindow.HeldOffsetFromCenter;
				SelectedWindow.Window.TeleportSafely(targetPos);
				SelectedWindow.Window.Position = SelectedWindow.Window.Position.Round();
			}
		}

#pragma warning disable IDE0060 // Remove unused parameter
		private void OnWindowInputEvent(object viewport, object @event, int shape_idx, BaseWindow window)
#pragma warning restore IDE0060 // Remove unused parameter
		{
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left && eventMouseButton.Pressed && SelectedWindow?.Window != window)
			{
				SelectWindow(window);
			}
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left && !eventMouseButton.Pressed)
			{
				DeselectWindow();
			}
		}

		private void SelectWindow(BaseWindow window)
		{
			SelectedWindow = new SelectedWindow(window, window.GetLocalMousePosition());
		}

		private void DeselectWindow()
		{
			SelectedWindow = null;
		}
	}
}
