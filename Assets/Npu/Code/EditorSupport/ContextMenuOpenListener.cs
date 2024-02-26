#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Npu.Helper;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;


namespace Npu.EditorSupport
{


    [InitializeOnLoad]
    public static class ContextMenuOpenListener
    {
        static ContextMenuOpenListener()
        {
            EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
        }

        private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property)
        {
            HandleFixAtlasMoved(menu, property);
            HandleSpritePrefixChanged(menu, property);
        }

        private static void HandleFixAtlasMoved(GenericMenu menu, SerializedProperty property)
        {
            if (!IsAssetReferenceProperty(property)) return;

            menu.AddItem(new GUIContent("Fix Addressable Sprite Moved to new Atlas"), false, x => FixMovedAtlas(x as SerializedProperty), property.Copy());

        }
        
        private static void HandleSpritePrefixChanged(GenericMenu menu, SerializedProperty property)
        {
            if (!IsAssetReferenceProperty(property)) return;

            menu.AddItem(new GUIContent("Fix Sprite Prefix Changed"), false, x => FixSpritePrefixChanged(x as SerializedProperty), property.Copy());

        }

        public static bool IsAssetReferenceProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("m_AssetGUID") != null &&
                   property.FindPropertyRelative("m_SubObjectName") != null;
        }
        
        private static void FixMovedAtlas(SerializedProperty property)
        {
            var assetProp = property.FindPropertyRelative("m_AssetGUID");
            var subAssetProp = property.FindPropertyRelative("m_SubObjectName");

            var assetGuid = assetProp.stringValue;
            if (string.IsNullOrEmpty(assetGuid))
            {
                Logger.Log("AddressablesFixer", "AssetGUID is null. Nothing to fix");
                return;
            }

            var asset = AssetUtils.LoadAssetByGuid(assetGuid, null);
            if (asset == null)
            {
                Logger.Log("AddressablesFixer", $"No Asset of GUID {assetGuid}. Nothing to fix");
                return;
            }
            
            if (!(asset is SpriteAtlas atlas))
            {
                Logger.Log("AddressablesFixer", $"Asset is not SpriteAtlas. Nothing to fix");
                return;
            }

            var subAssetName = subAssetProp.stringValue;
            if (string.IsNullOrEmpty(subAssetName))
            {
                Logger.Log("AddressablesFixer", "Sub Asset is null. Nothing to fix");
                return;
            }
            
            Logger.Log("AddressablesFixer", $"Try to fix {assetGuid}.{subAssetName}");
            
            // Find sprite

            var atlases = AssetUtils.FindAllAssets<SpriteAtlas>();
            var found = atlases.FirstOrDefault(i => i.Sprites().Any(s => s.name == subAssetName));

            if (!found)
            {
                Logger.Log("AddressablesFixer", $"No Atlas contains {subAssetName} found. Nothing to fix");
                return;
            }
            
            Logger.Log("AddressablesFixer", $"Found {found} ({found?.AssetGuid()})");

            assetProp.stringValue = found.AssetGuid();
            property.serializedObject.ApplyModifiedProperties();
        }
        
        public static void FixSpritePrefixChanged(SerializedProperty property)
        {
            var prefixMap = new Dictionary<string, string>
            {
                {"ev-1_", "lj_"},
                {"ev-2_", "fl_"}
            };
            var assetProp = property.FindPropertyRelative("m_AssetGUID");
            var subAssetProp = property.FindPropertyRelative("m_SubObjectName");

            var assetGuid = assetProp.stringValue;
            if (string.IsNullOrEmpty(assetGuid))
            {
                Logger.Log("AddressablesFixer", "AssetGUID is null. Nothing to fix");
                return;
            }

            var asset = AssetUtils.LoadAssetByGuid(assetGuid, null);
            if (asset == null)
            {
                Logger.Log("AddressablesFixer", $"No Asset of GUID {assetGuid}. Nothing to fix");
                return;
            }
            
            if (!(asset is SpriteAtlas atlas))
            {
                Logger.Log("AddressablesFixer", $"Asset is not SpriteAtlas. Nothing to fix");
                return;
            }

            var subAssetName = subAssetProp.stringValue;
            if (string.IsNullOrEmpty(subAssetName))
            {
                Logger.Log("AddressablesFixer", "Sub Asset is null. Nothing to fix");
                return;
            }

            var prefix = prefixMap.FirstOrDefault(i => subAssetName.StartsWith(i.Key));

            if (string.IsNullOrEmpty(prefix.Key))
            {
                Logger.Log("AddressablesFixer", "No match prefix. Nothing to fix");
                return;
            }

            var nonPrefixSubAssetName = subAssetName.Substring(prefix.Key.Length);
            var newSubAssetName = $"{prefix.Value}{nonPrefixSubAssetName}";

            if (atlas.Sprites().All(i => i.name != newSubAssetName))
            {
                Logger.Log("AddressablesFixer", $"No {newSubAssetName} in {atlas}. Nothing to fix");
                return;
            }

            Logger.Log("AddressablesFixer", $"{property.propertyPath}: {subAssetName} => {newSubAssetName}");
            
            subAssetProp.stringValue = newSubAssetName;
            property.serializedObject.ApplyModifiedProperties();
        }


    }

}
#endif