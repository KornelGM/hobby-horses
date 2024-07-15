using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    public VisualEffect[] vfxArray;  // Array to store your Visual Effects
    public float[] vfxDelays;  // Array to store delays for each VFX
    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(PlayVFXWithDelay());
    }

    private IEnumerator PlayVFXWithDelay()
    {
        // Ensure both arrays have the same length
        if (vfxArray.Length != vfxDelays.Length)
        {
            Debug.LogError("VFX array and delay array lengths do not match.");
            yield break;
        }

        while (true)  // Infinite loop
        {
            // Play the current VFX
            vfxArray[currentIndex].Play();

            // Wait for the specified delay for this VFX
            yield return new WaitForSeconds(vfxDelays[currentIndex]);

            // Move to the next VFX
            currentIndex = (currentIndex + 1) % vfxArray.Length;
        }
    }
}
