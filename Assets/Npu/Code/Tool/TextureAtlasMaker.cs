using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System.IO;
using System.Linq;
using Npu.EditorSupport;
using UnityEngine.Rendering;


namespace Npu.Helper
{

    [CreateAssetMenu]
    public class TextureAtlasMaker : ScriptableObject
    {
        [HideInInspector] public List<Textures> textures = new List<Textures>();
        [HideInInspector] public int padding = 0;
        [HideInInspector] public int maxSize = 2048;
        [HideInInspector] public Texture2D atlas;

        public void SetMaterials(object[] objects)
        {
            var materials = objects.OfType<Material>();
            textures = new List<Textures>();
            foreach (var i in materials)
            {
                var t = new Textures();
                t.Update(i);
                t.Populate();
                textures.Add(t);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Select Materials")]
        private void SelectMaterials()
        {
            Selection.objects = textures.Select(i => i.mat).ToArray();
        }

        [ContextMenu("Set Render Queue 2050")]
        private void RenderQueue2050() => RenderQueue(2050);

        private void RenderQueue(int queue)
        {
            Logger.Log<TextureAtlasMaker>("Queue Before = {0}",
                string.Join(",", textures.Select(i => i.mat.renderQueue).Distinct()));
            foreach (var i in textures)
            {
                i.mat.DirtyWithUndo();
                i.mat.renderQueue = queue;
            }

            Logger.Log<TextureAtlasMaker>("Queue After = {0}",
                string.Join(",", textures.Select(i => i.mat.renderQueue).Distinct()));
        }
#endif
    }

    [Serializable]
    public class Textures
    {
        public Material mat;
        public int matTexIndex;
        public Texture2D texture;
        public Rect rect;
        public List<TextureOffsetTitling> originalOffsetTilings;

        public List<string> MatTexNames => GetMatTexNames(mat);

        public void Update(Material material)
        {
            mat = material;
            texture = null;
            matTexIndex = 0;
            rect = new Rect(0, 0, 0, 0);
            UpdateOffsetTilings();
        }

        public void Populate()
        {
            if (mat == null) return;

            var names = MatTexNames;
            if (names.Count == 0) return;

            matTexIndex = 0;
            texture = mat.GetTexture(names[0]) as Texture2D;
        }

        public void ApplyAtlas(Texture2D result)
        {
            if (mat == null) return;

            var textName = ArrayExtensions.TryGet(MatTexNames, matTexIndex);
            if (textName == null)
            {
                Logger.Error<TextureAtlasMaker>("Invalid Texturew Name");
                return;
            }

            mat.SetTexture(textName, result);
            mat.SetTextureOffset(textName, rect.position);
            mat.SetTextureScale(textName, rect.size);

#if UNITY_EDITOR
            EditorUtility.SetDirty(mat);
#endif
        }

        public void Reverse()
        {
            if (mat == null) return;

            var textName = ArrayExtensions.TryGet(MatTexNames, matTexIndex);
            if (textName == null)
            {
                Logger.Error<TextureAtlasMaker>("Invalid Texture Name");
                return;
            }

            mat.SetTexture(textName, texture);

            var oft = originalOffsetTilings.FirstOrDefault(i => i.name.Equals(textName));
            if (oft != null)
            {
                mat.SetTextureOffset(textName, oft.value.position);
                mat.SetTextureScale(textName, oft.value.size);
            }
            else
            {
                Logger.Error<TextureAtlasMaker>($"Cannot find original offset for {textName}");
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(mat);
#endif
        }

        private void UpdateOffsetTilings()
        {
            originalOffsetTilings = GetMatOffsetTilings(mat);
        }

        public static List<string> GetMatTexNames(Material material)
        {
            if (!material || !material.shader) return new List<string>();

#if UNITY_EDITOR
            return Enumerable.Range(0, ShaderUtil.GetPropertyCount(material.shader))
                .Where(i => material.shader.GetPropertyType(i) == ShaderPropertyType.Texture)
                .Select(i => ShaderUtil.GetPropertyName(material.shader, i))
                .ToList();
#else
        return null;
#endif
        }

        public static List<TextureOffsetTitling> GetMatOffsetTilings(Material material)
        {
            var names = GetMatTexNames(material);
            return names.Select(i => new TextureOffsetTitling
            {
                name = i,
                value = new Rect(material.GetTextureOffset(i), material.GetTextureScale(i))
            }).ToList();
        }

    }

    [Serializable]
    public class TextureOffsetTitling
    {
        public string name;
        public Rect value;
    }

#if UNITY_EDITOR
    public class TextureSetting
    {
        public bool isReadable;
        public bool alphaIsTransparency;
        public TextureImporterCompression textureCompression;
    }
#endif

#if UNITY_EDITOR
    [CustomEditor(typeof(TextureAtlasMaker))]
    public class TextureAtlasMakerEditor : Editor
    {
        const float rowHeight = 80;
        const float rowSpace = 10;
        const float labelWidth = 20;
        const float colSpace = 10;

        TextureAtlasMaker tam;
        ReorderableList rList;
        int selected;

        Dictionary<Texture, TextureSetting> settings = new Dictionary<Texture, TextureSetting>();

        void OnEnable()
        {
            tam = target as TextureAtlasMaker;
            rList = new ReorderableList(tam.textures, typeof(Texture2D), true, true, true, true)
            {
                drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Textures"); },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var t = tam.textures[index];

                    //index
                    var label = (index + 1).ToString();
                    EditorGUI.LabelField(new Rect(rect.position.x, rect.position.y, labelWidth, rowHeight), label);

                    var width = (rect.width - labelWidth) / 3;
                    var pos = rect.position.x + labelWidth;

                    //mat
                    var mat = EditorGUI.ObjectField(
                        new Rect(pos, rect.position.y, width - colSpace, EditorGUIUtility.singleLineHeight), t.mat,
                        typeof(Material), false) as Material;

                    if (mat != t.mat)
                    {
                        t.Update(mat);
                        EditorUtility.SetDirty(tam);
                    }

                    //mattexname
                    var matTexIndex = -1;

                    if (t.MatTexNames.Count > 0)
                    {
                        matTexIndex =
                            EditorGUI.Popup(
                                new Rect(pos + width, rect.position.y, width - colSpace,
                                    EditorGUIUtility.singleLineHeight), t.matTexIndex, t.MatTexNames.ToArray());
                    }
                    else
                    {
                        EditorGUI.LabelField(
                            new Rect(pos + width, rect.position.y, width - colSpace, EditorGUIUtility.singleLineHeight),
                            "No texture property");
                    }

                    if (matTexIndex != t.matTexIndex)
                    {
                        t.matTexIndex = matTexIndex;
                        t.texture = null;
                        t.rect = new Rect(0, 0, 0, 0);
                    }

                    //tex
                    var size = Mathf.Min(width, rowHeight);

                    if (t.texture == null && t.MatTexNames.Count > t.matTexIndex && t.matTexIndex >= 0)
                    {
                        t.texture = t.mat.GetTexture(t.MatTexNames[t.matTexIndex]) as Texture2D;
                    }

                    t.texture = EditorGUI.ObjectField(new Rect(pos + width * 2, rect.position.y, size, size), t.texture,
                        typeof(Texture2D), false) as Texture2D;

                    var rectlabel = string.Format("{0}", t.rect.width);
                    EditorGUI.TextField(
                        new Rect(pos, rect.position.y + EditorGUIUtility.singleLineHeight * 1 + rowSpace,
                            width - colSpace, EditorGUIUtility.singleLineHeight), rectlabel);
                    rectlabel = string.Format("{0}", t.rect.height);
                    EditorGUI.TextField(
                        new Rect(pos + width, rect.position.y + EditorGUIUtility.singleLineHeight * 1 + rowSpace,
                            width - colSpace, EditorGUIUtility.singleLineHeight), rectlabel);
                    rectlabel = string.Format("{0}", t.rect.x);
                    EditorGUI.TextField(
                        new Rect(pos, rect.position.y + EditorGUIUtility.singleLineHeight * 2 + rowSpace,
                            width - colSpace, EditorGUIUtility.singleLineHeight), rectlabel);
                    rectlabel = string.Format("{0}", t.rect.y);
                    EditorGUI.TextField(
                        new Rect(pos + width, rect.position.y + EditorGUIUtility.singleLineHeight * 2 + rowSpace,
                            width - colSpace, EditorGUIUtility.singleLineHeight), rectlabel);

                    EditorUtility.SetDirty(tam);
                },
                onAddCallback = (list) =>
                {
                    list.list.Add(new Textures());
                    EditorUtility.SetDirty(tam);
                },
                onRemoveCallback = (list) =>
                {
                    try
                    {
                        list.list.RemoveAt(selected);
                    }
                    catch (Exception e)
                    {

                    }
                },
                onCanRemoveCallback = (list) => { return true; },
                onSelectCallback = (list) => { selected = list.index; }
            };
            rList.elementHeight = rowHeight + rowSpace;
        }

        public override void OnInspectorGUI()
        {
            rList.DoLayoutList();

            tam.padding = EditorGUILayout.IntField("Padding", tam.padding);
            tam.maxSize = EditorGUILayout.IntField("Max Size", tam.maxSize);
            tam.atlas = EditorGUILayout.ObjectField("Result", tam.atlas, typeof(Texture2D), false) as Texture2D;

            using (new HorizontalLayout())
            {
                if (GUILayout.Button("Create Atlas"))
                {
                    CreatePng(tam.maxSize);
                }

                if (tam.atlas != null)
                {
                    if (GUILayout.Button("Pack Atlas"))
                    {
                        Pack();
                    }

                    if (GUILayout.Button("Update Materials"))
                    {
                        SetMat();
                    }

                    if (GUILayout.Button("Reverse Materials"))
                    {
                        ReverseMat();
                    }
                }

            }

            EditorUtility.SetDirty(tam);
        }

        public void Pack()
        {
            settings = new Dictionary<Texture, TextureSetting>();

            var textures = GetDistinctTexture();
            SetReadableFlag(textures);
            CreateAtlas(textures, tam.maxSize);
            RestoreFlag(textures);
        }

        List<Texture2D> GetDistinctTexture()
        {
            var textures = new List<Texture2D>();
            foreach (var texture in tam.textures)
            {
                if (!textures.Contains(texture.texture))
                {
                    textures.Add(texture.texture);
                }
            }

            return textures;
        }

        void SetReadableFlag(List<Texture2D> textures)
        {
            foreach (var texture in textures)
            {
                SetReadableFlag(texture);
            }
        }

        void SetReadableFlag(Texture2D texture)
        {
            if (texture == null) return;

            var path = AssetDatabase.GetAssetPath(texture);

            var textureImporter = (TextureImporter) AssetImporter.GetAtPath(path);

            var cachedSetting = new TextureSetting()
            {
                isReadable = textureImporter.isReadable,
                alphaIsTransparency = textureImporter.alphaIsTransparency,
                textureCompression = textureImporter.textureCompression,

            };

            settings.Add(texture, cachedSetting);

            textureImporter.isReadable = true;
            textureImporter.alphaIsTransparency = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        void RestoreFlag(List<Texture2D> textures)
        {
            foreach (var texture in textures)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                var textureImporter = (TextureImporter) AssetImporter.GetAtPath(path);

                textureImporter.isReadable = settings[texture].isReadable;
                textureImporter.alphaIsTransparency = settings[texture].alphaIsTransparency;
                textureImporter.textureCompression = settings[texture].textureCompression;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        void SetResultFlag(Texture2D texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var textureImporter = (TextureImporter) AssetImporter.GetAtPath(path);
            textureImporter.isReadable = false;
            textureImporter.alphaIsTransparency = true;
            textureImporter.textureCompression = TextureImporterCompression.Compressed;
            textureImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        void CreateAtlas(List<Texture2D> textures, int maxSize)
        {
            var atlas = new Texture2D(maxSize, maxSize, TextureFormat.RGBA32, false);

            var rects = atlas.PackTextures(textures.ToArray(), Mathf.Max(0, tam.padding), maxSize);

            for (var i = 0; i < textures.Count; i++)
            {
                for (var j = 0; j < tam.textures.Count; j++)
                {
                    if (tam.textures[j].texture == textures[i])
                    {
                        tam.textures[j].rect = rects[i];
                    }
                }
            }

            atlas.Apply();

            var path1 = Application.dataPath.Replace("Assets", "");
            var path2 = AssetDatabase.GetAssetPath(tam.atlas);

            var fs = new FileStream(path1 + path2, FileMode.Create);
            var bw = new BinaryWriter(fs);
            bw.Write(atlas.EncodeToPNG());
            bw.Close();
            fs.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetResultFlag(tam.atlas);
        }

        void SetMat()
        {
            foreach (var texture in tam.textures)
            {
                texture.ApplyAtlas(tam.atlas);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void ReverseMat()
        {
            foreach (var texture in tam.textures)
            {
                texture.Reverse();
            }
        }

        void CreatePng(int maxSize)
        {
            var texture = new Texture2D(maxSize, maxSize, TextureFormat.RGBA32, false);

            var path1 = Application.dataPath.Replace("Assets", "");
            var path2 = AssetDatabase.GetAssetPath(tam).Replace(".asset", ".png");

            var fs = new FileStream(path1 + path2, FileMode.Create);
            var bw = new BinaryWriter(fs);
            bw.Write(texture.EncodeToPNG());
            bw.Close();
            fs.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            tam.atlas = AssetDatabase.LoadAssetAtPath(path2, typeof(Texture2D)) as Texture2D;

            SetResultFlag(tam.atlas);
        }
    }
#endif
}