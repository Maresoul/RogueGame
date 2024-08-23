using UnityEngine;

namespace DigitalRubyShared
{
    public class DemoScriltMultiPan : MonoBehaviour
    {
        private PanGestureRecognizer pan1;
        private PanGestureRecognizer pan2;

        private void PanCallback(GestureRecognizer pan)
        {
            // log the pan
            int idx = (pan == pan1 ? 1 : 2);
            Debug.LogFormat("Pan: {0}: {1}", idx, pan.State);

            // to prevent the touch from being applied to the other pan gesture, consume it
            if (pan.State == GestureRecognizerState.Began)
            {
                GestureRecognizer.ConsumeTouch(pan.CurrentTrackedTouches[0].Id);
            }
            // make sure to unconsume it when the gesture ends
            else if (pan.CurrentTrackedTouches.Count != 0 &&
                (pan.State == GestureRecognizerState.Ended || pan.State == GestureRecognizerState.Failed))
            {
                GestureRecognizer.UnconsumeTouch(pan.CurrentTrackedTouches[0].Id);
            }
        }

        private void OnEnable()
        {
            pan1 = new PanGestureRecognizer() { ThresholdUnits = 0.0f };
            pan1.StateUpdated += PanCallback;
            FingersScript.Instance.AddGesture(pan1);

            pan2 = new PanGestureRecognizer() { ThresholdUnits = 0.0f };
            pan2.StateUpdated += PanCallback;
            pan2.AllowSimultaneousExecution(pan1);
            FingersScript.Instance.AddGesture(pan2);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(pan1);
                FingersScript.Instance.RemoveGesture(pan2);
            }
        }
    }
}