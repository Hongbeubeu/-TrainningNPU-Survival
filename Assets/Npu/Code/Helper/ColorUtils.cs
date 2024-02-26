using System.Linq;
using UnityEngine;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.Helper
{
    public static class ColorUtils
    {
        public static readonly Color pink = new Color32(255, 105, 180, 255);
        public static readonly Color orange = new Color32(255, 162, 0, 255);
        public static readonly Color gold = new Color32(255, 208, 180, 255);
        public static readonly Color lightPink = new Color32(255, 182, 193, 255);
        public static readonly Color lightGreen = new Color32(11, 255, 111, 255);
        public static readonly Color lightBlue = new Color32(127, 232, 255, 255);
        public static readonly Color lightRed = new Color32(255, 89, 89, 255);
        public static readonly Color niceBlue = new Color32(51, 153, 255, 255);
        public static readonly Color assetPreviewBackground = new Color32(82, 82, 82, 255);

        public static bool Compare(this Color32 c1, Color32 c2, bool withAlpha = true)
        {
            return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && (!withAlpha || c1.a == c2.a);
        }

        public static Color SetAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }
        
        public static Color RandomColor(bool rndAlpha = false)
        {
            var r = Random.value;
            var g = Random.value;
            var b = Random.value;
            var a = rndAlpha ? Random.value : 1;
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Return new color with the HSV value shifted by <paramref name="dh"/>, <paramref name="ds"/>, <paramref name="dv"/>
        /// </summary>
        public static Color ShiftHSV(this Color c, float dh = 0, float ds = 0, float dv = 0)
        {
            void shift(ref float f, float df)
            {
                f += df;
                while (f < 0) f += 1;
                f = f % 1;
            }

            var a = c.a;
            Color.RGBToHSV(c, out var h, out var s, out var v);
            if (dh != 0) shift(ref h, dh);
            if (ds != 0) shift(ref s, ds);
            if (dv != 0) shift(ref v, dv);

            var col = Color.HSVToRGB(h, s, v);
            col.a = a;
            return col;
        }

        /// <summary>
        /// Return new color with the HSV value shifted by <paramref name="dh"/>, <paramref name="ds"/>, <paramref name="dv"/>
        /// </summary>
        public static Color OffsetHSV(Color c, float dh = 0, float ds = 0, float dv = 0)
        {
            void shift(ref float f, float df)
            {
                f = Mathf.Clamp01(f + df);
            }

            var a = c.a;
            Color.RGBToHSV(c, out var h, out var s, out var v);
            if (dh != 0) shift(ref h, dh);
            if (ds != 0) shift(ref s, ds);
            if (dv != 0) shift(ref v, dv);

            var col = Color.HSVToRGB(h, s, v);
            col.a = a;
            return col;
        }

        public static string ColoredText(string s, Color c)
        {
            return $"<color={RichTextTag(c)}>{s}</color>";
        }

        public static string RichTextTag(Color c)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(c)}";
        }

        public static float GetHSVValue(Color c, HSVValueType type = HSVValueType.Hue)
        {
            Color.RGBToHSV(c, out var h, out var s, out var v);
            if (type == HSVValueType.Hue) return h;
            else if (type == HSVValueType.Saturation) return s;
            else if (type == HSVValueType.Value) return v;
            else return 0;
        }

        public static Vector3 GetHSVVector(Color c)
        {
            Color.RGBToHSV(c, out var h, out var s, out var v);
            return new Vector3(h, s, v);
        }

        public static Color FromHSVVector(Vector3 v)
        {
            return Color.HSVToRGB(v.x, v.y, v.z);
        }

        public enum HSVValueType
        {
            Hue, Saturation, Value
        }

        public static Color Blend(Color src, Color dst, BlendFunc blendFunc = BlendFunc.AlphaBlend)
        {
            if (blendFunc == BlendFunc.Add) return src + dst;
            else if (blendFunc == BlendFunc.Multiply) return src * dst;
            else if (blendFunc == BlendFunc.Halfway) return Color.Lerp(src, dst, 0.5f);
            else if (blendFunc == BlendFunc.AlphaBlend) return src * src.a + dst * (1 - src.a);
            else if (blendFunc == BlendFunc.AlphaAdd)
            {
                var c = Color.Lerp(dst, src, src.a);
                c.a = Mathf.Clamp01(dst.a + src.a);
                return c;
            }
            return src;
        }

        public static Color Invert(Color src)
        {
            return new Color(1 - src.r, 1 - src.g, 1 - src.b, src.a);
        }

        public static Gradient MakeGradient(params Color[] colors)
        {
            var steps = colors.Length;
            if (steps >= 1)
            {
                var perStep = steps > 0 ? 1f / (steps - 1) : 1f;
                var colorKeys = colors.Select((c, i) => new GradientColorKey(c, i * perStep)).ToArray();
                var alphaKeys = colors.Select((c, i) => new GradientAlphaKey(c.a, i * perStep)).ToArray();
                return new Gradient() { alphaKeys = alphaKeys, colorKeys = colorKeys };
            }
            return new Gradient();
        }

        public static Gradient MakeGradient(params (Color, float)[] colors)
        {
            var colorKeys = colors.Select(ci => new GradientColorKey(ci.Item1, ci.Item2)).ToArray();
            var alphaKeys = colors.Select(ci => new GradientAlphaKey(ci.Item1.a, ci.Item2)).ToArray();
            return new Gradient() { alphaKeys = alphaKeys, colorKeys = colorKeys };
        }

        public enum BlendFunc
        {
            Add,
            Multiply,
            Halfway,
            AlphaBlend,
            AlphaAdd,
        }

    }

}