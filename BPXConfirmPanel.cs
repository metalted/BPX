using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BPX
{
    public class BPXConfirmPanel
    {
        //The panel this confirm panel belongs to.
        public BPXPanel panel;
        //The complete rect of this confirm panel.
        public RectTransform Rect;
        //The text displayed on this confirm panel.
        public TextMeshProUGUI panelText;
        public LEV_CustomButton saveButton;
        public LEV_CustomButton cancelButton;

        public BPXConfirmPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;
            panelText = rect.GetChild(2).GetComponent<TextMeshProUGUI>();
            saveButton = rect.GetChild(3).GetComponent<LEV_CustomButton>();
            cancelButton = rect.GetChild(4).GetComponent<LEV_CustomButton>();

            BPXUIManagement.UnbindButton(saveButton);
            BPXUIManagement.RecolorButton(saveButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(saveButton, () => OnSaveButton());

            BPXUIManagement.UnbindButton(cancelButton);
            BPXUIManagement.RecolorButton(cancelButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(cancelButton, () => OnCancelButton());
        }

        public void Enable(string displayText)
        {
            panelText.text = displayText;
            Rect.gameObject.SetActive(true);
        }

        public void Disable()
        {
            Rect.gameObject.SetActive(false);
        }        

        private void OnSaveButton()
        {
            panel.OnConfirmPanel(true);
            Disable();
        }

        private void OnCancelButton()
        {
            panel.OnConfirmPanel(false);
            Disable();
        }        
    }
}
