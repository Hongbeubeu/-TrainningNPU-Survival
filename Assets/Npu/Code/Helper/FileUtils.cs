using System.IO;
using UnityEngine;

#if UNITY_IPHONE

#endif


namespace Npu.Helper
{

    public static class FileUtils
    {
        private static string documentDirectory = null;
        private static string temporaryDirectory = null;

#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern string FileUtils_getDocumentDirectory();
	[DllImport ("__Internal")]
	private static extern string FileUtils_getTemporaryDirectory();
    [DllImport ("__Internal")]
    private static extern float FileUtils_getFreeDiskSpace();
#endif

        public static string GetDocumentDirectory()
        {
            if (documentDirectory == null)
            {
#if UNITY_IPHONE && !UNITY_EDITOR
			documentDirectory = FileUtils_getDocumentDirectory();
#elif UNITY_ANDROID && !UNITY_EDITOR
			documentDirectory = FileUtilsAndroid.GetDocumentDirectory();
#else
                documentDirectory = "TempStorage";
                if (!Directory.Exists(Application.dataPath + "/../" + documentDirectory))
                {
                    Directory.CreateDirectory(Application.dataPath + "/../" + documentDirectory);
                }
#endif
            }

            return documentDirectory;
        }

        public static string GetDocumentPath(string filename)
        {
            return Path.Combine(GetDocumentDirectory(), filename);
        }

        public static string GetTemporaryDirectory()
        {
            if (temporaryDirectory == null)
            {
#if UNITY_IPHONE && !UNITY_EDITOR
			temporaryDirectory = FileUtils_getTemporaryDirectory();
#elif UNITY_ANDROID && !UNITY_EDITOR
			temporaryDirectory = FileUtilsAndroid.GetTemporaryDirectory();
#else
                temporaryDirectory = "TempStorage";
                if (!Directory.Exists(Application.dataPath + "/../" + temporaryDirectory))
                {
                    Directory.CreateDirectory(Application.dataPath + "/../" + temporaryDirectory);
                }
#endif
            }

            return temporaryDirectory;
        }

        public static string GetTemporaryPath(string filename)
        {
            return Path.Combine(GetTemporaryDirectory(), filename);
        }

        public static void SpriteToFile(Sprite sprite, string filePath)
        {
            File.WriteAllBytes(filePath, GetSpriteBytes(sprite));
        }

        public static void TextureToFile(Texture2D texture, string filePath)
        {
            File.WriteAllBytes(filePath, GetTextureBytes(texture));
        }

        private static byte[] GetSpriteBytes(Sprite sp)
        {
            var tex = new Texture2D((int) sp.rect.width, (int) sp.rect.height);
            var pixels = sp.texture.GetPixels((int) sp.textureRect.x, (int) sp.textureRect.y,
                (int) sp.textureRect.width, (int) sp.textureRect.height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex.EncodeToPNG();
        }

        private static byte[] GetTextureBytes(Texture2D tex)
        {
            return tex.EncodeToPNG();
        }

        public static Texture2D FileToTexture(string path)
        {
            Texture2D tex = null;

            if (File.Exists(path))
            {
                var fileData = File.ReadAllBytes(path);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }

            return tex;
        }

        public static void CopyStream(Stream src, Stream dst)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dst.Write(buffer, 0, read);
            }
        }

        public static float GetFreeDiskSpace()
        {
#if UNITY_IPHONE && !UNITY_EDITOR
        return FileUtils_getFreeDiskSpace();
#else
            return 0;
#endif
        }
    }

#if UNITY_ANDROID
class FileUtilsAndroid {
	const string JavaClassName = "com.nopowerup.screw.utils.FileUtils";

	public static string GetDocumentDirectory () {
		// Java public static String getDocumentDirectory()
		using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName)) { 
			return jc.CallStatic<string>("getDocumentDirectory");
		}
	}

	public static string GetTemporaryDirectory () {
		// Java public static String getTemporaryDirectory()
		using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName)) { 
			return jc.CallStatic<string>("getTemporaryDirectory");
		}
	}


}
#endif
}