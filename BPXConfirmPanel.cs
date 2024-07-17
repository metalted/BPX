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
        public BPXPanel panel;
        public RectTransform Rect;
        public TextMeshProUGUI alreadyExists;
        public LEV_CustomButton saveButton;
        public LEV_CustomButton cancelButton;

        public BPXConfirmPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;
            alreadyExists = rect.GetChild(2).GetComponent<TextMeshProUGUI>();
            saveButton = rect.GetChild(3).GetComponent<LEV_CustomButton>();
            cancelButton = rect.GetChild(4).GetComponent<LEV_CustomButton>();

            UIManagement.UnbindButton(saveButton);
            UIManagement.RecolorButton(saveButton, UIManagement.blue);
            UIManagement.RebindButton(saveButton, () => OnSaveButton());

            UIManagement.UnbindButton(cancelButton);
            UIManagement.RecolorButton(cancelButton, UIManagement.blue);
            UIManagement.RebindButton(cancelButton, () => OnCancelButton());
        }

        private void OnSaveButton()
        {

        }

        private void OnCancelButton()
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
