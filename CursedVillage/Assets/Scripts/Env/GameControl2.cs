using UnityEngine;

public class GameControl2 : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject exitCanvas;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private bool isOpen = false;


    private void Start()
    {
        CloseMenu(); // start hidden
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMenu();
        }
    }


    public void ToggleMenu()
    {
        if (isOpen)
            CloseMenu();
        else
            OpenMenu();
    }


    public void OpenMenu()
    {
        isOpen = true;

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f; // pause game
    }


    public void CloseMenu()
    {
        isOpen = false;

        if (exitCanvas != null)
            exitCanvas.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f; // resume game
    }
}
