using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Modifier/Destroy After Delay")]
public class DestroyAfterDelay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, .5f);
    }
}
