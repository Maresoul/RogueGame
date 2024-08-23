//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using UnityEngine.UI;

namespace DigitalRubyShared
{
    /// <summary>
    /// Shows how to add a tap component by scripting
    /// </summary>
    public class DemoScriptTapComponentAddByScript : MonoBehaviour
    {
        /// <summary>
        /// Status label
        /// </summary>
        [Tooltip("Status label")]
        public UnityEngine.UI.Text StatusLabel;
        
        private void OnEnable()
        {
            TapGestureRecognizerComponentScript tapGesture = gameObject.AddComponent<TapGestureRecognizerComponentScript>();
            tapGesture.Gesture.StateUpdated += StateUpdated;
        }

        private void StateUpdated(GestureRecognizer gesture)
        {
            if (gesture.State != GestureRecognizerState.Possible)
            {
                StatusLabel.text = gesture.State.ToString();
            }
        }
    }
}
