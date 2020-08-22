using Godot;

namespace MultiWindows.windows
{
	public class SelectedWindow : Object
	{
		public BaseWindow Window;
		public Vector2 HeldOffsetFromCenter;

		public SelectedWindow(BaseWindow window, Vector2 heldOffsetFromCenter)
		{
			Window = window;
			HeldOffsetFromCenter = heldOffsetFromCenter;
		}
	}
}
