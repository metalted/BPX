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
        //The panel this folder panel belongs to.
        public BPXPanel panel;
        //The rect of this panel.
        public RectTransform Rect;
        //The folder name input field
        public TMP_InputField input;
        //A flag for keeping track if a folder was created or not.
        private bool folderCreated = false;

        public LEV_CustomButton exitButton;
        public LEV_CustomButton createButton;       

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
            BPXUIManagement.RebindButton(exitButton, () => Exit());

            BPXUIManagement.UnbindButton(createButton);
            BPXUIManagement.RecolorButton(createButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(createButton, () => OnCreateButton());
        }

        public void Enable()
        {
            Rect.gameObject.SetActive(true);
        }

        public void Disable()
        {
            Rect.gameObject.SetActive(false);
        }

        private void Exit()
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

            Exit();
        }        
    }
}
