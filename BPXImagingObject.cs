using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BPX
{
    public class BPXImagingObject : MonoBehaviour
    {
        private bool init = false;
        private Transform cameraTransform;
        private Camera captureCamera;
        private RenderTexture renderTexture;
        private Transform subjectPivot;
        private Transform subjectHolder;
        private List<GameObject> objects = new List<GameObject>();
        private Dictionary<string, UnityAction<List<Texture2D>>> callbackDictionary = new Dictionary<string, UnityAction<List<Texture2D>>>();
        public float orthoFactor = 0.5f;

        public void Initialize()
        {
            if (init) { return; }

            cameraTransform = new GameObject("Camera Container").transform;
            cameraTransform.parent = transform;

            captureCamera = cameraTransform.gameObject.AddComponent<Camera>();
            captureCamera.orthographic = true;

            renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            captureCamera.targetTexture = renderTexture;

            // Create the background, position it and child it to the camera.
            Transform background = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
            background.parent = cameraTransform;
            background.localPosition = new Vector3(0, 0, 128);
            background.Rotate(-90, 0, 0);

            // Create the background plane and set its color.
            Material backgroundMaterial = new Material(Shader.Find("Unlit/Color"));
            backgroundMaterial.color = Color.black;
            background.gameObject.GetComponent<Renderer>().material = backgroundMaterial;
            background.localScale = new Vector3(75f, 75f, 75f);

            // Create the subject pivot
            subjectPivot = new GameObject("Subject Pivot").transform;
            subjectPivot.parent = transform;
            subjectPivot.localPosition = Vector3.zero;

            // Create the subject holder
            subjectHolder = new GameObject("Subject Holder").transform;
            subjectHolder.parent = subjectPivot;
            subjectHolder.localPosition = Vector3.zero;

            // Move somewhere out of sight
            transform.position = new Vector3(0, 20000, 0);

            cameraTransform.localPosition = new Vector3(20, 20, -20);
            cameraTransform.LookAt(subjectPivot);

            Disable();

            init = true;
        }

        public void CaptureSubject(int imageSize, ZeeplevelFile zeeplevelFile, string tag, UnityAction<List<Texture2D>> callback)
        {
            Enable();

            renderTexture.Release();
            renderTexture.width = imageSize;
            renderTexture.height = imageSize;
            renderTexture.Create();

            if (callbackDictionary.ContainsKey(tag))
            {
                callbackDictionary[tag] = callback;
            }
            else
            {
                callbackDictionary.Add(tag, callback);
            }

            StartCoroutine(CaptureRoutine(zeeplevelFile, tag));
        }

        public IEnumerator CaptureRoutine(ZeeplevelFile zeeplevelFile, string tag)
        {
            Reset();
            LoadSubject(zeeplevelFile);

            yield return new WaitForEndOfFrame();

            List<Texture2D> captures = Capture();

            RemoveSubject();

            if (callbackDictionary.ContainsKey(tag))
            {
                callbackDictionary[tag]?.Invoke(captures);
                callbackDictionary.Remove(tag);
            }

            Disable();
        }

        private void Reset()
        {
            if (objects.Count > 0)
            {
                foreach (GameObject o in objects)
                {
                    if (o != null)
                    {
                        GameObject.Destroy(o);
                    }
                }
            }
            objects.Clear();

            subjectHolder.localPosition = Vector3.zero;
            subjectPivot.localRotation = Quaternion.identity;
        }

        private void Enable()
        {
            transform.gameObject.SetActive(true);
        }

        private void Disable()
        {
            transform.gameObject.SetActive(false);
        }

        private void LoadSubject(ZeeplevelFile zeeplevelFile)
        {
            for (int i = 0; i < zeeplevelFile.Blocks.Count; i++)
            {
                int id = zeeplevelFile.Blocks[i].BlockID;

                // Skip invalid block ids
                if (id < 0 || id >= PlayerManager.Instance.loader.globalBlockList.blocks.Count)
                {
                    continue;
                }

                // Create a blockPropertyJSON of the ZeeplevelBlock
                BlockPropertyJSON blockPropertyJSON = BPXUtils.ZeeplevelBlockToBlockPropertyJSON(zeeplevelFile.Blocks[i]);

                // Instantiate the block
                BlockProperties bp = GameObject.Instantiate<BlockProperties>(BPXManager.central.manager.loader.globalBlockList.blocks[id]);
                bp.CreateBlock();
                bp.properties.Clear();
                bp.isEditor = true;
                bp.LoadProperties_v15(blockPropertyJSON, false);
                bp.isLoading = false;

                // Get the object and remove the block properties
                GameObject bpObj = bp.gameObject;
                bpObj.transform.parent = subjectHolder;
                GameObject.Destroy(bp);
                objects.Add(bpObj);
            }

            Bounds bounds = BPXUtils.CalculateBounds(objects);

            // Scale the subject down so it fits inside a 16^3 cube
            float scaleFactor = 64f / bounds.size.magnitude;
            subjectHolder.transform.localScale = Vector3.one * scaleFactor;

            // Calculate the move vector applying the scale factor
            Vector3 move = (subjectPivot.position - bounds.center) * scaleFactor;
            subjectHolder.localPosition += move;

            captureCamera.orthographicSize = bounds.size.magnitude * orthoFactor * scaleFactor;
        }

        public List<Texture2D> Capture()
        {
            List<Texture2D> captures = new List<Texture2D>();

            subjectPivot.transform.localEulerAngles = new Vector3(0, -90f, 0);
            
            RenderTexture.active = renderTexture;

            for (int i = 0; i < 4; i++)
            {
                captureCamera.Render();

                Texture2D capture = new Texture2D(renderTexture.width, renderTexture.height);
                capture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                capture.Apply();

                captures.Add(capture);

                subjectPivot.Rotate(0, 90f, 0);
            }

            RenderTexture.active = null;
            return captures;
        }

        public void RemoveSubject()
        {
            GameObject.Destroy(subjectHolder.gameObject);

            // Create new subject holder
            subjectHolder = new GameObject("Subject Holder").transform;
            subjectHolder.parent = subjectPivot;
            subjectHolder.localPosition = Vector3.zero;
        }
    }
}
