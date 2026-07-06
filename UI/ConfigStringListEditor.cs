using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BPX.UI
{
    public class ConfigStringListEditor
    {
        private readonly ConfigEntry<string> configEntry;
        private readonly char separator;
        private readonly List<string> values = new List<string>();

        private string loadedValue = string.Empty;
        private bool initialized;
        private bool hasUnsavedEmptyRow;

        public ConfigStringListEditor(ConfigEntry<string> configEntry, char separator)
        {
            this.configEntry = configEntry;
            this.separator = separator;
        }

        public void Draw(ImGui gui)
        {
            if (configEntry == null)
            {
                gui.Text("Missing config entry.");
                return;
            }

            if (!initialized || (!hasUnsavedEmptyRow && loadedValue != configEntry.Value))
            {
                ReloadFromConfig();
            }

            DrawRows(gui);
            gui.AddSpacing();
            DrawAddButton(gui);
        }

        private void DrawRows(ImGui gui)
        {
            if (values.Count == 0)
            {
                gui.Text("No entries.");
                return;
            }

            int removeIndex = -1;

            for (int i = 0; i < values.Count; i++)
            {
                DrawRow(gui, i, ref removeIndex);
            }

            if (removeIndex >= 0)
            {
                values.RemoveAt(removeIndex);
                SaveToConfig();
            }
        }

        private void DrawRow(ImGui gui, int index, ref int removeIndex)
        {
            float totalWidth = gui.GetLayoutWidth();
            float height = gui.GetRowHeight();
            float spacing = gui.Style.Layout.Spacing;

            float deleteButtonWidth = 42f;
            float entryWidth = Mathf.Max(120f, totalWidth - deleteButtonWidth - spacing);

            gui.BeginHorizontal(totalWidth);

            string newValue = gui.TextEdit(values[index], new ImSize(entryWidth, height));

            if (newValue != values[index])
            {
                values[index] = newValue;
                SaveToConfig();
            }

            if (gui.Button("X", new ImSize(deleteButtonWidth, height)))
            {
                removeIndex = index;
            }

            gui.EndHorizontal();
        }

        private void DrawAddButton(ImGui gui)
        {
            if (gui.Button("Add", new ImSize(100f, gui.GetRowHeight())))
            {
                values.Add(string.Empty);
                hasUnsavedEmptyRow = true;
            }
        }

        private void ReloadFromConfig()
        {
            values.Clear();

            loadedValue = string.Empty;

            if (configEntry != null && configEntry.Value != null)
            {
                loadedValue = configEntry.Value;
            }

            string[] parts = loadedValue.Split(separator);

            foreach (string part in parts)
            {
                string value = part.Trim();

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                values.Add(value);
            }

            initialized = true;
            hasUnsavedEmptyRow = false;
        }

        private void SaveToConfig()
        {
            List<string> cleanValues = new List<string>();

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                cleanValues.Add(value.Trim());
            }

            string separatorText = separator.ToString();
            string newConfigValue = string.Join(separatorText, cleanValues.ToArray());

            configEntry.Value = newConfigValue;
            loadedValue = newConfigValue;
            hasUnsavedEmptyRow = HasEmptyRow();
        }

        private bool HasEmptyRow()
        {
            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}