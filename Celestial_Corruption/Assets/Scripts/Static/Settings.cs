using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region Camera Sensitivity
    public static float cameraSensitivityXaxis = 200f;
    public static float cameraSensitivityYaxis = 1.3f;
    public static float cameraSensitivityConstantX = 200f;
    public static float cameraSensitivityConstantY = 1.3f;
    #endregion

    #region Keybinds

    #region Move
    public static string moveUp = "<Keyboard>/w";
    public static string moveDown = "<Keyboard>/s";
    public static string moveLeft = "<Keyboard>/a";
    public static string moveRight = "<Keyboard>/d";
    #endregion

    #region Jump
    public static string jump = "<Keyboard>/space";
    #endregion

    #endregion
}
