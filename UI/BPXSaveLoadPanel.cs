using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Toolkist;

namespace BPX.UI
{   
    public class BPXSaveLoadPanel : BPXPanel
    {
        public Dictionary<BPXPanelComponentName, BPXPanelComponent> panelComponents;

        public BPXConfirmPanel confirmPanel;
        public BPXFolderPanel folderPanel;

        public DirectoryInfo blueprintDirectory;
        public DirectoryInfo levelDirectory;

        public BPXPanelState currentState = BPXPanelState.Closed;
        public BPXPanelMode currentMode = BPXPanelMode.Blueprint;

        private List<LEV_FileContent> currentExplorerElements = new List<LEV_FileContent>();

        public ZeeplevelData selectedBlueprintToLoad = null;
        public ZeeplevelData selectedBlueprintToSave = null;

        public void Update()
        {
            if (BPXManager.central.input.Escape.buttonDown)
            {
                if(currentState != BPXPanelState.Closed)
                {
                    confirmPanel.Disable();
                    folderPanel.Disable();
                    Close();
                }
            }
        }

        #region Initialization
        public void Initialize(LEV_LevelEditorCentral central)
        {
            GetPanelComponents();
            ConfigurePanel();

            blueprintDirectory = new DirectoryInfo(ZeeplevelIO.BlueprintBasePath);
            levelDirectory = new DirectoryInfo(ZeeplevelIO.LevelBasePath);
        }
        private void GetPanelComponents()
        {
            panelComponents = new Dictionary<BPXPanelComponentName, BPXPanelComponent>();

            RectTransform innerPanel = transform.GetChild(1).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.Background, new BPXPanelComponent(BPXPanelComponentType.Image, BPXPanelComponentName.Background, innerPanel));

            foreach(RectTransform rt in innerPanel)
            {
                switch(rt.gameObject.name)
                {
                    case "Save Panel Save Button":
                        panelComponents.Add(BPXPanelComponentName.Save, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Save, rt));
                        break;
                    case "Home Button":
                        panelComponents.Add(BPXPanelComponentName.Home, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Home, rt));
                        break;
                    case "Autosaves Button":
                        panelComponents.Add(BPXPanelComponentName.SwitchDir, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.SwitchDir, rt));
                        break;
                    case "Backups Button":
                        panelComponents.Add(BPXPanelComponentName.LoadPreview, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.LoadPreview, rt));
                        break;
                    case "Up One Level Button":
                        panelComponents.Add(BPXPanelComponentName.UpOneLevel, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.UpOneLevel, rt));
                        break;
                    case "New Folder Button":
                        panelComponents.Add(BPXPanelComponentName.NewFolder, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.NewFolder, rt));
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
                        panelComponents.Add(BPXPanelComponentName.TypeText, new BPXPanelComponent(BPXPanelComponentType.Text, BPXPanelComponentName.TypeText, rt));
                        break;
                    case "AreYouSure (false)":
                        confirmPanel = new BPXConfirmPanel(this, rt);
                        break;
                    case "Create New Folder Panel (false)":
                        folderPanel = new BPXFolderPanel(this, rt);
                        break;
                    case "Sort Regular Levels Button":
                        rt.gameObject.SetActive(false);
                        break;
                }
            }
        }            
        private void ConfigurePanel()
        {
            //Set the background color of the panel
            panelComponents[BPXPanelComponentName.Background].Image.color = BPXUIManagement.darkBlue;

            //Create a copy of the save button for the load button
            RectTransform loadRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.Save].Rect.gameObject, panelComponents[BPXPanelComponentName.Save].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.Load, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Load, loadRect));

            //Create a copy of the load preview for the save preview
            RectTransform savePreviewRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.LoadPreview].Rect.gameObject, panelComponents[BPXPanelComponentName.LoadPreview].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.SavePreview, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.SavePreview, savePreviewRect));

            //Create a copy of the save button so we can create the two smaller load buttons
            RectTransform loadHereRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.Save].Rect.gameObject, panelComponents[BPXPanelComponentName.Save].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.LoadHere, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.LoadHere, loadHereRect));
            RectTransform loadFileRect = BPXUIManagement.SplitLEVCustomButton(panelComponents[BPXPanelComponentName.LoadHere].Button, 0.05f).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.LoadFile, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.LoadFile, loadFileRect));

            //V18. Create a copy of the save button so we can create the treegun selection button.
            RectTransform treeGunRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.Save].Rect.gameObject, panelComponents[BPXPanelComponentName.Save].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.TreeGun, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.TreeGun, treeGunRect));
            
            //Create a copy of the filename to use for the searchbar
            RectTransform searchBarRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.FileName].Rect.gameObject, panelComponents[BPXPanelComponentName.FileName].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.SearchBar, new BPXPanelComponent(BPXPanelComponentType.TextInput, BPXPanelComponentName.SearchBar, searchBarRect));

            //Reposition components
            panelComponents[BPXPanelComponentName.TypeText].SetRectAnchors(0.03f, 0.8f, 0.23f, 0.85f);
            panelComponents[BPXPanelComponentName.Home].SetRectAnchors(0.03f, 0.71f, 0.07f, 0.79f);
            panelComponents[BPXPanelComponentName.UpOneLevel].SetRectAnchors(0.08f, 0.71f, 0.12f, 0.79f);
            panelComponents[BPXPanelComponentName.NewFolder].SetRectAnchors(0.13f, 0.71f, 0.17f, 0.79f);
            panelComponents[BPXPanelComponentName.SwitchDir].SetRectAnchors(0.18f, 0.71f, 0.23f, 0.79f);
            panelComponents[BPXPanelComponentName.SearchBar].SetRectAnchors(0.03f, 0.63f, 0.23f, 0.7f);
            panelComponents[BPXPanelComponentName.LoadPreview].SetRectAnchors(0.03f, 0.25f, 0.23f, 0.62f);
            panelComponents[BPXPanelComponentName.SavePreview].SetRectAnchors(0.03f, 0.25f, 0.23f, 0.62f);
            panelComponents[BPXPanelComponentName.TreeGun].SetRectAnchors(0.7425f, 0.05f, 0.81f, 0.2f);
            panelComponents[BPXPanelComponentName.FileName].SetRectAnchors(0.025f, 0.05f, 0.725f, 0.2f);

            //Bind functions to the buttons.
            panelComponents[BPXPanelComponentName.Save].BindButton(() => OnSaveButton());
            panelComponents[BPXPanelComponentName.Load].BindButton(() => OnLoadButton(true));
            panelComponents[BPXPanelComponentName.LoadHere].BindButton(() => OnLoadButton(true));
            panelComponents[BPXPanelComponentName.LoadFile].BindButton(() => OnLoadButton(false));
            panelComponents[BPXPanelComponentName.Home].BindButton(() => GoHome());
            panelComponents[BPXPanelComponentName.UpOneLevel].BindButton(()=> OnUpOneLevelButton());
            panelComponents[BPXPanelComponentName.NewFolder].BindButton(()=> OnNewFolderButton());
            panelComponents[BPXPanelComponentName.SwitchDir].BindButton(() => OnSwitchDirButton());
            panelComponents[BPXPanelComponentName.LoadPreview].BindButton(() => OnLoadPreviewButton());
            panelComponents[BPXPanelComponentName.SavePreview].BindButton(() => OnSavePreviewButton());
            panelComponents[BPXPanelComponentName.OpenFolder].BindButton(() => OnOpenFolderButton());
            panelComponents[BPXPanelComponentName.Exit].BindButton(() => Close());
            panelComponents[BPXPanelComponentName.SearchBar].textInputField.onValueChanged.AddListener(delegate { RefreshPanel(); });
            panelComponents[BPXPanelComponentName.TreeGun].BindButton(() => OnTreeGunButton());

            //Change button image sizes
            panelComponents[BPXPanelComponentName.Home].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.NewFolder].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.SwitchDir].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImageRectAnchors(0.0f, 0.0f, 1f, 1f);
            panelComponents[BPXPanelComponentName.SavePreview].SetButtonImageRectAnchors(0.0f, 0.0f, 1f, 1f);

            //Set sprites
            panelComponents[BPXPanelComponentName.LoadHere].SetButtonImage(BPXSprites.markerSprite);
            panelComponents[BPXPanelComponentName.LoadFile].SetButtonImage(BPXSprites.fileSprite);
            panelComponents[BPXPanelComponentName.TreeGun].SetButtonImage(BPXManager.central.tool.button_nature_gun.transform.GetChild(0).GetComponent<Image>().sprite);
            panelComponents[BPXPanelComponentName.SwitchDir].SetButtonImage(BPXSprites.fileSwitchSprite);
            panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImage(BPXSprites.blackPixelSprite);
            panelComponents[BPXPanelComponentName.SavePreview].SetButtonImage(BPXSprites.blackPixelSprite);
            panelComponents[BPXPanelComponentName.Save].SetButtonImage(BPXManager.central.saveload.saveImage);
            panelComponents[BPXPanelComponentName.Load].SetButtonImage(BPXManager.central.saveload.loadImage);
            
            //Turn preview button completely black
            BPXUIManagement.RecolorButton(panelComponents[BPXPanelComponentName.LoadPreview].Button, Color.black, Color.black, Color.black, true);
            BPXUIManagement.RecolorButton(panelComponents[BPXPanelComponentName.SavePreview].Button, Color.black, Color.black, Color.black, true);
            
            //Remove texts from buttons
            panelComponents[BPXPanelComponentName.Home].HideButtonText();
            panelComponents[BPXPanelComponentName.NewFolder].HideButtonText();
            panelComponents[BPXPanelComponentName.SwitchDir].HideButtonText();
            panelComponents[BPXPanelComponentName.LoadPreview].HideButtonText();
            panelComponents[BPXPanelComponentName.SavePreview].HideButtonText();

            confirmPanel.Rect.SetAsLastSibling();
            folderPanel.Rect.SetAsLastSibling();

            confirmPanel.Rect.GetComponent<Image>().color = BPXUIManagement.darkestBlue;
            confirmPanel.panelHeader.text = "Overwriting Local Blueprint!";
            confirmPanel.panelText.text = "A file with this name already exists locally. Continuing will overwrite the existing file. Do you want to proceed?";

            //Set some values 
            panelComponents[BPXPanelComponentName.URL].SetText("path/to/some/file");
            panelComponents[BPXPanelComponentName.FileName].SetPlaceHolderText("...");
            panelComponents[BPXPanelComponentName.SearchBar].SetPlaceHolderText("Search...");
            panelComponents[BPXPanelComponentName.TypeText].SetText("Blueprints");
        }
        #endregion
        
        #region ExplorerPanel
        private void OnDirectorySelectedInExplorer(DirectoryInfo directoryInfo)
        {
            if(currentMode == BPXPanelMode.Blueprint)
            {
                blueprintDirectory = directoryInfo;
            }
            else if(currentMode == BPXPanelMode.Level)
            {
                levelDirectory = directoryInfo;
            }

            RefreshPanel();
        }

        private void OnFileSelectedInExplorer(FileInfo fileInfo)
        {
            if (panelComponents == null)
            {
                Plugin.Instance.LogMessage("[BPX] panelComponents is null");
                return;
            }

            if (!panelComponents.ContainsKey(BPXPanelComponentName.FileName))
            {
                Plugin.Instance.LogMessage("[BPX] panelComponents does not contain FileName key");
            }

            if (currentState == BPXPanelState.Save)
            {
                if (fileInfo == null)
                {
                    Plugin.Instance.LogMessage("[BPX] fileInfo is null during Save state");
                    return;
                }

                string name = fileInfo.Name.Replace(".zeeplevel", "");
                panelComponents[BPXPanelComponentName.FileName].SetText(name);
            }
            else if (currentState == BPXPanelState.Load)
            {
                if (fileInfo == null)
                {
                    Plugin.Instance.LogMessage("[BPX] fileInfo is null during Load state");
                    return;
                }

                selectedBlueprintToLoad = ZeeplevelIO.FromFile(fileInfo);

                ManageLoadButtons();

                if (selectedBlueprintToLoad == null)
                {
                    Plugin.Instance.LogMessage("[BPX] OnFileSelectedInExplorer: Blueprint returned null");
                    panelComponents[BPXPanelComponentName.FileName].SetText("");
                    return;
                }

                if (selectedBlueprintToLoad.level == null)
                {
                    Plugin.Instance.LogMessage("[BPX] selectedBlueprintToLoad.level is null");
                }
                else
                {
                    Plugin.Instance.LogMessage($"[BPX] Loaded blueprint level name: {selectedBlueprintToLoad.level.Name}");
                }

                panelComponents[BPXPanelComponentName.FileName].SetText(selectedBlueprintToLoad.level?.Name ?? "null");

                Plugin.Instance.LogMessage("[BPX] Generating image preview...");
                ZeeplevelImager.Instance.CaptureSubject(512, 4, selectedBlueprintToLoad, OnLoadedBlueprintPreviewGenerated);
            }
        }

        private void GoHome()
        {
            if (currentMode == BPXPanelMode.Level)
            {
                levelDirectory = new DirectoryInfo(ZeeplevelIO.LevelBasePath);
            }
            else if (currentMode == BPXPanelMode.Blueprint)
            {
                blueprintDirectory = new DirectoryInfo(ZeeplevelIO.BlueprintBasePath);
            }

            RefreshPanel();
        }
        #endregion

        #region Panel        
        private void ManageLoadButtons()
        {
            if (selectedBlueprintToLoad != null)
            {
                if (BPXConfiguration.DoubleLoadButtons())
                {
                    panelComponents[BPXPanelComponentName.Load].Disable();
                    panelComponents[BPXPanelComponentName.LoadHere].Enable();
                    panelComponents[BPXPanelComponentName.LoadFile].Enable();
                }
                else
                {
                    panelComponents[BPXPanelComponentName.Load].Enable();
                    panelComponents[BPXPanelComponentName.LoadHere].Disable();
                    panelComponents[BPXPanelComponentName.LoadFile].Disable();
                }

                panelComponents[BPXPanelComponentName.TreeGun].Enable();
            }
            else
            {
                panelComponents[BPXPanelComponentName.Load].Disable();
                panelComponents[BPXPanelComponentName.LoadHere].Disable();
                panelComponents[BPXPanelComponentName.LoadFile].Disable();
                panelComponents[BPXPanelComponentName.TreeGun].Disable();
            }
        }
        public void Open(BPXPanelState panelMode)
        {
            if (panelMode == BPXPanelState.Closed || BPXManager.central == null)
            {
                Close();
                return;
            }

            if (panelMode == BPXPanelState.Save)
            {
                panelComponents[BPXPanelComponentName.Save].Enable();

                panelComponents[BPXPanelComponentName.FileName].SetInteractable(true);
                panelComponents[BPXPanelComponentName.TypeText].SetText("Save Blueprint");
                panelComponents[BPXPanelComponentName.FileName].SetText("");

                panelComponents[BPXPanelComponentName.LoadHere].Disable();
                panelComponents[BPXPanelComponentName.LoadFile].Disable();
                panelComponents[BPXPanelComponentName.SearchBar].Disable();
                panelComponents[BPXPanelComponentName.Load].Disable();
                panelComponents[BPXPanelComponentName.TreeGun].Disable();

                panelComponents[BPXPanelComponentName.LoadPreview].Disable();
                panelComponents[BPXPanelComponentName.SavePreview].Enable();
                panelComponents[BPXPanelComponentName.SavePreview].SetButtonImage(BPXSprites.blackPixelSprite);

                ZeeplevelImager.Instance.CaptureSubject(512, 4, selectedBlueprintToSave, OnSavedBlueprintPreviewGenerated);
            }
            else if (panelMode == BPXPanelState.Load)
            {
                panelComponents[BPXPanelComponentName.SearchBar].Enable();
                panelComponents[BPXPanelComponentName.LoadPreview].Enable();
                panelComponents[BPXPanelComponentName.FileName].SetInteractable(false);
                panelComponents[BPXPanelComponentName.TypeText].SetText("Import Blueprint");
                panelComponents[BPXPanelComponentName.Save].Disable();
                panelComponents[BPXPanelComponentName.SavePreview].Disable();

                if (selectedBlueprintToLoad != null)
                {
                    panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImage(BPXSprites.blackPixelSprite);
                    ZeeplevelImager.Instance.CaptureSubject(512, 4, selectedBlueprintToLoad, OnLoadedBlueprintPreviewGenerated);
                    panelComponents[BPXPanelComponentName.FileName].SetText(selectedBlueprintToLoad.level.Name);
                }
                else
                {
                    panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImage(BPXSprites.blackPixelSprite);
                    panelComponents[BPXPanelComponentName.FileName].SetText("");
                }

                ManageLoadButtons();
            }

            currentState = panelMode;

            RefreshPanel();
            ResetComponents();

            BPXUIManagement.OnPanelOpen();

            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
            confirmPanel.Disable();
            folderPanel.Disable();
            currentState = BPXPanelState.Closed;

            if(BPXConfiguration.ClearSearchOnExit())
            {
                panelComponents[BPXPanelComponentName.SearchBar].SetText("");
            }
            BPXUIManagement.OnPanelClose();
        }
        private void RefreshPanel()
        {
            try
            {
                DirectoryInfo directory = currentMode == BPXPanelMode.Blueprint ? blueprintDirectory : levelDirectory;
                string searchInput = currentState == BPXPanelState.Save ? "" : panelComponents[BPXPanelComponentName.SearchBar].GetText();

                BPXPanelExplorerContents explorerContents = new BPXPanelExplorerContents(directory, searchInput);
                panelComponents[BPXPanelComponentName.URL].SetText(directory.ToString());
                ClearExplorerElements();

                //The total count of elements in the explorer
                int amountOfElements = explorerContents.directories.Count + explorerContents.files.Count;
                //The amount of objects displayed on each row.
                int columnCount = 6;
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

                if (currentState == BPXPanelState.Save)
                {
                    panelComponents[BPXPanelComponentName.SwitchDir].Disable();
                    panelComponents[BPXPanelComponentName.TypeText].SetText("Save Blueprint");
                }
                else if (currentState == BPXPanelState.Load)
                {
                    panelComponents[BPXPanelComponentName.SwitchDir].Enable();
                    panelComponents[BPXPanelComponentName.TypeText].SetText(currentMode == BPXPanelMode.Blueprint ? "Import Blueprint" : "Import Level");

                    ManageLoadButtons();
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
        private void ResetComponents()
        {
            foreach (KeyValuePair<BPXPanelComponentName, BPXPanelComponent> comp in panelComponents)
            {
                comp.Value.Reset();
            }
        }
        public override string GetCurrentPath()
        {
            return currentMode == BPXPanelMode.Blueprint ? blueprintDirectory.FullName : levelDirectory.FullName;
        }
        #endregion

        #region Callbacks
        public override void OnConfirmPanel(bool confirmed)
        {
            if (confirmed)
            {
                ZeeplevelIO.SaveToFile(selectedBlueprintToSave, selectedBlueprintToSave.level.Path);
                Plugin.Instance.LogScreenMessage("Saved " + Path.GetFileNameWithoutExtension(selectedBlueprintToSave.level.Path));
                Close();
            }
        }
        public override void OnFolderPanel(bool folderCreated)
        {
            if (folderCreated)
            {
                RefreshPanel();
            }
        }
        public void SetBlueprintToSave(ZeeplevelData blueprintTosave)
        {
            selectedBlueprintToSave = blueprintTosave;
        }
        #endregion

        #region Buttons
        private void OnLoadButton(bool loadHere)
        {
            Close();
            List<BlockProperties> blocks = ZeeplevelHandler.LoadIntoEditor(selectedBlueprintToLoad, BPXManager.central, true);

            if(loadHere)
            {
                Vector3 requiredMove = ToolkitUtils.BlocksAtCameraGridMovement(BPXManager.central, blocks);
                EditorOperations.Move(BPXManager.central, blocks, requiredMove);
            }       
        }
        private void OnTreeGunButton()
        {
            if(selectedBlueprintToLoad == null)
            {
                Plugin.Instance.LogMessage("[BPX] Selected blueprint is null when trying to set TreeGun selection.");
                return;
            }

            BPXManager.SetTreegunBlueprint(ZeeplevelHandler.Copy(selectedBlueprintToLoad), loadBlueprintSprites[0]);

            Plugin.Instance.LogScreenMessage("Treegun blueprint set.");
        }

        private void OnSaveButton()
        {
            //Get the entered name
            string enteredName = panelComponents[BPXPanelComponentName.FileName].GetText();

            if (string.IsNullOrEmpty(enteredName))
            {
                Plugin.Instance.LogScreenErrorMessage("Please enter a name!");
                return;
            }

            //Remove the extension if the user has entered one.
            enteredName = enteredName.Replace(".zeeplevel", "");

            //Create the target path and check if there is already a file there.
            string directoryPath = (currentMode == BPXPanelMode.Level) ? levelDirectory.ToString() : blueprintDirectory.ToString();
            string targetPath = Path.Combine(directoryPath, enteredName + ".zeeplevel");
            selectedBlueprintToSave.level.Name = enteredName;
            selectedBlueprintToSave.json.level.name = enteredName;

            //Check if this file already exists
            if (File.Exists(targetPath))
            {
                selectedBlueprintToSave.level.Path = targetPath;
                confirmPanel.Enable();
            }
            else
            {
                //Save right away
                ZeeplevelIO.SaveToFile(selectedBlueprintToSave, targetPath);
                Plugin.Instance.LogScreenMessage("Saved " + Path.GetFileNameWithoutExtension(selectedBlueprintToSave.level.Path));
                Close();
            }
        }
        private void OnNewFolderButton()
        {
            folderPanel.Enable();
            ResetComponents();
        }
        private void OnUpOneLevelButton()
        {
            if (currentMode == BPXPanelMode.Level)
            {
                if (levelDirectory.Parent != null && levelDirectory.FullName != ZeeplevelIO.LevelBasePath)
                {
                    levelDirectory = levelDirectory.Parent;
                }
            }
            else if (currentMode == BPXPanelMode.Blueprint)
            {
                if (blueprintDirectory.Parent != null && blueprintDirectory.FullName != ZeeplevelIO.BlueprintBasePath)
                {
                    blueprintDirectory = blueprintDirectory.Parent;
                }
            }

            RefreshPanel();
        }
        private void OnSwitchDirButton()
        {
            if (currentMode == BPXPanelMode.Level)
            {
                currentMode = BPXPanelMode.Blueprint;
            }
            else if (currentMode == BPXPanelMode.Blueprint)
            {
                currentMode = BPXPanelMode.Level;
            }

            RefreshPanel();
        }
        private void OnOpenFolderButton()
        {
            string path = currentMode == BPXPanelMode.Level ? levelDirectory.FullName : blueprintDirectory.FullName;

            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Process.Start("explorer.exe", path);
            }
            else
            {
                BPXManager.central.manager.messenger.LogError("Not supported on this platform", 2f);
            }
        }      
        #endregion

        #region Preview
        public List<Texture2D> saveBlueprintTextures;
        public List<Texture2D> loadBlueprintTextures;
        public List<Sprite> saveBlueprintSprites = new List<Sprite>();
        public List<Sprite> loadBlueprintSprites = new List<Sprite>();
        public int saveBlueprintSpriteIndex = 0;
        public int loadBlueprintSpriteIndex = 0;
        private void OnLoadedBlueprintPreviewGenerated(List<Texture2D> captures)
        {
            loadBlueprintTextures = captures;
            loadBlueprintSprites.Clear();

            foreach(Texture2D tex in captures)
            {
                loadBlueprintSprites.Add(BPXSprites.Texture2DToSprite(tex));
            }

            loadBlueprintSpriteIndex = 0;
            panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImage(loadBlueprintSprites[loadBlueprintSpriteIndex]);
        }
        private void OnLoadPreviewButton()
        {
            loadBlueprintSpriteIndex++;
            if(loadBlueprintSpriteIndex >= loadBlueprintSprites.Count)
            {
                loadBlueprintSpriteIndex = 0;
            }
            panelComponents[BPXPanelComponentName.LoadPreview].SetButtonImage(loadBlueprintSprites[loadBlueprintSpriteIndex]);
        }
        private void OnSavedBlueprintPreviewGenerated(List<Texture2D> captures)
        {
            saveBlueprintTextures = captures;
            saveBlueprintSprites.Clear();

            foreach (Texture2D tex in captures)
            {
                saveBlueprintSprites.Add(BPXSprites.Texture2DToSprite(tex));
            }

            saveBlueprintSpriteIndex = 0;
            panelComponents[BPXPanelComponentName.SavePreview].SetButtonImage(saveBlueprintSprites[saveBlueprintSpriteIndex]);
        }        
        private void OnSavePreviewButton()
        {
            saveBlueprintSpriteIndex++;
            if (saveBlueprintSpriteIndex >= saveBlueprintSprites.Count)
            {
                saveBlueprintSpriteIndex = 0;
            }
            panelComponents[BPXPanelComponentName.SavePreview].SetButtonImage(saveBlueprintSprites[saveBlueprintSpriteIndex]);
        }
        #endregion
    }
}
