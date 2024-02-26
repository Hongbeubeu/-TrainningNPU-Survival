using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu.Helper
{

    public interface IBuildPreparation
    {
        void PrepareForBuild();
    }

    public static class Extensions
    {

        public static Vector3 AverageNormal(this Collision collison)
        {
            var normal = Vector3.zero;
            foreach (var i in collison.contacts)
            {
                normal += i.normal;
            }

            if (normal.sqrMagnitude > Mathf.Epsilon)
            {
                normal.Normalize();
            }

            return normal;
        }

        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static string GetResponseBody(this UnityWebRequest rq)
        {
            if (rq.downloadHandler == null || rq.downloadHandler.data == null)
                return null;

            return Encoding.UTF8.GetString(rq.downloadHandler.data);
        }

        public static List<T> GetEnumValues<T>(long mask, bool bitmask = false) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                Debug.LogErrorFormat("{0} is not an enum", type.ToString());
                return null;
            }

            var values = Enum.GetValues(type) as int[];
            var ret = new List<T>();
            for (var i = 0; i < values.Length; i++)
            {
                var v = values[i];
                if (
                    (bitmask && (mask & v) != 0) ||
                    (!bitmask && (mask & (1 << v)) != 0)
                )
                {
                    ret.Add((T) (object) v);
                }
            }

            return ret;
        }

        public static string AssetPath(this Object asset)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPath(asset);
#else
        return "";
#endif
        }

#if false
	public static Dictionary<string, object> GetJsonResponse (this UnityWebRequest request)
	{
		var body = request.GetResponseBody ();
		if (body == null) {
			return null;
		}

		try {
			var json = MiniJSONV.Json.Deserialize (body);
			return json as Dictionary<string, object>;	
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
			return null;
		}
	}
#endif

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }


        public static string ToSerializeString(this Vector3 v)
        {
            return $"{v.x:R}|{v.y:R}|{v.z:R}";
        }

        public static string ToSerializeString(this Vector2 v)
        {
            return $"{v.x:R}|{v.y:R}";
        }

        public static string ToSerializeString(this Vector4 v)
        {
            return $"{v.x:R}|{v.y:R}|{v.z:R}|{v.w:R}";
        }

        public static Vector2 ToVector2(this string s)
        {
            var v = Vector2.zero;
            var ss = s.Split('|');
            if (ss.Length < 2) return v;

            float x;
            if (float.TryParse(ss[0], out x)) v.x = x;
            if (float.TryParse(ss[1], out x)) v.y = x;

            return v;
        }

        public static Vector3 ToVector3(this string s)
        {
            var v = Vector3.zero;
            var ss = s.Split('|');
            if (ss.Length < 3) return v;

            float x;
            if (float.TryParse(ss[0], out x)) v.x = x;
            if (float.TryParse(ss[1], out x)) v.y = x;
            if (float.TryParse(ss[2], out x)) v.z = x;

            return v;
        }

        public static Vector4 ToVector4(this string s)
        {
            var v = Vector4.zero;
            var ss = s.Split('|');
            if (ss.Length < 4) return v;

            float x;
            if (float.TryParse(ss[0], out x)) v.x = x;
            if (float.TryParse(ss[1], out x)) v.y = x;
            if (float.TryParse(ss[2], out x)) v.z = x;
            if (float.TryParse(ss[3], out x)) v.w = x;

            return v;
        }
    }

}