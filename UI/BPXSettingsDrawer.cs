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
        private const int SelectionTabRows = 26;
        private const int TransformTabRows = 50;
        private const int CustomValuesTabRows = 50;
        private const int ClipboardTabRows = 45;
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

                if (gui.BeginTab("Clipboard"))
                {
                    selectedTab = BPXSettingsTab.Clipboard;
                    DrawClipboardTab(gui, context);
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

            gui.BeginVertical();
                gui.Text("General Controls");
                DrawDefaultEntry(gui, context, BPXConfiguration.enableKey, "Enable Key");
                DrawDefaultEntry(gui, context, BPXConfiguration.modifierKey, "Modifier Key");
            gui.EndVertical();

            gui.Separator();

            gui.BeginVertical();
                gui.Text("Save/Load Blueprint Shortcuts");
                DrawDefaultEntry(gui, context, BPXConfiguration.saveShortcutKey, "Save Shortcut Key");
                DrawDefaultEntry(gui, context, BPXConfiguration.loadShortcutKey, "Load Shortcut Key");
                DrawDefaultEntry(gui, context, BPXConfiguration.shortcutRequiresEnableKey, "Requires Enable");
            gui.EndVertical();

            gui.Separator();

            gui.BeginVertical();
                gui.Text("Save/Load Blueprint Panel");
                DrawDefaultEntry(gui, context, BPXConfiguration.doubleLoadButtons, "Double Load Buttons");
                DrawDefaultEntry(gui, context, BPXConfiguration.clearSearchOnExit, "Clear Search On Exit");

                gui.BeginVertical();
                    gui.Text("Allowed Extensions");
                    GetAllowedExtensionsEditor().Draw(gui);
                gui.EndVertical();
            gui.EndVertical();

            gui.Separator();

            gui.BeginVertical();
                gui.Text("Fast Travel");
                DrawDefaultEntry(gui, context, BPXConfiguration.fastTravelKey, "Fast Travel Key");
                DrawDefaultEntry(gui, context, BPXConfiguration.fastTravelRequiresEnableKey, "Requires Enable");
            gui.EndVertical();

            gui.Separator();

            gui.BeginVertical();
                gui.Text("Treegun");
                DrawDefaultEntry(gui, context, BPXConfiguration.treegunLiftFactor, "Lift Factor");
                DrawDefaultEntry(gui, context, BPXConfiguration.treegunUseRandomRotation, "Random Rotation");
                if (!BPXConfiguration.treegunUseRandomRotation.Value)
                {
                    DrawDefaultEntry(gui, context, BPXConfiguration.treegunObjectRotation, "Rotation");
                }
            gui.EndVertical();

            gui.Separator();

            gui.BeginVertical();
                gui.Text("Utility");

                if (gui.Button("Apply Basic Values"))
                {
                    BPXConfiguration.ApplyBasicValues();
                }
            gui.EndVertical();
        }

        private void DrawSelectionTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.Text("Axis Cycling");
            DrawDefaultEntry(gui, context, BPXConfiguration.axisCycleKey, "Axis Cycle Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.axisCycleRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.includePlanesInCycle, "Include Planes In Cycle");

            gui.Separator();
            gui.Text("Drag Selection");
            DrawDefaultEntry(gui, context, BPXConfiguration.dragSelectionKey, "Drag Selection Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.mmbSelection, "MMB Selection");
            DrawDefaultEntry(gui, context, BPXConfiguration.dragSelectionRequiresEnableKey, "Requires Enable");
        }

        private void DrawTransformTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.Text("Key Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.forwardUpMovement, "Forward / Up Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.backDownMovement, "Back / Down Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.leftMovement, "Left Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.rightMovement, "Right Movement");
            DrawDefaultEntry(gui, context, BPXConfiguration.movementRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.movementIfRotationIsDisabled, "Movement If Rotation Is Disabled");

            gui.Separator();
            gui.Text("Key Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.xPositiveRotation, "X Positive Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.xNegativeRotation, "X Negative Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.yzPositiveRotation, "YZ Positive Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.yzNegativeRotation, "YZ Negative Rotation");
            DrawDefaultEntry(gui, context, BPXConfiguration.rotationRequiresEnableKey, "Requires Enable");

            gui.Separator();
            gui.Text("Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.scrollScaling, "Scroll Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.invertScrollScaling, "Invert Scroll Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.negativeScalingKey, "Negative Scaling Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.positiveScalingKey, "Positive Scaling Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.scalingRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.unitBasedScaling, "Unit Based Scaling");
            DrawDefaultEntry(gui, context, BPXConfiguration.unitBasedScalingToggleKey, "Unit Based Toggle Key");

            gui.Separator();
            gui.Text("Mirroring");
            DrawDefaultEntry(gui, context, BPXConfiguration.mirrorKey, "Mirror Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.mirrorRequiresEnableKey, "Requires Enable");
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

        private void DrawClipboardTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.Text("Clipboard");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardCopy, "Clipboard Copy Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardPaste, "Clipboard Paste Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.clipboardRequiresEnableKey, "Requires Enable");
            DrawDefaultEntry(gui, context, BPXConfiguration.pasteClipboardToCamera, "Paste Clipboard To Camera");

            gui.Separator();
            gui.Text("Property Clipboard");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyPositionKey, "Copy / Paste Position Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyRotationKey, "Copy / Paste Rotation Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyScaleKey, "Copy / Paste Scale Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyOptionsKey, "Copy / Paste Options Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyPaintsKey, "Copy / Paste Paints Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.copyAllConfiguredKey, "Copy / Paste All Configured Key");
            DrawDefaultEntry(gui, context, BPXConfiguration.includePositionInCopyAll, "Include Position In Copy All");
            DrawDefaultEntry(gui, context, BPXConfiguration.includeRotationInCopyAll, "Include Rotation In Copy All");
            DrawDefaultEntry(gui, context, BPXConfiguration.includeScaleInCopyAll, "Include Scale In Copy All");
            DrawDefaultEntry(gui, context, BPXConfiguration.includeOptionsInCopyAll, "Include Options In Copy All");
            DrawDefaultEntry(gui, context, BPXConfiguration.includePaintsInCopyAll, "Include Paints In Copy All");
            DrawDefaultEntry(gui, context, BPXConfiguration.propertyClipboardRequiresEnableKey, "Requires Enable");
        }

        private void DrawHelpTab(ImGui gui, ZeepSettingsDrawContext context)
        {
            gui.Separator();
            gui.Text("BPX Help");
            gui.Text("Configure BPX controls and editor behavior here.");
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

            if (selectedTab == BPXSettingsTab.Clipboard)
            {
                return ClipboardTabRows;
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
        Clipboard,
        Help
    }
}