using UnityEngine;
using UnityEngine.VFX;
using System;

[Serializable]
public class CubeVFX
{
    public VisualEffect[] vfxArray; // Array of Visual Effects for this cube
}

public class CubeVFXManager : MonoBehaviour
{
    [SerializeField] private CubeVFX[] cubes; // Array of cube and their respective Visual Effects

    public void PlayCube(int cubeIndex)
    {
        if (cubeIndex >= 0 && cubeIndex < cubes.Length)
        {
            CubeVFX cubeVFX = cubes[cubeIndex];

            foreach (VisualEffect vfx in cubeVFX.vfxArray)
            {
                if (vfx != null)
                {
                    Debug.Log("PrintEvent: PlayCube" + cubeIndex + " called at: " + Time.time);
                    vfx.Play();
                }
            }
        }
    }
}
