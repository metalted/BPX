using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BPX
{
    public class BPXController : MonoBehaviour
    {
        private enum Direction { Up, Down, Left, Right };
        
        private bool AllowRun()
        {
            if(BPXManager.central == null)
            {
                return false;
            }

            //Not in building mode.
            if (BPXManager.central.tool.currentTool != 0)
            {
                return false;
            }

            //If we are currently dragging an object.
            if (BPXManager.central.gizmos.isDragging)
            {
                return false;
            }

            //If we are currently in GMode
            if (BPXManager.central.gizmos.isGrabbing)
            {
                return false;
            }

            //If the regular save panel is open we don't want to process any inputs.
            if (BPXManager.central.saveload.gameObject.activeSelf)
            {
                return false;
            }            

            //Blueprint panel is open.
            if (BPXUIManagement.IsPanelOpen())
            {
                return false;
            }            

            return true;
        }

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

        public void ResetController()
        {
            if(isDragging)
            {
                BPXManager.DeselectAllBlocks();
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

        public void Start()
        {
            BPXManager.central.cam.moveSpeed = BPXConfiguration.currentMoveSpeed;
        }

        public void Update()
        {
            if (!AllowRun()) { return; }

            bool enableKeyState = Input.GetKey(BPXConfiguration.GetEnableKey());
            bool modifierKeyState = Input.GetKey(BPXConfiguration.GetModifierKey());

            //Save shortcut
            if (GetKeyEnabled(BPXConfiguration.GetSaveShortcutKey(), enableKeyState, BPXConfiguration.ShortcutRequiresEnableKey()))
            {
                BPXUIManagement.PressButtonByName("Save");
            }

            //Load shortcut
            if (GetKeyEnabled(BPXConfiguration.GetLoadShortcutKey(), enableKeyState, BPXConfiguration.ShortcutRequiresEnableKey()))
            {
                BPXUIManagement.PressButtonByName("Load");
            }

            //Scale down with key
            if (GetKeyEnabled(BPXConfiguration.GetNegativeScalingKey(), enableKeyState, BPXConfiguration.ScalingRequiresEnableKey()))
            {
                HandleScaling(false, modifierKeyState);
            }
            //Scale up with key
            if (GetKeyEnabled(BPXConfiguration.GetPositiveScalingKey(), enableKeyState, BPXConfiguration.ScalingRequiresEnableKey()))
            {
                HandleScaling(true, modifierKeyState);
            }
            //Scale with scroll
            if (BPXConfiguration.DoScrollScaling())
            {
                int scrollDirection = GetScrollDirection(enableKeyState, BPXConfiguration.ScalingRequiresEnableKey(), BPXConfiguration.InvertScrollScaling());
                if (scrollDirection != 0)
                {
                    HandleScaling(scrollDirection > 0, modifierKeyState);
                }
            }

            //Move with keys
            if (
                (BPXManager.InMovementMode()) ||
                (BPXManager.InRotateMode() && !BPXConfiguration.KeyRotationIsEnabled() && BPXConfiguration.MovementIfRotationIsDisabled())) {

                //Key movement (up)
                if (GetKeyEnabled(BPXConfiguration.GetForwardUpMovementKey(), enableKeyState, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Up, modifierKeyState);
                }
                //Key movement (down)
                if (GetKeyEnabled(BPXConfiguration.GetBackDownMovementKey(), enableKeyState, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Down, modifierKeyState);
                }
                //Key movement (left)
                if (GetKeyEnabled(BPXConfiguration.GetLeftMovementKey(), enableKeyState, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Left, modifierKeyState);
                }
                //Key movement (right)
                if (GetKeyEnabled(BPXConfiguration.GetRightMovementKey(), enableKeyState, BPXConfiguration.MovementRequiresEnableKey()))
                {
                    HandleMovement(Direction.Right, modifierKeyState);
                }
            }

            //Rotate with key
            if (BPXManager.InRotateMode())
            {
                //Key rotation (up)
                if (GetKeyEnabled(BPXConfiguration.GetXPositiveRotationKey(), enableKeyState, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Up, modifierKeyState);
                }
                //Key rotation (down)
                if (GetKeyEnabled(BPXConfiguration.GetXNegativeRotationKey(), enableKeyState, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Down, modifierKeyState);
                }
                //Key rotation (left)
                if (GetKeyEnabled(BPXConfiguration.GetYZNegativeRotationKey(), enableKeyState, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Left, modifierKeyState);
                }
                //Key rotation (right)
                if (GetKeyEnabled(BPXConfiguration.GetYZPositiveRotationKey(), enableKeyState, BPXConfiguration.RotationRequiresEnableKey()))
                {
                    HandleRotation(Direction.Right, modifierKeyState);
                }
            }

            //Mirror with key
            if (GetKeyEnabled(BPXConfiguration.GetMirrorKey(), enableKeyState, BPXConfiguration.MirrorRequiresEnableKey()))
            {
                HandleMirroring();
            }

            //Clipboard
            if (GetKeyEnabled(BPXConfiguration.GetClipboardCopyKey(), enableKeyState, BPXConfiguration.ClipboardRequiresEnableKey()))
            {
                HandleClipboard(true);
            }
            if (GetKeyEnabled(BPXConfiguration.GetClipboardPasteKey(), enableKeyState, BPXConfiguration.ClipboardRequiresEnableKey()))
            {
                HandleClipboard(false);
            }

            //Property Clipboard
            if(GetKeyEnabled(BPXConfiguration.GetPropertyClipboardPositionKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("position", modifierKeyState);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardRotationKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("rotation", modifierKeyState);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardScaleKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("scale", modifierKeyState);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardOptionsKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("options", modifierKeyState);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardPaintsKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("paints", modifierKeyState);
            }
            if (GetKeyEnabled(BPXConfiguration.GetPropertyClipboardCopyAllKey(), enableKeyState, BPXConfiguration.PropertyClipboardRequiresEnableKey()))
            {
                HandlePropertyClipboard("all", modifierKeyState);
            }

            //Fast Travel
            if (GetKeyEnabled(BPXConfiguration.GetFastTravelKey(), enableKeyState, BPXConfiguration.FastTravelRequiresEnableKey()))
            {
                HandleFastTravel();
            }

            //Move speed selection
            if (GetKeyContinuouslyEnabled(BPXConfiguration.GetFastTravelKey(), enableKeyState, BPXConfiguration.FastTravelRequiresEnableKey()))
            {
                HandleMoveSpeedSelection();
            }

            //Axis Cycle
            if (GetKeyEnabled(BPXConfiguration.GetAxisCycleKey(), enableKeyState, BPXConfiguration.AxisCycleRequireEnableKey()))
            {
                HandleAxisCycle(modifierKeyState);
            }

            //Drag Selection
            if(Input.GetKeyDown(BPXConfiguration.GetDragSelectionKey()) || (BPXConfiguration.DoMMBSelection() && Input.GetMouseButtonDown(2)))
            {
                if(BPXConfiguration.DragSelectionRequiresEnableKey())
                {
                    if(enableKeyState)
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
        }        

        private void HandleScaling(bool scaleUp, bool modifierKeyState)
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }
            
            float amount = BPXUIManagement.GetGizmoValues().S;

            if(scaleUp)
            {
                amount = amount / 100f + 1f;
            }
            else
            {
                amount = 1f / (1f + amount / 100f);
            }

            Vector3 axis = BPXUIManagement.GetGizmo().GetCurrentAxes();

            if(modifierKeyState)
            {
                BPXOperations.ScaleInPlace(BPXManager.GetSelectedBlocks(), axis, amount);
            }
            else
            {
                BPXOperations.Scale(BPXManager.GetSelectedBlocks(), axis, amount);
            }
            
        }

        private void HandleMovement(Direction direction, bool modifierKeyState)
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }

            GizmoValues gizmoValues = BPXUIManagement.GetGizmoValues();
            Vector3 moveAxis = Vector3.zero;
            
            if (modifierKeyState)
            {
                switch(direction)
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
                switch(direction)
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

            BPXOperations.Move(BPXManager.GetSelectedBlocks(), moveDirection);            
        }

        private void HandleRotation(Direction direction, bool modifierKeyState)
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }

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

            BPXOperations.RotateSelection(rotationAxis, amount);
        }

        private void HandleMirroring()
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }

            Vector3 axis = BPXUIManagement.GetGizmo().GetCurrentAxes();

            if(axis[0] != 0)
            {
                axis = Vector3.right;
            }
            else if(axis[1] != 0)
            {
                axis = Vector3.up;
            }
            else if(axis[2] != 0)
            {
                axis = Vector3.forward;
            }
            else
            {
                axis = Vector3.right;
            }

            BPXOperations.Mirror(BPXManager.GetSelectedBlocks(), axis);
        }

        private void HandleClipboard(bool isCopyNotPaste)
        {
            if(isCopyNotPaste)
            {
                if (!BPXManager.AnyObjectsSelected()) {

                    Plugin.Instance.LogScreenMessage("No objects selected to copy to clipboard");
                    return; 
                }

                ZeeplevelFile copy = ZeeplevelHandler.FromBlockProperties(BPXManager.GetSelectedBlocks());
                Plugin.Instance.LogScreenMessage("Copied to clipboard");
                BPXManager.SetClipboard(copy);
            }
            else
            {
                ZeeplevelFile clipboard = BPXManager.GetClipboard();

                if(clipboard != null)
                {
                    ZeeplevelHandler.InstantiateBlueprintIntoEditor(clipboard, BPXConfiguration.PasteClipboardToCamera());
                }
                else
                {
                    Plugin.Instance.LogScreenMessage("No objects in clipboard");
                }
            }           
        }

        private void HandlePropertyClipboard(string propertyName, bool modifierKeyState)
        {
            //Debug.LogWarning($"Property: {propertyName}");
            //Debug.LogWarning($"Mod: {modifierKeyState}");

            //If nothing is selected abort early.
            if (!BPXManager.AnyObjectsSelected()) { return; }

            bool isCopy = !modifierKeyState;
            bool pos = propertyName == "position" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardPositionIncluded());
            bool rot = propertyName == "rotation" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardRotationIncluded());
            bool scale = propertyName == "scale" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardScaleIncluded());
            bool options = propertyName == "options" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardOptionsIncluded());
            bool paints = propertyName == "paints" || (propertyName == "all" && BPXConfiguration.IsPropertyClipboardPaintsIncluded());

            List<string> messageList = new List<string>();

            if (isCopy)
            {
                //Debug.LogWarning($"isCopy:{isCopy}; pos: {pos}; rot: {rot}; scale: {scale}; options: {options}; paints: {paints}");
                
                //Get the first object in the selection
                BlockProperties firstObject = BPXManager.central.selection.list[0];
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
                    PlayerManager.Instance.messenger.Log($"Copied: {string.Join(", ", messageList)}", 2f);
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

                //Debug.LogWarning($"isCopy:{isCopy}; pos: {pos}; rot: {rot}; scale: {scale}; options: {options}; paints: {paints}");

                //Go over all the blocks in the selection and apply the desired properties.
                List<BlockProperties> blockList = BPXManager.GetSelectedBlocks();
                BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
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
                    BPXManager.central.selection.SelectionPaint(bp);
                }

                registration.GenerateAfter();

                Change_Collection collection = registration.CreateCollection();
                BPXManager.central.validation.BreakLock(collection, "Gizmo1");

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
                    PlayerManager.Instance.messenger.Log($"Pasted: {string.Join(", ", messageList)}", 2f);
                }
            }            
        }

        private void HandleFastTravel()
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }

            // Calculate the new position with the offset
            Vector3 offsetPosition = BPXManager.central.gizmos.motherGizmo.transform.position
                                     - BPXManager.central.cam.cameraTransform.forward * 16;

            // Set the camera position to the new position with offset
            BPXManager.central.cam.transform.position = offsetPosition;
        }

        private void HandleMoveSpeedSelection()
        {
            if (BPXManager.AnyObjectsSelected()) { return; }

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
                BPXManager.central.cam.moveSpeed = BPXConfiguration.currentMoveSpeed;
                PlayerManager.Instance.messenger.Log("Move speed: " + BPXConfiguration.moveSpeedMultiplierNames[BPXConfiguration.currentMoveSpeedIndex], 1f);
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

        private Dictionary<Vector3, BlockProperties> bpPositionMap = new Dictionary<Vector3, BlockProperties>();
        private List<BlockProperties> selectionTargets = new List<BlockProperties>();
        private Vector2 dragStartPosition;
        private bool isDragging;
        private bool isDraggingTeamX;
        private Rect dragBox;
        private List<string> beforeSelection;
        private Vector3 tempDragVector;
        private int tempCounter;

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

            BPXManager.DeselectAllBlocks();
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
