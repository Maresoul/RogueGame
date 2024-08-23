using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRubyShared
{
    /// <summary>
    /// Demo script to show using a pan gesture in between scene reloads
    /// </summary>
    public class DemoScriptGestureSceneReload : MonoBehaviour
    {
        private PanGestureRecognizer panGesture;
        private bool unloading;

        // Start is called before the first frame update
        private void OnEnable()
        {
            if (panGesture == null)
            {
                // keep this script around forever
                DontDestroyOnLoad(gameObject);

                // make a pan gesture
                panGesture = new PanGestureRecognizer();
                panGesture.StateUpdated += StateUpdated;
                FingersScript.Instance.AddGesture(panGesture);

                Debug.Log("Created pan gesture and marked demo script as dont destroy on load");
            }
        }

        // Update is called once per frame
        private void OnDisable()
        {
            if (panGesture != null && FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(panGesture);
                panGesture = null;

                Debug.Log("Cleaned up pan gesture");
            }
        }

        private void Update()
        {
            const string additiveScene = "DemoSceneGestureSceneReloadAdditive";

            // if user hit esc key, add scene, gesture should continue
            if (!unloading && Input.GetKeyDown(KeyCode.Escape))
            {
                // check if our scene is loaded
                bool hasScene = false;
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).isLoaded && SceneManager.GetSceneAt(i).name == additiveScene)
                    {
                        hasScene = true;
                        break;
                    }
                }
                if (hasScene)
                {
                    unloading = true;
                    Debug.Log("Beginning scene unload and load...");
                    var unloader = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("DemoSceneGestureSceneReloadAdditive");
                    unloader.completed += op =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoSceneGestureSceneReloadAdditive", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                        Debug.Log("Finished unloading scene and loading DemoSceneGestureSceneReloadAdditive additively");
                        unloading = false;
                    };
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("DemoSceneGestureSceneReloadAdditive", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    Debug.Log("Loaded DemoSceneGestureSceneReloadAdditive additively");
                }
            }
        }

        private void StateUpdated(GestureRecognizer gesture)
        {
            Debug.LogFormat("Pan gesture state {0}, pos: {1},{2}", gesture.State, gesture.FocusX, gesture.FocusY);
        }
    }
}