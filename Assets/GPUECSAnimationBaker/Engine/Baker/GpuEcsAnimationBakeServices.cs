#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using GpuEcsAnimationBaker.Engine.Data;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.Baker
{
    public static class GpuEcsAnimationBakerServices
    {
        private static readonly int AnimatedBoneMatrices = Shader.PropertyToID("_AnimatedBoneMatrices");
        private static readonly int EnableAnimation = Shader.PropertyToID("_EnableAnimation");

        public static bool ValidateAnimationBakerData(GpuEcsAnimationBakerData bakerData, GameObject sourceModel, out string errors)
        {
            StringBuilder sbErrors = new StringBuilder();
            if (bakerData.animations.Length == 0)
                sbErrors.AppendLine("At least one animation must be baked.");

            foreach (AnimationData animation in bakerData.animations)
            {
                if (string.IsNullOrWhiteSpace(animation.animationID))
                    sbErrors.AppendLine("Animation ID is mandatory");
                if (!Regex.IsMatch(animation.animationID, @"^[a-zA-Z_][a-zA-Z0-9_]+$"))
                    sbErrors.AppendLine("Animation ID must be only letters, numbers or underscore, must not start with number");
                if (string.IsNullOrWhiteSpace(animation.animatorStateName))
                    sbErrors.AppendLine("Animation State Name is mandatory");
                if (animation.animationType == AnimationTypes.SingleClip)
                {
                    if (animation.singleClipData.animationClip == null)
                        sbErrors.AppendLine("Animation Clip is mandatory");
                }
                else if (animation.animationType == AnimationTypes.DualClipBlend)
                {
                    if (string.IsNullOrWhiteSpace(animation.dualClipBlendData.blendParameterName))
                        sbErrors.AppendLine("Blend parameter name is mandatory");
                    if (animation.dualClipBlendData.clip1.animationClip == null)
                        sbErrors.AppendLine("Animation Clip 1 is mandatory");
                    if (animation.dualClipBlendData.clip1.animationClip == null)
                        sbErrors.AppendLine("Animation Clip 2 is mandatory");
                    if (animation.dualClipBlendData.nbrOfInBetweenSamples < 2)
                        sbErrors.AppendLine("Nbr of in between samples must be at least 2");
                    if (animation.dualClipBlendData.nbrOfInBetweenSamples > 100)
                        sbErrors.AppendLine("Nbr of in between samples is maximum 100");
                }

                foreach (AnimatorParameter parameterValue in animation.additionalAnimatorParameterValues)
                {
                    if (string.IsNullOrWhiteSpace(parameterValue.parameterName))
                        sbErrors.AppendLine("Additional animator parameter name is mandatory");
                }
            }

            bool foundDouble = false;
            for (int i = 0; i < bakerData.animations.Length; i++)
            {
                for (int j = i + 1; j < bakerData.animations.Length; j++)
                {
                    if (bakerData.animations[i].animationID == bakerData.animations[j].animationID)
                    {
                        foundDouble = true; break;
                    }
                }
                if(foundDouble) break;
            }
            if(foundDouble) sbErrors.AppendLine("Animation IDs must be unique");

            foreach (AttachmentAnchor attachmentAnchor in bakerData.attachmentAnchors)
            {
                if (string.IsNullOrWhiteSpace(attachmentAnchor.attachmentAnchorID))
                    sbErrors.AppendLine("Attachment Anchor ID is mandatory");
                if(attachmentAnchor.attachmentAnchorTransform == null)
                    sbErrors.AppendLine("Attachment Anchor reference transform is mandatory");
                else if(!hasParent(attachmentAnchor.attachmentAnchorTransform, sourceModel.transform))
                    sbErrors.AppendLine($"Attachment Anchor reference transform must be nested inside {sourceModel.name}");
            }
            
            if(bakerData.boneUsage.numberOfBonesPerVertex < 1) 
                sbErrors.AppendLine("Nbr of bones per vertex must be at least 1");
            foreach (BoneUsagePerLoD boneUsagePerLOD in bakerData.boneUsage.boneUsagesPerLoD)
            {
                if(boneUsagePerLOD.maxNumberOfBonesPerVertex < 1) 
                    sbErrors.AppendLine("Nbr of bones per vertex must be at least 1");
            }
            
            errors = sbErrors.ToString();
            return sbErrors.Length == 0;
        }

        private static bool hasParent(Transform child, Transform parent)
        {
            if (child.parent == null) return false;
            if (child.parent.gameObject == parent.gameObject) return true;
            else return hasParent(child.parent, parent);
        }

        public static GameObject GenerateAnimationObject(string assetPath, 
            GpuEcsAnimationBakerData bakerData, string animatorName, string generatedAssetsFolder, 
            string nameSuffixAsset = "_GpuEcsAnimator",
            string nameSuffixAnimationIDsEnum = "_AnimationIDs",
            string nameSuffixAnimationInitializerBehaviour = "_AnimationInitializerBehaviour",
            string nameSuffixAnimationEventIDsEnum = "_AnimationEventIDs",
            string nameSuffixAnimationAnchorIDsEnum = "_AttachmentAnchorIDs",
            string nameSuffixAttachmentInitializerBehaviour = "_AttachmentInitializerBehaviour",
            string meshPartSuffix = "Mesh",
            string animationMatricesTexturePartSuffix = "AnimationMatricesTexture", 
            string materialPartSuffix = "Material"
            )
        {
            GameObject sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject refModel = PrefabUtility.LoadPrefabContents(assetPath);

            Debug.Log($"Generating Animation object for {assetPath}");
            GameObject animationObject = GenerateAnimationObjectFromModel(refModel, sourceModel, bakerData, animatorName, generatedAssetsFolder,
                nameSuffixAsset, nameSuffixAnimationIDsEnum, nameSuffixAnimationInitializerBehaviour, nameSuffixAnimationEventIDsEnum,
                nameSuffixAnimationAnchorIDsEnum, nameSuffixAttachmentInitializerBehaviour, meshPartSuffix, animationMatricesTexturePartSuffix, materialPartSuffix);
            PrefabUtility.UnloadPrefabContents(refModel);
            
            return animationObject;
        }
        
        public static GameObject GenerateAnimationObjectFromModel(GameObject refModel, GameObject sourceModel, 
            GpuEcsAnimationBakerData bakerData, string animatorName, string generatedAssetsFolder,
            string nameSuffixAsset = "_GpuEcsAnimator",
            string nameSuffixAnimationIDsEnum = "_AnimationIDs",
            string nameSuffixAnimationInitializerBehaviour = "_AnimationInitializerBehaviour",
            string nameSuffixAnimationEventIDsEnum = "_AnimationEventIDs",
            string nameSuffixAnimationAnchorIDsEnum = "_AttachmentAnchorIDs",
            string nameSuffixAttachmentInitializerBehaviour = "_AttachmentInitializerBehaviour",
            string meshPartSuffix = "Mesh",
            string animationMatricesTexturePartSuffix = "AnimationMatricesTexture", 
            string materialPartSuffix = "Material"
            ) 
        {
            if(!ValidateAnimationBakerData(bakerData, sourceModel, out string errors))
            {
                Debug.LogError(errors);
                return null;
            }

            string targetAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAsset}.prefab");
            string targetAnimationEnumAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAnimationIDsEnum}.cs");
            string targetAnimationInitializerBehaviourAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAnimationInitializerBehaviour}.cs");
            string targetAnimationEventEnumAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAnimationEventIDsEnum}.cs");
            string targetAttachmentAnchorEnumAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAnimationAnchorIDsEnum}.cs");
            string targetAttachmentInitializerBehaviourAssetPath =
                Path.Combine(generatedAssetsFolder, $"{animatorName}{nameSuffixAttachmentInitializerBehaviour}.cs");
            
            refModel.transform.position = Vector3.zero;
            refModel.transform.rotation = quaternion.identity;
            Animator refModelAnimator = refModel.GetComponent<Animator>();
            refModelAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            LODGroup refModelLoDGroup = refModel.GetComponent<LODGroup>();
            LOD[] refModelLoDs = refModelLoDGroup == null ? null : refModelLoDGroup.GetLODs();
            List<AnimationMatricesTexture> animationMatricesTexturesCache = new List<AnimationMatricesTexture>();
            List<EcsGpuMaterial> ecsGpuMaterialsCache = new List<EcsGpuMaterial>();

            GameObject test = AssetDatabase.LoadAssetAtPath<GameObject>(targetAssetPath);
            if (test == null)
            {
                GameObject empty = new GameObject(refModel.name);
                PrefabUtility.SaveAsPrefabAsset(empty, targetAssetPath);
                GameObject.DestroyImmediate(empty, allowDestroyingAssets:true);
            }
            GameObject target = PrefabUtility.LoadPrefabContents(targetAssetPath);
            target.transform.localScale = refModel.transform.localScale;
            GameObject[] toDestroy = new GameObject[target.transform.childCount];
            for (int childIndex = 0; childIndex < target.transform.childCount; childIndex++)
                toDestroy[childIndex] = target.transform.GetChild(childIndex).gameObject;
            foreach(GameObject go in toDestroy) Object.DestroyImmediate(go, allowDestroyingAssets: true);
            RemoveComponent<MeshRenderer>(target);
            RemoveComponent<MeshFilter>(target);
            RemoveComponent<LODGroup>(target);
            RemoveComponent<GpuEcsAnimatorBehaviour>(target);
            RemoveComponent<GpuEcsAnimatedMeshBehaviour>(target);
            
            GpuEcsAnimatorBehaviour gpuEcsAnimator = AddGpuEcsAnimationBehaviour(target, bakerData, out string[] eventNames);
            gpuEcsAnimator.transformUsageFlags = bakerData.transformUsageFlagsParent;

            Dictionary<GameObject, GameObject> sourceToTargetMapping = new Dictionary<GameObject, GameObject>();
            ProcessAnimationObjectRecursive(refModel, target, sourceToTargetMapping, animatorName,
                meshPartSuffix, animationMatricesTexturePartSuffix, materialPartSuffix,
                refModelAnimator, refModelLoDs, animationMatricesTexturesCache, ecsGpuMaterialsCache,
                bakerData, gpuEcsAnimator.animations, gpuEcsAnimator.totalNbrOfFrames, generatedAssetsFolder, gpuEcsAnimator);
            if (refModelLoDs != null)
            {
                LODGroup targetLoDGroup = target.AddComponent<LODGroup>();
                LOD[] targetLoDs = new LOD[refModelLoDs.Length];
                for (int lodIndex = 0; lodIndex < refModelLoDs.Length; lodIndex++)
                {
                    targetLoDs[lodIndex] = refModelLoDs[lodIndex];
                    Renderer[] refModelRenderers = refModelLoDs[lodIndex].renderers;
                    for (int rendererIndex = 0; rendererIndex < refModelRenderers.Length; rendererIndex++)
                    {
                        targetLoDs[lodIndex].renderers[rendererIndex] =
                            sourceToTargetMapping[refModelRenderers[rendererIndex].gameObject].GetComponent<MeshRenderer>();
                    }
                }
                targetLoDGroup.SetLODs(targetLoDs);
            }
            
            if(bakerData.generateAnimationIdsEnum)
                GenerateAnimationsEnumCode(bakerData, targetAnimationEnumAssetPath, targetAnimationInitializerBehaviourAssetPath);
            
            if(bakerData.generateAnimationEventIdsEnum)
                GenerateAnimationEventsEnumCode(bakerData, targetAnimationEventEnumAssetPath, eventNames );

            gpuEcsAnimator.nbrOfAttachmentAnchors = bakerData.attachmentAnchors.Length;
            gpuEcsAnimator.attachmentAnchorData = BakeAttachmentAnchorTransforms(generatedAssetsFolder, sourceModel, animatorName, 
                refModelAnimator, bakerData, gpuEcsAnimator.animations, gpuEcsAnimator.totalNbrOfFrames);
            
            if(bakerData.generateAttachmentAnchorIdsEnum)
                GenerateAttachmentAnchorsEnumCode(bakerData, targetAttachmentAnchorEnumAssetPath, targetAttachmentInitializerBehaviourAssetPath);
            
            PrefabUtility.SaveAsPrefabAsset(target, targetAssetPath);
            PrefabUtility.UnloadPrefabContents(target);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(targetAssetPath);
        }

        private const string enumFileTemplate = "namespace GPUECSAnimationBaker.Engine.AnimatorSystem\n"
                                                + "{\n"
                                                + "    public enum @ENUMNAME@\n"
                                                + "    {\n"
                                                + "@IDLIST@\n"
                                                + "    }\n"
                                                + "}";

        private const string behaviourFileTemplate = "namespace GPUECSAnimationBaker.Engine.AnimatorSystem\n"
                                                + "{\n"
                                                + "    public class @CLASSNAME_@ENUMNAME@_Behaviour : @CLASSNAMEBehaviour<@ENUMNAME@> { } \n"
                                                + "}";
        
        private static void GenerateAnimationsEnumCode(GpuEcsAnimationBakerData bakerData, string targetEnumAssetPath, string targetEnumBehaviourAssetPath)
        {
            GenerateEnumCodeFiles<AnimationData>($"AnimationIds{bakerData.animationIdsEnumName}", "GpuEcsAnimatorInitializer",
                bakerData.animations, (animationData) => animationData.animationID, targetEnumAssetPath, targetEnumBehaviourAssetPath);
        }

        private static void GenerateAnimationEventsEnumCode(GpuEcsAnimationBakerData bakerData, string targetEnumAssetPath, string[] eventNames)
        {
            GenerateEnumCodeFiles<string>($"AnimationEventIds{bakerData.animationEventIdsEnumName}", null,
                eventNames, (name) => name, targetEnumAssetPath, null);
        }
        
        private static void GenerateAttachmentAnchorsEnumCode(GpuEcsAnimationBakerData bakerData, string targetEnumAssetPath, string targetEnumBehaviourAssetPath)
        {
            GenerateEnumCodeFiles<AttachmentAnchor>($"AnchorIds{bakerData.attachmentAnchorIdsEnumName}", "GpuEcsAttachmentInitializer", 
                bakerData.attachmentAnchors, (attachmentAnchor) => attachmentAnchor.attachmentAnchorID, targetEnumAssetPath, targetEnumBehaviourAssetPath);
        }
        
        private static void GenerateEnumCodeFiles<T>(string enumName, string className, T[] list, System.Func<T, string> listIdGetter, 
            string targetEnumAssetPath, string targetEnumBehaviourAssetPath)
        {
            StringBuilder idList = new StringBuilder();
            for (int index = 0; index < list.Length; index++)
            {
                idList.Append($"        {listIdGetter(list[index])} = {index.ToString()}");
                if (index < list.Length - 1)
                {
                    idList.AppendLine(",");
                }
            }
            
            string enumCodeText = enumFileTemplate
                .Replace("@ENUMNAME@", enumName)
                .Replace("@IDLIST@", idList.ToString());
            string enumCodePath = Path.Combine(Application.dataPath, "../" + targetEnumAssetPath); 
            File.WriteAllText(enumCodePath, enumCodeText);

            if (targetEnumBehaviourAssetPath != null)
            {
                string enumBehaviourText = behaviourFileTemplate
                    .Replace("@CLASSNAME", className)
                    .Replace("@ENUMNAME@", enumName);
                string enumBehaviourPath = Path.Combine(Application.dataPath, "../" + targetEnumBehaviourAssetPath);
                File.WriteAllText(enumBehaviourPath, enumBehaviourText);
            }

            AssetDatabase.Refresh();
        }

        private static GpuEcsAnimatorBehaviour AddGpuEcsAnimationBehaviour(GameObject target,
            GpuEcsAnimationBakerData bakerData, out string[] eventNames)
        {
            GpuEcsAnimatorBehaviour gpuEcsAnimator = target.AddComponent<GpuEcsAnimatorBehaviour>();
            gpuEcsAnimator.animations = new GpuEcsAnimationData[bakerData.animations.Length];
            List<GpuEcsAnimationEventOccurence> occurences = new List<GpuEcsAnimationEventOccurence>();
            List<string> foundEvents = new List<string>();

            int currentFrameIndex = 0;
            int currentEventOccurenceId = 0;
            int currentFoundEventId = 0;
            for (int animationIndex = 0; animationIndex < bakerData.animations.Length; animationIndex++)
            {
                AnimationData animationData = bakerData.animations[animationIndex];
                int nbrOfFramesPerSample = 0;
                int nbrOfInBetweenSamples = 0;
                float blendTimeCorrection = 1;
                int startEventOccurenceId = currentEventOccurenceId;
                int nbrOfEventOccurenceIds = 0;
                if (animationData.animationType == AnimationTypes.SingleClip)
                {
                    SingleClipData singleClipData = animationData.singleClipData;
                    nbrOfFramesPerSample = (int)(singleClipData.animationClip.length * GlobalConstants.SampleFrameRate) + 1;
                    nbrOfInBetweenSamples = 1;
                    blendTimeCorrection = 1;
                    AddAnimationEvents(bakerData, animationData, occurences, ref currentEventOccurenceId, ref nbrOfEventOccurenceIds, foundEvents, 
                        ref currentFoundEventId, singleClipData.animationClip);
                }
                else if (animationData.animationType == AnimationTypes.DualClipBlend)
                {
                    DualClipBlendData dualClipBlendData = animationData.dualClipBlendData;
                    int clip1NbrOfFrames = (int)(dualClipBlendData.clip1.animationClip.length * GlobalConstants.SampleFrameRate) + 1;
                    int clip2NbrOfFrames = (int)(dualClipBlendData.clip2.animationClip.length * GlobalConstants.SampleFrameRate) + 1;
                    nbrOfFramesPerSample = math.max(clip1NbrOfFrames, clip2NbrOfFrames);
                    blendTimeCorrection = dualClipBlendData.clip1.animationClip.length / dualClipBlendData.clip2.animationClip.length;
                    nbrOfInBetweenSamples = dualClipBlendData.nbrOfInBetweenSamples;
                    AddAnimationEvents(bakerData, animationData, occurences, ref currentEventOccurenceId, ref nbrOfEventOccurenceIds, foundEvents, 
                        ref currentFoundEventId, dualClipBlendData.clip1.animationClip);
                    AddAnimationEvents(bakerData, animationData, occurences, ref currentEventOccurenceId, ref nbrOfEventOccurenceIds, foundEvents, 
                        ref currentFoundEventId, dualClipBlendData.clip2.animationClip);
                }
                bool loop = animationData.loop;
                
                gpuEcsAnimator.animations[animationIndex] = new GpuEcsAnimationData()
                {
                    startFrameIndex = currentFrameIndex,
                    nbrOfFramesPerSample = nbrOfFramesPerSample,
                    nbrOfInBetweenSamples = nbrOfInBetweenSamples,
                    blendTimeCorrection = blendTimeCorrection,
                    startEventOccurenceId = startEventOccurenceId,
                    nbrOfEventOccurenceIds = nbrOfEventOccurenceIds,
                    loop = loop
                };
                currentFrameIndex += nbrOfFramesPerSample * nbrOfInBetweenSamples;
            }
            gpuEcsAnimator.totalNbrOfFrames = currentFrameIndex;
            gpuEcsAnimator.animationEventOccurences = occurences.ToArray();
            eventNames = bakerData.usePredefinedAnimationEventIds ? bakerData.predefinedAnimationEventIds : foundEvents.ToArray();
            return gpuEcsAnimator;
        }

        private static void AddAnimationEvents(GpuEcsAnimationBakerData bakerData, AnimationData animationData, 
            List<GpuEcsAnimationEventOccurence> occurences, 
            ref int currentEventOccurenceId, ref int nbrOfEventOccurenceIds, 
            List<string> foundEvents, ref int currentFoundEventId, AnimationClip animationClip)
        {
            AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(animationClip);
            foreach (AnimationEvent animationEvent in animationEvents)
            {
                int eventId = -1;
                if (bakerData.usePredefinedAnimationEventIds)
                {
                    eventId = bakerData.predefinedAnimationEventIds.ToList().FindIndex((n) => n.Equals(animationEvent.stringParameter));
                    if(eventId == -1) Debug.LogWarning(
                        $"Found event {animationEvent.stringParameter} that is not in the predefined event Ids list, so it will be ignored.");
                }
                else
                {
                    string name = $"{animationData.animationID}_{animationEvent.stringParameter}";
                    eventId = foundEvents.FindIndex((n) => n.Equals(name));
                    if (eventId == -1)
                    {
                        eventId = currentFoundEventId;
                        foundEvents.Add(name);
                        currentFoundEventId++;
                    }
                }

                if (eventId != -1)
                {
                    occurences.Add(new GpuEcsAnimationEventOccurence()
                    {
                        eventId = eventId,
                        eventNormalizedTime = animationEvent.time / animationClip.length,
                    });
                    currentEventOccurenceId++;
                    nbrOfEventOccurenceIds++;
                }
            }
        }
        
        private static void ProcessAnimationObjectRecursive(GameObject sourceNode, GameObject targetNode, 
            Dictionary<GameObject, GameObject> sourceToTargetMapping, string animatorName, string meshPartSuffix,
            string animationMatricesTexturePartSuffix, string materialPartSuffix,
            Animator refModelAnimator, LOD[] refModelLoDs, 
            List<AnimationMatricesTexture> animationMatricesTexturesCache, List<EcsGpuMaterial> ecsGpuMaterialsCache, 
            GpuEcsAnimationBakerData bakerData, GpuEcsAnimationData[] animations, int totalNbrOfFrames,
            string generatedAssetsFolder, GpuEcsAnimatorBehaviour gpuEcsAnimator)
        {
            sourceToTargetMapping.Add(sourceNode, targetNode);
            SkinnedMeshRenderer existingSkinnedMeshRenderer = sourceNode.GetComponent<SkinnedMeshRenderer>();
            if (existingSkinnedMeshRenderer != null)
            {
                Texture2D animationMatricesTexture = BakeAndSaveAnimationMatricesTexture(
                    animationMatricesTexturesCache, generatedAssetsFolder, animatorName, animationMatricesTexturePartSuffix,
                    refModelAnimator, bakerData, existingSkinnedMeshRenderer, animations, totalNbrOfFrames);
                Mesh newMesh = BakeAndSaveBoneWeightsIntoMesh(refModelLoDs, generatedAssetsFolder, animatorName,
                    meshPartSuffix, bakerData, existingSkinnedMeshRenderer);

                MeshFilter meshFilter = targetNode.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = targetNode.AddComponent<MeshRenderer>();
                Material newMaterial = CreateAndSaveEcsGpuMaterial(ecsGpuMaterialsCache, generatedAssetsFolder,
                    animatorName, materialPartSuffix, existingSkinnedMeshRenderer);
                newMaterial.SetTexture(AnimatedBoneMatrices, animationMatricesTexture);
                meshRenderer.sharedMaterial = newMaterial;
                meshFilter.sharedMesh = newMesh;

                GpuEcsAnimatedMeshBehaviour gpuEcsAnimatedMesh = targetNode.AddComponent<GpuEcsAnimatedMeshBehaviour>();
                gpuEcsAnimatedMesh.animator = gpuEcsAnimator;
                gpuEcsAnimatedMesh.transformUsageFlags = bakerData.transformUsageFlagsChildren;
            }

            for (int childIndex = 0; childIndex < sourceNode.transform.childCount; childIndex++)
            {
                GameObject sourceChild = sourceNode.transform.GetChild(childIndex).gameObject;
                if (sourceChild.gameObject.activeSelf && !CheckEmptyRecursive(sourceChild))
                {
                    GameObject targetChild = new GameObject(sourceChild.name);
                    targetChild.transform.parent = targetNode.transform;
                    targetChild.transform.localPosition = sourceChild.transform.localPosition;
                    targetChild.transform.localRotation = sourceChild.transform.localRotation;
                    targetChild.transform.localScale = sourceChild.transform.localScale;
                    ProcessAnimationObjectRecursive(sourceChild, targetChild, sourceToTargetMapping,
                        animatorName, meshPartSuffix, animationMatricesTexturePartSuffix, materialPartSuffix,
                        refModelAnimator, refModelLoDs, animationMatricesTexturesCache, ecsGpuMaterialsCache,
                        bakerData, animations, totalNbrOfFrames, generatedAssetsFolder, gpuEcsAnimator);
                }
            }
        }

        private static void RemoveComponent<T>(GameObject gameObject)  where T : Component 
        {
            T component = gameObject.GetComponent<T>();
            if(component != null) Object.DestroyImmediate(component, allowDestroyingAssets:true);
        }
        
        private static bool CheckEmptyRecursive(GameObject node)
        {
            Component[] components = node.GetComponents<Component>();
            bool empty = !components.Any(c => (c is SkinnedMeshRenderer));
            if (empty)
            {
                for (int childIndex = 0; childIndex < node.transform.childCount; childIndex++)
                {
                    empty = CheckEmptyRecursive(node.transform.GetChild(childIndex).gameObject);
                    if (!empty) break;
                }
            }

            return empty;
        }
        
        private static Mesh BakeAndSaveBoneWeightsIntoMesh(LOD[] refModelLoDs,
            string generatedAssetsFolder, string animatorName, string meshPartSuffix, GpuEcsAnimationBakerData bakerData,
            SkinnedMeshRenderer skinnedMeshRenderer)
        {
            int maxNumberOfBonesPerVertex = GetMaxNumberOfBonesPerVertex(refModelLoDs, bakerData, skinnedMeshRenderer);
            Mesh newMesh = BakeBoneWeightsIntoMesh(skinnedMeshRenderer.sharedMesh, maxNumberOfBonesPerVertex);
            SavePartAsAsset(newMesh, generatedAssetsFolder, animatorName, skinnedMeshRenderer, meshPartSuffix, "mesh");
            return newMesh;
        }

        private static Texture2D BakeAndSaveAnimationMatricesTexture(List<AnimationMatricesTexture> cache,
            string generatedAssetsFolder, string animatorName, string animationMatricesTexturePartSuffix,
            Animator refModelAnimator, GpuEcsAnimationBakerData bakerData,
            SkinnedMeshRenderer skinnedMeshRenderer, GpuEcsAnimationData[] animations, int totalNbrOfFrames)
        {
            if (!CheckAnimationMatricesTextureInCache(cache, skinnedMeshRenderer, out Texture2D animationMatricesTexture))
            {
                animationMatricesTexture = BakeAnimationMatricesTexture(skinnedMeshRenderer, refModelAnimator, bakerData,
                    animations, totalNbrOfFrames);
                SavePartAsAsset(animationMatricesTexture, generatedAssetsFolder, animatorName, skinnedMeshRenderer,
                    animationMatricesTexturePartSuffix, "asset");
                    
                cache.Add(new AnimationMatricesTexture()
                {
                    texture = animationMatricesTexture,
                    skinnedMeshRenderer = skinnedMeshRenderer
                });
            }
            return animationMatricesTexture;
        }

        private static Material CreateAndSaveEcsGpuMaterial(List<EcsGpuMaterial> cache,
            string generatedAssetsFolder, string animatorName, string materialPartSuffix,
            SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if (!CheckEcsGpuMaterialInCache(cache, skinnedMeshRenderer, out Material ecsGpuMaterial))
            {
                ecsGpuMaterial = Object.Instantiate<Material>(skinnedMeshRenderer.sharedMaterial);
                ecsGpuMaterial.SetFloat(EnableAnimation, 0);
                SavePartAsAsset(ecsGpuMaterial, generatedAssetsFolder, animatorName, skinnedMeshRenderer,
                    materialPartSuffix, "mat");
                cache.Add(new EcsGpuMaterial()
                {
                    material = ecsGpuMaterial,
                    skinnedMeshRenderer = skinnedMeshRenderer
                });
            }
            return ecsGpuMaterial;
        }

        private static string GenerateAssetPath(string generatedAssetsFolder, string animatorName,
            string assetName, string assetFileExtension)
        {
            return Path.Combine(generatedAssetsFolder, $"{animatorName}_{assetName}.{assetFileExtension}");
        }
        
        private static void SavePartAsAsset(Object asset, string generatedAssetsFolder, string animatorName,
            SkinnedMeshRenderer skinnedMeshRenderer, string partName, string assetFileExtension)
        {
            string assetName = $"{partName}_{skinnedMeshRenderer.name}";
            string assetPath = GenerateAssetPath(generatedAssetsFolder, animatorName, assetName, assetFileExtension);
            AssetDatabase.CreateAsset(asset, assetPath);
        }
        
        private static int GetMaxNumberOfBonesPerVertex(LOD[] refModelLoDs, GpuEcsAnimationBakerData bakerData,
            SkinnedMeshRenderer skinnedMeshRenderer)
        {
            int maxNumberOfBonesPerVertex = bakerData.boneUsage.numberOfBonesPerVertex;
            if (refModelLoDs != null)
            {
                for (int lodIndex = 0; lodIndex < refModelLoDs.Length; lodIndex++)
                {
                    if (refModelLoDs[lodIndex].renderers.Any(r => r == (Renderer)skinnedMeshRenderer))
                    {
                        BoneUsagePerLoD boneUsagePerLoD = bakerData.boneUsage.boneUsagesPerLoD
                            .SingleOrDefault(b => b.lodIndex == lodIndex);
                        if (boneUsagePerLoD != null)
                            maxNumberOfBonesPerVertex = boneUsagePerLoD.maxNumberOfBonesPerVertex;
                        break;
                    }
                }
            }
            return maxNumberOfBonesPerVertex;
        }

        private static bool CheckAnimationMatricesTextureInCache(List<AnimationMatricesTexture> cache,
            SkinnedMeshRenderer skinnedMeshRenderer, out Texture2D foundTexture)
        {
            bool found = false;
            foundTexture = null; 
            foreach (AnimationMatricesTexture cachedTexture in cache)
            {
                if (CheckSkinnedMeshRendererHasSameBones(cachedTexture.skinnedMeshRenderer,
                        skinnedMeshRenderer))
                {
                    found = true;
                    foundTexture = cachedTexture.texture;
                    break;
                }
            }
            return found;
        }

        private static bool CheckEcsGpuMaterialInCache(List<EcsGpuMaterial> cache,
            SkinnedMeshRenderer skinnedMeshRenderer, out Material foundMaterial)
        {
            bool found = false;
            foundMaterial = null;
            foreach (EcsGpuMaterial cachedEcsGpuMaterial in cache)
            {
                if (cachedEcsGpuMaterial.skinnedMeshRenderer.sharedMaterial == skinnedMeshRenderer.sharedMaterial
                    && CheckSkinnedMeshRendererHasSameBones(cachedEcsGpuMaterial.skinnedMeshRenderer,
                        skinnedMeshRenderer))
                {
                    found = true;
                    foundMaterial = cachedEcsGpuMaterial.material;
                    break;
                }
            }
            return found;
        }
        
        private static bool CheckSkinnedMeshRendererHasSameBones(SkinnedMeshRenderer skinnedMeshRendererA,
            SkinnedMeshRenderer skinnedMeshRendererB)
        {
            bool hasSameBones = false;
            if (skinnedMeshRendererA.bones.Length == skinnedMeshRendererB.bones.Length)
            {
                hasSameBones = true;
                for (int i = 0; i < skinnedMeshRendererA.bones.Length; i++)
                {
                    if (!CheckMatrixEquality(skinnedMeshRendererA.bones[i].localToWorldMatrix,
                            skinnedMeshRendererB.bones[i].localToWorldMatrix))
                    {
                        hasSameBones = false; break;
                    }
                    if (!CheckMatrixEquality(skinnedMeshRendererA.sharedMesh.bindposes[i],
                            skinnedMeshRendererB.sharedMesh.bindposes[i]))
                    {
                        hasSameBones = false; break;
                    }
                }
            }
            return hasSameBones;
        }

        private static bool CheckMatrixEquality(Matrix4x4 m1, Matrix4x4 m2)
        {
            return m1.m00 == m2.m00 && m1.m01 == m2.m01 && m1.m02 == m2.m02 && m1.m03 == m2.m03
                   && m1.m10 == m2.m10 && m1.m11 == m2.m11 && m1.m12 == m2.m12 && m1.m13 == m2.m13
                   && m1.m20 == m2.m20 && m1.m21 == m2.m21 && m1.m22 == m2.m22 && m1.m23 == m2.m23
                   && m1.m30 == m2.m30 && m1.m31 == m2.m31 && m1.m32 == m2.m32 && m1.m33 == m2.m33;
        }

        private static void IterateOverAllFramesThroughAnimator(
            Animator refModelAnimator, GpuEcsAnimationBakerData bakerData, GpuEcsAnimationData[] animations,
            System.Action<int> actionPerFrame)
        {
            refModelAnimator.speed = 0;
            int currentFrameIndex = 0;

            for (int animationIndex = 0; animationIndex < bakerData.animations.Length; animationIndex++)
            {
                AnimationData animationData = bakerData.animations[animationIndex];
                GpuEcsAnimationData gpuEcsAnimationData = animations[animationIndex];
                foreach (AnimatorParameter parameterValue in animationData.additionalAnimatorParameterValues)
                {
                    if(parameterValue.parameterType == AnimatorParameterTypes.Bool)
                        refModelAnimator.SetBool(parameterValue.parameterName, parameterValue.boolValue);
                    else if(parameterValue.parameterType == AnimatorParameterTypes.Float)
                        refModelAnimator.SetFloat(parameterValue.parameterName, parameterValue.floatValue);
                    else if(parameterValue.parameterType == AnimatorParameterTypes.Integer)
                        refModelAnimator.SetInteger(parameterValue.parameterName, parameterValue.intValue);
                }
                
                for (int sampleIndex = 0; sampleIndex < gpuEcsAnimationData.nbrOfInBetweenSamples; sampleIndex++)
                {
                    if (animationData.animationType == AnimationTypes.DualClipBlend)
                    {
                        DualClipBlendData dualClipBlendData = animationData.dualClipBlendData;
                        float sampleRatio = (float)sampleIndex / (float)(gpuEcsAnimationData.nbrOfInBetweenSamples - 1);
                        float blendValue = dualClipBlendData.clip1.parameterValue +
                            (dualClipBlendData.clip2.parameterValue - dualClipBlendData.clip1.parameterValue) * sampleRatio;
                        refModelAnimator.SetFloat(dualClipBlendData.blendParameterName, blendValue);
                    }

                    for (int frameIndex = 0; frameIndex < gpuEcsAnimationData.nbrOfFramesPerSample; frameIndex++)
                    {
                        float progressRatio = (float)frameIndex / (float)(gpuEcsAnimationData.nbrOfFramesPerSample - 1);
                        refModelAnimator.Play(animationData.animatorStateName, -1, progressRatio);
                        foreach(AnimatorState animatorState in animationData.additionalAnimatorStatesPerLayer)
                            refModelAnimator.Play(animatorState.stateName, animatorState.layer, progressRatio);
                        refModelAnimator.Update(0);

                        actionPerFrame(currentFrameIndex);
                        
                        currentFrameIndex++;
                    }
                }
            }
        }

        private static GpuEcsAttachmentAnchorData BakeAttachmentAnchorTransforms(string generatedAssetsFolder, GameObject sourceModel,
            string animatorName, Animator refModelAnimator, GpuEcsAnimationBakerData bakerData, GpuEcsAnimationData[] animations,
            int totalNbrOfFrames)
        {
            int nbrOfAttachments = bakerData.attachmentAnchors.Length;
            if (nbrOfAttachments == 0) return null;
            else
            {
                GpuEcsAttachmentAnchorData gpuEcsAttachmentAnchorData = ScriptableObject.CreateInstance<GpuEcsAttachmentAnchorData>();
                gpuEcsAttachmentAnchorData.anchorTransforms = new float4x4[totalNbrOfFrames * nbrOfAttachments];

                for (int attachmentAnchorIndex = 0; attachmentAnchorIndex < nbrOfAttachments; attachmentAnchorIndex++)
                {
                    AttachmentAnchor attachmentAnchor = bakerData.attachmentAnchors[attachmentAnchorIndex];
                    Transform referenceAnchorTransform = attachmentAnchor.attachmentAnchorTransform;
                    Stack<int> siblingIndexStack = new Stack<int>();
                    GetSiblingIndexStack(referenceAnchorTransform, sourceModel.transform, siblingIndexStack);
                    Transform anchorTransform = refModelAnimator.transform;
                    while (siblingIndexStack.Count > 0)
                        anchorTransform = anchorTransform.GetChild(siblingIndexStack.Pop());
                    Debug.Assert(anchorTransform.name.Equals(referenceAnchorTransform.name));

                    int baseIndex = attachmentAnchorIndex * totalNbrOfFrames;
                    IterateOverAllFramesThroughAnimator(refModelAnimator, bakerData, animations,
                        (currentFrameIndex) =>
                        {
                            gpuEcsAttachmentAnchorData.anchorTransforms[baseIndex + currentFrameIndex] = anchorTransform.localToWorldMatrix;
                        });

                }

                string assetPath = GenerateAssetPath(generatedAssetsFolder, animatorName, "AttachmentAnchors", "asset");
                AssetDatabase.CreateAsset(gpuEcsAttachmentAnchorData, assetPath);
                return gpuEcsAttachmentAnchorData;
            }
        }

        private static void GetSiblingIndexStack(Transform current, Transform root, Stack<int> currentStack)
        {
            if(current.gameObject == root.gameObject) return;
            currentStack.Push(current.GetSiblingIndex());
            GetSiblingIndexStack(current.parent, root, currentStack);
        }
        
        private static Texture2D BakeAnimationMatricesTexture(SkinnedMeshRenderer skinnedMeshRenderer, 
            Animator refModelAnimator, GpuEcsAnimationBakerData bakerData, GpuEcsAnimationData[] animations,
            int totalNbrOfFrames)
        {
            int boneCount = skinnedMeshRenderer.bones.Length;
            int animationMatricesTextureWidth = boneCount * 3;

            NativeArray<half4> animatedBoneMatricesTextureData = new NativeArray<half4>(
                totalNbrOfFrames * animationMatricesTextureWidth, Allocator.Temp);

            float4x4 invSkinnedMeshRendererLocalToWorld =  math.inverse(skinnedMeshRenderer.transform.localToWorldMatrix);
            IterateOverAllFramesThroughAnimator(refModelAnimator, bakerData, animations,
                (currentFrameIndex) =>
                {
                    for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
                    {
                        Transform boneTransform = skinnedMeshRenderer.bones[boneIndex];
                        if (boneTransform != null)
                        {
                            float4x4 matrix = math.mul(
                                math.mul(invSkinnedMeshRendererLocalToWorld, boneTransform.localToWorldMatrix),
                                skinnedMeshRenderer.sharedMesh.bindposes[boneIndex]
                            );
                            Debug.Assert(Mathf.Approximately(matrix[0][3], 0f)
                                && Mathf.Approximately(matrix[1][3], 0f)
                                && Mathf.Approximately(matrix[2][3], 0f)
                                && Mathf.Approximately(matrix[3][3], 1f), "matrix row 4 must be 0,0,0,1");
                            int matrixIndex = (currentFrameIndex * animationMatricesTextureWidth) + (boneIndex * 3);
                            for (int i = 0; i < 3; i++)
                                animatedBoneMatricesTextureData[matrixIndex + i] = new half4(
                                    (half)matrix[0][i], (half)matrix[1][i], (half)matrix[2][i], (half)matrix[3][i]);
                        }
                    }
                });

            Texture2D animationMatricesTexture = new Texture2D(animationMatricesTextureWidth, totalNbrOfFrames, 
                TextureFormat.RGBAHalf, false);
            animationMatricesTexture.SetPixelData(animatedBoneMatricesTextureData, 0);
            animationMatricesTexture.Apply();

            animatedBoneMatricesTextureData.Dispose();

            return animationMatricesTexture;
        }
        
        private static Mesh BakeBoneWeightsIntoMesh(Mesh sourceMesh, int maxNumberOfBonesPerVertex)
        {
            Mesh mesh = GameObject.Instantiate(sourceMesh);
            
            int vertexCount = mesh.vertices.Length;
            NativeArray<Vector4> uvs1 = new NativeArray<Vector4>(vertexCount, Allocator.Temp);
            NativeArray<Vector4> uvs2 = new NativeArray<Vector4>(vertexCount, Allocator.Temp);
            NativeArray<Vector4> uvs3 = new NativeArray<Vector4>(vertexCount, Allocator.Temp);
            NativeArray<byte> bonesPerVertex = mesh.GetBonesPerVertex();
            NativeArray<BoneWeight1> boneWeightsSource = mesh.GetAllBoneWeights();

            int startBoneWeightIndex = 0;

            for (int vertIndex = 0; vertIndex < vertexCount; vertIndex++)
            {
                float totalWeight = 0f;
                float totalWeightCapped = 0f;
                int numberOfBonesForThisVertex = bonesPerVertex[vertIndex];

                int boneWeightIndexTemp = startBoneWeightIndex;
                for (int i = 0; i < numberOfBonesForThisVertex; i++)
                {
                    BoneWeight1 currentBoneWeight = boneWeightsSource[boneWeightIndexTemp];
                    totalWeight += currentBoneWeight.weight;
                    if (i < maxNumberOfBonesPerVertex) totalWeightCapped += currentBoneWeight.weight;
                    if (i > 0) Debug.Assert(boneWeightsSource[boneWeightIndexTemp - 1].weight >= currentBoneWeight.weight);
                    boneWeightIndexTemp++;
                }
                Debug.Assert(Mathf.Approximately(1f, totalWeight));

                float weightMultiplier = totalWeight / totalWeightCapped;
                int nbrOfBonesToBake = math.min(maxNumberOfBonesPerVertex, numberOfBonesForThisVertex);

                totalWeight = 0f;
                boneWeightIndexTemp = startBoneWeightIndex;
                float4 uv1 = float4.zero;
                float4 uv2 = float4.zero;
                float4 uv3 = float4.zero;
                for (int i = 0; i < nbrOfBonesToBake; i++)
                {
                    BoneWeight1 currentBoneWeight = boneWeightsSource[boneWeightIndexTemp];
                    float adjustedWeight = currentBoneWeight.weight * weightMultiplier; 
                    totalWeight += adjustedWeight;
                    boneWeightIndexTemp++;
                    if      (i == 0) uv1 = new float4(currentBoneWeight.boneIndex, adjustedWeight, uv1.z, uv1.w);
                    else if (i == 1) uv1 = new float4(uv1.x, uv1. y, currentBoneWeight.boneIndex, adjustedWeight);
                    else if (i == 2) uv2 = new float4(currentBoneWeight.boneIndex, adjustedWeight, uv2.z, uv2.w);
                    else if (i == 3) uv2 = new float4(uv2.x, uv2. y, currentBoneWeight.boneIndex, adjustedWeight);
                    else if (i == 4) uv3 = new float4(currentBoneWeight.boneIndex, adjustedWeight, uv3.z, uv3.w);
                    else if (i == 5) uv3 = new float4(uv3.x, uv3. y, currentBoneWeight.boneIndex, adjustedWeight);
                }
                Debug.Assert(Mathf.Approximately(1f, totalWeight));
                uvs1[vertIndex] = uv1;
                uvs2[vertIndex] = uv2;
                uvs3[vertIndex] = uv3;
                startBoneWeightIndex += numberOfBonesForThisVertex;
            }
            
            mesh.SetUVs(1, uvs1);
            mesh.SetUVs(2, uvs2);
            mesh.SetUVs(3, uvs3);
            
            uvs1.Dispose();
            uvs2.Dispose();
            uvs3.Dispose();
            bonesPerVertex.Dispose();
            boneWeightsSource.Dispose();
            return mesh;
        }
    }
}
#endif