using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx.Configuration;
using System.Globalization;
using BPX.UI;
using ZeepSDK.Settings;
using ZeepSDK.Settings.Drawers;

namespace BPX
{
    public static class BPXConfiguration
    {
        private static ConfigFile Config;

        //General Settings
        public static ConfigEntry<KeyCode> enableKey;
        public static ConfigEntry<KeyCode> modifierKey;

        //Axis Cycling
        public static ConfigEntry<KeyCode> axisCycleKey;
        public static ConfigEntry<bool> axisCycleRequiresEnableKey;
        public static ConfigEntry<bool> includePlanesInCycle;        

        //Drag Selection
        public static ConfigEntry<KeyCode> dragSelectionKey;
        public static ConfigEntry<bool> mmbSelection;
        public static ConfigEntry<bool> dragSelectionRequiresEnableKey;

        //Scaling
        public static ConfigEntry<bool> scrollScaling;
        public static ConfigEntry<bool> invertScrollScaling;
        public static ConfigEntry<KeyCode> negativeScalingKey;
        public static ConfigEntry<KeyCode> positiveScalingKey;
        public static ConfigEntry<bool> scalingRequiresEnableKey;
        public static ConfigEntry<string> scalingValues;
        public static ConfigEntry<string> defaultScalingValue;
        public static ConfigEntry<bool> resetScalingValues;
        public static ConfigEntry<bool> unitBasedScaling;
        public static ConfigEntry<KeyCode> unitBasedScalingToggleKey;

        //Key Movement
        public static ConfigEntry<KeyCode> forwardUpMovement;
        public static ConfigEntry<KeyCode> backDownMovement;
        public static ConfigEntry<KeyCode> leftMovement;
        public static ConfigEntry<KeyCode> rightMovement;
        public static ConfigEntry<bool> movementRequiresEnableKey;
        public static ConfigEntry<bool> movementIfRotationIsDisabled;

        //Key Rotation
        public static ConfigEntry<KeyCode> xPositiveRotation;
        public static ConfigEntry<KeyCode> xNegativeRotation;
        public static ConfigEntry<KeyCode> yzPositiveRotation;
        public static ConfigEntry<KeyCode> yzNegativeRotation;
        public static ConfigEntry<bool> rotationRequiresEnableKey;

        //Mirroring
        public static ConfigEntry<KeyCode> mirrorKey;
        public static ConfigEntry<bool> mirrorRequiresEnableKey;

        //Clipboard
        public static ConfigEntry<KeyCode> clipboardCopy;
        public static ConfigEntry<KeyCode> clipboardPaste;
        public static ConfigEntry<bool> clipboardRequiresEnableKey;
        public static ConfigEntry<bool> pasteClipboardToCamera;

        //Fast Travel
        public static ConfigEntry<KeyCode> fastTravelKey;
        public static ConfigEntry<bool> fastTravelRequiresEnableKey;

        //Speed setting (Not technically a config, more of an temp setting)
        public static float baseMoveSpeed = 20f;
        public static float currentMoveSpeed = 20f;
        public static float[] moveSpeedMultipliers = new float[] { 0.01f, 0.1f, 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2f, 4f, 8f, 16f };
        public static string[] moveSpeedMultiplierNames = new string[] { "1%", "10%", "25%", "50%", "75%", "Standard", "150%", "200%", "400%", "800%", "1600%" };
        public static int currentMoveSpeedIndex = 5;

        //Shortcuts
        public static ConfigEntry<KeyCode> saveShortcutKey;
        public static ConfigEntry<KeyCode> loadShortcutKey;
        public static ConfigEntry<bool> shortcutRequiresEnableKey;

        //Panel
        public static ConfigEntry<bool> clearSearchOnExit;
        public static ConfigEntry<bool> doubleLoadButtons;
        public static ConfigEntry<string> allowedExtensions;

        //Gizmo
        public static ConfigEntry<bool> useCustomValues;
        public static ConfigEntry<string> customXZValues;
        public static ConfigEntry<string> defaultCustomXZValue;
        public static ConfigEntry<string> customYValues;
        public static ConfigEntry<string> defaultCustomYValue;
        public static ConfigEntry<string> customRValues;
        public static ConfigEntry<string> defaultCustomRValue;
        public static ConfigEntry<bool> resetCustomValues;

        //Functions 
        //public static ConfigEntry<bool> applyBasicValues;

        //Property clipboard
        /*public static ConfigEntry<KeyCode> copyPositionKey;
        public static ConfigEntry<KeyCode> copyRotationKey;
        public static ConfigEntry<KeyCode> copyScaleKey;
        public static ConfigEntry<KeyCode> copyOptionsKey;
        public static ConfigEntry<KeyCode> copyPaintsKey;
        public static ConfigEntry<KeyCode> copyAllConfiguredKey;        
        public static ConfigEntry<bool> includePositionInCopyAll;
        public static ConfigEntry<bool> includeRotationInCopyAll;
        public static ConfigEntry<bool> includeScaleInCopyAll;
        public static ConfigEntry<bool> includeOptionsInCopyAll;
        public static ConfigEntry<bool> includePaintsInCopyAll;
        public static ConfigEntry<bool> propertyClipboardRequiresEnableKey;*/

        //Tree gun
        public static ConfigEntry<float> treegunLiftFactor;
        public static ConfigEntry<bool> treegunUseRandomRotation;
        public static ConfigEntry<float> treegunObjectRotation;

        public static void Initialize(ConfigFile cfg)
        {
            Config = cfg;
            SettingsApi.RegisterModSettingsDrawers(Plugin.Instance, BuildSettingsDrawers);
            BindConfig();
        }

        private static void BindConfig()
        { 
            // General Settings
            //applyBasicValues = Config.Bind("01.General Settings", "1.Apply Basic Values To Config", false, "[Button] Apply Basic Values to Config");
            //applyBasicValues.SettingChanged += ApplyBasicValues;
            enableKey = Config.Bind("01.General Settings", "2.Enable Key", KeyCode.None, "Optional key that must be held before another key can trigger,\nallowing combinations like CTRL + S instead of S.");
            modifierKey = Config.Bind("01.General Settings", "3.Modifier Key", KeyCode.LeftShift, "A function-shift key that changes behaviour,\nsuch as moving on the Y axis instead of the Z axis while held.");

            // Axis Cycling
            axisCycleKey = Config.Bind("02.Axis Cycling", "1.Axis Cycle Key", KeyCode.None, "Cycle through the gizmo arrows to select different axis while scaling or mirroring.");
            axisCycleRequiresEnableKey = Config.Bind("02.Axis Cycling", "2.Axis Cycle Requires Enable Key", false, "Require the enable key to cycle through axes.");
            includePlanesInCycle = Config.Bind("02.Axis Cycling", "3.Include Planes In Cycle", false, "Include XY, YZ and XZ planes in the axis cycling.");

            // Drag Selection
            dragSelectionKey = Config.Bind("03.Drag Selection", "1.Drag Selection Key", KeyCode.None, "Hold this key to start drawing a selection box.");
            mmbSelection = Config.Bind("03.Drag Selection", "2.MMB Selection", false, "Use middle mouse button for selection.");
            dragSelectionRequiresEnableKey = Config.Bind("03.Drag Selection", "3.Drag Selection Requires Enable Key", false, "Requires enable key for drag selection.");

            // Scaling
            scrollScaling = Config.Bind("04.Scaling", "1.Scroll Scaling", false, "Scale blocks when using scroll wheel.");
            invertScrollScaling = Config.Bind("04.Scaling", "2.Invert Scroll Scaling", false, "Invert scroll scaling direction");
            negativeScalingKey = Config.Bind("04.Scaling", "3.Negative Scaling Key", KeyCode.None, "Press this key to scale blocks down.");
            positiveScalingKey = Config.Bind("04.Scaling", "4.Positive Scaling Key", KeyCode.None, "Press this key to scale blocks up.");
            scalingRequiresEnableKey = Config.Bind("04.Scaling", "5.Scaling Requires Enable Key", false, "Require enable key for scaling.");
            scalingValues = Config.Bind("04.Scaling", "6.Scaling Values", "0.05;0.1;0.5;1;5;10;20", "Custom scaling values");
            defaultScalingValue = Config.Bind("04.Scaling", "7.Default Scaling Value", "10", "Default scaling value");
            resetScalingValues = Config.Bind("04.Scaling", "8.Reset Values To Default", false, "[Button] Reset the values in the text fields to their default values.");
            resetScalingValues.SettingChanged += ResetScalingValues;
            unitBasedScaling = Config.Bind("04.Scaling", "9.Unit Based Scaling", false, "True: Scale by unit, False: Scale by percentage.");
            unitBasedScalingToggleKey = Config.Bind("04.Scaling", "10.Unit Based Scaling Toggle Key", KeyCode.None, "Toggle between unit and percentage based scaling.");
            unitBasedScaling.SettingChanged += ColorGizmoButton;

            // Key Movement
            forwardUpMovement = Config.Bind("05.Key Movement", "1.Forward/Up Movement Key", KeyCode.None, "Key to move blocks forward, or up when modifier is held.");
            backDownMovement = Config.Bind("05.Key Movement", "2.Back/Down Movement Key", KeyCode.None, "Key to move blocks backwards or down when modifier is held.");
            leftMovement = Config.Bind("05.Key Movement", "3.Left Movement Key", KeyCode.None, "Key to move blocks left.");
            rightMovement = Config.Bind("05.Key Movement", "4.Right Movement Key", KeyCode.None, "Key to move blocks right.");
            movementRequiresEnableKey = Config.Bind("05.Key Movement", "5.Movement Requires Enable Key", false, "Require enable key for keyboard movement.");
            movementIfRotationIsDisabled = Config.Bind("05.Key Movement", "6.Movement If Rotation Is Disabled", false, "If key rotation is disabled, move the block when in rotation mode and pressing keys.");

            // Key Rotation
            xPositiveRotation = Config.Bind("06.Key Rotation", "1.X Positive Rotation Key", KeyCode.None, "Key for positive rotation around the X axis.");
            xNegativeRotation = Config.Bind("06.Key Rotation", "2.X Negative Rotation Key", KeyCode.None, "Key for negative rotation around the X axis.");
            yzPositiveRotation = Config.Bind("06.Key Rotation", "3.YZ Positive Rotation Key", KeyCode.None, "Key for positive rotation around the Z axis, or Y axis when modifier is held.");
            yzNegativeRotation = Config.Bind("06.Key Rotation", "4.YZ Negative Rotation Key", KeyCode.None, "Key for negative rotation around the Z axis, or Y axis when modifier is held.");
            rotationRequiresEnableKey = Config.Bind("06.Key Rotation", "5.Rotation Requires Enable Key", false, "Require enable key for rotation.");

            // Mirroring
            mirrorKey = Config.Bind("07.Mirroring", "1.Mirror Key", KeyCode.None, "Pressing this key will mirror the selected blocks.");
            mirrorRequiresEnableKey = Config.Bind("07.Mirroring", "2.Mirror Requires Enable Key", false, "Require enable key for mirroring.");

            // Clipboard
            clipboardCopy = Config.Bind("08.Clipboard", "1.Clipboard Copy Key", KeyCode.None, "Press this key to copy a selection to the clipboard.");
            clipboardPaste = Config.Bind("08.Clipboard", "2.Clipboard Paste Key", KeyCode.None, "Press this key to paste a selection to the editor.");
            clipboardRequiresEnableKey = Config.Bind("08.Clipboard", "3.Clipboard Requires Enable Key", false, "Require enable key for clipboard operations.");
            pasteClipboardToCamera = Config.Bind("08.Clipboard", "4.Paste Clipboard To Camera", false, "Paste clipboard content to camera, or to the original copy position.");

            // Fast Travel
            fastTravelKey = Config.Bind("09.Fast Travel", "1.Fast Travel Key", KeyCode.None, "Pressing this key will move the camera to the position of the selected block.");
            fastTravelRequiresEnableKey = Config.Bind("09.Fast Travel", "2.Fast Travel Requires Enable Key", false, "Require the enable key for fast traveling.");

            // Shortcuts
            saveShortcutKey = Config.Bind("10.Shortcuts", "1.Save Shortcut Key", KeyCode.None, "Pressing this key with an active selection will open the Blueprint Save Panel.");
            loadShortcutKey = Config.Bind("10.Shortcuts", "2.Load Shortcut Key", KeyCode.None, "Pressing this key will open the Blueprint Load Panel.");
            shortcutRequiresEnableKey = Config.Bind("10.Shortcuts", "3.Shortcut Requires Enable Key", false, "Should the save and load shortcut require enable to be held?");

            // Panel
            clearSearchOnExit = Config.Bind("11.Panel", "1.Clear Search On Exit", false, "Clear the search field when the Blueprint Load Panel is closed.");
            doubleLoadButtons = Config.Bind("11.Panel", "2.Double Load Buttons", false, "If true, the load panel will have an extra button that\nallows you to load the blueprint at the original location, instead of the grid position closest to you.");
            allowedExtensions = Config.Bind("11.Panel", "3.Allowed Extensions", ".png,.obj,.jpg,.realm,.zeeplist,.zip,.customsoapbox", "Allowed file extensions");

            // Gizmo
            useCustomValues = Config.Bind("12.Gizmo", "1.Use Custom Values", false, "Enabling this will allow you to set custom values for the gizmo.");
            customXZValues = Config.Bind("12.Gizmo", "2.Custom XZ Values", "0;0.2;0.8;1.6;4;8;16", "Custom XZ values for gizmo");
            defaultCustomXZValue = Config.Bind("12.Gizmo", "3.Default Custom XZ Value", "16", "Default custom XZ value for gizmo");
            customYValues = Config.Bind("12.Gizmo", "4.Custom Y Values", "0;0.2;0.8;1.6;4;8", "Custom Y values for gizmo");
            defaultCustomYValue = Config.Bind("12.Gizmo", "5.Default Custom Y Value", "8", "Default custom Y value for gizmo");
            customRValues = Config.Bind("12.Gizmo", "6.Custom R Values", "0;1;5;10;30;45;90", "Custom R values for gizmo");
            defaultCustomRValue = Config.Bind("12.Gizmo", "7.Default Custom R Value", "45", "Default custom R value for gizmo");
            resetCustomValues = Config.Bind("12.Gizmo", "8.Reset Values To Default", false, "[Button] Reset the values in the text fields to their default values.");
            resetCustomValues.SettingChanged += ResetCustomGridValues;

            //Property clipboard
            /*copyPositionKey = Config.Bind("14. Property Clipboard", "1. Copy Paste Position Key", KeyCode.None, "Key to copy the position (and paste when combined with modifier key) of the first selected object.");
            copyRotationKey = Config.Bind("14. Property Clipboard", "2. Copy Paste Rotation Key", KeyCode.None, "Key to copy the rotation (and paste when combined with modifier key) of the first selected object.");
            copyScaleKey = Config.Bind("14. Property Clipboard", "3. Copy Paste Scale Key", KeyCode.None, "Key to copy the scale (and paste when combined with modifier key) of the first selected object.");
            copyOptionsKey = Config.Bind("14. Property Clipboard", "4. Copy Paste Options Key", KeyCode.None, "Key to copy the options (and paste when combined with modifier key) of the first selected object.");
            copyPaintsKey = Config.Bind("14. Property Clipboard", "5. Copy Paste Paints Key", KeyCode.None, "Key to copy the paints (and paste when combined with modifier key) of the first selected object.");
            copyAllConfiguredKey = Config.Bind("14. Property Clipboard", "6. Copy Paste All Configured Key", KeyCode.None, "Key to copy all the configured options (and paste when combined with modifier key) of the first selected object.");
            includePositionInCopyAll = Config.Bind("14. Property Clipboard", "7. Include Position In Copy All", false, "When using the copy all configured key, should the position be part of the properties being copied?");
            includeRotationInCopyAll = Config.Bind("14. Property Clipboard", "8. Include Rotation In Copy All", false, "When using the copy all configured key, should the rotation be part of the properties being copied?");
            includeScaleInCopyAll = Config.Bind("14. Property Clipboard", "9. Include Scale In Copy All", false, "When using the copy all configured key, should the scale be part of the properties being copied?");
            includeOptionsInCopyAll = Config.Bind("14. Property Clipboard", "10. Include Options In Copy All", false, "When using the copy all configured key, should the options be part of the properties being copied?");
            includePaintsInCopyAll = Config.Bind("14. Property Clipboard", "11. Include Paints In Copy All", false, "When using the copy all configured key, should the paints be part of the properties being copied?");
            propertyClipboardRequiresEnableKey = Config.Bind("14. Property Clipboard", "12. Property Clipboard Requires Enable Key", false, "Requires enable key for property clipboard operations");*/

            //Treegun
            treegunLiftFactor = Config.Bind("15. Treegun", "1. Lift Factor", 0.8f, "Tune the height to the surface of a blueprint shot with the tree gun.");
            treegunUseRandomRotation = Config.Bind("15. Treegun", "2. Use Random Rotation", true, "Use a random rotation when placing with the treegun, otherwise use the value below.");
            treegunObjectRotation = Config.Bind("15. Treegun", "3. Non Random Object Rotation", 0f, "This value will be used for the Y rotation, when placing a blueprint with the treegun.");
            Config.SettingChanged += ConfigChanged;
        }

        private static IEnumerable<IZeepSettingsDrawer> BuildSettingsDrawers(ModSettingsDrawerBuildContext context)
        {
            yield return new BPXSettingsDrawer();
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

        private static void ColorGizmoButton(object sender, EventArgs e)
        {
            BPXUIManagement.ColorGizmoButton();
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

        public static void ApplyBasicValues()
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
            defaultScalingValue.Value = (string)defaultScalingValue.DefaultValue;

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
            customXZValues.Value = "0;0.2;0.8;1.6;4;8;16";
            defaultCustomXZValue.Value = "16";
            customYValues.Value = "0;0.2;0.8;1.6;4;8";
            defaultCustomYValue.Value = "8";
            customRValues.Value = "0;1;5;10;30;45;90";
            defaultCustomRValue.Value = "45";
            ReloadToApplyMessage();
        }

        #region GetSet
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

        public static bool ScaleUnitBased()
        {
            return unitBasedScaling.Value;
        }

        public static KeyCode ScaleUnitBasedToggleKey()
        {
            return unitBasedScalingToggleKey.Value;
        }

        public static void ToggleScaleUnitBased()
        {
            unitBasedScaling.Value = !unitBasedScaling.Value;
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
            return ext == ".zeeplevel" || ext == ".jpg" || ext == ".zeeplevel" || GetAllowedExtensions().Contains(ext);
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

        /*//Property Clipboard
        public static KeyCode GetPropertyClipboardPositionKey()
        {
            return copyPositionKey.Value;
        }

        public static KeyCode GetPropertyClipboardRotationKey()
        {
            return copyRotationKey.Value;
        }

        public static KeyCode GetPropertyClipboardScaleKey()
        {
            return copyScaleKey.Value;
        }

        public static KeyCode GetPropertyClipboardOptionsKey()
        {
            return copyOptionsKey.Value;
        }

        public static KeyCode GetPropertyClipboardPaintsKey()
        {
            return copyPaintsKey.Value;
        }

        public static KeyCode GetPropertyClipboardCopyAllKey()
        {
            return copyAllConfiguredKey.Value;
        }

        public static bool IsPropertyClipboardPositionIncluded()
        {
            return includePositionInCopyAll.Value;
        }

        public static bool IsPropertyClipboardRotationIncluded()
        {
            return includeRotationInCopyAll.Value;
        }

        public static bool IsPropertyClipboardScaleIncluded()
        {
            return includeScaleInCopyAll.Value;
        }

        public static bool IsPropertyClipboardOptionsIncluded()
        {
            return includeOptionsInCopyAll.Value;
        }

        public static bool IsPropertyClipboardPaintsIncluded()
        {
            return includePaintsInCopyAll.Value;
        }

        public static bool PropertyClipboardRequiresEnableKey()
        {
            return propertyClipboardRequiresEnableKey.Value;
        }*/

        public static float GetTreegunLiftFactor()
        {
            return treegunLiftFactor.Value;
        }

        public static bool UseRandomTreegunRotation()
        {
            return treegunUseRandomRotation.Value;
        }

        public static float GetTreegunObjectRotation()
        {
            return treegunObjectRotation.Value;
        }

        #endregion
    }
}
