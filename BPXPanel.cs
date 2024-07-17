using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BPX
{
    public enum BPXPanelComponentName { Background, Save, Load, LoadHere, LoadFile, Home, SwitchDir, Preview, UpOneLevel, NewFolder, Sort, OpenFolder, Exit, ScrollView, URL, FileName, TypeText, SearchBar };
    public enum BPXPanelComponentType { Button, Image, Text, ScrollView, TextInput };    
    public enum BPXPanelState { Closed, Save, Load };
    public enum BPXPanelMode { Blueprint, Level };
    
    public class BPXPanel : MonoBehaviour
    {
        public Dictionary<BPXPanelComponentName, BPXPanelComponent> panelComponents;

        public BPXConfirmPanel confirmPanel;
        public BPXFolderPanel folderPanel;

        public DirectoryInfo blueprintDirectory;
        public DirectoryInfo levelDirectory;

        public BPXPanelState currentState = BPXPanelState.Closed;
        public BPXPanelMode currentMode = BPXPanelMode.Blueprint;

        private List<LEV_FileContent> currentExplorerElements = new List<LEV_FileContent>();


        public void Initialize(LEV_LevelEditorCentral central)
        {
            GetPanelComponents();
            ConfigurePanel();
            RefreshDirectories();
        }

        public void RefreshDirectories()
        {
            blueprintDirectory = new DirectoryInfo(Plugin.Instance.pluginPath);
            levelDirectory = new DirectoryInfo(Plugin.Instance.levelPath);
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
                        panelComponents.Add(BPXPanelComponentName.Preview, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Preview, rt));
                        break;
                    case "Up One Level Button":
                        panelComponents.Add(BPXPanelComponentName.UpOneLevel, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.UpOneLevel, rt));
                        break;
                    case "New Folder Button":
                        panelComponents.Add(BPXPanelComponentName.NewFolder, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.NewFolder, rt));
                        break;
                    case "Sort Regular Levels Button":
                        panelComponents.Add(BPXPanelComponentName.Sort, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Sort, rt));
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
                }
            }
        }            
    
        private void ConfigurePanel()
        {
            //Set the background color of the panel
            panelComponents[BPXPanelComponentName.Background].Image.color = UIManagement.darkBlue;

            //Create a copy of the save button for the load button
            RectTransform loadRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.Save].Rect.gameObject, panelComponents[BPXPanelComponentName.Save].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.Load, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Load, loadRect));

            //Create a copy of the save button so we can create the two smaller load buttons
            RectTransform loadHereRect = GameObject.Instantiate(panelComponents[BPXPanelComponentName.Save].Rect.gameObject, panelComponents[BPXPanelComponentName.Save].Rect.transform.parent).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.LoadHere, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.LoadHere, loadHereRect));
            RectTransform loadFileRect = UIManagement.SplitLEVCustomButton(panelComponents[BPXPanelComponentName.LoadHere].Button, 0.05f).GetComponent<RectTransform>();
            panelComponents.Add(BPXPanelComponentName.LoadFile, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.LoadFile, loadFileRect));
            
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
            panelComponents[BPXPanelComponentName.Preview].SetRectAnchors(0.03f, 0.25f, 0.23f, 0.62f);

            //Bind functions to the buttons.
            panelComponents[BPXPanelComponentName.Save].BindButton(() => OnSaveButton());
            panelComponents[BPXPanelComponentName.Load].BindButton(() => OnLoadButton());
            panelComponents[BPXPanelComponentName.LoadHere].BindButton(() => OnLoadHereButton());
            panelComponents[BPXPanelComponentName.LoadFile].BindButton(() => OnLoadFileButton());
            panelComponents[BPXPanelComponentName.Home].BindButton(() => OnHomeButton());
            panelComponents[BPXPanelComponentName.UpOneLevel].BindButton(()=> OnUpOneLevelButton());
            panelComponents[BPXPanelComponentName.NewFolder].BindButton(()=> OnNewFolderButton());
            panelComponents[BPXPanelComponentName.SwitchDir].BindButton(() => OnSwitchDirButton());
            panelComponents[BPXPanelComponentName.Preview].BindButton(() => OnPreviewButton());
            panelComponents[BPXPanelComponentName.OpenFolder].BindButton(() => OnOpenFolderButton());
            panelComponents[BPXPanelComponentName.Exit].BindButton(() => OnExitButton());

            //Change button image sizes
            panelComponents[BPXPanelComponentName.Home].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.NewFolder].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.SwitchDir].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);
            panelComponents[BPXPanelComponentName.Preview].SetButtonImageRectAnchors(0.1f, 0.1f, 0.9f, 0.9f);

            //Set sprites
            panelComponents[BPXPanelComponentName.LoadHere].SetButtonImage(BPXSprites.markerSprite);
            panelComponents[BPXPanelComponentName.LoadFile].SetButtonImage(BPXSprites.fileSprite);
            panelComponents[BPXPanelComponentName.SwitchDir].SetButtonImage(BPXSprites.fileSprite);
            panelComponents[BPXPanelComponentName.Preview].SetButtonImage(BPXSprites.blackPixelSprite);
            panelComponents[BPXPanelComponentName.Save].SetButtonImage(BPXManager.central.saveload.saveImage);
            panelComponents[BPXPanelComponentName.Load].SetButtonImage(BPXManager.central.saveload.loadImage);

            //Turn preview button completely black
            UIManagement.RecolorButton(panelComponents[BPXPanelComponentName.Preview].Button, Color.black, true);

            //Remove texts from buttons
            panelComponents[BPXPanelComponentName.Home].HideButtonText();
            panelComponents[BPXPanelComponentName.NewFolder].HideButtonText();
            panelComponents[BPXPanelComponentName.SwitchDir].HideButtonText();
            panelComponents[BPXPanelComponentName.Preview].HideButtonText();

            //Hide the sort button
            panelComponents[BPXPanelComponentName.Sort].Button.gameObject.SetActive(false);

            confirmPanel.Rect.SetAsLastSibling();
            folderPanel.Rect.SetAsLastSibling();

            //Set some values 
            panelComponents[BPXPanelComponentName.URL].SetText("path/to/some/file");
            panelComponents[BPXPanelComponentName.FileName].SetPlaceHolderText("Filename placeholder...");
            panelComponents[BPXPanelComponentName.SearchBar].SetPlaceHolderText("Search...");
            panelComponents[BPXPanelComponentName.TypeText].SetText("Blueprints");
        }

        public void Open(BPXPanelState panelMode)
        {
            if(panelMode == BPXPanelState.Closed || BPXManager.central == null)
            {
                Close();
                return;
            }

            if(panelMode == BPXPanelState.Save)
            {
                panelComponents[BPXPanelComponentName.Save].Enable();
                panelComponents[BPXPanelComponentName.Load].Disable();
                        
                panelComponents[BPXPanelComponentName.FileName].SetInteractable(true);                
                panelComponents[BPXPanelComponentName.TypeText].SetText("Save Blueprint");

                panelComponents[BPXPanelComponentName.LoadHere].Disable();
                panelComponents[BPXPanelComponentName.LoadFile].Disable();
                panelComponents[BPXPanelComponentName.SearchBar].Disable();
            }
            else if(panelMode == BPXPanelState.Load)
            {
                panelComponents[BPXPanelComponentName.Save].Disable();
                panelComponents[BPXPanelComponentName.FileName].SetInteractable(false);
                panelComponents[BPXPanelComponentName.TypeText].SetText("Import Blueprint");
                panelComponents[BPXPanelComponentName.SearchBar].Enable();

                if(BPXConfiguration.DoubleLoadButton())
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

                if(BPXHistory.PreviouslyLoadedBlueprintAvailable())
                {
                    //....
                }
                else
                {
                    panelComponents[BPXPanelComponentName.Preview].SetButtonImage(BPXSprites.blackPixelSprite);
                    panelComponents[BPXPanelComponentName.FileName].SetText("");
                }
            }           

            currentState = panelMode;

            RefreshPanel();
            ResetComponents();

            UIManagement.OnPanelOpen();

            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            confirmPanel.Disable();
            folderPanel.Disable();            
            currentState = BPXPanelState.Closed;
            UIManagement.OnPanelClose();
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

                if(currentState == BPXPanelState.Save)
                {
                    panelComponents[BPXPanelComponentName.SwitchDir].Disable();
                    panelComponents[BPXPanelComponentName.TypeText].SetText("Save Blueprint");
                }
                else if(currentState == BPXPanelState.Load)
                {
                    panelComponents[BPXPanelComponentName.SwitchDir].Enable();
                    panelComponents[BPXPanelComponentName.TypeText].SetText(currentMode == BPXPanelMode.Blueprint ? "Import Blueprint" : "Import Level");
                }
            }
            catch
            {
                OnHomeButton();
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
            foreach(KeyValuePair<BPXPanelComponentName, BPXPanelComponent> comp in panelComponents)
            {
                comp.Value.Reset();
            }
        }

        private static void OnDirectorySelectedInExplorer(DirectoryInfo directoryInfo)
        {
            Debug.Log(directoryInfo.FullName);
        }

        private static void OnFileSelectedInExplorer(FileInfo fileInfo)
        {
            Debug.Log(fileInfo.FullName);
        }

        private void OnSaveButton()
        {
            Debug.Log("Save!");
        }

        private void OnLoadButton()
        {
            Debug.Log("Load!");
        }

        private void OnLoadHereButton()
        {
            Debug.Log("LoadHere!");
        }

        private void OnLoadFileButton()
        {
            Debug.Log("LoadFile!");
        }       
        
        private void OnHomeButton()
        {
            Debug.Log("Home!");
        }

        private void OnUpOneLevelButton()
        {
            Debug.Log("UpOneLevel");
        }

        private void OnNewFolderButton()
        {
            Debug.Log("NewFolder!");
        }

        private void OnSwitchDirButton()
        {
            Debug.Log("Switch!");
        }

        private void OnPreviewButton()
        {
            Debug.Log("Preview!");
        }

        private void OnOpenFolderButton()
        {
            Debug.Log("OpenFolder!");
        }

        private void OnExitButton()
        {
            Debug.Log("Exit!");
        }
    }
}
