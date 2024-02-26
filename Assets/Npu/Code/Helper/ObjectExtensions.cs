using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Helper
{
    public static class ObjectExtensions
    {

        #region GameObject

        public static void SetLayer(this GameObject go, int layer)
        {
            go.transform.DoDeep(t => t.gameObject.layer = layer);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (!comp) comp = go.AddComponent<T>();
            return comp;
        }

        #endregion


        #region Transform

        /// <summary>
        /// Set transform to pos 0, rot 0, scale 1
        /// </summary>
        public static void ResetToIdentity(this Transform t, bool withScale = true)
        {
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            if (withScale) t.localScale = Vector3.one;
        }

        /// <summary>
        /// Find deep children with name
        /// </summary>
        public static Transform FindDeep(this Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result) return result;

            foreach (Transform child in aParent)
            {
                result = child.FindDeep(aName);
                if (result) return result;
            }
            return null;
        }

        /// <summary>
        /// Iterate child transforms and invoke actions.
        /// </summary>
        /// <param name="parentFirst">whether the action is invoked on parent first or last children first</param>
        public static void DoDeep(this Transform aParent, System.Action<Transform> action, bool parentFirst = true)
        {
            if (action == null) return;

            if (parentFirst) action(aParent);

            foreach (Transform child in aParent)
            {
                child.DoDeep(action, parentFirst);
            }

            if (!parentFirst) action(aParent);
        }

        /// <summary>
        /// Destroy all children transforms
        /// </summary>
        public static void ClearChildren(this Transform t, bool immediate = false, bool asset = false, System.Func<Transform, bool> pred = null)
        {
            for (var i = t.childCount - 1; i >= 0; i--)
            {
                var child = t.GetChild(i);
                if (pred == null || pred(child))
                {
                    if (immediate) Object.DestroyImmediate(child.gameObject, asset);
                    else Object.Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Destroy all children having certain type <typeparamref name="T"/>
        /// </summary>
        public static void ClearChildrenOfType<T>(this Transform t, bool immediate = false, bool asset = false)
        {
            var ts = t.GetComponentsInChildren<T>(true);
            for (var i = ts.Length - 1; i >= 0; i--)
            {
                if (ts[i] is Component comp && comp.gameObject is GameObject go)
                {
                    if (immediate) Object.DestroyImmediate(go, asset);
                    else Object.Destroy(go);
                }
            }
        }

        /// <summary>
        /// Return a path from <paramref name="parent"/> to <paramref name="child"/> like (Parent/)A/B/C/(Child)
        /// </summary>
        public static string GetPathFrom(this Transform child, Transform parent, bool includeRoot, bool includeChild)
        {
            string s = null;
            if (child && parent && child.IsChildOf(parent))
            {
                var names = new Stack<string>();

                var tt = child;
                while (tt && tt != parent)
                {
                    if (includeChild || tt != child) names.Push(tt.name);
                    tt = tt.parent;
                }
                if (includeRoot) names.Push(parent.name);

                s = string.Join("/", names.ToArray());
            }
            return s;
        }

        /// <summary>
        /// Return number of layers needed to go from parent to child
        /// e.g. A->B->C->D, D.GetDepthFrom(A) returns 3
        /// </summary>
        public static int GetDepthFrom(this Transform child, Transform parent)
        {
            if (child && parent && child.IsChildOf(parent))
            {
                var i = 0;
                var tt = child;
                while (tt && tt != parent)
                {
                    i++;
                    tt = tt.parent;
                }
                return i;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Find a child transform with specified name or create one inside. Default TRS.
        /// </summary>
        public static Transform GetOrCreateChildTransform(this Transform tParent, string name)
        {
            var t = tParent.Find(name);
            if (t) return t;

            var go = new GameObject(name);
            go.transform.SetParent(tParent);
            go.transform.ResetToIdentity();

            return go.transform;
        }

        public static Dictionary<Transform, string> GetChildPathDictionary(this Transform transform, string separator = "/", bool appendRoot = false)
        {
            var dict = new Dictionary<Transform, string>();
            dict.Add(transform, appendRoot ? transform.name : "");
            void doAdd(Transform t)
            {
                if (t == transform || !t.IsChildOf(transform)) return;
                if (!dict.TryGetValue(t.parent, out var parentPath))
                {
                    doAdd(t.parent);
                    dict.TryGetValue(t.parent, out parentPath);
                }
                dict[t] = (string.IsNullOrEmpty(parentPath) ? "" : parentPath + separator) + t.name;
            }
            transform.DoDeep(doAdd, true);
            return dict;
        }

        #endregion


        #region RectTransform

        /// <summary>
        /// Calculate world center position of RectTransform
        /// </summary>
        public static Vector3 GetWorldCenter(this RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return (corners[0] + corners[2]) / 2;
        }

        /// <summary>
        /// Calculate world bounds of RectTransform
        /// </summary>
        public static Bounds GetWorldBounds(this RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;
            foreach (var c in corners)
            {
                minX = Mathf.Min(minX, c.x);
                minY = Mathf.Min(minY, c.y);
                minZ = Mathf.Min(minZ, c.z);
                maxX = Mathf.Max(maxX, c.x);
                maxY = Mathf.Max(maxY, c.y);
                maxZ = Mathf.Max(maxZ, c.z);
            }
            return new Bounds(
                new Vector3((maxX + minX) / 2f, (maxY + minY) / 2f, (maxZ + minZ) / 2f),
                new Vector3((maxX - minX), (maxY - minY), (maxZ - minZ))
            );
        }



        /// <summary>
        /// Calculate the screen position of the rectTransform.
        /// </summary>
        public static Rect GetScreenRect(this RectTransform rt)
        {
            var canvas = rt.GetComponentInParent<Canvas>();
            if (canvas && canvas.renderMode != RenderMode.WorldSpace)
            {
                var corners = new Vector3[4];
                rt.GetWorldCorners(corners);
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera)
                {
                    var p1 = canvas.worldCamera.WorldToScreenPoint(corners[0]);
                    var p2 = canvas.worldCamera.WorldToScreenPoint(corners[2]);
                    return new Rect(p1, p2 - p1);
                }
                else
                {
                    return new Rect(corners[0], corners[2] - corners[0]);
                }
            }
            return default;
        }

        /// <summary>
        /// Set anchors to (0,0) and (1,1), pivot (0.5,0.5), delta 0
        /// </summary>
        public static RectTransform SetFullParent(this RectTransform rt)
        {
            rt.SetFullParentAnchor();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.pivot = Vector2.one * 0.5f;
            return rt;
        }

        /// <summary>
        /// Set anchors to (0,0) and (1,1)
        /// </summary>
        public static RectTransform SetFullParentAnchor(this RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            return rt;
        }

        /// <summary>
        /// Set pivot to (x,y) (normalized)
        /// </summary>
        public static RectTransform SetPivot(this RectTransform rt, float x, float y)
        {
            rt.pivot = new Vector2(x, y);
            return rt;
        }

        #endregion


        #region Auto-stuffs

        /// <summary>
        /// Auto-assign <paramref name="target"/> from scene object
        /// </summary>
        public static void AutoFind<T>(this Component o, out T target) where T : Component
        {
            target = Object.FindObjectOfType<T>();
        }

        /// <summary>
        /// Auto-assign <paramref name="targets"/> from scene objects
        /// </summary>
        public static void AutoFind<T>(this Component o, out T[] targets) where T : Component
        {
            targets = Object.FindObjectsOfType<T>();
        }

        

        public static T AutoGetAndCache<T>(this Component o, ref T t, bool includeInactive = false)
        {
            if (!o) return t;
            if (Application.isPlaying) return t == null || t is Object ot && !ot ? (t = o.GetComponentInChildren<T>(includeInactive)) : t;
            else return o.GetComponentInChildren<T>(includeInactive);
        }

        public static T[] AutoGetAndCache<T>(this Component o, ref T[] ts, bool includeInactive = false)
        {
            if (!o) return ts;
            if (Application.isPlaying) return ts ?? (ts = o.GetComponentsInChildren<T>(includeInactive));
            else return o.GetComponentsInChildren<T>(includeInactive);
        }

        public static T AutoGetParentAndCache<T>(this Component o, ref T t, bool includeInactive = false)
        {
            if (!o) return t;
            T fullGet()
            {
                if (typeof(T).IsAssignableFrom(typeof(Component)))
                {
                    return o.GetComponentsInParent<T>(includeInactive).OrderBy(tp => o.transform.GetDepthFrom((tp as Component).transform)).FirstOrDefault();
                }
                return o.GetComponentsInParent<T>(includeInactive).FirstOrDefault();
            }

            if (!Application.isPlaying)
            {
                return includeInactive ? fullGet() : o.GetComponentInParent<T>();
            }
            else
            {
                if (t == null || t is Object ot && !ot) t = includeInactive ? fullGet() : o.GetComponentInParent<T>();
                return t;
            }
        }

        public static T[] AutoGetParentAndCache<T>(this Component o, ref T[] t, bool includeInactive = false) where T : Component
        {
            if (!o) return t;
            T[] fullGet()
            {
                if (typeof(T).IsAssignableFrom(typeof(Component)))
                {
                    return o.GetComponentsInParent<T>(includeInactive).OrderBy(tp => o.transform.GetDepthFrom((tp as Component).transform)).ToArray();
                }
                return o.GetComponentsInParent<T>(includeInactive);
            }

            if (Application.isPlaying) return t ?? (t = fullGet());
            else return fullGet();
        }

        //[System.Obsolete("Use CacheAny instead")]
        public static IE AutoIEAndCache<IE>(this Object o, ref IE l, System.Func<IE> getter) where IE : IEnumerable
        {
            if (Application.isPlaying) return l != null ? l : (l = getter());
            else return getter();
        }

        #endregion


        #region Sprite & Graphics

        /// <summary>
        /// Calculate normalized texturerect of a sprite (0->1)
        /// </summary>
        public static Rect LocalTextureRect(this Sprite sprite)
        {
            var txcPos = sprite.textureRect.position;
            var txcSize = sprite.textureRect.size;
            txcPos.x /= sprite.texture.width;
            txcPos.y /= sprite.texture.height;
            txcSize.x /= sprite.texture.width;
            txcSize.y /= sprite.texture.height;
            return new Rect(txcPos, txcSize);
        }

        /// <summary>
        /// Calculate object size of a sprite. Which is equal to the default size of a spriteRenderer with the sprite
        /// </summary>
        public static Vector2 ObjectSize(this Sprite sprite)
        {
            return sprite.rect.size / sprite.pixelsPerUnit;
        }




        /// <summary>
        /// Set the alpha value of the Graphic component
        /// </summary>
        public static void SetAlpha(this Graphic g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }

        /// <summary>
        /// Set the alpha value of the Sprite component
        /// </summary>
        public static void SetAlpha(this SpriteRenderer g, float a)
        {
            var c = g.color;
            c.a = a;
            g.color = c;
        }

        #endregion


        #region Colliders

        // public static Vector2[] GetLocalCorners(this BoxCollider2D bc2)
        // {
        //     var size = bc2.size;
        //     if (bc2.autoTiling)
        //     {
        //         var sr = bc2.GetComponent<SpriteRenderer>();
        //         if (sr) size = sr.size;
        //     }
        //     return new Rect().FromCenter(bc2.offset, size).GetCorners();
        // }

        // public static Vector2[] GetWorldCorners(this BoxCollider2D bc2)
        // {
        //     return bc2.GetLocalCorners().Select(v => (Vector2)bc2.transform.TransformPoint(v)).ToArray();
        // }

        #endregion


        #region Camera

        /// <summary>
        /// Calculate camera world corners at distance
        /// </summary>
        public static Vector3[] GetWorldCorners(this Camera cam, float distance)
        {
            var corners = new Vector3[4];
            if (cam.orthographic)
            {
                corners[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
                corners[1] = cam.ViewportToWorldPoint(new Vector3(0, 1, distance));
                corners[2] = cam.ViewportToWorldPoint(new Vector3(1, 1, distance));
                corners[3] = cam.ViewportToWorldPoint(new Vector3(1, 0, distance));
            }
            else
            {
                cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), distance, Camera.MonoOrStereoscopicEye.Mono, corners);
                corners = corners.Select(c => cam.transform.TransformPoint(c)).ToArray();
            }
            return corners;
        }

        /// <summary>
        /// Render the camera frustrum using Gizmos
        /// </summary>
        public static void DrawGizmosFrustum(this Camera camera)
        {
            var temp = Gizmos.matrix;
            //Gizmos.matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, Vector3.one);
            Gizmos.matrix = camera.transform.localToWorldMatrix;
            if (camera.orthographic)
            {
                var spread = camera.farClipPlane - camera.nearClipPlane;
                var center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
            }
            else
            {
                Gizmos.DrawFrustum(new Vector3(0, 0, (camera.nearClipPlane)), camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
            }
            Gizmos.matrix = temp;
        }

        public static Texture2D RenderToTexture2D(this Camera cam)
        {
            var w = Screen.width;
            var h = Screen.height;

            var oldCamTex = cam.targetTexture;
            var oldActiveTex = RenderTexture.active;

            var rt = RenderTexture.GetTemporary(w, h, 24);
            RenderTexture.active = rt;
            cam.targetTexture = rt;
            cam.Render();

            var txX = (int)(w * cam.rect.x);
            var txY = (int)(h * cam.rect.y);
            var txW = (int)(w * cam.rect.width);
            var txH = (int)(h * cam.rect.height);
            var texture = new Texture2D(txW, txH, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(txX, txY, txW, txH), 0, 0);
            texture.Apply();

            cam.targetTexture = oldCamTex;
            RenderTexture.active = oldActiveTex; //Clean
            RenderTexture.ReleaseTemporary(rt);

            return texture;
        }

        #endregion


        #region Mono

        /// <summary>
        /// Find a child transform with specified name or create one inside. Default TRS.
        /// </summary>
        public static Transform GetOrCreateChildTransform(this MonoBehaviour c, string name)
        {
            return c.transform.GetOrCreateChildTransform(name);
        }

        /// <summary>
        /// Mono-wait routine in real-time or game-time
        /// </summary>
        public static Coroutine TWLEWait(this MonoBehaviour c, float time, bool realtime, System.Action onComplete)
        {
            if (realtime) return c.StartCoroutine(WaitRoutine(new WaitForSecondsRealtime(time), onComplete));
            else return c.StartCoroutine(WaitRoutine(new WaitForSeconds(time), onComplete));
        }

        /// <summary>
        /// Mono-wait routine which yield anything specified. YieldInstruction, CustomYieldInstruction,...
        /// </summary>
        public static Coroutine TWLEWait(this MonoBehaviour c, object yield, System.Action onComplete)
        {
            return c.StartCoroutine(WaitRoutine(yield, onComplete));
        }

        static IEnumerator WaitRoutine(object yield, System.Action onComplete)
        {
            if (yield is CustomYieldInstruction yieldInstruction)
            {
                while (true)
                {
                    if (!yieldInstruction.keepWaiting)
                    {
                        onComplete?.Invoke();
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                yield return yield;
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Execute callback repeatedly in time duration, with (passed time, delta time) as parameters
        /// </summary>
        public static Coroutine TWLERepeatCall(this MonoBehaviour c, System.Action<float, float> call,
            float duration = -1, float interval = 0, bool realtime = false,
            System.Action doneCall = null)
        {
            return c.StartCoroutine(RepeatCallRoutine(call, duration, interval, realtime, doneCall));
        }

        static IEnumerator RepeatCallRoutine(System.Action<float, float> call, float duration, float interval, bool realtime, System.Action doneCall)
        {
            float t = 0;
            float dt = 0;
            while (true)
            {
                call?.Invoke(t, dt);

                if (interval > 0)
                {
                    if (realtime) yield return new WaitForSecondsRealtime(interval);
                    else yield return new WaitForSeconds(interval);
                    dt = Mathf.Max(interval, realtime ? Time.unscaledDeltaTime : Time.deltaTime);
                }
                else
                {
                    yield return null;
                    dt = realtime ? Time.unscaledDeltaTime : Time.deltaTime;
                }

                if (duration > 0)
                {
                    t += Mathf.Min(duration - t, dt);
                    if (t >= duration) break;
                }
                else
                {
                    t += dt;
                }
            }
            doneCall?.Invoke();
        }

        #endregion


        #region GUI

        public static bool EqualContent(this GUIContent gc1, GUIContent gc2)
        {
            if (gc1 == null && gc2 == null) return true;
            return gc1?.text == gc2?.text && gc1?.image == gc2?.image;
        }

        public static bool IsNullOrNone(this GUIContent gc1)
        {
            if (gc1 == null) return true;
            return gc1.EqualContent(GUIContent.none);
        }

        #endregion

    }
}