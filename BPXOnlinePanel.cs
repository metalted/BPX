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

/*
 * We need a double view, one for displaying the search results. One for chosen a folder
 * On the left we have search input and a search button.
 *
 */




namespace BPX
{
    public class BPXOnlinePanel : MonoBehaviour
    {
        public Dictionary<BPXPanelComponentName, BPXPanelComponent> panelComponents;
        public BPXPanelState currentState = BPXPanelState.Closed;
        private List<LEV_FileContent> currentExplorerElements = new List<LEV_FileContent>();
        private string searchValue = "";

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
                    case "Sort Regular Levels Button":
                        panelComponents.Add(BPXPanelComponentName.Upload, new BPXPanelComponent(BPXPanelComponentType.Button, BPXPanelComponentName.Upload, rt));
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
                        rt.gameObject.SetActive(false);
                        break;
                    case "Create New Folder Panel (false)":
                        rt.gameObject.SetActive(false);
                        break;
                }
            }
        }

        private void ConfigurePanel()
        {
            //Set the background color of the panel
            panelComponents[BPXPanelComponentName.Background].Image.color = BPXUIManagement.darkBlue;

            panelComponents[BPXPanelComponentName.Exit].BindButton(() => Close());
        }
        #endregion

        #region ExplorerPanel
        private void OnFileSelectedInExplorer(FileInfo fileInfo)
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
        #endregion
    }
}
