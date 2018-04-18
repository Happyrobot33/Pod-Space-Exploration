using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {
    //Speed of the conveyor
    public float speed = 1;
    public Rigidbody rb;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //When objects enter the conveyors range of effect
    void OnTriggerEnter(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }
	
	void OnDrawGizmos() {
		//Set gizmos color to green
        Gizmos.color = Color.green;
		// set out ray direction to forward of conveyor
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
		
		//draw ray gizmo to conveyor push direction
        Gizmos.DrawRay(transform.position, direction);
		//draw a label showing conveyor speed
		Handles.Label(transform.position, speed);
    }
}
