using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region Camera Sensitivity
    public static float cameraSensitivityXaxis = 200f;
    public static float cameraSensitivityYaxis = 1.3f;
    // Constants for camera sensitivity which are also defualt values
    public static float cameraSensitivityConstantX = 200f;
    public static float cameraSensitivityConstantY = 1.3f;
    #endregion

    #region KEYBINDS

    #region Move
    public static string moveUp = "<Keyboard>/w";
    public static string moveDown = "<Keyboard>/s";
    public static string moveLeft = "<Keyboard>/a";
    public static string moveRight = "<Keyboard>/d";
    #endregion

    #region Other
    public static string jump = "<Keyboard>/space";
    public static string WalkToggle = "<Keyboard>/c";
    public static string dash = "<Mouse>/leftButton";
    public static string AttackDash = "<Mouse>/rightButton";
    public static string LockOn = "<Mouse>/middleButton";
    #endregion

    #endregion

    #region Defualt Keybinds
    #endregion

    #region GRAPHICS
    #endregion
}
