using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyFinishedParticleSystem : MonoBehaviour
{
    public float delay = 1f;
    private float destroyDelay;

    private ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        destroyDelay = Time.time + delay;
    }

    void Update()
    {
        if (system.particleCount <= 0 && Time.time > destroyDelay)
            Destroy(gameObject);
    }
}
