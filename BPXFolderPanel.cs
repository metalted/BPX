using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BPX
{
    public class BPXFolderPanel
    {
        public BPXPanel panel;
        public RectTransform Rect;
        public LEV_CustomButton exitButton;
        public LEV_CustomButton createButton;
        public TMP_InputField input;

        public BPXFolderPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;
            RectTransform panelRect = rect.GetChild(0).GetComponent<RectTransform>();
            exitButton = panelRect.GetChild(0).GetComponent<LEV_CustomButton>();
            createButton = panelRect.GetChild(1).GetComponent<LEV_CustomButton>();
            input = panelRect.GetChild(2).GetComponent<TMP_InputField>();

            UIManagement.UnbindButton(exitButton);
            UIManagement.RecolorButton(exitButton, UIManagement.blue);
            UIManagement.RebindButton(exitButton, () => OnExitButton());

            UIManagement.UnbindButton(createButton);
            UIManagement.RecolorButton(createButton, UIManagement.blue);
            UIManagement.RebindButton(createButton, () => OnCreateButton());
        }

        private void OnExitButton()
        {

        }

        private void OnCreateButton()
        {

        }

        public void Disable()
        {
            Rect.gameObject.SetActive(false);
        }

        public void Enable()
        {
            Rect.gameObject.SetActive(true);
        }
    }
}
