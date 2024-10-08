﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using I2.Loc;

namespace BPX
{
    public class BPXPanelComponent
    {
        public BPXPanelComponentType ComponentType;
        public BPXPanelComponentName ComponentName;
        public LEV_CustomButton Button;
        public Image Image;
        public Image textInputBackground;
        public RectTransform Rect;
        public TextMeshProUGUI textMesh;
        public ScrollRect ScrollRect;
        public TMP_InputField textInputField;
        public RectTransform explorerPanel;
        public ContentSizeFitter contentSizeFitter;
        public GridLayoutGroup gridLayoutGroup;

        public BPXPanelComponent(BPXPanelComponentType componentType, BPXPanelComponentName componentName, RectTransform rect)
        {
            this.ComponentType = componentType;
            this.ComponentName = componentName;
            this.Rect = rect;
            this.Rect.gameObject.name = componentName.ToString();

            switch (ComponentType)
            {
                case BPXPanelComponentType.Button:
                    Button = rect.GetComponent<LEV_CustomButton>();
                    if (Button != null)
                    {
                        BPXUIManagement.UnbindButton(Button);
                        BPXUIManagement.StandardRecolorButton(Button);
                    }
                    break;
                case BPXPanelComponentType.Image:
                    Image = rect.GetComponent<Image>();
                    break;
                case BPXPanelComponentType.Text:
                    textMesh = rect.GetComponent<TextMeshProUGUI>();
                    textMesh.text = "";
                    break;
                case BPXPanelComponentType.ScrollView:
                    ScrollRect = rect.GetComponent<ScrollRect>();

                    // Configuring the ScrollRect as per the standalone code
                    explorerPanel = ScrollRect.content;
                    contentSizeFitter = explorerPanel.gameObject.GetComponent<ContentSizeFitter>();
                    if (contentSizeFitter == null)
                    {
                        contentSizeFitter = explorerPanel.gameObject.AddComponent<ContentSizeFitter>();
                    }
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    gridLayoutGroup = explorerPanel.gameObject.GetComponent<GridLayoutGroup>();
                    if(gridLayoutGroup == null)
                    {
                        gridLayoutGroup = explorerPanel.gameObject.AddComponent<GridLayoutGroup>();
                    }                       
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount; 
                    SetGridLayoutColumns(6);
                    break;
                case BPXPanelComponentType.TextInput:
                    textInputField = rect.GetComponent<TMP_InputField>();

                    textInputBackground = rect.GetComponent<Image>();
                    if (textInputBackground != null)
                    {
                        textInputBackground.color = Color.white;
                    }

                    Localize loc = textInputField.placeholder.GetComponent<Localize>();
                    if (loc != null)
                    {
                        GameObject.Destroy(loc);
                    }

                    SetPlaceHolderText("");
                    break;
            }
        }

        public void SetGridLayoutColumns(int count)
        {
            if(ComponentType != BPXPanelComponentType.ScrollView)
            {
                return;
            }

            gridLayoutGroup.constraintCount = count;

            Rect viewportRect = ScrollRect.viewport.rect;
            int paddingValue = Mathf.RoundToInt(viewportRect.width / 100f);
            float cellWidth = (viewportRect.width - paddingValue * 2) / ((float)count);
            float cellSpacing = cellWidth * 0.05f;

            gridLayoutGroup.cellSize = new Vector2(cellWidth - cellSpacing, cellWidth - cellSpacing);
            gridLayoutGroup.spacing = new Vector2(cellSpacing, cellSpacing);
            gridLayoutGroup.padding = new RectOffset(paddingValue, paddingValue, paddingValue, paddingValue);
        }

        public void Reset()
        {
            switch(ComponentType)
            {
                case BPXPanelComponentType.Button:
                    Button.ResetAllBools();
                    break;
            }
        }

        public void Enable()
        {
            Rect.gameObject.SetActive(true);
        }

        public void Disable()
        {
            Rect.gameObject.SetActive(false);
        }

        public void SetInteractable(bool state)
        {
            if(ComponentType == BPXPanelComponentType.TextInput)
            {
                textInputField.interactable = state;
            }
        }

        public void BindButton(UnityAction action)
        {
            if (ComponentType == BPXPanelComponentType.Button)
            {
                BPXUIManagement.RebindButton(Button, action);
            }
        }

        public void SetRectAnchors(float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY)
        {
            Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
        }

        public void SetButtonImageRectAnchors(float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY)
        {
            if (ComponentType == BPXPanelComponentType.Button)
            {
                RectTransform imageChild = Rect.GetChild(0).GetComponent<RectTransform>();
                imageChild.anchorMin = new Vector2(anchorMinX, anchorMinY);
                imageChild.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            }
        }

        public void ColorImage(Color color)
        {
            if (ComponentType == BPXPanelComponentType.Image)
            {
                Image.color = color;
            }
        }

        public void SetText(string text)
        {
            if (ComponentType == BPXPanelComponentType.Text)
            {
                textMesh.text = text;
            }
            else if (ComponentType == BPXPanelComponentType.TextInput)
            {
                textInputField.text = text;
            }
        }

        public void SetPlaceHolderText(string text)
        {
            if (ComponentType == BPXPanelComponentType.TextInput)
            {
                textInputField.placeholder.GetComponent<TMP_Text>().text = text;
            }
        }

        public string GetText()
        {
            if(ComponentType == BPXPanelComponentType.TextInput)
            {
                return textInputField.text;
            }

            return "";
        }

        public void SetButtonImage(Sprite sprite)
        {
            Rect.GetChild(0).GetComponent<Image>().sprite = sprite;
        }

        public void HideButtonText()
        {
            if (ComponentType == BPXPanelComponentType.Button)
            {
                TextMeshProUGUI textGUI = Button.GetComponentInChildren<TextMeshProUGUI>();
                if (textGUI != null)
                {
                    textGUI.gameObject.SetActive(false);
                }
            }
        }
    }
}
