using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    public class RunnerSpawnerControlBehaviour : MonoBehaviour
    {
        public Scrollbar fieldSizeXScrollbar;
        public Scrollbar fieldSizeZScrollbar;
        public Scrollbar nbrOfEntitiesScrollbar;
        public TextMeshProUGUI fieldSizeXText;
        public TextMeshProUGUI fieldSizeZText;
        public TextMeshProUGUI nbrOfEntitiesText;
        
        private IEnumerator Start()
        {
            while(World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                      typeof(RunnerSpawnerUpdateComponent)).CalculateEntityCount() == 0)
                yield return null;
            ScrollUpdate();
        }
        
        public void ScrollUpdate()
        {
            RunnerSpawnerControlSystem runnerSpawnerControlSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RunnerSpawnerControlSystem>();
            runnerSpawnerControlSystem.doScrollUpdate = true;
            runnerSpawnerControlSystem.fieldSizeX = fieldSizeXScrollbar.value * 1000f;
            fieldSizeXText.text = runnerSpawnerControlSystem.fieldSizeX.ToString("#.##");
            runnerSpawnerControlSystem.fieldSizeZ = fieldSizeZScrollbar.value * 1000f;
            fieldSizeZText.text = runnerSpawnerControlSystem.fieldSizeZ.ToString("#.##");
            runnerSpawnerControlSystem.nbrOfEntities = Mathf.RoundToInt(nbrOfEntitiesScrollbar.value * 100000);
            nbrOfEntitiesText.text = runnerSpawnerControlSystem.nbrOfEntities.ToString();
        }
    }
    
    public partial class RunnerSpawnerControlSystem : SystemBase
    {
        public bool doScrollUpdate;
        public float fieldSizeX;
        public float fieldSizeZ;
        public int nbrOfEntities;

        protected override void OnUpdate()
        {
            if (doScrollUpdate)
            {
                doScrollUpdate = false;
                Entities.ForEach((ref RunnerSpawnerUpdateComponent runnerSpawnerUpdate) =>
                {
                    runnerSpawnerUpdate.fieldSizeX = fieldSizeX;
                    runnerSpawnerUpdate.fieldSizeZ = fieldSizeZ;
                    runnerSpawnerUpdate.nbrOfRunners = nbrOfEntities;
                    runnerSpawnerUpdate.updateTime = 0.5f;
                }).WithoutBurst().Run();
            }
        }
    }
}
