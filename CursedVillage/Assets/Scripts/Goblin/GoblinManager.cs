using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoblinManager : MonoBehaviour
{
    public static GoblinManager Instance;

    [Header("All Goblins in Scene (Drag & Drop)")]
    [SerializeField] private List<GameObject> allGoblins;

    [Header("Lose Conditions (Drag these)")]
    [SerializeField] private CrystalHealth mainCrystal;   // drag MAIN crystal here
    [SerializeField] private PlayerHealth playerHealth;   // drag player health here

    [Header("UI")]
    [SerializeField] private GameObject WonCanvas;
    [SerializeField] private GameObject LostCanvas;

    private bool gameEnded = false;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (WonCanvas) WonCanvas.SetActive(false);
        if (LostCanvas) LostCanvas.SetActive(false);

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void Update()
    {
        if (gameEnded) return;

        CheckPlayerDeath();
        CheckCrystalDestroyed();
        CheckAllGoblinsDead();
    }


    void CheckPlayerDeath()
    {
        if (playerHealth == null) return;

        if (playerHealth.GetHealth() <= 0)
        {
            TriggerLose();
        }
    }


    void CheckCrystalDestroyed()
    {
        // Crystal reference missing or destroyed
        if (mainCrystal == null)
        {
            TriggerLose();
            return;
        }

        // Crystal disabled in scene
        if (!mainCrystal.gameObject.activeInHierarchy)
        {
            TriggerLose();
            return;
        }

        // Crystal script says dead
        if (mainCrystal.IsDead)
        {
            TriggerLose();
        }
    }



    void CheckAllGoblinsDead()
    {
        if (allGoblins == null || allGoblins.Count == 0) return;

        bool allDead = true;

        foreach (var g in allGoblins)
        {
            // destroyed or inactive = dead
            if (g != null && g.activeInHierarchy)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            TriggerWin();
        }
    }



    void TriggerLose()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (LostCanvas)
            LostCanvas.SetActive(true);

        FreezeGame();
    }


    void TriggerWin()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (WonCanvas)
            WonCanvas.SetActive(true);

        FreezeGame();
    }


    void FreezeGame()
    {
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }



    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
