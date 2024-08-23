#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace HovlStudio
{

    [InitializeOnLoad]
    public class RPChanger : EditorWindow
    {
        [InitializeOnLoadMethod]
        private static void LoadWindow()
        {
            string[] checkAsset = AssetDatabase.FindAssets("HSstartupCheck");
            foreach (var guid in checkAsset)
            {
                ShowWindow();
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
            }
        }

        static private int pipeline;
        [MenuItem("Tools/RP changer for Hovl Studio assets")]

        public static void ShowWindow()
        {
            RPChanger window = (RPChanger)EditorWindow.GetWindow(typeof(RPChanger));
            window.minSize = new Vector2(250, 140);
            window.maxSize = new Vector2(250, 140);
        }


        public void OnGUI()
        {
            GUILayout.Label("Change VFX pipeline to:");
            if (GUILayout.Button("Standard RP"))
            {
                FindShaders();
                ChangeToSRP();
            }
            if (GUILayout.Button("Universal RP"))
            {
                pipeline = 1;
                ImportPipelinePackage();
            }
            if (GUILayout.Button("Universal RP 2D or orthographic camera"))
            {
                pipeline = 3;
                ImportPipelinePackage();
            }
            GUILayout.Label("Don't forget to enable Depth and Opaque\ncheck-buttons in your URP asset seeting.", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("HDRP"))
            {
                pipeline = 2;
                ImportPipelinePackage();
            }
        }

        static Shader Add_CG, Blend_CG, LightGlow, Lit_CenterGlow, Blend_TwoSides, Blend_Normals, Ice, Distortion, ParallaxIce, BlendDistort, VolumeLaser, Explosion, SwordSlash, ShockWave, SoftNoise;
        static Shader Add_CG_URP, Blend_CG_URP, LightGlow_URP, Lit_CenterGlow_URP, Blend_TwoSides_URP, Blend_Normals_URP, Ice_URP, Distortion_URP, ParallaxIce_URP,
            BlendDistort_URP, VolumeLaser_URP, Explosion_URP, SwordSlash_URP, ShockWave_URP, SoftNoise_URP;
        static Shader Add_CG_HDRP, Blend_CG_HDRP, LightGlow_HDRP, Lit_CenterGlow_HDRP, Blend_TwoSides_HDRP, Blend_Normals_HDRP, Ice_HDRP, Distortion_HDRP,
            ParallaxIce_HDRP, BlendDistort_HDRP, VolumeLaser_HDRP, Explosion_HDRP, SwordSlash_HDRP, ShockWave_HDRP, SoftNoise_HDRP;
        static Material[] shaderMaterials;

        private static void FindShaders()
        {
            if (Shader.Find("Hovl/Particles/Add_CenterGlow") != null) Add_CG = Shader.Find("Hovl/Particles/Add_CenterGlow");
            if (Shader.Find("Hovl/Particles/Blend_CenterGlow") != null) Blend_CG = Shader.Find("Hovl/Particles/Blend_CenterGlow");
            if (Shader.Find("Hovl/Particles/LightGlow") != null) LightGlow = Shader.Find("Hovl/Particles/LightGlow");
            if (Shader.Find("Hovl/Particles/Lit_CenterGlow") != null) Lit_CenterGlow = Shader.Find("Hovl/Particles/Lit_CenterGlow");
            if (Shader.Find("Hovl/Particles/Blend_TwoSides") != null) Blend_TwoSides = Shader.Find("Hovl/Particles/Blend_TwoSides");
            if (Shader.Find("Hovl/Particles/Blend_Normals") != null) Blend_Normals = Shader.Find("Hovl/Particles/Blend_Normals");
            if (Shader.Find("Hovl/Particles/Ice") != null) Ice = Shader.Find("Hovl/Particles/Ice");
            if (Shader.Find("Hovl/Particles/Distortion") != null) Distortion = Shader.Find("Hovl/Particles/Distortion");
            if (Shader.Find("Hovl/Opaque/ParallaxIce") != null) ParallaxIce = Shader.Find("Hovl/Opaque/ParallaxIce");
            if (Shader.Find("Hovl/Particles/BlendDistort") != null) BlendDistort = Shader.Find("Hovl/Particles/BlendDistort");
            if (Shader.Find("Hovl/Particles/VolumeLaser") != null) VolumeLaser = Shader.Find("Hovl/Particles/VolumeLaser");
            if (Shader.Find("Hovl/Particles/Explosion") != null) Explosion = Shader.Find("Hovl/Particles/Explosion");
            if (Shader.Find("Hovl/Particles/SwordSlash") != null) SwordSlash = Shader.Find("Hovl/Particles/SwordSlash");
            if (Shader.Find("Hovl/Particles/ShockWave") != null) ShockWave = Shader.Find("Hovl/Particles/ShockWave");
            if (Shader.Find("Hovl/Particles/SoftNoise") != null) SoftNoise = Shader.Find("Hovl/Particles/SoftNoise");

            if (Shader.Find("Shader Graphs/URP_LightGlow") != null) LightGlow_URP = Shader.Find("Shader Graphs/URP_LightGlow");
            if (Shader.Find("Shader Graphs/URP_Lit_CenterGlow") != null) Lit_CenterGlow_URP = Shader.Find("Shader Graphs/URP_Lit_CenterGlow");
            if (Shader.Find("Shader Graphs/URP_Blend_TwoSides") != null) Blend_TwoSides_URP = Shader.Find("Shader Graphs/URP_Blend_TwoSides");
            if (Shader.Find("Shader Graphs/URP_Blend_Normals") != null) Blend_Normals_URP = Shader.Find("Shader Graphs/URP_Blend_Normals");
            if (Shader.Find("Shader Graphs/URP_Ice") != null) Ice_URP = Shader.Find("Shader Graphs/URP_Ice");
            if (Shader.Find("Shader Graphs/URP_Distortion") != null) Distortion_URP = Shader.Find("Shader Graphs/URP_Distortion");
            if (Shader.Find("Shader Graphs/URP_ParallaxIce") != null) ParallaxIce_URP = Shader.Find("Shader Graphs/URP_ParallaxIce");
            if (Shader.Find("Shader Graphs/URP_Add_CG") != null) Add_CG_URP = Shader.Find("Shader Graphs/URP_Add_CG");
            if (Shader.Find("Shader Graphs/URP_Blend_CG") != null) Blend_CG_URP = Shader.Find("Shader Graphs/URP_Blend_CG");
            if (Shader.Find("Shader Graphs/URP_BlendDistort") != null) BlendDistort_URP = Shader.Find("Shader Graphs/URP_BlendDistort");
            if (Shader.Find("Shader Graphs/URP_VolumeLaser") != null) VolumeLaser_URP = Shader.Find("Shader Graphs/URP_VolumeLaser");
            if (Shader.Find("Shader Graphs/URP_Explosion") != null) Explosion_URP = Shader.Find("Shader Graphs/URP_Explosion");
            if (Shader.Find("Shader Graphs/URP_SwordSlash") != null) SwordSlash_URP = Shader.Find("Shader Graphs/URP_SwordSlash");
            if (Shader.Find("Shader Graphs/URP_ShockWave") != null) ShockWave_URP = Shader.Find("Shader Graphs/URP_ShockWave");
            if (Shader.Find("Shader Graphs/URP_SoftNoise") != null) SoftNoise_URP = Shader.Find("Shader Graphs/URP_SoftNoise");

            if (Shader.Find("Shader Graphs/HDRP_LightGlow") != null) LightGlow_HDRP = Shader.Find("Shader Graphs/HDRP_LightGlow");
            if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null) Lit_CenterGlow_HDRP = Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow");
            if (Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null) Blend_TwoSides_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_TwoSides");
            if (Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null) Blend_Normals_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_Normals");
            if (Shader.Find("Shader Graphs/HDRP_Ice") != null) Ice_HDRP = Shader.Find("Shader Graphs/HDRP_Ice");
            if (Shader.Find("Shader Graphs/HDRP_Distortion") != null) Distortion_HDRP = Shader.Find("Shader Graphs/HDRP_Distortion");
            if (Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null) ParallaxIce_HDRP = Shader.Find("Shader Graphs/HDRP_ParallaxIce");
            if (Shader.Find("Shader Graphs/HDRP_Add_CG") != null) Add_CG_HDRP = Shader.Find("Shader Graphs/HDRP_Add_CG");
            if (Shader.Find("Shader Graphs/HDRP_Blend_CG") != null) Blend_CG_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_CG");
            if (Shader.Find("Shader Graphs/HDRP_BlendDistort") != null) BlendDistort_HDRP = Shader.Find("Shader Graphs/HDRP_BlendDistort");
            if (Shader.Find("Shader Graphs/HDRP_VolumeLaser") != null) VolumeLaser_HDRP = Shader.Find("Shader Graphs/HDRP_VolumeLaser");
            if (Shader.Find("Shader Graphs/HDRP_Explosion") != null) Explosion_HDRP = Shader.Find("Shader Graphs/HDRP_Explosion");
            if (Shader.Find("Shader Graphs/HDRP_SwordSlash") != null) SwordSlash_HDRP = Shader.Find("Shader Graphs/HDRP_SwordSlash");
            if (Shader.Find("Shader Graphs/HDRP_ShockWave") != null) ShockWave_HDRP = Shader.Find("Shader Graphs/HDRP_ShockWave");
            if (Shader.Find("Shader Graphs/HDRP_SoftNoise") != null) SoftNoise_HDRP = Shader.Find("Shader Graphs/HDRP_SoftNoise");

            string[] folderMat = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            shaderMaterials = new Material[folderMat.Length];

            for (int i = 0; i < folderMat.Length; i++)
            {
                var patch = AssetDatabase.GUIDToAssetPath(folderMat[i]);
                shaderMaterials[i] = (Material)AssetDatabase.LoadAssetAtPath(patch, typeof(Material));
            }
        }

        private void ImportPipelinePackage()
        {
            switch (pipeline)
            {
                case 1:
                    string[] unityPackagesURP = AssetDatabase.FindAssets("Unity 2021+ URPHS");
                    foreach (var guid in unityPackagesURP)
                    {
                        AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(guid), false);
                    }
                    AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                    break;
                case 2:
                    string[] unityPackagesHD = AssetDatabase.FindAssets("Unity 2021+ HDRPHS");
                    foreach (var guid in unityPackagesHD)
                    {
                        AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(guid), false);
                    }
                    AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                    break;
                case 3:
                    string[] unityPackagesURP2D = AssetDatabase.FindAssets("Solves transparent problem Unity 2021+ URPHS");
                    foreach (var guid in unityPackagesURP2D)
                    {
                        AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(guid), false);
                    }
                    AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                    break;
                default:
                    Debug.Log("You didn't choose pipeline");
                    break;
            }
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            FindShaders();
            switch (pipeline)
            {
                case 1:
                    ChangeToURP();
                    break;
                case 2:
                    ChangeToHDRP();
                    break;
                case 3:
                    ChangeToURP();
                    break;
                default:
                    Debug.Log("You didn't choose pipeline");
                    break;
            }
        }

        static private void ChangeToURP()
        {
            foreach (var material in shaderMaterials)
            {
                if (Shader.Find("Shader Graphs/URP_LightGlow") != null)
                {
                    if (material.shader == LightGlow || material.shader == LightGlow_HDRP)
                    {
                        material.shader = LightGlow_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Lit_CenterGlow") != null)
                {
                    if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_HDRP)
                    {
                        material.shader = Lit_CenterGlow_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Blend_TwoSides") != null)
                {
                    if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_HDRP)
                    {
                        material.shader = Blend_TwoSides_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Blend_Normals") != null)
                {
                    if (material.shader == Blend_Normals || material.shader == Blend_Normals_HDRP)
                    {
                        material.shader = Blend_Normals_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Ice") != null)
                {
                    if (material.shader == Ice || material.shader == Ice_HDRP)
                    {
                        material.shader = Ice_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_ParallaxIce") != null)
                {
                    if (material.shader == ParallaxIce || material.shader == ParallaxIce_HDRP)
                    {
                        if (material.GetTexture("_Emission") != null)
                        {
                            material.shader = ParallaxIce_URP;
                        }
                        else
                            material.shader = ParallaxIce_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Distortion") != null)
                {
                    if (material.shader == Distortion || material.shader == Distortion_HDRP)
                    {
                        material.SetFloat("_ZWrite", 0);
                        material.shader = Distortion_URP;
                        material.SetFloat("_QueueControl", 1);
                        material.renderQueue = 2750;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Add_CG") != null)
                {
                    if (material.shader == Add_CG || material.shader == Add_CG_HDRP)
                    {
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        material.shader = Add_CG_URP;       
                        if (material.HasProperty("_CullMode"))
                        {
                            var cull = material.GetFloat("_CullMode");
                            material.SetFloat("_Cull", cull);
                        }
                        Debug.Log("Shaders changed successfully");
                    }
                }
                else Debug.Log("First import shaders!");

                if (Shader.Find("Shader Graphs/URP_Blend_CG") != null)
                {
                    if (material.shader == Blend_CG || material.shader == Blend_CG_HDRP)
                    {
                        if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                        material.shader = Blend_CG_URP;
                        if (material.HasProperty("_CullMode"))
                        {
                            var cull = material.GetFloat("_CullMode");
                            material.SetFloat("_Cull", cull);
                        }
                    }
                }

                if (Shader.Find("Shader Graphs/URP_BlendDistort") != null)
                {
                    if (material.shader == BlendDistort || material.shader == BlendDistort_HDRP)
                    {
                            material.shader = BlendDistort_URP;
                            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                    }
                }

                if (Shader.Find("Shader Graphs/URP_VolumeLaser") != null)
                {
                    if (material.shader == VolumeLaser || material.shader == VolumeLaser_HDRP)
                    {
                        material.shader = VolumeLaser_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_Explosion") != null)
                {
                    if (material.shader == Explosion || material.shader == Explosion_HDRP)
                    {
                        material.shader = Explosion_URP;
                    }
                }

                if (Shader.Find("Shader Graphs/URP_SwordSlash") != null)
                {
                    if (material.shader == SwordSlash || material.shader == SwordSlash_HDRP)
                    {

                            material.shader = SwordSlash_URP;
                            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                    }
                }

                if (Shader.Find("Shader Graphs/URP_ShockWave") != null)
                {
                    if (material.shader == ShockWave || material.shader == ShockWave_HDRP)
                    {
                            material.shader = ShockWave_URP;
                            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);

                    }
                }

                if (Shader.Find("Shader Graphs/URP_SoftNoise") != null)
                {
                    if (material.shader == SoftNoise || material.shader == SoftNoise_HDRP)
                    {
                            material.shader = SoftNoise_URP;
                            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);

                    }
                }
            }
        }

        static private void ChangeToSRP()
        {

            foreach (var material in shaderMaterials)
            {
                if (Shader.Find("Hovl/Particles/LightGlow") != null)
                {
                    if (material.shader == LightGlow_URP || material.shader == LightGlow_HDRP)
                    {
                        material.shader = LightGlow;
                    }
                }

                if (Shader.Find("Hovl/Particles/Lit_CenterGlow") != null)
                {
                    if (material.shader == Lit_CenterGlow_URP || material.shader == Lit_CenterGlow_HDRP)
                    {
                        material.shader = Lit_CenterGlow;
                    }
                }

                if (Shader.Find("Hovl/Particles/Blend_TwoSides") != null)
                {
                    if (material.shader == Blend_TwoSides_URP || material.shader == Blend_TwoSides_HDRP)
                    {
                            material.shader = Blend_TwoSides;
                    }
                }

                if (Shader.Find("Hovl/Particles/Blend_Normals") != null)
                {
                    if (material.shader == Blend_Normals_URP || material.shader == Blend_Normals_HDRP)
                    {
                            material.shader = Blend_Normals;
                    }
                }

                if (Shader.Find("Hovl/Particles/Ice") != null)
                {
                    if (material.shader == Ice_URP || material.shader == Ice_HDRP)
                    {
                            material.shader = Ice;
                    }
                }

                if (Shader.Find("Hovl/Opaque/ParallaxIce") != null)
                {
                    if (material.shader == ParallaxIce_URP || material.shader == ParallaxIce_HDRP)
                    {
                            material.shader = ParallaxIce;
                    }
                }

                if (Shader.Find("Hovl/Particles/Distortion") != null)
                {
                    if (material.shader == Distortion_URP || material.shader == Distortion_HDRP)
                    {
                        material.shader = Distortion;
                        material.renderQueue = 2750;
                    }
                }

                if (Shader.Find("Hovl/Particles/Add_CenterGlow") != null)
                {
                    if (material.shader == Add_CG_URP || material.shader == Add_CG_HDRP)
                    {
                        material.shader = Add_CG;
                        Debug.Log("Shaders changed successfully");
                    }
                }
                else Debug.Log("First import shaders!");

                if (Shader.Find("Hovl/Particles/Blend_CenterGlow") != null)
                {
                    if (material.shader == Blend_CG_URP || material.shader == Blend_CG_HDRP)
                    {
                        material.shader = Blend_CG;
                    }
                }

                if (Shader.Find("Hovl/Particles/BlendDistort") != null)
                {
                    if (material.shader == BlendDistort_URP || material.shader == BlendDistort_HDRP)
                    {
                        material.shader = BlendDistort;
                    }
                }

                if (Shader.Find("Hovl/Particles/VolumeLaser") != null)
                {
                    if (material.shader == VolumeLaser_URP || material.shader == VolumeLaser_HDRP)
                    {
                        material.shader = VolumeLaser;
                    }
                }

                if (Shader.Find("Hovl/Particles/Explosion") != null)
                {
                    if (material.shader == Explosion_URP || material.shader == Explosion_HDRP)
                    {
                        material.shader = Explosion;
                    }
                }

                if (Shader.Find("Hovl/Particles/SwordSlash") != null)
                {
                    if (material.shader == SwordSlash_URP || material.shader == SwordSlash_HDRP)
                    {
                        material.shader = SwordSlash;
                    }
                }

                if (Shader.Find("Hovl/Particles/ShockWave") != null)
                {
                    if (material.shader == ShockWave_URP || material.shader == ShockWave_HDRP)
                    {
                        material.shader = ShockWave;
                    }
                }

                if (Shader.Find("Hovl/Particles/SoftNoise") != null)
                {
                    if (material.shader == SoftNoise_URP || material.shader == SoftNoise_HDRP)
                    {
                        material.shader = SoftNoise;
                    }
                }
            }
        }

        static private void ChangeToHDRP()
        {
            foreach (var material in shaderMaterials)
            {
                if (Shader.Find("Shader Graphs/HDRP_LightGlow") != null)
                {
                    if (material.shader == LightGlow || material.shader == LightGlow_URP)
                    {
                        material.shader = LightGlow_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null)
                {
                    if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_URP)
                    {
                        material.shader = Lit_CenterGlow_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null)
                {
                    if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_URP)
                    {
                        material.shader = Blend_TwoSides_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null)
                {
                    if (material.shader == Blend_Normals || material.shader == Blend_Normals_URP)
                    {
                        material.shader = Blend_Normals_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Ice") != null)
                {
                    if (material.shader == Ice || material.shader == Ice_URP)
                    {
                        material.shader = Ice_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null)
                {
                    if (material.shader == ParallaxIce || material.shader == ParallaxIce_URP)
                    {
                        material.shader = ParallaxIce_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Distortion") != null)
                {
                    if (material.shader == Distortion || material.shader == Distortion_URP)
                    {
                        material.SetFloat("_ZWrite", 0);
                        material.shader = Distortion_HDRP;
                        material.renderQueue = 2750;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Add_CG") != null)
                {
                    if (material.shader == Add_CG || material.shader == Add_CG_URP)
                    {
                        if (material.HasProperty("_CullMode"))
                        {
                            var cull = material.GetFloat("_CullMode");
                            if (cull == 0)
                                material.SetFloat("_DoubleSidedEnable", 1);
                            else
                                material.SetFloat("_DoubleSidedEnable", 0);
                        }
                        material.SetFloat("_StencilRef", 0);
                        material.SetFloat("_AlphaDstBlend", 1);
                        material.SetFloat("_DstBlend", 1);
                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_SrcBlend", 1);
                        material.EnableKeyword("_BLENDMODE_ADD _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                        material.SetShaderPassEnabled("TransparentBackface", false);
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetFloat("_CullModeForward", 0);
                        material.shader = Add_CG_HDRP;
                        Debug.Log("Shaders changed successfully");
                    }
                }
                else Debug.Log("First import shaders!");

                if (Shader.Find("Shader Graphs/HDRP_Blend_CG") != null)
                {
                    if (material.shader == Blend_CG || material.shader == Blend_CG_URP)
                    {
                        if (material.HasProperty("_CullMode"))
                        {
                            var cull = material.GetFloat("_CullMode");
                            if (cull == 0)
                                material.SetFloat("_DoubleSidedEnable", 1);
                            else
                                material.SetFloat("_DoubleSidedEnable", 0);
                        }
                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_StencilRef", 0);
                        material.SetShaderPassEnabled("TransparentBackface", false);
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetFloat("_AlphaDstBlend", 10);
                        material.SetFloat("_DstBlend", 10);
                        material.SetFloat("_SrcBlend", 1);
                        material.EnableKeyword("_BLENDMODE_ALPHA _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                        if (material.HasProperty("_CullModeForward")) material.SetFloat("_CullModeForward", 0);
                        material.shader = Blend_CG_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_BlendDistort") != null)
                {
                    if (material.shader == BlendDistort || material.shader == BlendDistort_URP)
                    {
                        material.shader = BlendDistort_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_VolumeLaser") != null)
                {
                    if (material.shader == VolumeLaser || material.shader == VolumeLaser_URP)
                    {
                        material.shader = VolumeLaser_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_Explosion") != null)
                {
                    if (material.shader == Explosion || material.shader == Explosion_URP)
                    {
                        material.SetFloat("_StencilRef", 0);
                        material.SetFloat("_AlphaDstBlend", 1);
                        material.SetFloat("_DstBlend", 1);
                        material.SetFloat("_ZWrite", 0);
                        material.SetFloat("_SrcBlend", 1);
                        material.EnableKeyword("_BLENDMODE_ADD _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                        material.SetShaderPassEnabled("TransparentBackface", false);
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetFloat("_CullModeForward", 0);
                        material.shader = Explosion_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_SwordSlash") != null)
                {
                    if (material.shader == SwordSlash || material.shader == SwordSlash_URP)
                    {
                            material.SetFloat("_ZWrite", 0);
                            material.SetFloat("_StencilRef", 0);
                            material.SetShaderPassEnabled("TransparentBackface", false);
                            material.SetOverrideTag("RenderType", "Transparent");
                            material.SetFloat("_AlphaDstBlend", 10);
                            material.SetFloat("_DstBlend", 10);
                            material.SetFloat("_SrcBlend", 1);
                            material.EnableKeyword("_BLENDMODE_ALPHA _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                            material.shader = SwordSlash_HDRP;
                            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0);
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_ShockWave") != null)
                {
                    if (material.shader == ShockWave || material.shader == ShockWave_URP)
                    {
                            material.SetFloat("_StencilRef", 0);
                            material.SetFloat("_AlphaDstBlend", 1);
                            material.SetFloat("_DstBlend", 1);
                            material.SetFloat("_ZWrite", 0);
                            material.SetFloat("_SrcBlend", 1);
                            material.EnableKeyword("_BLENDMODE_ADD _DOUBLESIDED_ON _SURFACE_TYPE_TRANSPARENT");
                            material.SetShaderPassEnabled("TransparentBackface", false);
                            material.SetOverrideTag("RenderType", "Transparent");
                            material.SetFloat("_CullModeForward", 0);
                            material.shader = ShockWave_HDRP;
                    }
                }

                if (Shader.Find("Shader Graphs/HDRP_SoftNoise") != null)
                {
                    if (material.shader == SoftNoise || material.shader == SoftNoise_URP)
                    {
                        if (material.HasProperty("_CullMode"))
                        {
                            var cull = material.GetFloat("_CullMode");
                            if (cull == 0)
                                material.SetFloat("_DoubleSidedEnable", 1);
                            else
                                material.SetFloat("_DoubleSidedEnable", 0);
                        }
                        material.shader = SoftNoise_HDRP;
                    }
                }
            }
        }
    }
}
#endif