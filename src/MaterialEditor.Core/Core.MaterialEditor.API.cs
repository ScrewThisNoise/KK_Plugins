﻿using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace KK_Plugins.MaterialEditor
{
    public static class MaterialAPI
    {
        /// <summary>
        /// List of parts that comprise the body, used to distinguish between clothes, accessories, etc. attached to the body.
        /// </summary>
#if AI
        public static readonly HashSet<string> BodyParts = new HashSet<string> {
            "o_eyebase_L", "o_eyebase_R", "o_eyelashes", "o_eyeshadow", "o_head", "o_namida", "o_tang", "o_tooth", "o_body_cf", "o_mnpa", "o_mnpb", "cm_o_dan00", "o_tang",
            "cm_o_dan00", "o_tang", "o_silhouette_cf", "o_body_cf", "o_head" };
#else
        public static readonly HashSet<string> BodyParts = new HashSet<string> {
            "cf_O_tooth", "cf_O_canine", "cf_O_tang", "o_tang", "n_tang", "n_tang_silhouette",  "cf_O_eyeline", "cf_O_eyeline_low", "cf_O_mayuge", "cf_Ohitomi_L", "cf_Ohitomi_R",
            "cf_Ohitomi_L02", "cf_Ohitomi_R02", "cf_O_noseline", "cf_O_namida_L", "cf_O_namida_M", "o_dankon", "o_gomu", "o_dan_f", "cf_O_namida_S", "cf_O_gag_eye_00", "cf_O_gag_eye_01",
            "cf_O_gag_eye_02", "o_shadowcaster", "o_shadowcaster_cm", "o_mnpa", "o_mnpb", "n_body_silhouette", "o_body_a", "cf_O_face" };
#endif

        /// <summary>
        /// Get a list of all the renderers. If gameObject is a ChaControl, only gets renderers of the body and face (i.e. not clothes, accessories, etc.)
        /// </summary>
        public static List<Renderer> GetRendererList(GameObject gameObject)
        {
            List<Renderer> rendList = new List<Renderer>();
            if (gameObject == null) return rendList;
            var chaControl = gameObject.GetComponent<ChaControl>();

            if (chaControl != null)
                _GetRendererList(gameObject, rendList);
            else
                rendList = gameObject.GetComponentsInChildren<Renderer>(true).ToList();

            return rendList;
        }
        /// <summary>
        /// Recursively iterates over game objects to create the list. Use GetRendererList intead.
        /// </summary>
        private static void _GetRendererList(GameObject gameObject, List<Renderer> rendList)
        {
            if (gameObject == null) return;

            Renderer rend = gameObject.GetComponent<Renderer>();
            if (rend != null && BodyParts.Contains(rend.NameFormatted()))
                rendList.Add(rend);

            for (int i = 0; i < gameObject.transform.childCount; i++)
                _GetRendererList(gameObject.transform.GetChild(i).gameObject, rendList);
        }

        /// <summary>
        /// Set the value of the specified material property
        /// </summary>
        /// <param name="chaControl">ChaControl to search for the material. Only parts comprising the body and face will be searched, not clothes, accessories, etc.</param>
        /// <param name="materialName">Name of the material being set</param>
        /// <param name="propertyName">Property of the material being set</param>
        /// <param name="value">Value to be set</param>
        /// <returns>True if the material was found and the value set</returns>
        public static bool SetFloatProperty(ChaControl chaControl, string materialName, string propertyName, string value) => SetFloatProperty(chaControl.gameObject, materialName, propertyName, value);
        /// <summary>
        /// Set the value of the specified material property
        /// </summary>
        /// <param name="gameObject">GameObject to search for the material. If this GameObject is a ChaControl, only parts comprising the body and face will be searched, not clothes, accessories, etc.</param>
        /// <param name="materialName">Name of the material being set</param>
        /// <param name="propertyName">Property of the material being set</param>
        /// <param name="value">Value to be set</param>
        /// <returns>True if the material was found and the value set</returns>
        public static bool SetFloatProperty(GameObject gameObject, string materialName, string propertyName, string value)
        {
            float floatValue = float.Parse(value);
            bool didSet = false;

            foreach (var obj in GetRendererList(gameObject))
                foreach (var objMat in obj.sharedMaterials)
                    if (objMat.NameFormatted() == materialName)
                    {
                        objMat.SetFloat($"_{propertyName}", floatValue);
                        didSet = true;
                    }
            return didSet;
        }

        /// <summary>
        /// Set the value of the specified material property
        /// </summary>
        /// <param name="chaControl">ChaControl to search for the material. Only parts comprising the body and face will be searched, not clothes, accessories, etc.</param>
        /// <param name="materialName">Name of the material being set</param>
        /// <param name="propertyName">Property of the material being set</param>
        /// <param name="value">Value to be set</param>
        /// <returns>True if the material was found and the value set</returns>
        public static bool SetColorProperty(ChaControl chaControl, string materialName, string propertyName, string value) => SetColorProperty(chaControl.gameObject, materialName, propertyName, value.ToColor());
        public static bool SetColorProperty(ChaControl chaControl, string materialName, string propertyName, Color value) => SetColorProperty(chaControl.gameObject, materialName, propertyName, value);
        public static bool SetColorProperty(GameObject gameObject, string materialName, string propertyName, string value) => SetColorProperty(gameObject, materialName, propertyName, value.ToColor());
        public static bool SetColorProperty(GameObject gameObject, string materialName, string propertyName, Color value)
        {
            bool didSet = false;

            foreach (var obj in GetRendererList(gameObject))
                foreach (var objMat in obj.sharedMaterials)
                    if (objMat.NameFormatted() == materialName)
                    {
                        objMat.SetColor($"_{propertyName}", value);
                        didSet = true;
                    }
            return didSet;
        }

        public static bool SetRendererProperty(ChaControl chaControl, string rendererName, RendererProperties propertyName, string value) => SetRendererProperty(chaControl.gameObject, rendererName, propertyName, int.Parse(value));
        public static bool SetRendererProperty(ChaControl chaControl, string rendererName, RendererProperties propertyName, int value) => SetRendererProperty(chaControl.gameObject, rendererName, propertyName, value);
        public static bool SetRendererProperty(GameObject gameObject, string rendererName, RendererProperties propertyName, string value) => SetRendererProperty(gameObject, rendererName, propertyName, int.Parse(value));
        public static bool SetRendererProperty(GameObject gameObject, string rendererName, RendererProperties propertyName, int value)
        {
            bool didSet = false;
            foreach (var rend in GetRendererList(gameObject))
            {
                if (rend.NameFormatted() == rendererName)
                {
                    if (propertyName == RendererProperties.ShadowCastingMode)
                        rend.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)value;
                    else if (propertyName == RendererProperties.ReceiveShadows)
                        rend.receiveShadows = value == 1;
                    else if (propertyName == RendererProperties.Enabled)
                        rend.enabled = value == 1;
                    didSet = true;
                }
            }
            return didSet;
        }

        public static bool SetTextureProperty(ChaControl chaControl, string materialName, string propertyName, TexturePropertyType propertyType, Vector2? value) => value == null ? false : SetTextureProperty(chaControl.gameObject, materialName, propertyName, propertyType, (Vector2)value);
        public static bool SetTextureProperty(ChaControl chaControl, string materialName, string propertyName, TexturePropertyType propertyType, Vector2 value) => SetTextureProperty(chaControl.gameObject, materialName, propertyName, propertyType, value);
        public static bool SetTextureProperty(GameObject gameObject, string materialName, string propertyName, TexturePropertyType propertyType, Vector2? value) => value == null ? false : SetTextureProperty(gameObject, materialName, propertyName, propertyType, (Vector2)value);
        public static bool SetTextureProperty(GameObject gameObject, string materialName, string propertyName, TexturePropertyType propertyType, Vector2 value)
        {
            bool didSet = false;

            foreach (var obj in GetRendererList(gameObject))
                foreach (var objMat in obj.sharedMaterials)
                    if (objMat.NameFormatted() == materialName)
                    {
                        if (propertyType == TexturePropertyType.Offset)
                            objMat.SetTextureOffset($"_{propertyName}", value);
                        else
                            objMat.SetTextureScale($"_{propertyName}", value);
                        didSet = true;
                    }
            return didSet;
        }

        public static bool SetTextureProperty(ChaControl chaControl, string materialName, string propertyName, Texture2D value) => SetTextureProperty(chaControl.gameObject, materialName, propertyName, value);
        public static bool SetTextureProperty(GameObject gameObject, string materialName, string propertyName, Texture2D value)
        {
            bool didSet = false;
            foreach (var rend in GetRendererList(gameObject))
                foreach (var mat in rend.sharedMaterials)
                    if (mat.NameFormatted() == materialName)
                    {
                        var wrapMode = mat.GetTexture($"_{propertyName}")?.wrapMode;
                        if (wrapMode != null)
                            value.wrapMode = (TextureWrapMode)wrapMode;
                        mat.SetTexture($"_{propertyName}", value);
                        didSet = true;
                    }
            return didSet;
        }

        public static bool SetShader(ChaControl chaControl, string materialName, string shaderName) => SetShader(chaControl.gameObject, materialName, shaderName);
        public static bool SetShader(GameObject gameObject, string materialName, string shaderName)
        {
            bool didSet = false;
            if (shaderName.IsNullOrEmpty()) return false;

            foreach (var rend in GetRendererList(gameObject))
                foreach (var mat in rend.sharedMaterials)
                    if (mat.NameFormatted() == materialName)
                    {
                        if (MaterialEditorPlugin.LoadedShaders.TryGetValue(shaderName, out var shaderData) && shaderData.Shader != null)
                        {
                            mat.shader = shaderData.Shader;

                            if (shaderData.RenderQueue != null)
                                mat.renderQueue = (int)shaderData.RenderQueue;

                            if (MaterialEditorPlugin.XMLShaderProperties.TryGetValue(shaderName, out var shaderPropertyDataList))
                                foreach (var shaderPropertyData in shaderPropertyDataList.Values)
                                    if (!shaderPropertyData.DefaultValue.IsNullOrEmpty())
                                    {
                                        switch (shaderPropertyData.Type)
                                        {
                                            case ShaderPropertyType.Float:
                                                SetFloatProperty(gameObject, materialName, shaderPropertyData.Name, shaderPropertyData.DefaultValue);
                                                break;
                                            case ShaderPropertyType.Color:
                                                SetColorProperty(gameObject, materialName, shaderPropertyData.Name, shaderPropertyData.DefaultValue);
                                                break;
                                            case ShaderPropertyType.Texture:
                                                if (shaderPropertyData.DefaultValue.IsNullOrEmpty()) continue;
                                                try
                                                {
                                                    var tex = CommonLib.LoadAsset<Texture2D>(shaderPropertyData.DefaultValueAssetBundle, shaderPropertyData.DefaultValue);
                                                    SetTextureProperty(gameObject, materialName, shaderPropertyData.Name, tex);
                                                }
                                                catch
                                                {
                                                    MaterialEditorPlugin.Logger.LogWarning($"[{MaterialEditorPlugin.PluginNameInternal}] Could not load default texture:{shaderPropertyData.DefaultValueAssetBundle}:{shaderPropertyData.DefaultValue}");
                                                }
                                                break;
                                        }
                                    }
                            didSet = true;
                        }
                        else
                            MaterialEditorPlugin.Logger.Log(BepInEx.Logging.LogLevel.Warning | BepInEx.Logging.LogLevel.Message, $"[{MaterialEditorPlugin.PluginNameInternal}] Could not load shader:{shaderName}");
                    }

            return didSet;
        }

        public static bool SetRenderQueue(ChaControl chaControl, string materialName, int? value) => SetRenderQueue(chaControl.gameObject, materialName, value);
        public static bool SetRenderQueue(GameObject gameObject, string materialName, int? value)
        {
            bool didSet = false;
            if (value == null) return false;

            foreach (var obj in GetRendererList(gameObject))
                foreach (var objMat in obj.sharedMaterials)
                    if (objMat.NameFormatted() == materialName)
                    {
                        objMat.renderQueue = (int)value;
                        didSet = true;
                    }
            return didSet;
        }

        public enum ObjectType { StudioItem, Clothing, Accessory, Hair, Character };
        public enum ShaderPropertyType { Texture, Color, Float }
        public enum TexturePropertyType { Texture, Offset, Scale }
        public enum RendererProperties { Enabled, ShadowCastingMode, ReceiveShadows }
    }
}