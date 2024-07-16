using System;
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

        public BPXPanelComponent(BPXPanelComponentType componentType, BPXPanelComponentName componentName, RectTransform rect)
        {
            this.ComponentType = componentType;
            this.ComponentName = componentName;
            this.Rect = rect;

            switch (ComponentType)
            {
                case BPXPanelComponentType.Button:
                    Button = rect.GetComponent<LEV_CustomButton>();
                    if (Button != null)
                    {
                        UIManagement.UnbindButton(Button);
                        UIManagement.RecolorButton(Button, UIManagement.blue);
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
                    ContentSizeFitter contentSizeFitter = explorerPanel.gameObject.AddComponent<ContentSizeFitter>();
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    GridLayoutGroup gridLayoutGroup = explorerPanel.gameObject.AddComponent<GridLayoutGroup>();
                    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    gridLayoutGroup.constraintCount = 6;

                    Rect viewportRect = ScrollRect.viewport.rect;
                    int paddingValue = Mathf.RoundToInt(viewportRect.width / 100f);
                    float cellWidth = (viewportRect.width - paddingValue * 2) / 6.0f;
                    float cellSpacing = cellWidth * 0.05f;

                    gridLayoutGroup.cellSize = new Vector2(cellWidth - cellSpacing, cellWidth - cellSpacing);
                    gridLayoutGroup.spacing = new Vector2(cellSpacing, cellSpacing);
                    gridLayoutGroup.padding = new RectOffset(paddingValue, paddingValue, paddingValue, paddingValue);
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

        public void BindButton(UnityAction action)
        {
            if (ComponentType == BPXPanelComponentType.Button)
            {
                UIManagement.RebindButton(Button, action);
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

            }
        }

        public void SetPlaceHolderText(string text)
        {
            if (ComponentType == BPXPanelComponentType.TextInput)
            {
                textInputField.placeholder.GetComponent<TMP_Text>().text = text;
            }
        }

        public void GetText()
        {

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
