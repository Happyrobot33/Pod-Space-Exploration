using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

    public float timeMultiplier = 1f;
    public bool followCanvas = true;

	void Update ()
    {
        timeMultiplier = Mathf.Clamp(timeMultiplier, 0.0001f, 100f);

        Time.timeScale = 1f * timeMultiplier;
        Time.fixedDeltaTime = 0.02f * timeMultiplier;
	}
}
