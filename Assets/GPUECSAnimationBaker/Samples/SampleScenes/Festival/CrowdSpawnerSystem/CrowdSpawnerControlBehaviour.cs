using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Festival.CrowdSpawnerSystem
{
    public class CrowdSpawnerControlBehaviour : MonoBehaviour
    {
        public Scrollbar rowsScrollbar;
        public Scrollbar colsScrollbar;
        public TextMeshProUGUI rowsText;
        public TextMeshProUGUI colsText;
        public TextMeshProUGUI totalAmountText;

        private IEnumerator Start()
        {
            while(World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                      typeof(CrowdSpawnerUpdateComponent)).CalculateEntityCount() == 0)
                yield return null;
            ScrollUpdate();
        }
        
        public void ScrollUpdate()
        {
            CrowdSpawnerControlSystem crowdSpawnerControlSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CrowdSpawnerControlSystem>();
            crowdSpawnerControlSystem.doScrollUpdate = true;
            crowdSpawnerControlSystem.rows = Mathf.RoundToInt(rowsScrollbar.value * 500);
            rowsText.text = crowdSpawnerControlSystem.rows.ToString();
            crowdSpawnerControlSystem.cols = Mathf.RoundToInt(colsScrollbar.value * 500);
            colsText.text = crowdSpawnerControlSystem.cols.ToString();
            totalAmountText.text = "Total amount of units: " + (crowdSpawnerControlSystem.rows * crowdSpawnerControlSystem.cols).ToString();
        }
    }
    
    public partial class CrowdSpawnerControlSystem : SystemBase
    {
        public bool doScrollUpdate;
        public int cols;
        public int rows;

        protected override void OnUpdate()
        {
            if (doScrollUpdate)
            {
                doScrollUpdate = false;
                Entities.ForEach((ref CrowdSpawnerUpdateComponent crowdSpawnerUpdate) =>
                {
                    crowdSpawnerUpdate.cols = cols;
                    crowdSpawnerUpdate.rows = rows;
                    crowdSpawnerUpdate.updateTime = 0.5f;
                }).WithoutBurst().Run();
            }
        }
    }
}
