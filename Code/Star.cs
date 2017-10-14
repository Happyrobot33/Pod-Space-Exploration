using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsController), typeof(Light), typeof(LensFlare))]
public class Star : MonoBehaviour {

    new Light light;
    LensFlare flare;
    void OnEnable()
    {
        light = GetComponent<Light>();
        flare = GetComponent<LensFlare>();

        flare.fadeSpeed = flare.fadeSpeed;
    }

    void Update()
    {
        // this is done in update to allow time for physics calculations
        light.range = transform.localScale.sqrMagnitude;
    }
}
