using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

namespace BPX
{
    public static class BPXUIManagement
    {
        public static Color blue = new Color(0, 0.547f, 0.82f, 1f);
        public static Color darkBlue = new Color(0, 0.371f, 0.547f, 1f);
        private static LEV_CustomButton toolbarSaveButton;
        private static LEV_CustomButton toolbarLoadButton;
        private static LEV_CustomButton gizmoScaleButton;
        private static BPXPanel panel;
        
        public static void InitializeLevelEditor(LEV_LevelEditorCentral central)
        {
            InitializePanel(central);
            InitializeToolbar(central);
            InitializeGizmoButton(central);
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
            GameObject scaleButton = GameObject.Instantiate(xzButtonRect.gameObject, xzButtonRect.parent);
            RectTransform scaleButtonRect = scaleButton.GetComponent<RectTransform>();
            scaleButton.name = "S Grid Button";

            //Change the label text for the scaling button.
            scaleButtonRect.GetChild(2).GetComponent<TextMeshProUGUI>().text = "S";

            //Move the header up one button height.
            gridSizeTextRect.anchorMin += buttonShift;
            gridSizeTextRect.anchorMax += buttonShift;

            //Move the new scaling button up
            scaleButtonRect.anchorMin += buttonShift;
            scaleButtonRect.anchorMax += buttonShift;

            //Set a new color to the button.
            gizmoScaleButton = scaleButton.GetComponent<LEV_CustomButton>();
            RecolorButton(gizmoScaleButton, blue);
            UnbindButton(gizmoScaleButton);
            RebindButton(gizmoScaleButton, () => OnGizmoScaleButton());
        }

        private static void OnToolbarSaveButton()
        {
            if(!BPXManager.AnyObjectsSelected())
            {
                PlayerManager.Instance.messenger.Log("No selection!", 2f);
                return;
            }

            ZeeplevelFile toSave = ZeeplevelHandler.BlockPropertiesToZeeplevelFile(BPXManager.central.selection.list);
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
            BPXManager.central.tool.DisableAllTools();
            BPXManager.central.tool.RecolorButtons();
            BPXManager.central.tool.currentTool = 3;
            BPXManager.central.tool.inspectorTitle.text = "";
        }

        public static void OnPanelClose()
        {
            toolbarLoadButton.isSelected = false;
            toolbarSaveButton.isSelected = false;

            BPXManager.central.tool.EnableEditTool();
            BPXManager.central.tool.RecolorButtons();
            BPXManager.central.cam.OverrideOutsideGameView(false);
        }

        private static void OnGizmoScaleButton()
        {

        }

        private static void SetGizmoScaleButtonText(string value)
        {
            gizmoScaleButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = value;
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
    }
}
