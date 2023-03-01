using UnityEngine;

public class SinewaveExample : MonoBehaviour
{
    [Range(1, 20000)] //Creates a slider in the inspector
    public float frequency1;

    [Range(1, 20000)] //Creates a slider in the inspector
    public float frequency2;

    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;

    private AudioSource audioSource;
    private int timeIndex;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            frequency1 = 550;
            frequency2 = 550;
            Debug.Log("keydown");
            if (!audioSource.isPlaying)
            {
                timeIndex = 0; //resets timer before playing sound
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }

        Debug.Log("keyup");
        frequency1 = 2;
        frequency2 = 2;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (var i = 0; i < data.Length; i += channels)
        {
            data[i] = CreateSine(timeIndex, frequency1, sampleRate);

            if (channels == 2)
                data[i + 1] = CreateSine(timeIndex, frequency2, sampleRate);

            timeIndex++;

            //if timeIndex gets too big, reset it to 0
            if (timeIndex >= sampleRate * waveLengthInSeconds) timeIndex = 0;
        }
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }
}