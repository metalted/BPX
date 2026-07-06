using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BPX.UI
{
    public class ConfigOrderedStringListEditor
    {
        private readonly ConfigEntry<string> configEntry;
        private readonly ConfigEntry<string> defaultValueEntry;
        private readonly char separator;
        private readonly List<string> values = new List<string>();

        private string loadedValue = string.Empty;
        private bool initialized;
        private bool hasUnsavedEmptyRow;

        public ConfigOrderedStringListEditor(
            ConfigEntry<string> configEntry,
            ConfigEntry<string> defaultValueEntry,
            char separator)
        {
            this.configEntry = configEntry;
            this.defaultValueEntry = defaultValueEntry;
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
            DrawBottomButtons(gui);
        }

        private void DrawRows(ImGui gui)
        {
            if (values.Count == 0)
            {
                gui.Text("No values.");
                return;
            }

            int moveUpIndex = -1;
            int moveDownIndex = -1;
            int removeIndex = -1;

            for (int i = 0; i < values.Count; i++)
            {
                DrawRow(gui, i, ref moveUpIndex, ref moveDownIndex, ref removeIndex);
            }

            if (moveUpIndex >= 0)
            {
                MoveUp(moveUpIndex);
            }

            if (moveDownIndex >= 0)
            {
                MoveDown(moveDownIndex);
            }

            if (removeIndex >= 0)
            {
                Remove(removeIndex);
            }
        }

        private void DrawRow(
            ImGui gui,
            int index,
            ref int moveUpIndex,
            ref int moveDownIndex,
            ref int removeIndex)
        {
            float totalWidth = gui.GetLayoutWidth();
            float height = gui.GetRowHeight();
            float spacing = gui.Style.Layout.Spacing;

            float upButtonWidth = 54f;
            float downButtonWidth = 70f;
            float deleteButtonWidth = 42f;
            float defaultButtonWidth = 46f;
            float defaultLabelWidth = 80f;

            float valueWidth =
                totalWidth -
                upButtonWidth -
                downButtonWidth -
                deleteButtonWidth -
                defaultButtonWidth -
                defaultLabelWidth -
                spacing * 5f;

            if (valueWidth < 120f)
            {
                valueWidth = 120f;
            }

            gui.BeginHorizontal(totalWidth);

            string oldValue = values[index];
            bool wasDefault = IsDefaultValue(oldValue);

            string newValue = gui.TextEdit(oldValue, new ImSize(valueWidth, height));

            if (newValue != oldValue)
            {
                values[index] = newValue;

                if (wasDefault && defaultValueEntry != null)
                {
                    defaultValueEntry.Value = newValue.Trim();
                }

                SaveToConfig();
            }

            if (index > 0)
            {
                if (gui.Button("▲", new ImSize(upButtonWidth, height)))
                {
                    moveUpIndex = index;
                }
            }
            else
            {
                DrawPlaceholderButton(gui, upButtonWidth, height);
            }

            if (index < values.Count - 1)
            {
                if (gui.Button("▼", new ImSize(downButtonWidth, height)))
                {
                    moveDownIndex = index;
                }
            }
            else
            {
                DrawPlaceholderButton(gui, downButtonWidth, height);
            }

            if (gui.Button("X", new ImSize(deleteButtonWidth, height)))
            {
                removeIndex = index;
            }

            DrawDefaultButton(gui, index, defaultButtonWidth, height);
            DrawTextCell(gui, "default", defaultLabelWidth, height);

            gui.EndHorizontal();
        }

        private void DrawDefaultButton(ImGui gui, int index, float width, float height)
        {
            if (defaultValueEntry == null)
            {
                DrawPlaceholderButton(gui, width, height);
                return;
            }

            string value = values[index] ?? string.Empty;
            bool isDefault = IsDefaultValue(value);

            string buttonText = isDefault ? "●" : "○";

            if (gui.Button(buttonText, new ImSize(width, height)))
            {
                string cleanValue = value.Trim();

                if (!string.IsNullOrWhiteSpace(cleanValue))
                {
                    defaultValueEntry.Value = cleanValue;
                }
            }
        }

        private void DrawBottomButtons(ImGui gui)
        {
            float totalWidth = gui.GetLayoutWidth();
            float height = gui.GetRowHeight();
            float spacing = gui.Style.Layout.Spacing;

            float buttonWidth = (totalWidth - spacing) * 0.5f;

            gui.BeginHorizontal(totalWidth);

            if (gui.Button("Add Entry", new ImSize(buttonWidth, height)))
            {
                values.Add(string.Empty);
                hasUnsavedEmptyRow = true;
            }

            if (gui.Button("Reset To Default", new ImSize(buttonWidth, height)))
            {
                ResetToDefault();
            }

            gui.EndHorizontal();
        }

        private void DrawPlaceholderButton(ImGui gui, float width, float height)
        {
            gui.Button("•", new ImSize(width, height));
        }

        private void DrawTextCell(ImGui gui, string text, float width, float height)
        {
            ImRect rect = gui.Layout.GetRect(width, height);
            gui.Text(text, new ImTextSettings(18), rect);
            gui.Layout.AddRect(rect);
        }

        private void MoveUp(int index)
        {
            if (index <= 0 || index >= values.Count)
            {
                return;
            }

            string value = values[index];
            values.RemoveAt(index);
            values.Insert(index - 1, value);

            SaveToConfig();
        }

        private void MoveDown(int index)
        {
            if (index < 0 || index >= values.Count - 1)
            {
                return;
            }

            string value = values[index];
            values.RemoveAt(index);
            values.Insert(index + 1, value);

            SaveToConfig();
        }

        private void Remove(int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return;
            }

            string removedValue = values[index];
            bool removedDefault = IsDefaultValue(removedValue);

            values.RemoveAt(index);

            if (removedDefault && defaultValueEntry != null)
            {
                defaultValueEntry.Value = GetFirstCleanValue();
            }

            SaveToConfig();
        }

        private void ResetToDefault()
        {
            if (configEntry == null)
            {
                return;
            }

            configEntry.Value = (string)configEntry.DefaultValue;

            if (defaultValueEntry != null)
            {
                defaultValueEntry.Value = (string)defaultValueEntry.DefaultValue;
            }

            ReloadFromConfig();
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

            string newConfigValue = string.Join(separator.ToString(), cleanValues.ToArray());

            configEntry.Value = newConfigValue;
            loadedValue = newConfigValue;
            hasUnsavedEmptyRow = HasEmptyRow();

            EnsureDefaultValueStillExists();
        }

        private void EnsureDefaultValueStillExists()
        {
            if (defaultValueEntry == null)
            {
                return;
            }

            string defaultValue = defaultValueEntry.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(defaultValue))
            {
                defaultValueEntry.Value = GetFirstCleanValue();
                return;
            }

            foreach (string value in values)
            {
                if (ValuesMatch(value, defaultValue))
                {
                    return;
                }
            }

            defaultValueEntry.Value = GetFirstCleanValue();
        }

        private string GetFirstCleanValue()
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
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

        private bool IsDefaultValue(string value)
        {
            if (defaultValueEntry == null)
            {
                return false;
            }

            return ValuesMatch(value, defaultValueEntry.Value);
        }

        private bool ValuesMatch(string a, string b)
        {
            return string.Equals(
                (a ?? string.Empty).Trim(),
                (b ?? string.Empty).Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}