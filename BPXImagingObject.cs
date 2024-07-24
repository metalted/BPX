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

            Transform background = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
            background.parent = cameraTransform;
            background.localPosition = new Vector3(0, 0, 128);
            background.Rotate(-90, 0, 0);

            Material backgroundMaterial = new Material(Shader.Find("Unlit/Color"));
            backgroundMaterial.color = Color.black;
            background.gameObject.GetComponent<Renderer>().material = backgroundMaterial;
            background.localScale = new Vector3(75f, 75f, 75f);

            subjectPivot = new GameObject("Subject Pivot").transform;
            subjectPivot.parent = transform;
            subjectPivot.localPosition = Vector3.zero;

            subjectHolder = new GameObject("Subject Holder").transform;
            subjectHolder.parent = subjectPivot;
            subjectHolder.localPosition = Vector3.zero;

            transform.position = new Vector3(0, 20000, 0);

            cameraTransform.localPosition = new Vector3(20, 20, -20);
            cameraTransform.LookAt(subjectPivot);

            Disable();

            init = true;
        }

        public void CaptureSubject(int imageSize, ZeeplevelFile zeeplevelFile, UnityAction<List<Texture2D>> callback)
        {
            Enable();

            renderTexture.Release();
            renderTexture.width = imageSize;
            renderTexture.height = imageSize;
            renderTexture.Create();

            StartCoroutine(CaptureRoutine(zeeplevelFile, callback));
        }

        private IEnumerator CaptureRoutine(ZeeplevelFile zeeplevelFile, UnityAction<List<Texture2D>> callback)
        {
            Reset();
            LoadSubject(zeeplevelFile);

            yield return new WaitForEndOfFrame();

            List<Texture2D> captures = Capture();

            RemoveSubject();

            callback?.Invoke(captures);

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

                if (id < 0 || id >= PlayerManager.Instance.loader.globalBlockList.blocks.Count)
                {
                    continue;
                }

                BlockPropertyJSON blockPropertyJSON = BPXUtils.ZeeplevelBlockToBlockPropertyJSON(zeeplevelFile.Blocks[i]);

                BlockProperties bp = GameObject.Instantiate<BlockProperties>(BPXManager.central.manager.loader.globalBlockList.blocks[id]);
                bp.CreateBlock();
                bp.properties.Clear();
                bp.isEditor = true;
                bp.LoadProperties_v15(blockPropertyJSON, false);
                bp.isLoading = false;

                GameObject bpObj = bp.gameObject;
                bpObj.transform.parent = subjectHolder;
                GameObject.Destroy(bp);
                objects.Add(bpObj);
            }

            Bounds bounds = BPXUtils.CalculateBounds(objects);

            float scaleFactor = 64f / bounds.size.magnitude;
            subjectHolder.transform.localScale = Vector3.one * scaleFactor;

            Vector3 move = (subjectPivot.position - bounds.center) * scaleFactor;
            subjectHolder.localPosition += move;

            captureCamera.orthographicSize = bounds.size.magnitude * 0.5f * scaleFactor;
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

            subjectHolder = new GameObject("Subject Holder").transform;
            subjectHolder.parent = subjectPivot;
            subjectHolder.localPosition = Vector3.zero;
        }
    }
}
