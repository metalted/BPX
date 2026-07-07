using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using ZeepSDK.Settings.Drawers;

namespace BPX.UI
{
    public class BPXSettingsDrawer : IZeepSettingsDrawer
    {
        private ConfigStringListEditor allowedExtensionsEditor;
        private BPXSettingsTab selectedTab = BPXSettingsTab.General;

        private const int GeneralTabRows = 50;
        private const int SelectionTabRows = 25;
        private const int TransformTabRows = 50;
        private const int CustomValuesTabRows = 50;
        private const int UtilitiesTabRows = 25;
        private const int HelpTabRows = 25;

        private ConfigOrderedStringListEditor customXZValuesEditor;
        private ConfigOrderedStringListEditor customYValuesEditor;
        private ConfigOrderedStringListEditor customRValuesEditor;
        private ConfigOrderedStringListEditor scalingValuesEditor;

        public void Draw(ImGui gui, ZeepSettingsDrawContext context)
        {
            using (gui.Indent())
            {
                gui.BeginTabsPane(GetTabsPaneRect(gui));

                if (gui.BeginTab("General"))
                {
                    selectedTab = BPXSettingsTab.General;
                    DrawGeneralTab(gui, context);
                    gui.EndTab();
                }

                if (gui.BeginTab("Selection"))
                {
                    selectedTab = BPXSettingsTab.Selection;
                    DrawSelectionTab(gui, context);
                    gui.EndTab();
                }

                if (gui.BeginTab("Transform"))
                {
                    selectedTab = BPXSettingsTab.Transform;
                    DrawTransformTab(gui, context);
                    gui.EndTab();
                }

                if (gui.BeginTab("Custom Values"))
                {
                    selectedTab = BPXSettingsTab.CustomValues;
                    DrawCustomValuesTab(gui, context);
                    gui.EndTab();
                }

                if (gui.BeginTab("Utilities"))
                {
                    selectedTab = BPXSettingsTab.Utilities;
                    DrawUtilitiesTab(gui, context);
                    gui.EndTab();
                }

                if (gui.BeginTab("Help"))
                {
                    selectedTab = BPXSettingsTab.Help;
                    DrawHelpTab(gui, context);
                    gui.EndTab();
                }

                gui.EndTabsPane();
            }
        }

        private void DrawGeneralTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.AddSpacing();
            gui.Text("General Controls");
            DrawDefaultEntry(gui, context, BPXConfiguration.enableKey, "Enable Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.modifierKey, "Modifier Key");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Save/Load Blueprint Panel");
            DrawDefaultEntry(gui, context, BPXConfiguration.doubleLoadButtons, "Double Load Buttons");
            DrawDefaultEntry(gui, context, BPXConfiguration.clearSearchOnExit, "Clear Search On Exit");
            gui.AddSpacing();
            gui.Text("Allowed Extensions");
            GetAllowedExtensionsEditor().Draw(gui);
            gui.AddSpacing();
            gui.Text("(i) When files have extension that are not allowed, the directory will be hidden in the Save- and Load Blueprint Panels.", wrap:true);
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Treegun");
            DrawDefaultEntry(gui, context, BPXConfiguration.treegunLiftFactor, "Lift Factor");
            DrawDefaultEntry(gui, context, BPXConfiguration.treegunUseRandomRotation, "Random Rotation");
            if (!BPXConfiguration.treegunUseRandomRotation.Value)
            {
                DrawDefaultEntry(gui, context, BPXConfiguration.treegunObjectRotation, "Rotation");
            }
            gui.AddSpacing();           
        }

        private void DrawSelectionTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.AddSpacing();
            gui.Text("Axis Cycling");
            DrawDefaultEntry(gui, context, BPXConfiguration.axisCycleKey, "Axis Cycle Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.axisCycleRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.includePlanesInCycle, "Include Planes In Cycle");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Drag Selection");
            DrawDefaultEntry(gui, context, BPXConfiguration.dragSelectionKey, "Drag Selection Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.mmbSelection, "MMB Selection");
            DrawDefaultEntry(gui, context, BPXConfiguration.dragSelectionRequiresEnableKey, "Requires Enable");
            gui.AddSpacing();
        }

        private void DrawTransformTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.AddSpacing();
            gui.Text("Key Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.forwardUpMovement, "Forward / Up Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.backDownMovement, "Back / Down Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.leftMovement, "Left Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.rightMovement, "Right Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.movementRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.movementIfRotationIsDisabled, "Movement If Rotation Is Disabled");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Key Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.xPositiveRotation, "X Positive Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.xNegativeRotation, "X Negative Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.yzPositiveRotation, "YZ Positive Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.yzNegativeRotation, "YZ Negative Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.rotationRequiresEnableKey, "Requires Enable");
            gui.AddSpacing();

            gui.Separator();
            gui.Text("Scaling");
            gui.AddSpacing();
            DrawDefaultEntry(gui, context, BPXConfiguration.scrollScaling, "Scroll Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.invertScrollScaling, "Invert Scroll Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.negativeScalingKey, "Negative Scaling Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.positiveScalingKey, "Positive Scaling Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.scalingRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.unitBasedScaling, "Unit Based Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.unitBasedScalingToggleKey, "Unit Based Toggle Key");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Mirroring");
            DrawDefaultEntry(gui, context, BPXConfiguration.mirrorKey, "Mirror Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.mirrorRequiresEnableKey, "Requires Enable");
            gui.AddSpacing();
        }

        private void DrawCustomValuesTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            DrawDefaultEntry(gui, context, BPXConfiguration.useCustomValues, "Use Custom Gizmo Values");

            if(BPXConfiguration.useCustomValues.Value)
            {
                gui.Separator();
                gui.AddSpacing();
                gui.Text("Custom XZ Values");
                GetCustomXZValuesEditor().Draw(gui);
                gui.AddSpacing();

                gui.Separator();
                gui.AddSpacing();
                gui.Text("Custom Y Values");
                GetCustomYValuesEditor().Draw(gui);
                gui.AddSpacing();

                gui.Separator();
                gui.AddSpacing();
                gui.Text("Custom Rotation Values");
                GetCustomRValuesEditor().Draw(gui);
                gui.AddSpacing();
            }           

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Scaling Values");
            GetScalingValuesEditor().Draw(gui);
            gui.AddSpacing();
        }

        private void DrawUtilitiesTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.AddSpacing();
            gui.Text("Save/Load Blueprint Shortcuts");
            DrawDefaultEntry(gui, context, BPXConfiguration.saveShortcutKey, "Save Shortcut Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.loadShortcutKey, "Load Shortcut Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.shortcutRequiresEnableKey, "Requires Enable");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Clipboard");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardCopy, "Clipboard Copy Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardPaste, "Clipboard Paste Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.pasteClipboardToCamera, "Paste Clipboard To Camera");
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Fast Travel");
            DrawDefaultEntry(gui, context, BPXConfiguration.fastTravelKey, "Fast Travel Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.fastTravelRequiresEnableKey, "Requires Enable");
            gui.AddSpacing();
        }

        private void DrawHelpTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.AddSpacing();
            gui.Text("If this is your first time using BlueprintsX, press the button below to apply a basic recommended configuration. For this setup to work properly, open the game's Input Settings and unbind F from Flipping Objects, and unbind Space from Delete Objects.", wrap: true);
            gui.AddSpacing();

            if (gui.Button("Apply Basic Values"))
            {
                BPXConfiguration.ApplyBasicValues();
            }

            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Axis Selection");
            gui.Text("Press Space while you have objects selected to cycle through the available axes. The gizmo arrows will grow or shrink to show the active axis. This is useful for scaling on a specific axis, and it is also used for mirroring.", wrap: true);
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Camera Speed");
            gui.Text("The Fast Travel key also works as the Camera Speed Configuration key. Make sure no objects are selected, then hold your assigned Fast Travel key and use the scroll wheel to change the editor camera speed.", wrap: true);
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Treegun");
            gui.Text("The Treegun can be used to place blueprints. To assign a blueprint, open the Load Blueprint panel, select a blueprint, then press the button with the Treegun icon. This sets the selected blueprint as a Treegun option. After that, press the Treegun button in the toolbar and select your blueprint from the extra icon. Seed mode is not supported for blueprints, only lightning mode is supported.", wrap: true);
            gui.AddSpacing();

            gui.Separator();
            gui.AddSpacing();
            gui.Text("Scale In Place");
            gui.Text("When scaling multiple objects, you may notice that the objects also move relative to each other. Scaling up increases the distance between them, and scaling down pulls them closer together. Hold the Modifier key while scaling to scale in place. The objects will still resize, but they will stay in their original positions.", wrap: true);
            gui.AddSpacing();
        }

        private void DrawDefaultEntry(ImGui gui, ZeepSettingsDrawContext context, ConfigEntryBase entry, string label)
        {
            if (entry == null)
            {
                return;
            }

            IZeepSettingsDrawer drawer = new ZeepSettingsEntryDrawer(entry, label);
            drawer.Draw(gui, context);
        }

        private ImRect GetTabsPaneRect(ImGui gui)
        {
            float width = gui.GetLayoutWidth();
            float height = gui.GetRowsHeightWithSpacing(GetSelectedTabRows());

            return gui.AddLayoutRect(width, height);
        }

        private int GetSelectedTabRows()
        {
            if (selectedTab == BPXSettingsTab.General)
            {
                return GeneralTabRows;
            }

            if (selectedTab == BPXSettingsTab.Selection)
            {
                return SelectionTabRows;
            }

            if (selectedTab == BPXSettingsTab.Transform)
            {
                return TransformTabRows;
            }

            if (selectedTab == BPXSettingsTab.CustomValues)
            {
                return CustomValuesTabRows;
            }

            if (selectedTab == BPXSettingsTab.Utilities)
            {
                return UtilitiesTabRows;
            }

            if (selectedTab == BPXSettingsTab.Help)
            {
                return HelpTabRows;
            }

            return GeneralTabRows;
        }

        private ConfigStringListEditor GetAllowedExtensionsEditor()
        {
            if (allowedExtensionsEditor == null)
            {
                allowedExtensionsEditor = new ConfigStringListEditor(
                    BPXConfiguration.allowedExtensions,
                    ',');
            }

            return allowedExtensionsEditor;
        }

        private ConfigOrderedStringListEditor GetCustomXZValuesEditor()
        {
            if (customXZValuesEditor == null)
            {
                customXZValuesEditor = new ConfigOrderedStringListEditor(
                    BPXConfiguration.customXZValues,
                    BPXConfiguration.defaultCustomXZValue,
                    ';');
            }

            return customXZValuesEditor;
        }

        private ConfigOrderedStringListEditor GetCustomYValuesEditor()
        {
            if (customYValuesEditor == null)
            {
                customYValuesEditor = new ConfigOrderedStringListEditor(
                    BPXConfiguration.customYValues,
                    BPXConfiguration.defaultCustomYValue,
                    ';');
            }

            return customYValuesEditor;
        }

        private ConfigOrderedStringListEditor GetCustomRValuesEditor()
        {
            if (customRValuesEditor == null)
            {
                customRValuesEditor = new ConfigOrderedStringListEditor(
                    BPXConfiguration.customRValues,
                    BPXConfiguration.defaultCustomRValue,
                    ';');
            }

            return customRValuesEditor;
        }

        private ConfigOrderedStringListEditor GetScalingValuesEditor()
        {
            if (scalingValuesEditor == null)
            {
                scalingValuesEditor = new ConfigOrderedStringListEditor(
                    BPXConfiguration.scalingValues,
                    BPXConfiguration.defaultScalingValue,
                    ';');
            }

            return scalingValuesEditor;
        }
    }

    public enum BPXSettingsTab
    {
        General,
        Selection,
        Transform,
        CustomValues,
        Utilities,
        Help
    }
}