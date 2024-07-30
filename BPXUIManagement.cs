using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

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
        public static Color grey = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static Color darkBlue = new Color(0, 0.371f, 0.547f, 1f);
        private static LEV_CustomButton toolbarSaveButton;
        private static LEV_CustomButton toolbarLoadButton;
        private static LEV_CustomButton toolbarOnlineButton;
        private static BPXScaleButton scaleButton;
        private static BPXGizmo gizmo;
        private static BPXPanel panel;
        private static BPXOnlinePanel onlinePanel;
        private static bool panelIsOpen = false;
        
        public static void InitializeLevelEditor(LEV_LevelEditorCentral central)
        {
            InitializePanels(central);
            InitializeToolbar(central);
            InitializeGizmoButton(central);
            InitializeGizmo(central);

            SaveOriginalGridButtonInfo();
            UpdateGridButtons();

            central.gameObject.AddComponent<BPXController>();
        }

        private static void InitializePanels(LEV_LevelEditorCentral central)
        {
            Transform panelCopy = GameObject.Instantiate<Transform>(central.saveload.transform, central.saveload.transform.parent);
            panelCopy.gameObject.name = "BPXPanel";
            GameObject.Destroy(panelCopy.GetComponent<LEV_SaveLoad>());
            panel = panelCopy.gameObject.AddComponent<BPXPanel>();
            panel.Initialize(central);

            Transform onlinePanelCopy = GameObject.Instantiate<Transform>(central.saveload.transform, central.saveload.transform.parent);
            panelCopy.gameObject.name = "BPXOnlinePanel";
            GameObject.Destroy(onlinePanelCopy.GetComponent<LEV_SaveLoad>());
            onlinePanel = onlinePanelCopy.gameObject.AddComponent<BPXOnlinePanel>();
            onlinePanel.Initialize(central);
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

            toolbarOnlineButton = SplitLEVCustomButton(central.tool.button_settings);
            RecolorButton(toolbarOnlineButton, blue);
            UnbindButton(toolbarOnlineButton);
            RebindButton(toolbarOnlineButton, () => OnToolbarOnlineButton());
            toolbarOnlineButton.transform.GetChild(0).GetComponent<Image>().sprite = BPXSprites.onlineSprite;
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

        private static void OnToolbarOnlineButton()
        {
            BPXManager.DeselectAllBlocks();
            if (onlinePanel != null)
            {
                onlinePanel.Open(BPXPanelState.Open);
                toolbarOnlineButton.isSelected = true;
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

        public static void OnOnlinePanelOpen()
        {
            panelIsOpen = true;
            BPXManager.central.tool.DisableAllTools();
            BPXManager.central.tool.RecolorButtons();
            BPXManager.central.tool.currentTool = 3;
            BPXManager.central.tool.inspectorTitle.text = "";
        }

        public static void OnOnlinePanelClose()
        {
            panelIsOpen = false;

            toolbarOnlineButton.isSelected = false;

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

        private static List<float> xzOriginalList;
        private static List<float> yOriginalList;
        private static List<float> rOriginalList;
        private static int startixz;
        private static int startiy;
        private static int startir;

        private static void SaveOriginalGridButtonInfo()
        {
            xzOriginalList = new List<float>(BPXManager.central.gizmos.list_gridXZ);
            yOriginalList = new List<float>(BPXManager.central.gizmos.list_gridY);
            rOriginalList = new List<float>(BPXManager.central.gizmos.list_gridR);

            startixz = BPXManager.central.gizmos.startiXZ;
            startiy = BPXManager.central.gizmos.startiY;
            startir = BPXManager.central.gizmos.startiR;
        }

        public static void UpdateGridButtons()
        {
            if (BPXConfiguration.UseCustomValues())
            {
                float[] xz = BPXConfiguration.GetCustomXZValues();
                float[] y = BPXConfiguration.GetCustomYValues();
                float[] r = BPXConfiguration.GetCustomRValues();

                float defaultXZ = BPXConfiguration.GetDefaultCustomXZValue();
                float defaultY = BPXConfiguration.GetDefaultCustomYValue();
                float defaultR = BPXConfiguration.GetDefaultCustomRValue();

                int xzDefaultIndex = Array.IndexOf(xz, defaultXZ);
                int yDefaultIndex = Array.IndexOf(y, defaultY);
                int rDefaultIndex = Array.IndexOf(r, defaultR);

                // Handle XZ list
                if (xz.Length != 0)
                {
                    BPXManager.central.gizmos.list_gridXZ = xz.ToList();
                    int xzi = (xzDefaultIndex != -1) ? xzDefaultIndex : 0;
                    BPXManager.central.gizmos.startiXZ = xzi;
                    BPXManager.central.gizmos.index_gridXZ = xzi;
                }
                else
                {
                    BPXManager.central.gizmos.list_gridXZ = new List<float>(xzOriginalList);
                    BPXManager.central.gizmos.startiXZ = startixz;
                    BPXManager.central.gizmos.index_gridXZ = startixz;
                }

                // Handle Y list
                if (y.Length != 0)
                {
                    BPXManager.central.gizmos.list_gridY = y.ToList();
                    int yi = (yDefaultIndex != -1) ? yDefaultIndex : 0;
                    BPXManager.central.gizmos.startiY = yi;
                    BPXManager.central.gizmos.index_gridY = yi;
                }
                else
                {
                    BPXManager.central.gizmos.list_gridY = new List<float>(yOriginalList);
                    BPXManager.central.gizmos.startiY = startiy;
                    BPXManager.central.gizmos.index_gridY = startiy;
                }

                // Handle R list
                if (r.Length != 0)
                {
                    BPXManager.central.gizmos.list_gridR = r.ToList();
                    int ri = (rDefaultIndex != -1) ? rDefaultIndex : 0;
                    BPXManager.central.gizmos.startiR = ri;
                    BPXManager.central.gizmos.index_gridR = ri;
                }
                else
                {
                    BPXManager.central.gizmos.list_gridR = new List<float>(rOriginalList);
                    BPXManager.central.gizmos.startiR = startir;
                    BPXManager.central.gizmos.index_gridR = startir;
                }
            }
        }
    }
}
