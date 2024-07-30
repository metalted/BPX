using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public enum BPXPanelComponentName { Background, Save, Load, LoadHere, LoadFile, Home, SwitchDir, LoadPreview, SavePreview, UpOneLevel, NewFolder, Upload, OpenFolder, Exit, ScrollView, URL, FileName, TypeText, SearchBar, Download, Search, PreviousPage, NextPage, PageCounter, SelectedName, SearchResultScrollView };
    public enum BPXPanelComponentType { Button, Image, Text, ScrollView, TextInput };
    public enum BPXPanelState { Closed, Save, Load, Open };
    public enum BPXPanelMode { Blueprint, Level };

    public class BPXPanel : MonoBehaviour
    {
        public virtual void OnConfirmPanel(bool confirmed)
        {
        }

        public virtual void OnFolderPanel(bool folderCreated)
        {
        }

        public virtual string GetCurrentPath()
        {
            return "";
        }
    }
}
