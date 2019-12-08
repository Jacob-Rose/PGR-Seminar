using UnityEditor;
using UnityEngine;

public class ScreenResolutionFixer
{
    static ScreenResolutionFixer()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }
}
