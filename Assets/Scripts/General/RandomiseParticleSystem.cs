using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class RandomiseParticleSystem : MonoBehaviour
{

    Transform player;

	void Start ()
    {
        transform.localRotation = Quaternion.Euler(Random.insideUnitSphere * 360);
	}
}
