using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using I2.Loc;

namespace BPX
{
    public class BPXUploadPanel
    {
        //The panel this folder panel belongs to.
        public BPXPanel panel;
        //The rect of this panel.
        public RectTransform Rect;       

        public LEV_CustomButton exitButton;
        public LEV_CustomButton uploadButton;
        public LEV_CustomButton imageButton;

        public TextMeshProUGUI titleBar;

        public TextMeshProUGUI nameTitle;
        public TMP_InputField nameInput;
        public TextMeshProUGUI creatorTitle;
        public TMP_InputField creatorInput;
        public TextMeshProUGUI tagsTitle;
        public TMP_InputField tagsInput;

        public GameObject confirmPanel;
        public TextMeshProUGUI confirmPanelText;
        public LEV_CustomButton confirmSaveButton;
        public LEV_CustomButton confirmCancelButton;

        private bool waitingForServer = false;

        public BPXUploadPanel(BPXPanel panel, RectTransform rect)
        {
            this.panel = panel;
            Rect = rect;

            RectTransform panelRect = rect.GetChild(0).GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.05f, 0.05f);
            panelRect.anchorMax = new Vector2(0.95f, 0.95f);
            panelRect.GetComponent<Image>().color = BPXUIManagement.darkBlue;

            exitButton = panelRect.GetChild(0).GetComponent<LEV_CustomButton>();
            RectTransform exitButtonRect = exitButton.GetComponent<RectTransform>();
            exitButtonRect.anchorMin = new Vector2(0.86f, 0.86f);
            exitButtonRect.anchorMax = new Vector2(0.95f, 0.95f);

            uploadButton = panelRect.GetChild(1).GetComponent<LEV_CustomButton>();
            RectTransform uploadButtonRect = uploadButton.GetComponent<RectTransform>();
            uploadButtonRect.anchorMin = new Vector2(0.86f, 0.05f);
            uploadButtonRect.anchorMax = new Vector2(0.95f, 0.14f);

            imageButton = GameObject.Instantiate(uploadButton.gameObject, uploadButton.transform.parent).GetComponent<LEV_CustomButton>();
            RectTransform imageButtonRect = imageButton.GetComponent<RectTransform>();
            imageButtonRect.anchorMin = new Vector2(0.51f, 0.15f);
            imageButtonRect.anchorMax = new Vector2(0.95f, 0.85f);

            nameInput = panelRect.GetChild(2).GetComponent<TMP_InputField>();
            RectTransform nameInputRect = nameInput.GetComponent<RectTransform>();
            nameInputRect.anchorMin = new Vector2(0.05f, 0.63f);
            nameInputRect.anchorMax = new Vector2(0.49f, 0.73f);

            Localize nameloc = nameInput.placeholder.GetComponent<Localize>();
            if (nameloc != null)
            {
                GameObject.Destroy(nameloc);
            }

            nameInput.placeholder.GetComponent<TMP_Text>().text = "Blueprint name...";
            nameInput.text = "";

            creatorInput = GameObject.Instantiate(nameInputRect.gameObject, nameInput.transform.parent).GetComponent<TMP_InputField>();
            RectTransform creatorInputRect = creatorInput.GetComponent<RectTransform>();
            creatorInputRect.anchorMin = new Vector2(0.05f, 0.39f);
            creatorInputRect.anchorMax = new Vector2(0.49f, 0.49f);

            Localize creatorloc = creatorInput.placeholder.GetComponent<Localize>();
            if (creatorloc != null)
            {
                GameObject.Destroy(creatorloc);
            }

            creatorInput.placeholder.GetComponent<TMP_Text>().text = "Creator...";
            creatorInput.text = "";
            creatorInput.interactable = false;

            tagsInput = GameObject.Instantiate(nameInputRect.gameObject, nameInput.transform.parent).GetComponent<TMP_InputField>();
            RectTransform tagsInputRect = tagsInput.GetComponent<RectTransform>();
            tagsInputRect.anchorMin = new Vector2(0.05f, 0.15f);
            tagsInputRect.anchorMax = new Vector2(0.49f, 0.25f);

            Localize tagloc = tagsInput.placeholder.GetComponent<Localize>();
            if (tagloc != null)
            {
                GameObject.Destroy(tagloc);
            }

            tagsInput.placeholder.GetComponent<TMP_Text>().text = "Tag1,tag2,tag3...";
            tagsInput.text = "";

            titleBar = panelRect.GetChild(3).GetComponent<TextMeshProUGUI>();
            RectTransform titleBarRect = titleBar.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0.05f, 0.86f);
            titleBarRect.anchorMax = new Vector2(0.85f, 0.95f);
            titleBar.text = "Uploading blueprint";

            nameTitle = GameObject.Instantiate(titleBar.gameObject, titleBar.transform.parent).GetComponent<TextMeshProUGUI>();
            RectTransform nameTitleRect = nameTitle.GetComponent<RectTransform>();
            nameTitleRect.anchorMin = new Vector2(0.05f, 0.75f);
            nameTitleRect.anchorMax = new Vector2(0.49f, 0.85f);
            nameTitle.text = "Name:";

            creatorTitle = GameObject.Instantiate(titleBar.gameObject, titleBar.transform.parent).GetComponent<TextMeshProUGUI>();
            RectTransform creatorTitleRect = creatorTitle.GetComponent<RectTransform>();
            creatorTitleRect.anchorMin = new Vector2(0.05f, 0.51f);
            creatorTitleRect.anchorMax = new Vector2(0.49f, 0.61f);
            creatorTitle.text = "Creator:";

            tagsTitle = GameObject.Instantiate(titleBar.gameObject, titleBar.transform.parent).GetComponent<TextMeshProUGUI>();
            RectTransform tagsTitleRect = tagsTitle.GetComponent<RectTransform>();
            tagsTitleRect.anchorMin = new Vector2(0.05f, 0.27f);
            tagsTitleRect.anchorMax = new Vector2(0.49f, 0.37f);
            tagsTitle.text = "Tags (Seperate by comma):";

            BPXUIManagement.UnbindButton(exitButton);
            BPXUIManagement.RecolorButton(exitButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(exitButton, () => Exit());

            BPXUIManagement.UnbindButton(uploadButton);
            BPXUIManagement.RecolorButton(uploadButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(uploadButton, () => OnUploadButton());

            BPXUIManagement.UnbindButton(imageButton);
            BPXUIManagement.RecolorButton(imageButton, Color.black, true);

            SetImage(BPXSprites.blackPixelSprite);

            uploadButton.transform.GetChild(0).GetComponent<Image>().sprite = BPXSprites.uploadImageSprite;
        }

        public void InitializeConfirmPanel(RectTransform confirm)
        {
            confirm.SetAsLastSibling();
            this.confirmPanel = confirm.gameObject;

            confirmPanelText = confirm.GetChild(2).GetComponent<TextMeshProUGUI>();
            confirmPanelText.text = "Overwrite?";

            confirmSaveButton = confirm.GetChild(3).GetComponent<LEV_CustomButton>();
            confirmCancelButton = confirm.GetChild(4).GetComponent<LEV_CustomButton>();

            BPXUIManagement.UnbindButton(confirmSaveButton);
            BPXUIManagement.RecolorButton(confirmSaveButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(confirmSaveButton, () => OnConfirmSaveButton());

            BPXUIManagement.UnbindButton(confirmCancelButton);
            BPXUIManagement.RecolorButton(confirmCancelButton, BPXUIManagement.blue);
            BPXUIManagement.RebindButton(confirmCancelButton, () => OnConfirmCancelButton());

            confirmPanel.SetActive(false);
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
            nameInput.text = "";
            creatorInput.text = "";
            tagsInput.text = "";

            exitButton.ResetAllBools();
            uploadButton.ResetAllBools();
            imageButton.ResetAllBools();

            fileImage = null;

            waitingForServer = false;

            Disable();
        }

        private void OnUploadButton()
        {
            Debug.Log("Waiting1: " + waitingForServer);
            if (waitingForServer) { return; }

            string saveName = nameInput.text.Replace(".zeeplevel", "").Trim();

            //Is the file name filled in?
            if (string.IsNullOrEmpty(saveName))
            {
                Plugin.Instance.LogScreenMessage("Please fill in a name...");
                return;
            }

            if(fileImage == null)
            {
                Plugin.Instance.LogScreenMessage("No image...");
                return;
            }

            string[] tagsInputComponents = tagsInput.text.Split(",");
            List<string> tags = new List<string>();

            foreach(string t in tagsInputComponents)
            {
                string result = t.Trim();
                if(!string.IsNullOrEmpty(result))
                {
                    tags.Add(result);
                }
            }

            BPXOnlineUploadFile toUpload = new BPXOnlineUploadFile();
            toUpload.name = saveName;
            toUpload.file = fileToUpload;
            toUpload.creator = creatorInput.text;
            toUpload.tags = tags.ToArray();
            toUpload.thumbnail = fileImage;

            waitingForServer = true;
            BPXOnline.SetFileToUpload(toUpload);
            BPXOnline.CheckForOverwrite(OnServerOverwriteCheck);            
        }

        private void OnServerOverwriteCheck(bool isOverwrite)
        {            
            waitingForServer = false;

            if (!isOverwrite)
            {
                BPXOnline.Upload();
                Exit();
            }
            else
            {
                ShowConfirmPanel();
            }
        }

        private void ShowConfirmPanel()
        {
            confirmPanel.SetActive(true);
        }

        private void CloseConfirmPanel()
        {
            confirmPanel.SetActive(false);
        }

        public void OnConfirmSaveButton()
        {
            BPXOnline.Upload();            
            CloseConfirmPanel();
            Exit();
        }

        public void OnConfirmCancelButton()
        {
            CloseConfirmPanel();
        }

        private ZeeplevelFile fileToUpload;
        private Texture2D fileImage;

        public void SetFileToUpload(ZeeplevelFile file)
        {
            fileToUpload = file;
            SetImage(BPXSprites.blackPixelSprite);
            nameInput.text = Path.GetFileNameWithoutExtension(file.FileName);
            creatorInput.text = BPXManager.GetPlayerName();
            tagsInput.text = "";

            BPXManager.GenerateImage(fileToUpload, 256, OnThumbnailCreated);

        }

        public void SetImage(Sprite img)
        {
            imageButton.transform.GetChild(0).GetComponent<Image>().sprite = img;
        }

        public void OnThumbnailCreated(List<Texture2D> texs)
        {
            fileImage = texs[0];
            SetImage(BPXSprites.Texture2DToSprite(texs[0]));
        }
    }
}
