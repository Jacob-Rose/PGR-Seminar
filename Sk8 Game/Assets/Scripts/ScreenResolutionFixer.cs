
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ScreenResolutionFixer
{
    static ScreenResolutionFixer()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }
}
