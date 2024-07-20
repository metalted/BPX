using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.IO;

namespace BPX
{
    public class BPXFolderPanel
    {
        public BPXPanel panel;
        public RectTransform Rect;
        public LEV_CustomButton exitButton;
        public LEV_CustomButton createButton;
        public TMP_InputField input;
        private bool folderCreated = false;

        public BPXFolderPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;
            RectTransform panelRect = rect.GetChild(0).GetComponent<RectTransform>();
            exitButton = panelRect.GetChild(0).GetComponent<LEV_CustomButton>();
            createButton = panelRect.GetChild(1).GetComponent<LEV_CustomButton>();
            input = panelRect.GetChild(2).GetComponent<TMP_InputField>();

            BPXUIManagement.UnbindButton(exitButton);
            BPXUIManagement.RecolorButton(exitButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(exitButton, () => OnExitButton());

            BPXUIManagement.UnbindButton(createButton);
            BPXUIManagement.RecolorButton(createButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(createButton, () => OnCreateButton());
        }

        private void OnExitButton()
        {
            input.text = "";

            exitButton.ResetAllBools();
            createButton.ResetAllBools();

            panel.OnFolderPanel(folderCreated);
            folderCreated = false;
            Disable();
        }

        private void OnCreateButton()
        {
            if(input.text.Trim() == "")
            {
                Plugin.Instance.LogScreenMessage("Invalid Folder Name");
                return;
            }

            string currentlyOpenedPath = panel.currentMode == BPXPanelMode.Blueprint ? panel.blueprintDirectory.FullName : panel.levelDirectory.FullName;
            string newFolderPath = Path.Combine(currentlyOpenedPath, input.text);
            
            if(!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
                folderCreated = true;
            }
            else
            {
                Plugin.Instance.LogScreenMessage("Folder already exists");
            }

            OnExitButton();
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
