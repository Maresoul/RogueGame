using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GPUECSAnimationBaker.Samples
{
    public class SampleSceneSelectionScript : MonoBehaviour
    {
        public string sceneName;

        public void LoadScene()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = true;
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(sceneName) && Input.GetKeyDown(KeyCode.Escape))
            {
                sceneName = "SampleScenesMenu";
                LoadScene();
            }
                
        }
    }
}
