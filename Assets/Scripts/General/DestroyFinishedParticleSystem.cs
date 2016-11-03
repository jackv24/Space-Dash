using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyFinishedParticleSystem : MonoBehaviour
{
    private ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (system.particleCount <= 0)
            Destroy(gameObject);
    }
}
