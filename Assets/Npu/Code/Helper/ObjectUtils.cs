using System.Collections.Generic;
using System.Linq;
using Npu.EditorSupport;
using Npu.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

#endif

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Helper
{
    public static class ObjectUtils
    {
        #region FIND OBJECTS

        /// <summary>
        /// Get all assets of type <typeparamref name="T"/> in the current scene and/or in all assets. The value string is the location of the object: scene - or - asset
        /// </summary>
        public static List<KeyValuePair<T, string>> GetObjectsOfType<T>(bool scene = true, bool assets = true) where T : Object
        {
            var l = new List<KeyValuePair<T, string>>();
            if (scene)
            {
                foreach (var t0 in Object.FindObjectsOfType<T>())
                {
                    l.Add(new KeyValuePair<T, string>(t0, "scene"));
                }
            }
#if UNITY_EDITOR
            if (assets)
            {
                foreach (var t0 in AssetUtils.FindAllAssets<T>())
                {
                    l.Add(new KeyValuePair<T, string>(t0, "asset"));
                }
            }
#endif
            return l;
        }

        /// <summary>
        /// Get the first object of type T in the active scene, including inactive
        /// </summary>
        public static T GetSceneObjectOfType<T>()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene != null)
            {
                var rootObjs = scene.GetRootGameObjects();
                var obj = rootObjs.Select(ro => ro.GetComponentInChildren<T>(true)).FirstOrDefault();
                return obj;
            }
            return default;
        }

        /// <summary>
        /// Get scene objects of type T in the active scene, including inactive
        /// </summary>
        public static IEnumerable<T> GetSceneObjectsOfType<T>()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene != null)
            {
                var rootObjs = scene.GetRootGameObjects();
                var objs = rootObjs.SelectMany(ro => ro.GetComponentsInChildren<T>(true));
                return objs;
            }
            return default;
        }

        #endregion

        #region BOUNDS

        public static Bounds GetRendererBounds(GameObject go, bool includeInactive = true)
        {
            return GetBounds<Renderer>(go, includeInactive);
        }

        public static Bounds GetColliderBounds(GameObject go, bool includeInactive = true)
        {
            return GetBounds<Collider>(go, includeInactive);
        }

        public static Bounds GetCollider2DBounds(GameObject go, bool includeInactive = true)
        {
            return GetBounds<Collider2D>(go, includeInactive);
        }

        public static Bounds GetBounds<T>(GameObject go, bool includeInactive = true, System.Func<T, Bounds> getBounds = null) where T : Component
        {
            if (getBounds == null) getBounds = (t) => (t as Collider)?.bounds ?? (t as Collider2D)?.bounds ?? (t as Renderer)?.bounds ?? default;
            var comps = go.GetComponentsInChildren<T>(includeInactive);

            return Math3DUtils.UnionBounds(comps.Where(comp =>
            {
                if (!comp) return false;
                if (comp is ParticleSystemRenderer) return false;
                if (!includeInactive)
                {
                    if (!(comp as Collider)?.enabled ?? false) return false;
                    if (!(comp as Collider2D)?.enabled ?? false) return false;
                    if (!(comp as Renderer)?.enabled ?? false) return false;
                    if (!(comp as MonoBehaviour)?.enabled ?? false) return false;
                }
                return true;
            }).Select(getBounds));
        }

        public static Bounds GetBounds<T>(IEnumerable<GameObject> gos, bool includeInactive = true, System.Func<T, Bounds> getBounds = null) where T : Component
        {
            return Math3DUtils.UnionBounds(gos.Where(go => go).Select(go => GetBounds<T>(go, includeInactive, getBounds)));
        }

        public static Bounds GetAccurateLocalMeshBounds(GameObject go, bool includeInactive = true)
        {
            var localBoundsCorners = go.GetComponentsInChildren<Renderer>(includeInactive).SelectMany(r =>
            {
                Bounds bounds = default;
                Transform transform = default;
                if (r is SkinnedMeshRenderer smr)
                {
                    bounds = smr.bounds;
                    transform = smr.transform;
                }
                else if (r is MeshRenderer mr && mr.GetComponent<MeshFilter>() is MeshFilter mf && mf && mf.sharedMesh)
                {
                    bounds = mf.sharedMesh.bounds;
                    transform = mf.transform;
                }

                if (transform) return bounds.GetCorners().Select(c => go.transform.InverseTransformPoint(transform.TransformPoint(c)));
                else return Enumerable.Empty<Vector3>();
            });
            return Math3DUtils.GetBounds(localBoundsCorners);
        }

        #endregion

        #region OBJECT
        /// <summary>
        /// Instantiate object and connect prefab if possible
        /// </summary>
        public static T Instantiate<T>(T prefab, Transform parent, bool connectPrefab = true) where T : Object
        {
#if UNITY_EDITOR
            if (!connectPrefab || Application.isPlaying || PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.NotAPrefab)
                return Object.Instantiate(prefab, parent);
            return PrefabUtility.InstantiatePrefab(prefab, parent) as T;
#else
            return Object.Instantiate(prefab, parent);
#endif
        }

        public static GameObject GetGameObject<T>(T obj)
        {
            if (obj is GameObject go) return go;
            else if (obj is Component comp) return comp.gameObject;
            return default;
        }

        #endregion

        /// <summary>
        /// Distribute objects using their renderer bounds among X and Y
        /// </summary>
        public static void DistributeObjects<T>(List<T> list, Transform parent, Vector2 spacing, float minRowX = 0) where T : Component
        {
            var sumLengthX = list.Sum(item => GetRendererBounds(item.gameObject).size.x + spacing.x);
            var rowX = Mathf.Max(minRowX, Mathf.Pow(sumLengthX, 0.5f));
            float usedX = 0;
            float usedY = 0;
            float thisRowMaxY = 0;

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var bounds = GetRendererBounds(item.gameObject);
                var size = bounds.size;
                var useUpX = size.x + spacing.x;

                if (usedX + useUpX > rowX)
                {
                    usedX = 0;
                    usedY += thisRowMaxY + spacing.y;
                    thisRowMaxY = 0;
                }

                var botLeftCorner = bounds.min;
                var offset = new Vector3(usedX, usedY) - botLeftCorner;
                item.transform.position += offset;

                usedX += useUpX;
                thisRowMaxY = Mathf.Max(thisRowMaxY, size.y);
            }

            var totalBounds = GetRendererBounds(parent.gameObject);
            var offsetAll = parent.position - totalBounds.center;
            foreach (var item in list)
            {
                item.transform.position += offsetAll;
            }
        }

        /// <summary>
        /// Copy configuration from a RB to another
        /// </summary>
        public static void CopyRigidbody(Rigidbody from, Rigidbody to)
        {
            to.mass = from.mass;
            to.drag = from.drag;
            to.angularDrag = from.angularDrag;
            to.constraints = from.constraints;
            to.freezeRotation = from.freezeRotation;
            to.useGravity = from.useGravity;
            to.isKinematic = from.isKinematic;
            to.collisionDetectionMode = from.collisionDetectionMode;
        }

        /// <summary>
        /// Get pixels of texture regardless of isReadable
        /// </summary>
        public static Color[] GetTexturePixelsSafe(Texture2D tex)
        {
            if (tex.isReadable) return tex.GetPixels();
            else
            {
                var tmp = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(tex, tmp);
                var previous = RenderTexture.active;
                RenderTexture.active = tmp;
                var myTexture2D = new Texture2D(tex.width, tex.height);
                myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                myTexture2D.Apply();
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(tmp);
                return myTexture2D.GetPixels();
            }
        }

    }
}