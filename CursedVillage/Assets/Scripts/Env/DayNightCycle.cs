using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = Color.black;
    [SerializeField] private float cycleDuration = 60f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource dayMusic;
    [SerializeField] private AudioSource nightMusic;

    private float timer = 0f;
    private bool isDayPlaying = false;
    private bool isNightPlaying = false;

    void Start()
    {
        // Ensure only one audio is playing at start
        if (dayMusic != null) { dayMusic.loop = true; dayMusic.Stop(); }
        if (nightMusic != null) { nightMusic.loop = true; nightMusic.Stop(); }
    }

    void Update()
    {
        if (directionalLight == null) return;

        // Increment timer
        timer += Time.deltaTime;
        if (timer > cycleDuration) timer -= cycleDuration;

        // Normalized cycle (0 -> 1)
        float cycleNorm = timer / cycleDuration;

        // Light color lerp (0->1->0)
        float t = Mathf.PingPong(cycleNorm * 2f, 1f);
        directionalLight.color = Color.Lerp(dayColor, nightColor, t);

        // Rotate light
        directionalLight.transform.Rotate(Vector3.right, (360f / cycleDuration) * Time.deltaTime, Space.Self);

        // Handle audio
        HandleAudio(cycleNorm);
    }

    private void HandleAudio(float cycleNorm)
    {
        if (cycleNorm <= 0.5f) // Day
        {
            if (!isDayPlaying)
            {
                if (dayMusic != null && !dayMusic.isPlaying) dayMusic.Play();
                if (nightMusic != null && nightMusic.isPlaying) nightMusic.Stop();
                isDayPlaying = true;
                isNightPlaying = false;
            }
        }
        else // Night
        {
            if (!isNightPlaying)
            {
                if (nightMusic != null && !nightMusic.isPlaying) nightMusic.Play();
                if (dayMusic != null && dayMusic.isPlaying) dayMusic.Stop();
                isNightPlaying = true;
                isDayPlaying = false;
            }
        }
    }
}
