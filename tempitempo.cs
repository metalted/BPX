using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    internal class tempitempo
    {
        /*
         * BPXUI.blueprintSavePanel = UnityEngine.Object.Instantiate<Transform>(BPXManager.central.saveload.transform, BPXManager.central.saveload.transform.parent);
      BPXUI.blueprintSavePanel.gameObject.name = "Blueprint Save Panel";
      UnityEngine.Object.Destroy((UnityEngine.Object) BPXUI.blueprintSavePanel.GetComponent<LEV_SaveLoad>());
      RectTransform innerPanel = BPXUI.blueprintSavePanel.GetChild(1).GetComponent<RectTransform>();
      innerPanel.GetComponent<Image>().color = BPXManager.darkerBlue;
      innerPanel.gameObject.SetActive(true);
      BPXUI.allButtons.Clear();
      BPXUI.saveButton = innerPanel.GetChild(0).GetComponent<LEV_CustomButton>();
      LEV_CustomButton homeButton = innerPanel.GetChild(1).GetComponent<LEV_CustomButton>();
      LEV_CustomButton autosavesButton = innerPanel.GetChild(2).GetComponent<LEV_CustomButton>();
      LEV_CustomButton backupsButton = innerPanel.GetChild(3).GetComponent<LEV_CustomButton>();
      LEV_CustomButton upOneLevelButton = innerPanel.GetChild(4).GetComponent<LEV_CustomButton>();
      LEV_CustomButton newFolderButton = innerPanel.GetChild(5).GetComponent<LEV_CustomButton>();
      LEV_CustomButton sortButton = innerPanel.GetChild(6).GetComponent<LEV_CustomButton>();
      LEV_CustomButton openFolderButton = innerPanel.GetChild(7).GetComponent<LEV_CustomButton>();
      LEV_CustomButton exitButton = innerPanel.GetChild(8).GetComponent<LEV_CustomButton>();
      
      BPXUI.URLText = innerPanel.GetChild(9).GetComponent<TextMeshProUGUI>();
      
        
        
      ScrollRect scrollView = innerPanel.GetChild(10).GetComponent<ScrollRect>();
      BPXUI.fileName = innerPanel.GetChild(11).GetComponent<TMP_InputField>();
      BPXUI.zeepfileTypeText = innerPanel.GetChild(12).GetComponent<TextMeshProUGUI>();
      BPXUI.areYouSurePanel = innerPanel.GetChild(13).GetComponent<RectTransform>();
      BPXUI.newFolderDialogPanel = innerPanel.GetChild(14).GetComponent<RectTransform>();
      RectTransform homeButtonRect = homeButton.GetComponent<RectTransform>();
      RectTransform upOneLevelRect = upOneLevelButton.GetComponent<RectTransform>();
      RectTransform autoSavesButtonRect = autosavesButton.GetComponent<RectTransform>();
      RectTransform newFolderButtonRect = newFolderButton.GetComponent<RectTransform>();
      RectTransform scrollViewRect = scrollView.GetComponent<RectTransform>();
      RectTransform typeTextRect = BPXUI.zeepfileTypeText.GetComponent<RectTransform>();
      RectTransform component21 = backupsButton.GetComponent<RectTransform>();









      BPXUI.ReconfigureCustomButton(BPXUI.saveButton, new UnityAction(BPXUI.OnPanelSaveButton));
      BPXUI.allButtons.Add(BPXUI.saveButton);

      GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(BPXUI.saveButton.gameObject, BPXUI.saveButton.transform.parent);
      gameObject2.name = "Save Button Small Left";
      BPXUI.saveButtonSmallLeft = gameObject2.GetComponent<LEV_CustomButton>();
      BPXUI.ReconfigureCustomButton(BPXUI.saveButtonSmallLeft, new UnityAction(BPXUI.OnPanelSaveButtonSmallLeft));
      BPXUI.saveButtonSmallLeft.transform.GetChild(0).GetComponent<Image>().sprite = BlueprintXPlugin.markerSprite;
      BPXUI.allButtons.Add(BPXUI.saveButtonSmallLeft);

      BPXUI.saveButtonSmallRight = BPXUI.SplitLEVCustomButton(gameObject2.transform, "Save Button Small Right", (UnityAction) (() => BPXUI.OnPanelSaveButtonSmallRight()), 0.05f);
      BPXUI.saveButtonSmallRight.transform.GetChild(0).GetComponent<Image>().sprite = BlueprintXPlugin.fileSprite;
      BPXUI.allButtons.Add(BPXUI.saveButtonSmallRight);


      float num1 = (float) (((double) homeButtonRect.anchorMin.y - (double) autoSavesButtonRect.anchorMax.y) / 2.0);
      Vector2 vector2_2 = new Vector2(homeButtonRect.anchorMax.x - homeButtonRect.anchorMin.x, scrollViewRect.anchorMax.y - scrollViewRect.anchorMin.y);
      Vector3 vector3_1 = (Vector3) new Vector2(vector2_2.x, vector2_2.x);
      Vector3 vector3_2 = (Vector3) new Vector2(vector2_2.x, (float) (((double) vector2_2.y - (double) vector3_1.y) / 3.0));
      Vector2 vector2_3 = new Vector2(homeButtonRect.anchorMin.x, homeButtonRect.anchorMax.y);




      typeTextRect.anchorMin = new Vector2(0.03f, 0.8f);
      typeTextRect.anchorMax = new Vector2(0.23f, 0.85f);
      homeButtonRect.anchorMin = new Vector2(0.03f, 0.71f);
      homeButtonRect.anchorMax = new Vector2(0.07f, 0.79f);
      BPXUI.ReconfigureCustomButton(homeButton, new UnityAction(BPXUI.OnPanelHomeButton));
      BPXUI.allButtons.Add(homeButton);








      upOneLevelRect.anchorMin = new Vector2(0.08f, 0.71f);
      upOneLevelRect.anchorMax = new Vector2(0.12f, 0.79f);
      BPXUI.ReconfigureCustomButton(upOneLevelButton, new UnityAction(BPXUI.OnPanelParentFolderButton));
      BPXUI.allButtons.Add(upOneLevelButton);





      newFolderButtonRect.anchorMin = new Vector2(0.13f, 0.71f);
      newFolderButtonRect.anchorMax = new Vector2(0.17f, 0.79f);
      BPXUI.ReconfigureCustomButton(newFolderButton, new UnityAction(BPXUI.OnPanelNewFolderButton));
      BPXUI.allButtons.Add(newFolderButton);

      autoSavesButtonRect.anchorMin = new Vector2(0.18f, 0.71f);
      autoSavesButtonRect.anchorMax = new Vector2(0.23f, 0.79f);
      BPXUI.blueprintLevelSwitchButton = autosavesButton;
      BPXUI.ReconfigureCustomButton(BPXUI.blueprintLevelSwitchButton, new UnityAction(BPXUI.OnPanelBlueprintLevelSwitchButton));

      BPXUI.allButtons.Add(BPXUI.blueprintLevelSwitchButton);


      BPXUI.blueprintLevelSwitchButton.transform.GetChild(0).GetComponent<Image>().sprite = BlueprintXPlugin.fileSwitchSprite;
      BPXUI.fileName.GetComponent<Image>().color = Color.white;

        ///////////////////////WE GOT SOEMWHERE AROUND HERE

      Transform transform = UnityEngine.Object.Instantiate<Transform>(BPXUI.fileName.transform, BPXUI.fileName.transform.parent);
      BPXUI.searchBar = transform.GetComponent<TMP_InputField>();
      transform.GetComponent<Image>().color = Color.white;
      UnityEngine.Object.Destroy((UnityEngine.Object) BPXUI.searchBar.placeholder.GetComponent<Localize>());
      BPXUI.searchBar.placeholder.GetComponent<TMP_Text>().text = "Search...";

      RectTransform component22 = BPXUI.searchBar.GetComponent<RectTransform>();
      component22.anchorMin = new Vector2(0.03f, 0.63f);
      component22.anchorMax = new Vector2(0.23f, 0.7f);
      component21.anchorMin = new Vector2(0.03f, 0.25f);
      component21.anchorMax = new Vector2(0.23f, 0.62f);
      BPXUI.ReconfigureCustomButton(backupsButton, new UnityAction(BPXUI.OnPreviewClicked));
      backupsButton.clickColor = Color.black;
      backupsButton.hoverColor = Color.black;
      backupsButton.normalColor = Color.black;
      backupsButton.selectedColor = Color.black;
      backupsButton.isDisabled_clickColor = Color.black;
      backupsButton.isDisabled_hoverColor = Color.black;
      backupsButton.isDisabled_normalColor = Color.black;
      backupsButton.isDisabled_selectedColor = Color.black;
      RectTransform component23 = backupsButton.transform.GetChild(0).GetComponent<RectTransform>();
      component23.anchorMax = Vector2.one;
      component23.anchorMin = Vector2.zero;
      backupsButton.transform.GetChild(0).GetComponent<Image>().sprite = BlueprintXPlugin.blackPixelSprite;
      BPXUI.previewContainer = backupsButton;
      homeButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
      RectTransform component24 = homeButton.transform.GetChild(0).GetComponent<RectTransform>();
      component24.anchorMin = new Vector2(0.1f, 0.1f);
      component24.anchorMax = new Vector2(0.9f, 0.9f);
      newFolderButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
      RectTransform component25 = newFolderButton.transform.GetChild(0).GetComponent<RectTransform>();
      component25.anchorMin = new Vector2(0.1f, 0.1f);
      component25.anchorMax = new Vector2(0.9f, 0.9f);
      autosavesButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
      RectTransform component26 = autosavesButton.transform.GetChild(0).GetComponent<RectTransform>();
      component26.anchorMin = new Vector2(0.1f, 0.1f);
      component26.anchorMax = new Vector2(0.9f, 0.9f);
      backupsButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
      RectTransform component27 = backupsButton.transform.GetChild(0).GetComponent<RectTransform>();
      component27.anchorMin = new Vector2(0.1f, 0.1f);
      component27.anchorMax = new Vector2(0.9f, 0.9f);
      BPXUI.ReconfigureCustomButton(openFolderButton, new UnityAction(BPXUI.OnPanelOpenInExplorerButton));
      BPXUI.allButtons.Add(openFolderButton);
      BPXUI.ReconfigureCustomButton(exitButton, new UnityAction(BPXUI.CloseSavePanel));
      BPXUI.allButtons.Add(exitButton);
      sortButton.gameObject.SetActive(false);
      BPXUI.explorerPanel = scrollView.content;
      ContentSizeFitter contentSizeFitter = BPXUI.explorerPanel.gameObject.AddComponent<ContentSizeFitter>();
      contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
      GridLayoutGroup gridLayoutGroup = BPXUI.explorerPanel.gameObject.AddComponent<GridLayoutGroup>();
      gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
      gridLayoutGroup.constraintCount = 6;
      Rect rect = scrollView.viewport.rect;
      int num2 = Mathf.RoundToInt(rect.width / 100f);
      rect = scrollView.viewport.rect;
      float num3 = (float) (((double) rect.width - (double) num2) / 6.0);
      rect = scrollView.viewport.rect;
      float num4 = (float) (((double) rect.width - (double) num2) / 6.0 * 0.05000000074505806);
      gridLayoutGroup.cellSize = new Vector2(num3 - num4, num3 - num4);
      gridLayoutGroup.spacing = new Vector2(num4, num4);
      gridLayoutGroup.padding = new RectOffset(num2, num2, num2, num2);
      BPXUI.alreadyExistsText = BPXUI.areYouSurePanel.GetChild(2).GetComponent<TextMeshProUGUI>();
      LEV_CustomButton component28 = BPXUI.areYouSurePanel.GetChild(3).GetComponent<LEV_CustomButton>();
      BPXUI.ReconfigureCustomButton(component28, new UnityAction(BPXUI.OnPanelAreYouSureSave));
      BPXUI.allButtons.Add(component28);
      LEV_CustomButton component29 = BPXUI.areYouSurePanel.GetChild(4).GetComponent<LEV_CustomButton>();
      BPXUI.ReconfigureCustomButton(component29, new UnityAction(BPXUI.OnPanelAreYouSureCancel));
      BPXUI.allButtons.Add(component29);
      Transform child = BPXUI.newFolderDialogPanel.GetChild(0);
      LEV_CustomButton component30 = child.GetChild(0).GetComponent<LEV_CustomButton>();
      BPXUI.ReconfigureCustomButton(component30, new UnityAction(BPXUI.OnNewFolderExit));
      BPXUI.allButtons.Add(component30);
      LEV_CustomButton component31 = child.GetChild(1).GetComponent<LEV_CustomButton>();
      BPXUI.ReconfigureCustomButton(component31, new UnityAction(BPXUI.OnNewFolderCreate));
      BPXUI.allButtons.Add(component31);
      BPXUI.newFolderNameText = child.GetChild(2).GetComponent<TMP_InputField>();
      BPXUI.levelDirectory = new DirectoryInfo(BPXManager.levelHomeDirectory);
      BPXUI.blueprintDirectory = new DirectoryInfo(BPXManager.blueprintHomeDirectory);
      BPXUI.areYouSurePanel.SetAsLastSibling();
      BPXUI.newFolderDialogPanel.SetAsLastSibling();
      if (BPXConfig.doubleLoadButtons)
      {
        BPXUI.saveButton.gameObject.SetActive(false);
        BPXUI.saveButtonSmallLeft.gameObject.SetActive(true);
        BPXUI.saveButtonSmallRight.gameObject.SetActive(true);
      }
      else
      {
        BPXUI.saveButton.gameObject.SetActive(true);
        BPXUI.saveButtonSmallLeft.gameObject.SetActive(false);
        BPXUI.saveButtonSmallRight.gameObject.SetActive(false);
      }
      BPXUI.RefreshSavePanel();
    }
        */
    }
}
