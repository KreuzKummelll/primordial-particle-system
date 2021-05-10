using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimoridalParticleSystem : MonoBehaviour
{

    [SerializeField] private GameObject partcilePrefab;
    [SerializeField] private float particleAwarenessRadius = 5;

    [SerializeField] private int xDim = 10;
    [SerializeField] private int yDim = 10;
    [SerializeField] private int placementincriment = 4;


    private void Start()
    {

        for (int i = -yDim; i < yDim; i+=placementincriment)
        {
            for (int j = -xDim; j < xDim; j+=placementincriment)
            {
                var particleInstance = Instantiate(partcilePrefab, transform, true);
                particleInstance.GetComponent<PrimordialParticle>().r = particleAwarenessRadius;    
                particleInstance.transform.position = new Vector3(
                    j + (IsNegative() ? -Random.value : Random.value),
                    i + (IsNegative() ? -Random.value : Random.value),
                    0);
                particleInstance.transform.GetComponent<Rigidbody2D>().SetRotation(Random.value * 100);
            }
        }
    }

    
    bool IsNegative()
    {
        return Random.value < 0.5 ? false : true;
    }
}
