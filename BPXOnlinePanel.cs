using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace BPX
{
    public class BPXOnlinePanel : BPXPanel
    {
        public Dictionary<BPXPanelComponentName, BPXPanelComponent> panelComponents;

        public BPXConfirmPanel confirmPanel;
        public BPXFolderPanel folderPanel;
        public DirectoryInfo downloadDirectory;

        public BPXPanelState currentState = BPXPanelState.Closed;
        private List<LEV_FileContent> currentExplorerElements = new List<LEV_FileContent>();

        public void Update()
        {
            if (BPXManager.central.input.Escape.buttonDown)
            {
                if (currentState != BPXPanelState.Closed)
                {
                    Close();
                }
            }
        }

        #region Initialization
        public void Initialize(LEV_LevelEditorCentral central)
        {
            GetPanelComponents();
            ConfigurePanel();

            downloadDirectory = new DirectoryInfo(Plugin.Instance.pluginPath);
        }

        private void GetPanelComponents()
        {
            panelComponents = new Dictionary<BPXPanelComponentName, BPXPanelComponent>();

            RectTransform innerPanel = transform.GetChild(1).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.Background, new BPXPanelComponent(BPXPanelComponentType.Image, BPXPanelComponentName.Background, innerPanel));

            foreach (RectTransform rt in innerPanel)
            {
                switch (rt.gameObject.name)
                {
                    case "Save Panel Save Button":
                        panelComponents.Add(BPXPanelComponentName.Search, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Search, rt));
                        break;
                    case "Home Button":
                        panelComponents.Add(BPXPanelComponentName.Home, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Home, rt));
                        break;
                    case "Autosaves Button":
                        panelComponents.Add(BPXPanelComponentName.Download, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Download, rt));
                        break;
                    case "Backups Button":
                        panelComponents.Add(BPXPanelComponentName.PreviousPage, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.PreviousPage, rt));
                        break;
                    case "Up One Level Button":
                        panelComponents.Add(BPXPanelComponentName.UpOneLevel, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.UpOneLevel, rt));
                        break;
                    case "New Folder Button":
                        panelComponents.Add(BPXPanelComponentName.NewFolder, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.NewFolder, rt));
                        break;
                    case "Sort Regular Levels Button":
                        panelComponents.Add(BPXPanelComponentName.NextPage, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.NextPage, rt));
                        break;
                    case "Open Folder Button":
                        panelComponents.Add(BPXPanelComponentName.OpenFolder, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.OpenFolder, rt));
                        break;
                    case "Exit Saving":
                        panelComponents.Add(BPXPanelComponentName.Exit, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Exit, rt));
                        break;
                    case "URL":
                        panelComponents.Add(BPXPanelComponentName.URL, new BPXPanelComponent(BPXPanelComponentType.Text, BPXPanelComponentName.URL, rt));
                        break;
                    case "Scroll View":
                        panelComponents.Add(BPXPanelComponentName.ScrollView, new BPXPanelComponent(BPXPanelComponentType.ScrollView, BPXPanelComponentName.ScrollView, rt));
                        break;
                    case "TextMeshPro - InputField":
                        panelComponents.Add(BPXPanelComponentName.FileName, new BPXPanelComponent(BPXPanelComponentType.TextInput, BPXPanelComponentName.FileName, rt));
                        break;
                    case "Medal Times Warning":
                        panelComponents.Add(BPXPanelComponentName.SelectedName, new BPXPanelComponent(BPXPanelComponentType.Text, BPXPanelComponentName.SelectedName, rt));
                        break;
                    case "AreYouSure (false)":
                        confirmPanel = new BPXConfirmPanel(this, rt);
                        break;
                    case "Create New Folder Panel (false)":
                        folderPanel = new BPXFolderPanel(this, rt);
                        break;
                }
            }
        }

        private void ConfigurePanel()
        {
            //Set the background color of the panel
            panelComponents[BPXPanelComponentName.Background].Image.color = BPXUIManagement.darkBlue;

            //Create a copy of the scroll view for the search result
            RectTransform scrollViewRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.ScrollView].Rect.gameObject, panelComponents[BPXPanelComponentName.ScrollView].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.SearchResultScrollView, new BPXPanelComponent(BPXPanelComponentType.ScrollView, BPXPanelComponentName.SearchResultScrollView, scrollViewRect));

            //Create a copy of the filename to use for the searchbar
            RectTransform searchBarRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.FileName].Rect.gameObject, panelComponents[BPXPanelComponentName.FileName].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.SearchBar, new BPXPanelComponent(BPXPanelComponentType.TextInput, BPXPanelComponentName.SearchBar, searchBarRect));

            //Create a copy of the selected blueprint name for the page counter
            RectTransform selectedTextRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.SelectedName].Rect.gameObject, panelComponents[BPXPanelComponentName.SelectedName].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.PageCounter, new BPXPanelComponent(BPXPanelComponentType.Text, BPXPanelComponentName.PageCounter, selectedTextRect));

            //Reposition
            panelComponents[BPXPanelComponentName.URL].SetRectAnchors(0.025f, 0.88f, 0.91f, 0.975f);

            panelComponents[BPXPanelComponentName.Home].SetRectAnchors(0.025f, 0.775f, 0.0925f, 0.87f);
            panelComponents[BPXPanelComponentName.UpOneLevel].SetRectAnchors(0.1025f, 0.775f, 0.17f, 0.87f);
            panelComponents[BPXPanelComponentName.NewFolder].SetRectAnchors(0.18f, 0.775f, 0.2475f, 0.87f);
            panelComponents[BPXPanelComponentName.OpenFolder].SetRectAnchors(0.2575f, 0.775f, 0.325f, 0.87f);

            panelComponents[BPXPanelComponentName.SearchBar].SetRectAnchors(0.335f, 0.775f, 0.855f, 0.87f);
            panelComponents[BPXPanelComponentName.Search].SetRectAnchors(0.865f, 0.775f, 0.975f, 0.87f);

            panelComponents[BPXPanelComponentName.ScrollView].SetRectAnchors(0.025f, 0.13f, 0.325f, 0.765f);
            panelComponents[BPXPanelComponentName.SearchResultScrollView].SetRectAnchors(0.335f, 0.13f, 0.975f, 0.765f);

            panelComponents[BPXPanelComponentName.FileName].SetRectAnchors(0.025f, 0.025f, 0.2475f, 0.12f);
            panelComponents[BPXPanelComponentName.Download].SetRectAnchors(0.2575f, 0.025f, 0.325f, 0.12f);
            
            panelComponents[BPXPanelComponentName.SelectedName].SetRectAnchors(0.335f, 0.025f, 0.745f, 0.12f);
            panelComponents[BPXPanelComponentName.PreviousPage].SetRectAnchors(0.755f, 0.025f, 0.81f, 0.12f);
            panelComponents[BPXPanelComponentName.PageCounter].SetRectAnchors(0.81f, 0.025f, 0.92f, 0.12f);
            panelComponents[BPXPanelComponentName.NextPage].SetRectAnchors(0.92f, 0.025f, 0.975f, 0.12f);

            //Remove texts from buttons
            panelComponents[BPXPanelComponentName.Home].HideButtonText();
            panelComponents[BPXPanelComponentName.NewFolder].HideButtonText();
            panelComponents[BPXPanelComponentName.Download].HideButtonText();
            panelComponents[BPXPanelComponentName.PreviousPage].HideButtonText();

            //Change button image sizes
            panelComponents[BPXPanelComponentName.Home].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.NewFolder].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.Download].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.PreviousPage].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.NextPage].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);

            //Set Sprites
            panelComponents[BPXPanelComponentName.Download].SetButtonImage(BPXSprites.downloadSprite);
            panelComponents[BPXPanelComponentName.Search].SetButtonImage(BPXSprites.searchSprite);
            panelComponents[BPXPanelComponentName.PreviousPage].SetButtonImage(BPXSprites.arrowLeftSprite);
            panelComponents[BPXPanelComponentName.NextPage].SetButtonImage(BPXSprites.arrowRightSprite);

            confirmPanel.Rect.SetAsLastSibling();
            folderPanel.Rect.SetAsLastSibling();

            //Set some values 
            panelComponents[BPXPanelComponentName.URL].SetText("path/to/some/folder");
            panelComponents[BPXPanelComponentName.FileName].SetPlaceHolderText("...");
            panelComponents[BPXPanelComponentName.SearchBar].SetPlaceHolderText("Search...");
            panelComponents[BPXPanelComponentName.SelectedName].SetText("[Selected blueprint to download] by [some creator]");
            panelComponents[BPXPanelComponentName.PageCounter].SetText("0 / 0");

            //Center the page counter
            panelComponents[BPXPanelComponentName.PageCounter].textMesh.alignment = TextAlignmentOptions.Center;

            //Bind functions
            panelComponents[BPXPanelComponentName.Exit].BindButton(() => Close());

            panelComponents[BPXPanelComponentName.Home].BindButton(() => GoHome());
            panelComponents[BPXPanelComponentName.UpOneLevel].BindButton(() => OnUpOneLevelButton());
            panelComponents[BPXPanelComponentName.NewFolder].BindButton(() => OnNewFolderButton());
            panelComponents[BPXPanelComponentName.OpenFolder].BindButton(() => OnOpenFolderButton());
            panelComponents[BPXPanelComponentName.Search].BindButton(() => OnSearchButton());
            panelComponents[BPXPanelComponentName.Download].BindButton(() => OnDownloadButton());
            panelComponents[BPXPanelComponentName.PreviousPage].BindButton(() => OnPreviousPageButton());
            panelComponents[BPXPanelComponentName.NextPage].BindButton(() => OnNextPageButton());
        }
        #endregion

        #region ExplorerPanel
        private void OnDirectorySelectedInExplorer(DirectoryInfo directoryInfo)
        {
            downloadDirectory = directoryInfo;
            RefreshPanel();
        }
        private void OnFileSelectedInExplorer(FileInfo fileInfo)
        {
            
        }
        #endregion

        #region SearchResultPanel
        private void OnFileSelectedInSearchResult(FileInfo fileInfo)
        {

        }
        #endregion

        #region Panel      

        public void Open(BPXPanelState panelMode)
        {
            if (panelMode == BPXPanelState.Closed || BPXManager.central == null)
            {
                Close();
                return;
            }           

            currentState = panelMode;

            RefreshPanel();
            ResetComponents();

            BPXUIManagement.OnOnlinePanelOpen();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            currentState = BPXPanelState.Closed;
            BPXUIManagement.OnOnlinePanelClose();
        }

        private void RefreshPanel()
        {
            
        }

        private void ClearExplorerElements()
        {
            for (int i = 0; i < currentExplorerElements.Count; i++)
            {
                if (currentExplorerElements[i] != null)
                {
                    currentExplorerElements[i].button.onClick.RemoveAllListeners();
                    GameObject.Destroy(currentExplorerElements[i].gameObject);
                }
            }
            currentExplorerElements.Clear();
        }

        private void ResetComponents()
        {
            foreach (KeyValuePair<BPXPanelComponentName, BPXPanelComponent> comp in panelComponents)
            {
                comp.Value.Reset();
            }
        }

        public override string GetCurrentPath()
        {
            return downloadDirectory.FullName;
        }
        #endregion

        #region Callback
        public override void OnConfirmPanel(bool confirmed)
        {
            if (confirmed)
            {
            }
        }
        public override void OnFolderPanel(bool folderCreated)
        {
            if (folderCreated)
            {
            }
        }
        #endregion

        #region Buttons
        private void GoHome()
        {
            Plugin.Instance.LogScreenMessage("Go Home");
        }

        private void OnUpOneLevelButton()
        {
            Plugin.Instance.LogScreenMessage("Up One Level");
        }

        private void OnNewFolderButton()
        {
            Plugin.Instance.LogScreenMessage("New Folder");
        }

        private void OnOpenFolderButton()
        {
            Plugin.Instance.LogScreenMessage("Open Folder");
        }

        private void OnSearchButton()
        {
            Plugin.Instance.LogScreenMessage("Search");
        }

        private void OnDownloadButton()
        {
            Plugin.Instance.LogScreenMessage("Download");
        }

        private void OnPreviousPageButton()
        {
            Plugin.Instance.LogScreenMessage("Previous");
        }

        private void OnNextPageButton()
        {
            Plugin.Instance.LogScreenMessage("Next");
        }
        #endregion
    }
}
