using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    // How fast the light flickers
    public float minFlickerSpeed = 0.05f;
    public float maxFlickerSpeed = 0.2f;

    // How bright the light gets
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;

    private Light pointLight;
    private float timer;
    private float nextFlicker;

    void Start()
    {
        // Get the Light component on this same object
        pointLight = GetComponent<Light>();
        nextFlicker = Random.Range(minFlickerSpeed, maxFlickerSpeed);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextFlicker)
        {
            // Randomly change brightness
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);

            // Set next flicker time
            nextFlicker = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            timer = 0;
        }
    }
}