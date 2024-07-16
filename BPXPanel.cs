using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BPX
{
    public enum BPXPanelComponentName { Background, Save, LoadHere, LoadFile, Home, SwitchDir, Preview, UpOneLevel, NewFolder, Sort, OpenFolder, Exit, ScrollView, URL, FileName, TypeText, SearchBar };
    public enum BPXPanelComponentType { Button, Image, Text, ScrollView, TextInput };    
    
    public class BPXPanel : MonoBehaviour
    {
        public Dictionary<BPXPanelComponentName, BPXPanelComponent> panelComponents;
        public BPXConfirmPanel confirmPanel;
        public BPXFolderPanel folderPanel;

        public void Initialize(LEV_LevelEditorCentral central)
        {
            GetPanelComponents();
            ConfigurePanel();
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

            //Change sprites
            //panelComponents[BPXPanelComponentName.LoadHere].SetButtonImage(...);
            //panelComponents[BPXPanelComponentName.LoadFile].SetButtonImage(...);
            //panelComponents[BPXPanelComponentName.SwitchDir].SetButtonImage(...);
            //panelComponents[BPXPanelComponentName.Preview].SetButtonImage(...);

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
            panelComponents[BPXPanelComponentName.URL].SetText("Bla/bli/blo/blu/yadayada!");
            panelComponents[BPXPanelComponentName.FileName].SetPlaceHolderText("This is place holder for filename!");
            panelComponents[BPXPanelComponentName.SearchBar].SetPlaceHolderText("Search...");
            panelComponents[BPXPanelComponentName.TypeText].SetText("Blueprints");
        }

        private void OnSaveButton()
        {
            Debug.Log("Save!");
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
