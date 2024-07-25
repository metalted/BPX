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

            //Fast Travel
            if (GetKeyEnabled(BPXConfiguration.GetFastTravelKey(), enableKeyState, BPXConfiguration.FastTravelRequiresEnableKey()))
            {
                HandleFastTravel();
            }

            //Axis Cycle
            if(GetKeyEnabled(BPXConfiguration.GetAxisCycleKey(), enableKeyState, BPXConfiguration.AxisCycleRequireEnableKey()))
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

            BPXOperations.Move(BPXManager.GetSelectedBlocks(), moveAxis);            
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
                        amount = -gizmoValues.R;
                        break;
                    case Direction.Down:
                        rotationAxis = BPXManager.central.gizmos.rotationGizmos.transform.right;
                        amount = gizmoValues.R;
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
                if (!BPXManager.AnyObjectsSelected()) { return; }

                ZeeplevelFile copy = ZeeplevelHandler.FromBlockProperties(BPXManager.GetSelectedBlocks());
                BPXManager.SetClipboard(copy);
            }
            else
            {
                ZeeplevelFile clipboard = BPXManager.GetClipboard();

                if(clipboard != null)
                {
                    ZeeplevelHandler.InstantiateBlueprintIntoEditor(clipboard, BPXConfiguration.PasteClipboardToCamera());
                }
            }           
        }

        private void HandleFastTravel()
        {
            if (!BPXManager.AnyObjectsSelected()) { return; }
            BPXManager.central.cam.transform.position = BPXManager.central.gizmos.motherGizmo.transform.position;
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
        private Vector2 dragStartPosition;
        private bool isDragging;
        private Rect dragBox;
        private List<string> beforeSelection;
        private Vector3 tempDragVector;
        private int tempCounter;

        private void HandleDragSelection()
        {
            if(isDragging)
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
        }
        private void StartDragSelect()
        {
            FillDragPositionMap();
            dragStartPosition = Input.mousePosition;
            isDragging = true;
            BPXManager.DeselectAllBlocks();
            beforeSelection = BPXManager.central.undoRedo.ConvertSelectionToStringList(BPXManager.central.selection.list);
        }

        private void StopDragSelect()
        {
            isDragging = false;
            dragBox = new Rect();
            bpPositionMap.Clear();
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
