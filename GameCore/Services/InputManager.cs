using OpenTK;
using OpenTK.Input;

namespace GameCore.Services
{
    public class InputManager
    {
        public KeyboardDevice Keyboard { get; set; }
        public MouseDevice Mouse { get; set; }
        public Vector2 ScreenDeviceSize { get; set; }
        public Vector2 ScreenSize { get; set; }
    }
}