using UnityEngine;

public static class InputManager
{
    private static JoystickController _joystickCtrl;

    public static Vector2 MoveDir => _joystickCtrl?.InputDir ?? Vector2.zero;
    public static bool IsJoystickActive => _joystickCtrl?.IsActive ?? false;

    public static void SetJoystick(JoystickController joystickCtrl)
    {
        _joystickCtrl = joystickCtrl;
    }
}
