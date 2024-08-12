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
        public TextMeshProUGUI panelHeader;
        public TextMeshProUGUI panelText;
        public LEV_CustomButton saveButton;
        public LEV_CustomButton cancelButton;

        public BPXConfirmPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;
            panelHeader = rect.GetChild(1).GetComponent<TextMeshProUGUI>();
            panelText = rect.GetChild(2).GetComponent<TextMeshProUGUI>();
            saveButton = rect.GetChild(3).GetComponent<LEV_CustomButton>();
            cancelButton = rect.GetChild(4).GetComponent<LEV_CustomButton>();

            BPXUIManagement.UnbindButton(saveButton);
            BPXUIManagement.StandardRecolorButton(saveButton);
            BPXUIManagement.RebindButton(saveButton, () => OnSaveButton());

            BPXUIManagement.UnbindButton(cancelButton);
            BPXUIManagement.StandardRecolorButton(cancelButton);
            BPXUIManagement.RebindButton(cancelButton, () => OnCancelButton());

            I2.Loc.Localize[] localizers = rect.GetComponentsInChildren<I2.Loc.Localize>();
            foreach(I2.Loc.Localize loc in localizers)
            {
                loc.enabled = false;
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
