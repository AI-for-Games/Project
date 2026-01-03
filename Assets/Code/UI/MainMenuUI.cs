using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public string gameSceneName = "Game";

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))  // Escape to quit
                QuitGame();
        }
    }
}