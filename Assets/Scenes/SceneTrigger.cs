using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class SceneTrigger : MonoBehaviour
    {
        [Tooltip("Name of the scene to load")]
        public string sceneToLoad;

        [Tooltip("Tag used to identify the player")]
        public string playerTag = "Player";

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Scene change triggered");
            if (!other.GetComponent<Collider>().CompareTag(playerTag)) return;
            
            Debug.Log("Valid collision");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

