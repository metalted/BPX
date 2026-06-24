using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using BPX.UI;
using Toolkist;

namespace BPX
{
    public class BPXController : MonoBehaviour
    {
        private enum Direction { Up, Down, Left, Right };
        private enum ClipboardAction { Copy, Paste };

        private LEV_LevelEditorCentral central;
        private bool init = false;

        //Drag Selection
        private Dictionary<Vector3, BlockProperties> bpPositionMap = new Dictionary<Vector3, BlockProperties>();
        private List<BlockProperties> selectionTargets = new List<BlockProperties>();
        private Vector2 dragStartPosition;
        private bool isDragging;
        private bool isDraggingTeamX;
        private Rect dragBox;
        private List<string> beforeSelection;
        private Vector3 tempDragVector;
        private int tempCounter;

        public void Initialize(LEV_LevelEditorCentral central)
        {
            this.central = central;
            init = true;
        }

        public void Start()
        {
            BPXManager.central.cam.moveSpeed = BPXConfiguration.currentMoveSpeed;
        }
        
        private bool AllowRun()
        {
            if(!init) { return false; }

            if (EditorOperations.IsInputBlocked(central)) { return false; }
            if (EditorOperations.IsDragging(central)) { return false; }
            if (EditorOperations.IsInGMode(central)) { return false; }
            if (EditorOperations.InUIPanelMode(central)) { return false; }
            if (BPXUIManagement.IsPanelOpen()) { return false; }

            return true;
        }
        public void ResetController()
        {
            if (isDragging)
            {
                EditorOperations.DeselectAllBlocks(central);
                bpPositionMap.Clear();
                dragStartPosition = Vector3.zero;
                isDragging = false;
                dragBox = new Rect();
            }
        }
        public void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                ResetController();
            }
        }

        //Inputs
        private bool GetKeyEnabled(KeyCode controlKey, bool enableKeyState, bool enableKeyRequired)
        {
            if(controlKey == KeyCode.None) { return false; }

            if(enableKeyRequired)
            {
                return Input.GetKeyDown(controlKey) && enableKeyState;
            }
            else
            {
                return Input.GetKeyDown(controlKey);
            }
        }
        private bool GetKeyContinuouslyEnabled(KeyCode controlKey, bool enableKeyState, bool enableKeyRequired)
        {
            if (controlKey == KeyCode.None) { return false; }

            if (enableKeyRequired)
            {
                return Input.GetKey(controlKey) && enableKeyState;
            }
            else
            {
                return Input.GetKey(controlKey);
            }
        }
        private int GetScrollDirection(bool enableKeyState, bool enableKeyRequired, bool invertScroll)
        {
            if (enableKeyRequired && !enableKeyState)
            {
                return 0;
            }

            float scrollDelta = Input.mouseScrollDelta.y;

            if (scrollDelta == 0)
            {
                return 0;
            }

            if (invertScroll)
            {
                return scrollDelta > 0 ? -1 : 1;
            }
            else
            {
                return scrollDelta > 0 ? 1 : -1;
            }
        }

        //Controls
        public void Update()
        {
            if (!AllowRun()) { return; }

            bool enableKeyState = Input.GetKey(BPXConfiguration.GetEnableKey());
            bool modifierKeyState = Input.GetKey(BPXConfiguration.GetModifierKey());

            GeneralControls(enableKeyState, modifierKeyState);

            if (EditorOperations.InEditMode(central))
            {
                EditModeControls(enableKeyState, modifierKeyState);
            }
        }

        private void GeneralControls(bool enable, bool modifier)
        {
            //Save shortcut
            if (GetKeyEnabled(BPXConfiguration.GetSaveShortcutKey(), enable, BPXConfiguration.ShortcutRequiresEnableKey()))
            {
                BPXUIManagement.PressButtonByName("Save");
            }

            //Load shortcut
            if (GetKeyEnabled(BPXConfiguration.GetLoadShortcutKey(), enable, BPXConfiguration.ShortcutRequiresEnableKey()))
            {
                BPXUIManagement.PressButtonByName("Load");
            }

            //Move speed selection
            if (GetKeyContinuouslyEnabled(BPXConfiguration.GetFastTravelKey(), enable, BPXConfiguration.FastTravelRequiresEnableKey()))
            {
                HandleMoveSpeedSelection();
            }
        }

        private void EditModeControls(bool enable, bool modifier)
        {
            //Move with keys
            if (
                (EditorOperations.InBlockMovementMode(central)) ||
                (EditorOperations.InBlockRotationMode(central) && !BPXConfiguration.KeyRotationIsEnabled() && BPXConfiguration.MovementIfRotationIsDisabled()))
            {

                //Key movement (up)
                if (GetKeyEnabled(BPXConfiguration.GetForwardUpMovementKey(), enable, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Up, modifier);
                }
                //Key movement (down)
                if (GetKeyEnabled(BPXConfiguration.GetBackDownMovementKey(), enable, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Down, modifier);
                }
                //Key movement (left)
                if (GetKeyEnabled(BPXConfiguration.GetLeftMovementKey(), enable, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Left, modifier);
                }
                //Key movement (right)
                if (GetKeyEnabled(BPXConfiguration.GetRightMovementKey(), enable, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Right, modifier);
                }
            }

            //Rotate with key
            if (EditorOperations.InBlockRotationMode(central) && BPXConfiguration.KeyRotationIsEnabled())
            {
                //Key rotation (up)
                if (GetKeyEnabled(BPXConfiguration.GetXPositiveRotationKey(), enable, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Up, modifier);
                }
                //Key rotation (down)
                if (GetKeyEnabled(BPXConfiguration.GetXNegativeRotationKey(), enable, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Down, modifier);
                }
                //Key rotation (left)
                if (GetKeyEnabled(BPXConfiguration.GetYZNegativeRotationKey(), enable, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Left, modifier);
                }
                //Key rotation (right)
                if (GetKeyEnabled(BPXConfiguration.GetYZPositiveRotationKey(), enable, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Right, modifier);
                }
            }

            //Scale down with key
            if (GetKeyEnabled(BPXConfiguration.GetNegativeScalingKey(), enable, BPXConfiguration.ScalingRequiresEnableKey()))
            {
                HandleScaling(false, modifier);
            }
            //Scale up with key
            if (GetKeyEnabled(BPXConfiguration.GetPositiveScalingKey(), enable, BPXConfiguration.ScalingRequiresEnableKey()))
            {
                HandleScaling(true, modifier);
            }
            //Scale with scroll
            if (BPXConfiguration.DoScrollScaling())
            {
                int scrollDirection = GetScrollDirection(enable, BPXConfiguration.ScalingRequiresEnableKey(), BPXConfiguration.InvertScrollScaling());
                if (scrollDirection != 0)
                {
                    HandleScaling(scrollDirection > 0, modifier);
                }
            }            

            //Mirror with key
            if (GetKeyEnabled(BPXConfiguration.GetMirrorKey(), enable, BPXConfiguration.MirrorRequiresEnableKey()))
            {
                HandleMirroring();
            }

            //Clipboard
            if (GetKeyEnabled(BPXConfiguration.GetClipboardCopyKey(), enable, BPXConfiguration.ClipboardRequiresEnableKey()))
            {
                HandleClipboard(ClipboardAction.Copy);
            }
            if (GetKeyEnabled(BPXConfiguration.GetClipboardPasteKey(), enable, BPXConfiguration.ClipboardRequiresEnableKey()))
            {
                HandleClipboard(ClipboardAction.Paste);
            }

            //Property Clipboard (This should be ui based or something.
            if(GetKeyEnabled(BPXConfiguration.GetPropertyClipboardPositionKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("position", modifier);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardRotationKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("rotation", modifier);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardScaleKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("scale", modifier);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardOptionsKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("options", modifier);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardPaintsKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("paints", modifier);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardCopyAllKey(), enable, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("all", modifier);
            }

            //Fast Travel
            if (GetKeyEnabled(BPXConfiguration.GetFastTravelKey(), enable, BPXConfiguration.FastTravelRequiresEnableKey()))
            {
                HandleFastTravel();
            }            

            //Axis Cycle
            if (GetKeyEnabled(BPXConfiguration.GetAxisCycleKey(), enable, BPXConfiguration.AxisCycleRequireEnableKey()))
            {
                HandleAxisCycle(modifier);
            }

            //Drag Selection
            if(Input.GetKeyDown(BPXConfiguration.GetDragSelectionKey()) || (BPXConfiguration.DoMMBSelection() && Input.GetMouseButtonDown(2)))
            {
                if(BPXConfiguration.DragSelectionRequiresEnableKey())
                {
                    if(enable)
                    {
                        StartDragSelect();
                    }
                }
                else
                {
                    StartDragSelect();
                }
            }

            if (Input.GetKeyUp(BPXConfiguration.GetDragSelectionKey()) || (BPXConfiguration.DoMMBSelection() && Input.GetMouseButtonUp(2)))
            {
                StopDragSelect();
            }

            HandleDragSelection();

            //Mode toggle
            if (Input.GetKeyDown(BPXConfiguration.ScaleUnitBasedToggleKey()))
            {
                BPXConfiguration.ToggleScaleUnitBased();
            }
        }

        private void HandleMovement(Direction direction, bool modifierKeyState)
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }

            GizmoValues gizmoValues = BPXUIManagement.GetGizmoValues();
            Vector3 moveAxis = Vector3.zero;

            if (modifierKeyState)
            {
                switch (direction)
                {
                    case Direction.Left:
                    case Direction.Right:
                        return;
                    case Direction.Up:
                        moveAxis = Vector3.up;
                        break;
                    case Direction.Down:
                        moveAxis = Vector3.down;
                        break;
                }

                moveAxis *= gizmoValues.Y;
            }
            else
            {
                switch (direction)
                {
                    case Direction.Left:
                        moveAxis = Vector3.left;
                        break;
                    case Direction.Right:
                        moveAxis = Vector3.right;
                        break;
                    case Direction.Up:
                        moveAxis = Vector3.forward;
                        break;
                    case Direction.Down:
                        moveAxis = Vector3.back;
                        break;
                }

                moveAxis *= gizmoValues.XZ;
            }

            Vector3 camDirection = BPXManager.central.cam.cameraTransform.forward;
            Vector3 moveDirection = BPXUtils.WorldSpaceRelativeMovement(camDirection, moveAxis);
            EditorOperations.MoveSelection(central, moveDirection);
        }
        private void HandleRotation(Direction direction, bool modifierKeyState)
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }

            GizmoValues gizmoValues = BPXUIManagement.GetGizmoValues();
            Vector3 rotationAxis = Vector3.zero;
            float amount = 0;

            if (modifierKeyState)
            {
                switch (direction)
                {
                    case Direction.Left:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.up;
                        amount = gizmoValues.R;
                        break;
                    case Direction.Right:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.up;
                        amount = -gizmoValues.R;
                        break;
                    case Direction.Up:
                    case Direction.Down:
                        return;
                }
            }
            else
            {
                switch (direction)
                {
                    case Direction.Left:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.forward;
                        amount = gizmoValues.R;
                        break;
                    case Direction.Right:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.forward;
                        amount = -gizmoValues.R;
                        break;
                    case Direction.Up:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.right;
                        amount = gizmoValues.R;
                        break;
                    case Direction.Down:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.right;
                        amount = -gizmoValues.R;
                        break;
                }
            }

            EditorOperations.RotateSelection(central, rotationAxis, amount);
        }
        private void HandleScaling(bool scaleUp, bool modifierKeyState)
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }

            float gizmoS = BPXUIManagement.GetGizmoValues().S;

            if (BPXConfiguration.ScaleUnitBased())
            {
                float amount = gizmoS;
                if (!scaleUp)
                {
                    amount *= -1;
                }

                Axis axis = BPXUIManagement.GetGizmo().GetCurrent();

                if (modifierKeyState)
                {
                    EditorOperations.ScaleSelection(central, axis, amount, ScalingStyle.UnitInPlace);
                }
                else
                {
                    EditorOperations.ScaleSelection(central, axis, amount, ScalingStyle.Unit);
                }
            }
            else
            {
                float amount = gizmoS;

                if (scaleUp)
                {
                    amount = amount / 100f + 1f;
                }
                else
                {
                    amount = 1f / (1f + amount / 100f);
                }

                Axis axis = BPXUIManagement.GetGizmo().GetCurrent();

                if (modifierKeyState)
                {
                    EditorOperations.ScaleSelection(central, axis, amount, ScalingStyle.PercentageInPlace);
                }
                else
                {
                    EditorOperations.ScaleSelection(central, axis, amount, ScalingStyle.Percentage);
                }
            }
        }
        private void HandleMirroring()
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }
            Axis axis = BPXUIManagement.GetGizmo().GetCurrent();
            EditorOperations.MirrorSelection(central, axis);
        }
        private void HandleClipboard(ClipboardAction action)
        {
            if(action == ClipboardAction.Copy)
            {
                if (!EditorOperations.AnyObjectsSelected(central)) { Plugin.Instance.LogScreenErrorMessage("No objects selected to copy to clipboard"); return; }
                List<BlockProperties> selected = EditorOperations.GetSelectedBlocks(central);
                BPXManager.SetClipboard(ZeeplevelHandler.FromEditor(selected, "clipboard", central, central.skybox));
                Plugin.Instance.LogScreenMessage("Copied " + selected.Count + " blocks to clipboard!");
            }
            else if(action == ClipboardAction.Paste)
            {
                ZeeplevelData clipboard = BPXManager.GetClipboard();
                if (clipboard == null)
                {
                    Plugin.Instance.LogScreenErrorMessage("No objects in clipboard");
                    return;
                }

                List<BlockProperties> pasted = ZeeplevelHandler.LoadIntoEditor(clipboard, central, true);
                Plugin.Instance.LogScreenMessage("Pasted " + pasted.Count + " blocks!");
                if (BPXConfiguration.PasteClipboardToCamera())
                {
                    Vector3 requiredMove = ToolkitUtils.BlocksAtCameraGridMovement(central, pasted);
                    EditorOperations.Move(central, pasted, requiredMove);
                }
            }           
        }

        private void HandlePropertyClipboard(string propertyName, bool modifierKeyState)
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }

            bool isCopy = !modifierKeyState;
            bool pos = propertyName == "position" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardPositionIncluded());
            bool rot = propertyName == "rotation" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardRotationIncluded());
            bool scale = propertyName == "scale" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardScaleIncluded());
            bool options = propertyName == "options" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardOptionsIncluded());
            bool paints = propertyName == "paints" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardPaintsIncluded());

            List<string> messageList = new List<string>();

            if (isCopy)
            {
                //Get the first object in the selection
                BlockProperties firstObject = central.selection.list[0];
                if (pos)
                {
                    messageList.Add("Position");
                    BPXManager.positionClipboard = firstObject.transform.position;
                }

                if (rot)
                {
                    messageList.Add("Rotation");
                    BPXManager.rotationClipboard = firstObject.transform.eulerAngles;
                }

                if (scale)
                {
                    messageList.Add("Scale");
                    BPXManager.scaleClipboard = firstObject.transform.localScale;
                }

                if (options)
                {
                    messageList.Add("Options");
                    BPXManager.optionsClipboard = firstObject.properties.Skip(Math.Max(0, firstObject.properties.Count - 11)).ToList();
                }

                if (paints)
                {
                    messageList.Add("Paints");
                    BPXManager.paintsClipboard = firstObject.properties.Skip(9).Take(17).ToList();
                }

                if(messageList.Count > 0)
                {
                    Plugin.Instance.LogScreenMessage($"Copied: {string.Join(", ", messageList)}");
                }
                
            }
            //Paste
            else
            {
                pos = (pos && BPXManager.positionClipboard != null);
                rot = (rot && BPXManager.rotationClipboard != null);
                scale = (scale && BPXManager.scaleClipboard != null);
                options = (options && BPXManager.optionsClipboard != null);
                paints = (paints && BPXManager.paintsClipboard != null);

                //Go over all the blocks in the selection and apply the desired properties.
                List<BlockProperties> blockList = EditorOperations.GetSelectedBlocks(central);
                UndoRedoRegistration registration = new UndoRedoRegistration(central);
                registration.SetBefore(blockList);

                //Apply changes to each block.
                foreach (BlockProperties bp in blockList)
                {
                    if(pos)
                    {
                        bp.transform.position = BPXManager.positionClipboard;
                    }

                    if(rot)
                    {
                        bp.transform.eulerAngles = BPXManager.rotationClipboard;
                    }

                    if(scale)
                    {
                        bp.transform.localScale = BPXManager.scaleClipboard;
                    }

                    if(options)
                    {
                        // Set the last 11 values from options
                        for (int i = 0; i < 11; i++)
                        {
                            bp.properties[bp.properties.Count - 11 + i] = BPXManager.optionsClipboard[i];
                        }
                    }

                    if(paints)
                    {
                        // Set 17 values starting from index 9
                        for (int i = 0; i < 17; i++)
                        {
                            bp.properties[9 + i] = BPXManager.paintsClipboard[i];
                        }
                    }

                    if (pos || rot || scale)
                    {
                        bp.SomethingChanged();
                    }   
                    
                    if(options || paints)
                    {
                        bp.SpreadProperties();                        
                    }

                    bp.LoadProperties();
                    central.selection.SelectionPaint(bp);
                }

                registration.GenerateAfter();

                Change_Collection collection = registration.CreateCollection();
                central.validation.BreakLock(collection, "Gizmo1");

                if (pos)
                {
                    messageList.Add("Position");
                }

                if (rot)
                {
                    messageList.Add("Rotation");
                }

                if (scale)
                {
                    messageList.Add("Scale");
                }

                if (options)
                {
                    messageList.Add("Options");
                }

                if (paints)
                {
                    messageList.Add("Paints");
                }

                if (messageList.Count > 0)
                {
                    Plugin.Instance.LogScreenMessage($"Pasted: {string.Join(", ", messageList)}");
                }
            }            
        }

        private void HandleFastTravel()
        {
            if (!EditorOperations.AnyObjectsSelected(central)) { return; }

            // Calculate the new position with the offset
            Vector3 offsetPosition = central.gizmos.motherGizmo.transform.position
                                     - central.cam.cameraTransform.forward * 16;

            // Set the camera position to the new position with offset
            central.cam.transform.position = offsetPosition;
        }

        private void HandleMoveSpeedSelection()
        {
            if (EditorOperations.AnyObjectsSelected(central)) { return; }

            //1 if up, -1 if down.
            int scroll = GetScrollDirection(true, false, false);
            int index = BPXConfiguration.currentMoveSpeedIndex;
            int i2 = index;

            if (scroll < 0)
            {
                index--;
                if(index < 0)
                {
                    index = 0;
                }

            }
            else if(scroll > 0)
            {
                index++;
                if(index >= BPXConfiguration.moveSpeedMultipliers.Length)
                {
                    index = BPXConfiguration.moveSpeedMultipliers.Length - 1;
                }
            }

            //Has changed
            if(i2 != index)
            {
                BPXConfiguration.currentMoveSpeedIndex = index;
                BPXConfiguration.currentMoveSpeed = BPXConfiguration.baseMoveSpeed * BPXConfiguration.moveSpeedMultipliers[BPXConfiguration.currentMoveSpeedIndex];
                central.cam.moveSpeed = BPXConfiguration.currentMoveSpeed;
                Plugin.Instance.LogScreenMessage("Move speed: " + BPXConfiguration.moveSpeedMultiplierNames[BPXConfiguration.currentMoveSpeedIndex]);
            }            
        }

        private void HandleAxisCycle(bool modifierKeyState)
        {
            BPXUIManagement.GetGizmo().Cycle(!modifierKeyState, BPXConfiguration.IncludePlanesInCycle());
        }

        private void OnGUI()
        {
            if (dragBox != null)
            {
                BPXUtils.DrawScreenRect(dragBox, new Color(1.0f, 0.568f, 0f, 0.2f));
                BPXUtils.DrawScreenRectBorder(dragBox, 1, new Color(1.0f, 0.568f, 0f));
            }
        }

        private void HandleDragSelection()
        {
            if(isDragging)
            {
                if(!isDraggingTeamX)
                {
                    dragBox = BPXUtils.GetScreenRect(dragStartPosition, Input.mousePosition);

                    foreach (KeyValuePair<Vector3, BlockProperties> bp in bpPositionMap)
                    {
                        if (dragBox.Contains((Vector2)bp.Key))
                        {
                            if (!BPXManager.central.selection.list.Contains(bp.Value))
                            {
                                BPXManager.central.selection.AddThisBlock(bp.Value);
                            }
                        }
                        else
                        {
                            if (BPXManager.central.selection.list.Contains(bp.Value))
                            {
                                int index = BPXManager.central.selection.list.IndexOf(bp.Value);
                                BPXManager.central.selection.RemoveBlockAt(index, false, false);
                            }
                        }
                    }
                }     
                else
                {
                    dragBox = BPXUtils.GetScreenRect(dragStartPosition, Input.mousePosition);

                    foreach (KeyValuePair<Vector3, BlockProperties> bp in bpPositionMap)
                    {
                        if (dragBox.Contains((Vector2)bp.Key))
                        {
                            if (!selectionTargets.Contains(bp.Value))
                            {
                                selectionTargets.Add(bp.Value);

                                //Set the selection color
                                BPXManager.central.selection.SelectionPaint(bp.Value);
                            }
                        }
                        else
                        {
                            if (selectionTargets.Contains(bp.Value))
                            {
                                int index = selectionTargets.IndexOf(bp.Value);
                                selectionTargets.RemoveAt(index);

                                //Reset to normal color
                                BPXManager.central.selection.RestorePaint(bp.Value);
                            }
                        }
                    }
                }
            }
        }

        private void StartDragSelect()
        {
            FillDragPositionMap();
            dragStartPosition = Input.mousePosition;
            isDragging = true;
            isDraggingTeamX = TeamXMessaging.IsTeamXEditor();

            //Debug.LogWarning("TeamX dragging: " + isDraggingTeamX);

            EditorOperations.DeselectAllBlocks(central);
            beforeSelection = BPXManager.central.undoRedo.ConvertSelectionToStringList(BPXManager.central.selection.list);
            selectionTargets.Clear();
        }

        private void StopDragSelect()
        {
            isDragging = false;
            dragBox = new Rect();
            bpPositionMap.Clear();

            if(isDraggingTeamX)
            {
                //Go over all the selection target, restore their paint back to normal. Then start selecting everything all at once.
                foreach (BlockProperties bp in selectionTargets)
                {
                    BPXManager.central.selection.RestorePaint(bp);
                }

                foreach (BlockProperties bp in selectionTargets)
                {
                    if (!BPXManager.central.selection.list.Contains(bp))
                    {
                        BPXManager.central.selection.AddThisBlock(bp);
                    }
                }
            }

            isDraggingTeamX = false;
            List<string> afterSelection = BPXManager.central.undoRedo.ConvertSelectionToStringList(BPXManager.central.selection.list);
            BPXManager.central.selection.RegisterManualSelectionBreakLock(beforeSelection, afterSelection);
        }

        private void FillDragPositionMap()
        {
            bpPositionMap.Clear();
            GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found");
                return;
            }

            tempCounter = 0;
            foreach(GameObject obj in allObjects)
            {
                BlockProperties bp = obj.GetComponent<BlockProperties>();
                if(bp != null)
                {
                    tempDragVector = mainCamera.WorldToScreenPoint(obj.transform.position);

                    if(tempDragVector.z >= 0)
                    {
                        tempDragVector.y = Screen.height - tempDragVector.y;
                        tempDragVector.z = tempCounter;
                        bpPositionMap.Add(tempDragVector, bp);
                        tempCounter++;
                    }
                }
            }
        }
    }
}
