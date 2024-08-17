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
using ZeepSDK.External.Cysharp.Threading.Tasks;
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
        public bool isFirstOpen = true;

        #region Initialization
        public void Initialize(LEV_LevelEditorCentral central)
        {
            GetPanelComponents();
            ConfigurePanel();

            downloadDirectory = new DirectoryInfo(Plugin.Instance.blueprintPath);
        }

        public void Update()
        {
            if (BPXManager.central.input.Escape.buttonDown)
            {
                if (currentState != BPXPanelState.Closed)
                {
                    confirmPanel.Disable();
                    folderPanel.Disable();
                    Close();
                }
            }
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

            confirmPanel.Rect.GetComponent<Image>().color = BPXUIManagement.darkestBlue;
            confirmPanel.panelHeader.text = "Overwriting local blueprint!";
            confirmPanel.panelText.text = "A file with this name already exists locally. Continuing will overwrite the existing file. Do you want to proceed?"; ;

            //Set some values 
            panelComponents[BPXPanelComponentName.URL].SetText("path/to/some/folder");
            panelComponents[BPXPanelComponentName.FileName].SetPlaceHolderText("...");
            panelComponents[BPXPanelComponentName.SearchBar].SetPlaceHolderText("Search...");
            panelComponents[BPXPanelComponentName.SelectedName].SetText("");
            panelComponents[BPXPanelComponentName.PageCounter].SetText("1 / 1");

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

            panelComponents[BPXPanelComponentName.SearchBar].textInputField.onSubmit.AddListener(OnSubmit);

            panelComponents[BPXPanelComponentName.ScrollView].SetGridLayoutColumns(3);
            panelComponents[BPXPanelComponentName.SearchResultScrollView].SetGridLayoutColumns(6);

            //Add tooltips
            try
            {
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.Home].Rect.gameObject, "Navigate to the home directory");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.UpOneLevel].Rect.gameObject, "Navigate to the parent directory");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.NewFolder].Rect.gameObject, "Create a new folder");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.OpenFolder].Rect.gameObject, "View the opened folder in windows explorer");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.Search].Rect.gameObject, "Search");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.Download].Rect.gameObject, "Download the selected blueprint");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.PreviousPage].Rect.gameObject, "Go to the previous page");
                ZeepSDK.UI.UIApi.AddTooltip(panelComponents[BPXPanelComponentName.NextPage].Rect.gameObject, "Go to the next page");
            }
            catch
            {
                Plugin.Instance.LogMessage("Something went wrong while adding the tooltip. Probably wrong SDK version...");
            }
        }
        #endregion

        public void OnSubmit(string input)
        {
            OnSearchButton();
        }
        #region States
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

            if(isFirstOpen)
            {
                FillExplorerWithLatest();
                isFirstOpen = false;
            }
        }
        public void Close()
        {
            gameObject.SetActive(false);
            currentState = BPXPanelState.Closed;
            BPXUIManagement.OnOnlinePanelClose();
        }
        private void ResetComponents()
        {
            foreach (KeyValuePair<BPXPanelComponentName, BPXPanelComponent> comp in panelComponents)
            {
                comp.Value.Reset();
            }
        }
        #endregion

        #region ExplorerPanel
        private List<LEV_FileContent> currentExplorerElements = new List<LEV_FileContent>();
        private void OnDirectorySelectedInExplorer(DirectoryInfo directoryInfo)
        {
            downloadDirectory = directoryInfo;
            RefreshPanel();
        }
        private void OnFileSelectedInExplorer(FileInfo fileInfo)
        {
            panelComponents[BPXPanelComponentName.FileName].SetText(Path.GetFileNameWithoutExtension(fileInfo.Name));
        }
        private void RefreshPanel()
        {
            try
            {
                BPXPanelExplorerContents explorerContents = new BPXPanelExplorerContents(downloadDirectory, "");
                panelComponents[BPXPanelComponentName.URL].SetText(downloadDirectory.ToString());
                ClearExplorerElements();

                //The total count of elements in the explorer
                int amountOfElements = explorerContents.directories.Count + explorerContents.files.Count;
                //The amount of objects displayed on each row.
                int columnCount = 3;
                //The amount of rows needed for all elements.
                int rowCount = Mathf.CeilToInt((float)amountOfElements / (float)columnCount);
                //The horizontal padding for each element.
                float horizontalPadding = 0.1f / (columnCount + 1);
                //The vertical padding for each element.
                float verticalPadding = 0.12f / rowCount;
                //The width of each element.
                float elementWidth = 0.9f / columnCount;
                //The height of each element.
                float elementHeight = 1f - (rowCount + 1) * verticalPadding / rowCount;

                //Flag for completion.
                bool allElementsPlaced = false;

                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < columnCount; col++)
                    {
                        //In case of half filled rows, we can quit halfway through
                        if (row * columnCount + col >= amountOfElements)
                        {
                            allElementsPlaced = true;
                            break;
                        }

                        //Create the element
                        LEV_FileContent element = GameObject.Instantiate(BPXManager.central.saveload.filePrefab);
                        element.central = BPXManager.central;

                        int currentButtonIndex = row * columnCount + col;
                        bool isDirectory = currentButtonIndex < explorerContents.directories.Count;

                        //Initialize the directory or file.
                        if (isDirectory)
                        {
                            BPXPanelExplorerDirectory explorerDirectory = explorerContents.directories[currentButtonIndex];
                            element.directory = explorerDirectory.directoryInfo;
                            element.fileNameText.text = explorerDirectory.DisplayName;
                            element.thumbnail.sprite = BPXManager.central.saveload.folder;
                            element.fileType = 0;
                            element.button.onClick.AddListener(() => OnDirectorySelectedInExplorer(element.directory));
                        }
                        else
                        {
                            BPXPanelExplorerFile explorerFile = explorerContents.files[currentButtonIndex - explorerContents.directories.Count];
                            FileInfo fileInfo = explorerFile.fileInfo;
                            element.directory = explorerFile.fileInfo.Directory;
                            element.fileNameText.text = explorerFile.DisplayName;
                            element.thumbnail.sprite = BPXManager.central.saveload.file;
                            element.fileType = 2;
                            element.button.onClick.AddListener(() => OnFileSelectedInExplorer(fileInfo));
                        }

                        //Positioning
                        element.transform.SetParent(panelComponents[BPXPanelComponentName.ScrollView].ScrollRect.content, false);
                        float xPosition = horizontalPadding + (horizontalPadding * col) + (elementWidth * col);
                        float yPosition = 1f - (verticalPadding + (verticalPadding * row) + (elementHeight * row));

                        RectTransform elementRectTransform = element.GetComponent<RectTransform>();
                        elementRectTransform.anchorMin = new Vector2(xPosition, yPosition - elementHeight);
                        elementRectTransform.anchorMax = new Vector2(xPosition + elementWidth, yPosition);

                        currentExplorerElements.Add(element);
                    }

                    if (allElementsPlaced)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Instance.LogMessage(ex.Message);
                GoHome();
            }
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
        #endregion

        #region SearchResultPanel
        private List<LEV_FileContent> currentOnlineExplorerElements = new List<LEV_FileContent>();
        private List<BPXOnlineSearchResult> currentOnlineSearchResults = new List<BPXOnlineSearchResult>();
        private int currentOnlineSearchResultPageCount = 0;
        private int currentOnlineSearchResultPage = 0;
        private BPXOnlineSearchResult selectedResult = null;
        
        private void OnFileSelectedInOnlineExplorer(BPXOnlineSearchResult result)
        {
            selectedResult = result;
            panelComponents[BPXPanelComponentName.FileName].SetText(Path.GetFileNameWithoutExtension(result.name));
            panelComponents[BPXPanelComponentName.SelectedName].SetText("[" + result.name + "] by [" + result.creator + "]");
        }

        private void ResetFileSelectionInOnlineExplorer()
        {
            selectedResult = null;
            panelComponents[BPXPanelComponentName.SelectedName].SetText("");
        }

        private void ClearOnlineExplorerElements()
        {
            for (int i = 0; i < currentOnlineExplorerElements.Count; i++)
            {
                if (currentOnlineExplorerElements[i] != null)
                {
                    currentOnlineExplorerElements[i].button.onClick.RemoveAllListeners();
                    GameObject.Destroy(currentOnlineExplorerElements[i].gameObject);
                }
            }
            currentOnlineExplorerElements.Clear();
        }

        public void RefreshOnlinePanel()
        {            
            try
            {
                ClearOnlineExplorerElements();

                //Set the page numbering
                panelComponents[BPXPanelComponentName.PageCounter].SetText("" + (currentOnlineSearchResultPage + 1) + " / " + currentOnlineSearchResultPageCount);

                if (currentOnlineSearchResults.Count == 0)
                {
                    return;
                }

                //Calculate the startIndex
                int onlineResultsPerPage = Mathf.Max(1, BPXConfiguration.GetBPXOnlineResultsPerPage());
                int itemStartIndex = currentOnlineSearchResultPage * onlineResultsPerPage;

                if(itemStartIndex >= currentOnlineSearchResults.Count)
                {
                    return;
                }

                //Calculate the number of items to take
                int count = Math.Min(onlineResultsPerPage, currentOnlineSearchResults.Count - itemStartIndex);

                BPXOnlineSearchResult[] pageResults = new BPXOnlineSearchResult[count];
                Array.Copy(currentOnlineSearchResults.ToArray(), itemStartIndex, pageResults, 0, count);

                //The total count of elements in the explorer
                int amountOfElements = pageResults.Length;
                //The amount of objects displayed on each row.
                int columnCount = 5;
                //The amount of rows needed for all elements.
                int rowCount = Mathf.CeilToInt((float)amountOfElements / (float)columnCount);
                //The horizontal padding for each element.
                float horizontalPadding = 0.1f / (columnCount + 1);
                //The vertical padding for each element.
                float verticalPadding = 0.12f / rowCount;
                //The width of each element.
                float elementWidth = 0.9f / columnCount;
                //The height of each element.
                float elementHeight = 1f - (rowCount + 1) * verticalPadding / rowCount;

                //Flag for completion.
                bool allElementsPlaced = false;

                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col < columnCount; col++)
                    {
                        //In case of half filled rows, we can quit halfway through
                        if (row * columnCount + col >= amountOfElements)
                        {
                            allElementsPlaced = true;
                            break;
                        }

                        //Create the element
                        LEV_FileContent element = GameObject.Instantiate(BPXManager.central.saveload.filePrefab);
                        element.central = BPXManager.central;

                        int currentButtonIndex = row * columnCount + col;

                        element.fileNameText.text = pageResults[currentButtonIndex].name;
                        element.fileType = 2;
                        element.button.onClick.AddListener(() => OnFileSelectedInOnlineExplorer(pageResults[currentButtonIndex]));
                        LoadThumbnail(element, pageResults[currentButtonIndex]).Forget();

                        //Positioning
                        element.transform.SetParent(panelComponents[BPXPanelComponentName.SearchResultScrollView].ScrollRect.content, false);
                        float xPosition = horizontalPadding + (horizontalPadding * col) + (elementWidth * col);
                        float yPosition = 1f - (verticalPadding + (verticalPadding * row) + (elementHeight * row));

                        RectTransform elementRectTransform = element.GetComponent<RectTransform>();
                        elementRectTransform.anchorMin = new Vector2(xPosition, yPosition - elementHeight);
                        elementRectTransform.anchorMax = new Vector2(xPosition + elementWidth, yPosition);

                        currentOnlineExplorerElements.Add(element);

                        element.GetComponent<LEV_CustomButton>().normalColor = Color.white;
                    }

                    if (allElementsPlaced)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Instance.LogMessage(ex.Message);
            }
        }

        private async UniTaskVoid LoadThumbnail(LEV_FileContent element, BPXOnlineSearchResult result)
        {
            byte[] bytes = await BPXApi.DownloadImage((int)result.steamID, result.path);
            Texture2D textured = new Texture2D(1, 1);
            textured.LoadImage(bytes);
            if (element != null)
                element.thumbnail.sprite = BPXSprites.Texture2DToSprite(textured);
        }

        #endregion

        #region GetSet
        public override string GetCurrentPath()
        {
            return downloadDirectory.FullName;
        }
        #endregion

        #region Callback
        private string downloadTargetPath = "";
        public override void OnConfirmPanel(bool confirmed)
        {
            if (confirmed)
            {
                if(!string.IsNullOrEmpty(downloadTargetPath))
                {
                    BPXOnline.DownloadSearchResultTo(selectedResult, downloadTargetPath, OnDownloadComplete);                    
                }                
            }
            downloadTargetPath = "";
        }

        public override void OnFolderPanel(bool folderCreated)
        {
            if (folderCreated)
            {
                RefreshPanel();
            }
        }

        private void OnSearchQueryCompleted(List<BPXOnlineSearchResult> results)
        {
            if(results.Count == 0)
            {
                Plugin.Instance.LogScreenMessage("No Results :(");
            }
            else
            {
                Plugin.Instance.LogScreenMessage("Search Result: " + results.Count + (results.Count > 1  ? " items." : " item."));
            }
            
            int onlineResultsPerPage = Mathf.Max(1, BPXConfiguration.GetBPXOnlineResultsPerPage());
            currentOnlineSearchResults = results;
            currentOnlineSearchResultPageCount = Mathf.CeilToInt(((float)results.Count) / ((float)onlineResultsPerPage));
            currentOnlineSearchResultPage = 0;

            RefreshOnlinePanel();
        }

        private void OnDownloadComplete()
        {
            Plugin.Instance.LogScreenMessage("Download complete :)");
            RefreshPanel();
        }
        #endregion

        #region Buttons
        private void GoHome()
        {
            downloadDirectory = new DirectoryInfo(Plugin.Instance.blueprintPath);
            RefreshPanel();
        }

        private void OnUpOneLevelButton()
        {
            if (downloadDirectory.Parent != null && downloadDirectory.FullName != Plugin.Instance.pluginPath)
            {
                downloadDirectory = downloadDirectory.Parent;
            }

            RefreshPanel();
        }

        private void OnNewFolderButton()
        {
            folderPanel.Enable();
            ResetComponents();
        }

        private void OnOpenFolderButton()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Process.Start("explorer.exe", downloadDirectory.FullName);
            }
            else
            {
                Plugin.Instance.LogScreenMessage("Not supported on this platform");
            }
        }

        private void OnSearchButton()
        {           
            string query = panelComponents[BPXPanelComponentName.SearchBar].GetText().Trim();            
            
            if(string.IsNullOrEmpty(query))
            {
                FillExplorerWithLatest();
            }
            else
            {
                ResetFileSelectionInOnlineExplorer();
                BPXOnline.SearchQuery(query, OnSearchQueryCompleted);
            }            
        }

        private void FillExplorerWithLatest()
        {
            ResetFileSelectionInOnlineExplorer();
            Plugin.Instance.LogScreenMessage("Getting latest blueprints...");
            BPXOnline.GetLatest(Mathf.Max(1, BPXConfiguration.GetBPXOnlineResultsPerPage()), OnLatestRequestCompleted);
        }

        public void OnLatestRequestCompleted(List<BPXOnlineSearchResult> results)
        {
            if (results.Count == 0)
            {
                Plugin.Instance.LogScreenMessage("Couldn't get latest blueprints... :S");
                return;
            }

            Plugin.Instance.LogScreenMessage("Latest: " + results.Count + (results.Count > 1 ? " items." : " item."));

            int onlineResultsPerPage = Mathf.Max(1, BPXConfiguration.GetBPXOnlineResultsPerPage());
            currentOnlineSearchResults = results;
            currentOnlineSearchResultPageCount = Mathf.CeilToInt(((float)results.Count) / ((float)onlineResultsPerPage));
            currentOnlineSearchResultPage = 0;

            RefreshOnlinePanel();
        }

        private void OnDownloadButton()
        {
            if(selectedResult == null)
            {
                return;
            }

            //Get the entered name
            string enteredName = panelComponents[BPXPanelComponentName.FileName].GetText();

            if (string.IsNullOrEmpty(enteredName))
            {
                Plugin.Instance.LogScreenMessage("Please enter a name!");
                return;
            }

            //Remove the extension if the user has entered one.
            enteredName = enteredName.Replace(".zeeplevel", "");

            //Create the target path and check if there is already a file there.
            string directoryPath = downloadDirectory.ToString();
            string targetPath = Path.Combine(directoryPath, enteredName + ".zeeplevel");

            //Check if this file already exists
            if (File.Exists(targetPath))
            {
                downloadTargetPath = targetPath;
                confirmPanel.Enable();
            }
            else
            {
                BPXOnline.DownloadSearchResultTo(selectedResult, targetPath, OnDownloadComplete);
            }
        }

        private void OnPreviousPageButton()
        {
            if(currentOnlineSearchResultPage > 0)
            {
                currentOnlineSearchResultPage--;
                RefreshOnlinePanel();
            }
        }

        private void OnNextPageButton()
        {
            if(currentOnlineSearchResultPage + 1 < currentOnlineSearchResultPageCount)
            {
                currentOnlineSearchResultPage++;
                RefreshOnlinePanel();
            }
        }
        #endregion

        public void OnDestroy()
        {
            // Remove the listener when the object is destroyed
            try
            {
                panelComponents[BPXPanelComponentName.SearchBar].textInputField.onSubmit.RemoveListener(OnSubmit);
            }
            catch { }
        }
    }
}
