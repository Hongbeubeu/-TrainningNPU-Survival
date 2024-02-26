using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace Npu.Helper
{

    public static class CommonExtensions
    {
        public static void CopyToClipboard(this string txt) => GUIUtility.systemCopyBuffer = txt;

        public static string MaxLength(this string txt, int max)
        {
            return txt.Length <= max ? txt : txt.Substring(0, max);
        }
        
        public static string MaxLengthInversed(this string txt, int max)
        {
            return txt.Length <= max ? txt : txt.Substring(txt.Length - max);
        }
        
        public static bool ContainsIgnoreCase(this string @string, string pattern)
        => CultureInfo.InvariantCulture.CompareInfo.IndexOf(@string, pattern, CompareOptions.IgnoreCase) >= 0;
        
        public static Rect Indented(this Rect rect, float indent = 1f)
        {
            return new Rect(rect.x + indent * 15, rect.y, rect.width - indent * 15, rect.height);
        }

        public static Rect Extended(this Rect rect, float top=0, float right=0, float bottom=0, float left=0)
        {
            return new Rect(rect.x - left, rect.y - top, rect.width + left + right, rect.height + top + bottom);
        }

        public static Rect Extended(this Rect rect, float extends)
        {
            return new Rect(rect.x - extends, rect.y - extends, rect.width + 2 * extends, rect.height + 2 * extends);
        }

        public static (Rect, Rect) HSplit(this Rect rect, float ratio = 0.5f)
        {
            var r1 = new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
            var r2 = new Rect(r1.x + r1.width, rect.y, rect.width * (1 - ratio), rect.height);
            return (r1, r2);
        }

        public static (Rect, Rect) HFixedSplit(this Rect rect, float width)
        {
            var r1 = new Rect(rect.x, rect.y, width, rect.height);
            var r2 = new Rect(r1.x + r1.width, rect.y, rect.width - width, rect.height);
            return (r1, r2);
        }

        public static Rect[] HSplit(this Rect rect, int count = 2)
        {
            var rects = new Rect[count];
            var w = rect.width / count;
            for (var i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rect(rect.x + i * w, rect.y, w, rect.height);
            }
            return rects;
        }

        public static (Rect, Rect) VSplit(this Rect rect, float ratio = 0.5f)
        {
            var r1 = new Rect(rect.x, rect.y, rect.width, rect.height * ratio);
            var r2 = new Rect(rect.x, rect.y + r1.height, rect.width, rect.height * (1 - ratio));
            return (r1, r2);
        }

        public static Rect[] VSplit(this Rect rect, int count = 2)
        {
            var rects = new Rect[count];
            var h = rect.height / count;
            for (var i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rect(rect.x, rect.y + i * h, rect.width, h);
            }
            return rects;
        }

        public static Vector3 MiddleTop(this Rect rect)
        {
            return rect.center + new Vector2(0, rect.height / 2);
        }

        public static Vector3 MiddleBottom(this Rect rect)
        {
            return rect.center - new Vector2(0, rect.height / 2);
        }

        public static Vector3 Center(this RectTransform transform)
        {
            return transform.TransformPoint(transform.rect.center);
        }

        public static Vector3 MiddleTop(this RectTransform transform)
        {
            return transform.TransformPoint(transform.rect.MiddleTop());
        }

        public static Vector3 MiddleBottom(this RectTransform transform)
        {
            return transform.TransformPoint(transform.rect.MiddleBottom());
        }

        public static Vector3 Shifted(this Vector3 v, float dx = 0, float dy = 0, float dz = 0) => v + new Vector3(dx, dy, dz);
        
        public static object DeepClone(this object src)
        {
            //step : 1 Get the type of source object and create a new instance of that type
            var typeSource = src.GetType();
            var dst = Activator.CreateInstance(typeSource);

            //Step2 : Get all the properties of source object type
            var fieldInfo = typeSource.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //Step : 3 Assign all source property to taget object 's properties
            foreach (var field in fieldInfo)
            {
                //Check whether property can be written to
                if (field.FieldType.IsValueType || field.FieldType.IsEnum || field.FieldType == typeof(string))
                {
                    field.SetValue(dst, field.GetValue(src));
                }
                //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
                else
                {
                    var value = field.GetValue(src);
                    if (value == null || value is UnityEngine.Object)
                    {
                        field.SetValue(dst, value);
                    }
                    else
                    {
                        field.SetValue(dst, DeepClone(value));
                    }
                }

            }
            return dst;
        }
    }


}