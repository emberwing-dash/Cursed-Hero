using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Animator cameraAnimator;

    [Header("Animation")]
    [SerializeField] private string introAnimationName = "Intro"; // animation state name
    [SerializeField] private float introLength = 5f; // fallback time if no events

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Game";

    private void Start()
    {
        // Hide menu at start
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(false);

        // Play camera intro
        if (cameraAnimator != null)
            cameraAnimator.Play(introAnimationName);

        // Show menu after animation
        StartCoroutine(ShowMenuAfterIntro());
    }

    private System.Collections.IEnumerator ShowMenuAfterIntro()
    {
        yield return new WaitForSeconds(introLength);

        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
