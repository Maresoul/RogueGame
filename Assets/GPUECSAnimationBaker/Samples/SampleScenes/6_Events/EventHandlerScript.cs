using System;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes._6_Events
{
    public class EvenLoggerBehaviour : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI text;

        public void Start()
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
            EventHandlerSystem eventHandler =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EventHandlerSystem>();
            eventHandler.logger = this;
        }

        public void Log(string toLog)
        {
            text.text = text.text.Insert(0, toLog);
        }
    }
    
    public partial class EventHandlerSystem : SystemBase 
    {
        public EvenLoggerBehaviour logger;
                
        protected override void OnUpdate()
        {
            if (logger != null)
            {
                EntityManager entityManager = World.EntityManager;
                Entities.ForEach((in DynamicBuffer<GpuEcsAnimatorEventBufferElement> gpuEcsAnimatorEventBuffer, in Entity eventEntity) =>
                {
                    foreach (GpuEcsAnimatorEventBufferElement gpuEcsAnimatorEvent in gpuEcsAnimatorEventBuffer)
                    {
                        string entityName = eventEntity.ToString();
                        string animationId = ((AnimationIdsMaria)gpuEcsAnimatorEvent.animationId).ToString();
                        string eventId = ((AnimationEventIdsMaria)gpuEcsAnimatorEvent.eventId).ToString();
                        string time = UnityEngine.Time.time.ToString();
                        logger.Log($"Entity:{entityName}, Animation:{animationId}, Event: {eventId}, Time: {time}\n");
                    }
                }).WithoutBurst().WithStructuralChanges().Run();
            }
        }
    }
}