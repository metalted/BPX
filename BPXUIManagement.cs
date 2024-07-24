using UnityEngine.Events;
using UnityEngine;

namespace BPX
{
    public struct GizmoValues
    {
        public float XZ;
        public float Y;
        public float R;
        public float S;
    }

    public static class BPXUIManagement
    {
        public static Color blue = new Color(0, 0.547f, 0.82f, 1f);
        public static Color darkBlue = new Color(0, 0.371f, 0.547f, 1f);
        private static LEV_CustomButton toolbarSaveButton;
        private static LEV_CustomButton toolbarLoadButton;
        private static BPXScaleButton scaleButton;
        private static BPXGizmo gizmo;
        private static BPXPanel panel;
        private static bool panelIsOpen = false;
        
        public static void InitializeLevelEditor(LEV_LevelEditorCentral central)
        {
            InitializePanel(central);
            InitializeToolbar(central);
            InitializeGizmoButton(central);
            InitializeGizmo(central);

            central.gameObject.AddComponent<BPXController>();
        }

        private static void InitializePanel(LEV_LevelEditorCentral central)
        {
            Transform panelCopy = GameObject.Instantiate<Transform>(central.saveload.transform, central.saveload.transform.parent);
            panelCopy.gameObject.name = "BPXPanel";
            GameObject.Destroy(panelCopy.GetComponent<LEV_SaveLoad>());
            panel = panelCopy.gameObject.AddComponent<BPXPanel>();
            panel.Initialize(central);
        }

        private static void InitializeToolbar(LEV_LevelEditorCentral central)
        {
            toolbarSaveButton = SplitLEVCustomButton(central.tool.button_save);
            RecolorButton(toolbarSaveButton, blue);
            UnbindButton(toolbarSaveButton);
            RebindButton(toolbarSaveButton, () => OnToolbarSaveButton());

            toolbarLoadButton = SplitLEVCustomButton(central.tool.button_load);
            RecolorButton(toolbarLoadButton, blue);
            UnbindButton(toolbarLoadButton);
            RebindButton(toolbarLoadButton, () => OnToolbarLoadButton());
        }

        private static void InitializeGizmoButton(LEV_LevelEditorCentral central)
        {
            RectTransform xzButtonRect = central.gizmos.value_XZ.transform.parent.GetComponent<RectTransform>();
            RectTransform yButtonRect = central.gizmos.value_Y.transform.parent.GetComponent<RectTransform>();
            RectTransform gridSizeTextRect = xzButtonRect.parent.GetChild(0).GetComponent<RectTransform>();

            //Calculate the distance between the top left corners of the two buttons to shift the elements up.
            Vector2 buttonShift = new Vector2(0, xzButtonRect.anchorMin.y - yButtonRect.anchorMin.y);

            //Create a copy of the XZ button and set the name.
            GameObject buttonCopy = GameObject.Instantiate(xzButtonRect.gameObject, xzButtonRect.parent);
            RectTransform scaleButtonRect = buttonCopy.GetComponent<RectTransform>();
            buttonCopy.name = "S Grid Button";            

            //Move the header up one button height.
            gridSizeTextRect.anchorMin += buttonShift;
            gridSizeTextRect.anchorMax += buttonShift;

            //Move the new scaling button up
            scaleButtonRect.anchorMin += buttonShift;
            scaleButtonRect.anchorMax += buttonShift;

            //Set a new color to the button.
            LEV_CustomButton gizmoScaleButton = buttonCopy.GetComponent<LEV_CustomButton>();
            RecolorButton(gizmoScaleButton, blue);
            UnbindButton(gizmoScaleButton);

            //Add the GizmoScaler monobehaviour to the button.
            scaleButton = gizmoScaleButton.gameObject.AddComponent<BPXScaleButton>();

            //Assign the click to the function in the behaviour.
            RebindButton(gizmoScaleButton, () => scaleButton.OnClick());
        }

        private static void InitializeGizmo(LEV_LevelEditorCentral central)
        {
            gizmo = BPXManager.central.gizmos.gameObject.AddComponent<BPXGizmo>();
        }

        private static void OnToolbarSaveButton()
        {
            if(!BPXManager.AnyObjectsSelected())
            {
                PlayerManager.Instance.messenger.Log("No selection!", 2f);
                return;
            }

            ZeeplevelFile toSave = ZeeplevelHandler.FromBlockProperties(BPXManager.central.selection.list);
            toSave.SetPlayerName(BPXManager.GetPlayerName());

            BPXManager.DeselectAllBlocks();

            if (panel != null)
            {
                panel.SetBlueprintToSave(toSave);
                panel.Open(BPXPanelState.Save);
                toolbarSaveButton.isSelected = true;
            }
        }

        private static void OnToolbarLoadButton()
        {
            BPXManager.DeselectAllBlocks();
            if(panel != null)
            {
                panel.Open(BPXPanelState.Load);
                toolbarLoadButton.isSelected = true;
            }
        }

        public static void OnPanelOpen()
        {
            panelIsOpen = true;
            BPXManager.central.tool.DisableAllTools();
            BPXManager.central.tool.RecolorButtons();
            BPXManager.central.tool.currentTool = 3;
            BPXManager.central.tool.inspectorTitle.text = "";
        }

        public static void OnPanelClose()
        {
            panelIsOpen = false;

            toolbarLoadButton.isSelected = false;
            toolbarSaveButton.isSelected = false;

            BPXManager.central.tool.EnableEditTool();
            BPXManager.central.tool.RecolorButtons();
            BPXManager.central.cam.OverrideOutsideGameView(false);
        }

        public static void UnbindButton(LEV_CustomButton button)
        {
            button.onClick.RemoveAllListeners();

            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }

            //Disable the hotkey script.
            LEV_HotkeyButton hotkeybutton = button.GetComponent<LEV_HotkeyButton>();
            if (hotkeybutton != null)
            {
                hotkeybutton.enabled = false;
            }
        }

        public static void RebindButton(LEV_CustomButton button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public static void RecolorButton(LEV_CustomButton button, Color color, bool recolorAll = false)
        {
            button.normalColor = color;
            button.overrideNormalColor = true;
            button.buttonImage.color = color;

            if(recolorAll)
            {
                button.clickColor = color;
                button.hoverColor = color;
                button.normalColor = color;
                button.selectedColor = color;
                button.isDisabled_clickColor = color;
                button.isDisabled_hoverColor = color;
                button.isDisabled_normalColor = color;
                button.isDisabled_selectedColor = color;
            }
        }

        public static LEV_CustomButton SplitLEVCustomButton(LEV_CustomButton original, float padding = 0)
        {
            //Get the rect of the original button and calculate the size of the button based on the anchor points.
            RectTransform originalButtonRect = original.GetComponent<RectTransform>();
            Vector2 originalButtonSize = originalButtonRect.anchorMax - originalButtonRect.anchorMin;

            float paddingAmount = originalButtonSize.x * padding;

            //Duplicate the original and set the name.
            GameObject addedButton = GameObject.Instantiate(original.gameObject, original.transform.parent);
            RectTransform addedButtonRect = addedButton.GetComponent<RectTransform>();

            //Resize the original button so it only takes up half the horizontal space.
            originalButtonRect.anchorMax = new Vector2(originalButtonRect.anchorMax.x - (originalButtonSize.x / 2) - paddingAmount, originalButtonRect.anchorMax.y);

            //Resize the added button to take up the rest of the space.            
            addedButtonRect.anchorMin = new Vector2(addedButtonRect.anchorMin.x + originalButtonSize.x / 2 + paddingAmount, addedButtonRect.anchorMin.y);            

            //Get the LEV_CustomButton script of the new button and set the color and on click listener.
            LEV_CustomButton addedCustomButton = addedButton.GetComponent<LEV_CustomButton>();
            return addedCustomButton;
        }

        public static GizmoValues GetGizmoValues()
        {
            return new GizmoValues()
            {
                XZ = BPXManager.central.gizmos.list_gridXZ[BPXManager.central.gizmos.index_gridXZ],
                Y = BPXManager.central.gizmos.list_gridY[BPXManager.central.gizmos.index_gridY],
                R = BPXManager.central.gizmos.list_gridR[BPXManager.central.gizmos.index_gridR],
                S = scaleButton.GetCurrentValue()
            };
        }

        public static bool IsPanelOpen()
        {
            if(panel == null)
            {
                return false;
            }

            return panelIsOpen;
        }

        public static void PressButtonByName(string buttonName)
        {
            switch(buttonName)
            {
                case "Save":
                    if(toolbarSaveButton != null)
                    {
                        OnToolbarSaveButton();
                    }
                    break;
                case "Load":
                    if(toolbarLoadButton != null)
                    {
                        OnToolbarLoadButton();
                    }
                    break;
                case "Scale":
                    if(scaleButton != null)
                    {
                        scaleButton.OnClick();
                    }
                    break;
            }
        }

        public static BPXGizmo GetGizmo()
        {
            return gizmo;
        }
    }
}
