using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Configuration;
using System.Globalization;

namespace BPX
{
    public static class BPXConfiguration
    {
        private static ConfigFile Config;

        //General Settings
        private static ConfigEntry<KeyCode> enableKey;
        private static ConfigEntry<KeyCode> modifierKey;

        //Axis Cycling
        private static ConfigEntry<KeyCode> axisCycleKey;
        private static ConfigEntry<bool> axisCycleRequiresEnableKey;
        private static ConfigEntry<bool> includePlanesInCycle;        

        //Drag Selection
        private static ConfigEntry<KeyCode> dragSelectionKey;
        private static ConfigEntry<bool> mmbSelection;
        private static ConfigEntry<bool> dragSelectionRequiresEnableKey;

        //Scaling
        private static ConfigEntry<bool> scrollScaling;
        private static ConfigEntry<bool> invertScrollScaling;
        private static ConfigEntry<KeyCode> negativeScalingKey;
        private static ConfigEntry<KeyCode> positiveScalingKey;
        private static ConfigEntry<bool> scalingRequiresEnableKey;
        private static ConfigEntry<string> scalingValues;
        private static ConfigEntry<string> defaultScalingValue;
        private static ConfigEntry<bool> resetScalingValues;

        //Key Movement
        private static ConfigEntry<KeyCode> forwardUpMovement;
        private static ConfigEntry<KeyCode> backDownMovement;
        private static ConfigEntry<KeyCode> leftMovement;
        private static ConfigEntry<KeyCode> rightMovement;
        private static ConfigEntry<bool> movementRequiresEnableKey;
        private static ConfigEntry<bool> movementIfRotationIsDisabled;

        //Key Rotation
        private static ConfigEntry<KeyCode> xPositiveRotation;
        private static ConfigEntry<KeyCode> xNegativeRotation;
        private static ConfigEntry<KeyCode> yzPositiveRotation;
        private static ConfigEntry<KeyCode> yzNegativeRotation;
        private static ConfigEntry<bool> rotationRequiresEnableKey;

        //Mirroring
        private static ConfigEntry<KeyCode> mirrorKey;
        private static ConfigEntry<bool> mirrorRequiresEnableKey;

        //Clipboard
        private static ConfigEntry<KeyCode> clipboardCopy;
        private static ConfigEntry<KeyCode> clipboardPaste;
        private static ConfigEntry<bool> clipboardRequiresEnableKey;
        private static ConfigEntry<bool> pasteClipboardToCamera;

        //Fast Travel
        private static ConfigEntry<KeyCode> fastTravelKey;
        private static ConfigEntry<bool> fastTravelRequiresEnableKey;

        //Shortcuts
        private static ConfigEntry<KeyCode> saveShortcutKey;
        private static ConfigEntry<KeyCode> loadShortcutKey;
        private static ConfigEntry<bool> shortcutRequiresEnableKey;

        //Panel
        private static ConfigEntry<bool> clearSearchOnExit;
        private static ConfigEntry<bool> doubleLoadButtons;
        private static ConfigEntry<string> allowedExtensions;

        //Gizmo
        private static ConfigEntry<bool> useCustomValues;
        private static ConfigEntry<string> customXZValues;
        private static ConfigEntry<string> defaultCustomXZValue;
        private static ConfigEntry<string> customYValues;
        private static ConfigEntry<string> defaultCustomYValue;
        private static ConfigEntry<string> customRValues;
        private static ConfigEntry<string> defaultCustomRValue;
        private static ConfigEntry<bool> resetCustomValues;

        //Functions 
        private static ConfigEntry<bool> applyBasicValues;

        //BPX Online
        private static ConfigEntry<string> bpxOnlineTestingDirectory;
        private static ConfigEntry<string> bpxOnlineApiUrl;
        private static ConfigEntry<int> bpxOnlineResultsPerPage;
        

        public static void Initialize(ConfigFile cfg)
        {
            Config = cfg;

            // General Settings
            applyBasicValues = Config.Bind("01.General Settings", "1.Apply Basic Values To Config", false, "[Button] Apply Basic Values to Config");
            applyBasicValues.SettingChanged += ApplyBasicValues;
            enableKey = Config.Bind("01.General Settings", "2.Enable Key", KeyCode.None, "Key to enable functionality");
            modifierKey = Config.Bind("01.General Settings", "3.Modifier Key", KeyCode.None, "Modifier key for additional controls");

            // Axis Cycling
            axisCycleKey = Config.Bind("02.Axis Cycling", "1.Axis Cycle Key", KeyCode.None, "Key to cycle through axes");
            axisCycleRequiresEnableKey = Config.Bind("02.Axis Cycling", "2.Axis Cycle Requires Enable Key", false, "Requires enable key to cycle through axes");
            includePlanesInCycle = Config.Bind("02.Axis Cycling", "3.Include Planes In Cycle", false, "Include planes in the axis cycling");

            // Drag Selection
            dragSelectionKey = Config.Bind("03.Drag Selection", "1.Drag Selection Key", KeyCode.None, "Key for drag selection");
            mmbSelection = Config.Bind("03.Drag Selection", "2.MMB Selection", false, "Use middle mouse button for selection");
            dragSelectionRequiresEnableKey = Config.Bind("03.Drag Selection", "3.Drag Selection Requires Enable Key", false, "Requires enable key for drag selection");

            // Scaling
            scrollScaling = Config.Bind("04.Scaling", "1.Scroll Scaling", false, "Enable scaling with scroll");
            invertScrollScaling = Config.Bind("04.Scaling", "2.Invert Scroll Scaling", false, "Invert scroll scaling direction");
            negativeScalingKey = Config.Bind("04.Scaling", "3.Negative Scaling Key", KeyCode.None, "Key for negative scaling");
            positiveScalingKey = Config.Bind("04.Scaling", "4.Positive Scaling Key", KeyCode.None, "Key for positive scaling");
            scalingRequiresEnableKey = Config.Bind("04.Scaling", "5.Scaling Requires Enable Key", false, "Requires enable key for scaling");
            scalingValues = Config.Bind("04.Scaling", "6.Scaling Values", "0.05;0.1;0.5;1;5;10;20", "Custom scaling values");
            defaultScalingValue = Config.Bind("04.Scaling", "7.Default Scaling Value", "10", "Default scaling value");
            resetScalingValues = Config.Bind("04.Scaling", "8.Reset Values To Default", false, "[Button] Reset the values in the text fields to their default values.");
            resetScalingValues.SettingChanged += ResetScalingValues;

            // Key Movement
            forwardUpMovement = Config.Bind("05.Key Movement", "1.Forward/Up Movement Key", KeyCode.None, "Key for forward/up movement");
            backDownMovement = Config.Bind("05.Key Movement", "2.Back/Down Movement Key", KeyCode.None, "Key for back/down movement");
            leftMovement = Config.Bind("05.Key Movement", "3.Left Movement Key", KeyCode.None, "Key for left movement");
            rightMovement = Config.Bind("05.Key Movement", "4.Right Movement Key", KeyCode.None, "Key for right movement");
            movementRequiresEnableKey = Config.Bind("05.Key Movement", "5.Movement Requires Enable Key", false, "Requires enable key for movement");
            movementIfRotationIsDisabled = Config.Bind("05.Key Movement", "6.Movement If Rotation Is Disabled", false, "Allow movement if rotation is disabled");

            // Key Rotation
            xPositiveRotation = Config.Bind("06.Key Rotation", "1.X Positive Rotation Key", KeyCode.None, "Key for positive rotation around the X axis");
            xNegativeRotation = Config.Bind("06.Key Rotation", "2.X Negative Rotation Key", KeyCode.None, "Key for negative rotation around the X axis");
            yzPositiveRotation = Config.Bind("06.Key Rotation", "3.YZ Positive Rotation Key", KeyCode.None, "Key for positive rotation around the Y and Z axes");
            yzNegativeRotation = Config.Bind("06.Key Rotation", "4.YZ Negative Rotation Key", KeyCode.None, "Key for negative rotation around the Y and Z axes");
            rotationRequiresEnableKey = Config.Bind("06.Key Rotation", "5.Rotation Requires Enable Key", false, "Requires enable key for rotation");

            // Mirroring
            mirrorKey = Config.Bind("07.Mirroring", "1.Mirror Key", KeyCode.None, "Key for mirroring");
            mirrorRequiresEnableKey = Config.Bind("07.Mirroring", "2.Mirror Requires Enable Key", false, "Requires enable key for mirroring");

            // Clipboard
            clipboardCopy = Config.Bind("08.Clipboard", "1.Clipboard Copy Key", KeyCode.None, "Key for copying to clipboard");
            clipboardPaste = Config.Bind("08.Clipboard", "2.Clipboard Paste Key", KeyCode.None, "Key for pasting from clipboard");
            clipboardRequiresEnableKey = Config.Bind("08.Clipboard", "3.Clipboard Requires Enable Key", false, "Requires enable key for clipboard operations");
            pasteClipboardToCamera = Config.Bind("08.Clipboard", "4.Paste Clipboard To Camera", false, "Paste clipboard content to camera");

            // Fast Travel
            fastTravelKey = Config.Bind("09.Fast Travel", "1.Fast Travel Key", KeyCode.None, "Key for fast travel");
            fastTravelRequiresEnableKey = Config.Bind("09.Fast Travel", "2.Fast Travel Requires Enable Key", false, "Requires enable key for fast travel");

            // Shortcuts
            saveShortcutKey = Config.Bind("10.Shortcuts", "1.Save Shortcut Key", KeyCode.None, "Key for save shortcut");
            loadShortcutKey = Config.Bind("10.Shortcuts", "2.Load Shortcut Key", KeyCode.None, "Key for load shortcut");
            shortcutRequiresEnableKey = Config.Bind("10.Shortcuts", "3.Shortcut Requires Enable Key", false, "Requires enable key for shortcuts");

            // Panel
            clearSearchOnExit = Config.Bind("11.Panel", "1.Clear Search On Exit", false, "Clear search field on exit");
            doubleLoadButtons = Config.Bind("11.Panel", "2.Double Load Buttons", false, "Enable double load buttons");
            allowedExtensions = Config.Bind("11.Panel", "3.Allowed Extensions", ".png,.obj,.jpg,.realm,.zeeplist,.zip,.customsoapbox", "Allowed file extensions");

            // Gizmo
            useCustomValues = Config.Bind("12.Gizmo", "1.Use Custom Values", false, "Use custom values for gizmo");
            customXZValues = Config.Bind("12.Gizmo", "2.Custom XZ Values", "0;0.2;0.8;1.6;4;8;16", "Custom XZ values for gizmo");
            defaultCustomXZValue = Config.Bind("12.Gizmo", "3.Default Custom XZ Value", "16", "Default custom XZ value for gizmo");
            customYValues = Config.Bind("12.Gizmo", "4.Custom Y Values", "0;0.2;0.8;1.6;4;8", "Custom Y values for gizmo");
            defaultCustomYValue = Config.Bind("12.Gizmo", "5.Default Custom Y Value", "8", "Default custom Y value for gizmo");
            customRValues = Config.Bind("12.Gizmo", "6.Custom R Values", "0;1;5;10;30;45;90", "Custom R values for gizmo");
            defaultCustomRValue = Config.Bind("12.Gizmo", "7.Default Custom R Value", "45", "Default custom R value for gizmo");
            resetCustomValues = Config.Bind("12.Gizmo", "8.Reset Values To Default", false, "[Button] Reset the values in the text fields to their default values.");
            resetCustomValues.SettingChanged += ResetCustomGridValues;

            //BPXOnline
            //bpxOnlineTestingDirectory = Config.Bind("13.BPXOnline", "2. Testing Directory", "D:/BPXOnline", "");
            bpxOnlineApiUrl = Config.Bind("13.BPXOnline", "1.API URL", "http://195.201.16.152:5204/", "");
            bpxOnlineResultsPerPage = Config.Bind("13.BPXOnline", "2.Results Per Page", 20, "The amount of files displayed on a single page");

            Config.SettingChanged += ConfigChanged;
        }

        private static void ReloadToApplyMessage()
        {
            Plugin.Instance.LogScreenMessage("Press [Apply] and reopen settings window to show changes!");
        }

        private static void ResetScalingValues(object sender, EventArgs e)
        {
            scalingValues.Value = (string)scalingValues.DefaultValue;
            defaultScalingValue.Value = (string)defaultScalingValue.DefaultValue;
            ReloadToApplyMessage();
        }

        private static void ResetCustomGridValues(object sender, EventArgs e)
        {
            customXZValues.Value = (string) customXZValues.DefaultValue;
            defaultCustomXZValue.Value = (string)defaultCustomXZValue.DefaultValue;
            customYValues.Value = (string)customYValues.DefaultValue;
            defaultCustomYValue.Value = (string)defaultCustomYValue.DefaultValue;
            customRValues.Value = (string)customRValues.DefaultValue;
            defaultCustomRValue.Value = (string)defaultCustomRValue.DefaultValue;
            ReloadToApplyMessage();
        }

        private static void ConfigChanged(object sender, SettingChangedEventArgs e)
        {
            if (BPXManager.central != null)
            {
                BPXUIManagement.UpdateGridButtons();
            }
        }

        private static void ApplyBasicValues(object sender, EventArgs e)
        {
            enableKey.Value = KeyCode.LeftControl;
            modifierKey.Value = KeyCode.LeftShift;

            axisCycleKey.Value = KeyCode.Space;
            axisCycleRequiresEnableKey.Value = false;
            includePlanesInCycle.Value = false;

            dragSelectionKey.Value = KeyCode.LeftAlt;
            mmbSelection.Value = true;
            dragSelectionRequiresEnableKey.Value = false;

            scrollScaling.Value = true;
            invertScrollScaling.Value = false;
            negativeScalingKey.Value = KeyCode.Minus;
            positiveScalingKey.Value = KeyCode.Equals;
            scalingRequiresEnableKey.Value = false;
            scalingValues.Value = (string)scalingValues.DefaultValue;
            defaultScalingValue.Value = (string)scalingValues.DefaultValue;

            forwardUpMovement.Value = KeyCode.UpArrow;
            backDownMovement.Value = KeyCode.DownArrow;
            leftMovement.Value = KeyCode.LeftArrow;
            rightMovement.Value = KeyCode.RightArrow;
            movementRequiresEnableKey.Value = false;
            movementIfRotationIsDisabled.Value = false;

            xPositiveRotation.Value = KeyCode.UpArrow;
            xNegativeRotation.Value = KeyCode.DownArrow;
            yzPositiveRotation.Value = KeyCode.RightArrow;
            yzNegativeRotation.Value = KeyCode.LeftArrow;
            rotationRequiresEnableKey.Value = false;

            mirrorKey.Value = KeyCode.F;
            mirrorRequiresEnableKey.Value = false;

            clipboardCopy.Value = KeyCode.J;
            clipboardPaste.Value = KeyCode.K;
            clipboardRequiresEnableKey.Value = true;
            pasteClipboardToCamera.Value = true;

            fastTravelKey.Value = KeyCode.U;
            fastTravelRequiresEnableKey.Value = false;

            saveShortcutKey.Value = KeyCode.S;
            loadShortcutKey.Value = KeyCode.L;
            shortcutRequiresEnableKey.Value = true;

            clearSearchOnExit.Value = false;
            doubleLoadButtons.Value = false;
            allowedExtensions.Value = ".png,.obj,.jpg,.realm,.zeeplist,.zip,.customsoapbox";

            useCustomValues.Value = false;
            customXZValues.Value = "0,0.2,0.8,1.6,4,8,16";
            defaultCustomXZValue.Value = "16";
            customYValues.Value = "0,0.2,0.8,1.6,4,8";
            defaultCustomYValue.Value = "8";
            customRValues.Value = "0,1,5,10,30,45,90";
            defaultCustomRValue.Value = "45";
            ReloadToApplyMessage();
        }

        // General Settings
        public static KeyCode GetEnableKey()
        {
            return enableKey.Value;
        }

        public static KeyCode GetModifierKey()
        {
            return modifierKey.Value;
        }

        // Axis Cycling
        public static KeyCode GetAxisCycleKey()
        {
            return axisCycleKey.Value;
        }

        public static bool AxisCycleRequireEnableKey()
        {
            return axisCycleRequiresEnableKey.Value;
        }

        public static bool IncludePlanesInCycle()
        {
            return includePlanesInCycle.Value;
        }

        // Drag Selection
        public static KeyCode GetDragSelectionKey()
        {
            return dragSelectionKey.Value;
        }

        public static bool DoMMBSelection()
        {
            return mmbSelection.Value;
        }

        public static bool DragSelectionRequiresEnableKey()
        {
            return dragSelectionRequiresEnableKey.Value;
        }

        // Scaling
        public static bool DoScrollScaling()
        {
            return scrollScaling.Value;
        }

        public static bool InvertScrollScaling()
        {
            return invertScrollScaling.Value;
        }

        public static KeyCode GetNegativeScalingKey()
        {
            return negativeScalingKey.Value;
        }

        public static KeyCode GetPositiveScalingKey()
        {
            return positiveScalingKey.Value;
        }

        public static bool ScalingRequiresEnableKey()
        {
            return scalingRequiresEnableKey.Value;
        }

        public static float[] GetScalingValues()
        {
            return ParseFloatArray(scalingValues.Value);
        }

        public static float GetDefaultScalingValue()
        {
            return ParseFloatValue(defaultScalingValue.Value);
        }

        // Key Movement
        public static KeyCode GetForwardUpMovementKey()
        {
            return forwardUpMovement.Value;
        }

        public static KeyCode GetBackDownMovementKey()
        {
            return backDownMovement.Value;
        }

        public static KeyCode GetLeftMovementKey()
        {
            return leftMovement.Value;
        }

        public static KeyCode GetRightMovementKey()
        {
            return rightMovement.Value;
        }

        public static bool MovementRequiresEnableKey()
        {
            return movementRequiresEnableKey.Value;
        }

        public static bool MovementIfRotationIsDisabled()
        {
            return movementIfRotationIsDisabled.Value;
        }

        // Key Rotation
        public static bool KeyRotationIsEnabled()
        {
            return GetXPositiveRotationKey() != KeyCode.None || GetXNegativeRotationKey() != KeyCode.None || GetYZPositiveRotationKey() != KeyCode.None || GetYZNegativeRotationKey() != KeyCode.None;     
        }
        public static KeyCode GetXPositiveRotationKey()
        {
            return xPositiveRotation.Value;
        }

        public static KeyCode GetXNegativeRotationKey()
        {
            return xNegativeRotation.Value;
        }

        public static KeyCode GetYZPositiveRotationKey()
        {
            return yzPositiveRotation.Value;
        }

        public static KeyCode GetYZNegativeRotationKey()
        {
            return yzNegativeRotation.Value;
        }

        public static bool RotationRequiresEnableKey()
        {
            return rotationRequiresEnableKey.Value;
        }

        // Mirroring
        public static KeyCode GetMirrorKey()
        {
            return mirrorKey.Value;
        }

        public static bool MirrorRequiresEnableKey()
        {
            return mirrorRequiresEnableKey.Value;
        }

        // Clipboard
        public static KeyCode GetClipboardCopyKey()
        {
            return clipboardCopy.Value;
        }

        public static KeyCode GetClipboardPasteKey()
        {
            return clipboardPaste.Value;
        }

        public static bool ClipboardRequiresEnableKey()
        {
            return clipboardRequiresEnableKey.Value;
        }

        public static bool PasteClipboardToCamera()
        {
            return pasteClipboardToCamera.Value;
        }

        // Fast Travel
        public static KeyCode GetFastTravelKey()
        {
            return fastTravelKey.Value;
        }

        public static bool FastTravelRequiresEnableKey()
        {
            return fastTravelRequiresEnableKey.Value;
        }

        // Shortcuts
        public static KeyCode GetSaveShortcutKey()
        {
            return saveShortcutKey.Value;
        }

        public static KeyCode GetLoadShortcutKey()
        {
            return loadShortcutKey.Value;
        }

        public static bool ShortcutRequiresEnableKey()
        {
            return shortcutRequiresEnableKey.Value;
        }

        // Panel
        public static bool ClearSearchOnExit()
        {
            return clearSearchOnExit.Value;
        }

        public static bool DoubleLoadButtons()
        {
            return doubleLoadButtons.Value;
        }

        public static bool IsAllowedExtension(string ext)
        {
            return GetAllowedExtensions().Contains(ext);
        }

        public static string[] GetAllowedExtensions()
        {
            return allowedExtensions.Value.Split(',');
        }

        // Gizmo
        public static bool UseCustomValues()
        {
            return useCustomValues.Value;
        }

        // Custom XZ Values
        public static float[] GetCustomXZValues()
        {
            return ParseFloatArray(customXZValues.Value);
        }

        public static float GetDefaultCustomXZValue()
        {
            return ParseFloatValue(defaultCustomXZValue.Value);
        }

        // Custom Y Values
        public static float[] GetCustomYValues()
        {
            return ParseFloatArray(customYValues.Value);
        }

        public static float GetDefaultCustomYValue()
        {
            return ParseFloatValue(defaultCustomYValue.Value);
        }

        // Custom R Values
        public static float[] GetCustomRValues()
        {
            return ParseFloatArray(customRValues.Value);
        }

        public static float GetDefaultCustomRValue()
        {
            return ParseFloatValue(defaultCustomRValue.Value);
        }

        // Helper methods to parse float arrays and float values
        private static float[] ParseFloatArray(string value)
        {
            string[] values = value.Split(';');
            List<float> floatValues = new List<float>();

            foreach (var val in values)
            {
                if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
                {
                    floatValues.Add(parsedValue);
                }
            }

            return floatValues.ToArray();
        }

        private static float ParseFloatValue(string value)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
            {
                return parsedValue;
            }
            else
            {
                return -1.0f;
            }
        }

        //BPXOnline
        public static string GetBPXOnlineTestingDirectory()
        {
            return bpxOnlineTestingDirectory.Value;
        }
        
        public static string GetBPXOnlineApiUrl()
        {
            return bpxOnlineApiUrl.Value;
        }

        public static int GetBPXOnlineResultsPerPage()
        {
            return bpxOnlineResultsPerPage.Value;
        }
    }
}
