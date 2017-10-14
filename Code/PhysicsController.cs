using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsController : MonoBehaviour {


    Rigidbody rb;

    public float actualDensity = 5000f; // kg per m^3
    
    // sample stats.. don't mess with the player
    
    // SUN - density: 1408 kg/m^3 || mass: 1.9891e30 kg, 1.42842941e8 ug || 1.41200e27 m^3 || 0 m from sun
    // EARTH - density: 5514 kg/m^3 ||  /// mass: 5.9723e24 kg, 195581.755 ug || volume: 1.08321e21 m^3 || 1.49597871e11 m from sun
    // player = density: 5000 kg/m^3 || mass: 5000 kg || volume: 1 m^3 || diameter: 10 meters
    public Vector3 initialVelocity = new Vector3(0f, 0f, 0f);

    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
        // sets unity velocity, so velocity in decameters
    }

    void FixedUpdate()
    {
        // unity's mass system clamps at high and low values,
        //// so we adjust the scale of everything
        //// 1 unity unity = 10 meters = 1 decameter

        // scale adjustments
        double actualMass = Extensions.ToActualMass(rb.mass); // in kg

        float actualVolume = (float) actualMass / actualDensity; // in m^3
        float actualDiameter = 2f * Mathf.Pow((3f * actualVolume) / (4f * Mathf.PI), 1f / 3f); // in meters
        float unityDiameter = actualDiameter * 0.1f; // in decameters

        transform.localScale = new Vector3(unityDiameter, unityDiameter, unityDiameter);
        //Debug.Log("m=" + actualMass + ", v=" + actualVolume + ", d=" + actualDensity);

        // gravity towards all other rigidbodies
        //// store in gravAcceleration, then apply
        Vector3 gravAcceleration = Vector3.zero;
        foreach (Rigidbody otherRb in FindObjectsOfType<Rigidbody>())
        {
            // disable gravity from kinematic or inactive rigidbodies
            if (otherRb == rb ||
                otherRb.isKinematic ||
                !otherRb.gameObject.activeInHierarchy)
                continue;

            // gravity calculations
            double otherActualMass = Extensions.ToActualMass(otherRb.mass); // kg
            // distance between the objects, converted from decameters to meters, squared
            float sqrDistance = Mathf.Pow(10f * Vector3.Distance(rb.position, otherRb.position), 2f); // meters

            double actualGravForce = (Extensions.gravityConstant * actualMass * otherActualMass)
                / sqrDistance; // N (newtons)
            
            // convert to a "single" aka a float
            float actualGravAcceleration = System.Convert.ToSingle(actualGravForce / actualMass); // meters per sec
            float unityGravAcceleration = actualGravAcceleration * 0.1f * Time.fixedDeltaTime; // decameters / iteration 

            Vector3 directionToOther = (otherRb.position - rb.position).normalized;
            gravAcceleration += directionToOther * unityGravAcceleration;
        }
        rb.AddForce(gravAcceleration, ForceMode.Acceleration);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, Extensions.lightSpeed * 0.1f);
    }



}
