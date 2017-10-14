using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    CanvasController canvas;
    PlayerResources resources;
    Rigidbody rb;
    Transform torqueDisk;
    List<ParticleSystem> thrustFlames = new List<ParticleSystem>();
    List<ParticleSystem> torqueFlames = new List<ParticleSystem>();
    Transform velocityMarkerParent;
    Transform progradeMarker;
    Transform retrogradeMarker;
    Transform forwardMarker;
    Transform forwardDeltaMarker;
    public int targetMode = 0;

    void OnEnable ()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasController>();
        resources = GetComponent<PlayerResources>();
        rb = GetComponent<Rigidbody>();
        torqueDisk = transform.extFind("Torque Disk");

        velocityMarkerParent = transform.Find("Velocity Marker");
        progradeMarker = transform.extFind("Prograde");
        retrogradeMarker = transform.extFind("Retrograde");
        forwardMarker = transform.extFind("Forward");
        forwardDeltaMarker = forwardMarker.Find("Forward Delta");

        // find particle systems
        thrustFlames.Clear();
        List<Transform> tmp = transform.extFindAll("Thrust Flame");
        for(int i = 0; i < tmp.Count; i++)
            thrustFlames.Add(tmp[i].GetComponent<ParticleSystem>());

        torqueFlames.Clear();
        tmp = torqueDisk.extFindAll("Torque Flame");
        for (int i = 0; i < tmp.Count; i++)
            torqueFlames.Add(tmp[i].GetComponent<ParticleSystem>());
    }

    

    public float thrustFuelUsage = 0.001f;
    public float torqueFuelUsage = 0.0001f;
    public float thrustMultiplier = 1f;
    public float torqueMultiplier = 1f;
    public float stabilizeMultiplier = 0.01f;
    public float particleMultiplier = 1f;
    public float rotationSpeed = 0.001f;

    void FixedUpdate()
    {
        

        // thrust
        // find player input
        float throttle = canvas.throttleSlider.value;
        //// only if throttle > 0, and fuel > 0
        if (throttle > 0 && resources.fuel > 0)
        {
            resources.fuel -= throttle * thrustFuelUsage;
            rb.AddForce(transform.forward * throttle * thrustMultiplier, ForceMode.Acceleration);
            for (int i = 0; i < thrustFlames.Count; i++)
            {
                // change particle size based on strength of thrust
                var main = thrustFlames[i].main;
                main.startLifetimeMultiplier = 4f * particleMultiplier * throttle;
                main.startSizeMultiplier = particleMultiplier * throttle;
                thrustFlames[i].Play();
            }
        }
        else
            for (int i = 0; i < thrustFlames.Count; i++)
                thrustFlames[i].Stop();


        // torque -
        float pitch = canvas.pitchSlider.value;
        float yaw = canvas.yawSlider.value;
        float roll = canvas.rollSlider.value;
        //// only if the sliders aren't zero, and fuel > 0
        if ( (pitch != 0f || yaw != 0f || roll != 0f) && resources.fuel > 0)
        {

            Vector3 torque = torqueMultiplier * new Vector3(pitch, yaw, roll);
            torque = transform.InverseTransformVector(torque);
            rb.angularVelocity = torque;

            for (int i = 0; i < torqueFlames.Count; i++)
            {
                // change particle size based on strength of torque
                var main = torqueFlames[i].main;
                torqueFlames[i].Play();
            }
        }
        else
            for (int i = 0; i < torqueFlames.Count; i++)
                torqueFlames[i].Stop();
        




        // update markers
        velocityMarkerParent.LookAt(transform.position + rb.velocity);
        progradeMarker.LookAt(Camera.main.transform);
        retrogradeMarker.LookAt(Camera.main.transform);
        forwardMarker.LookAt(Camera.main.transform);

        // point toward target based on target mode
        Transform target = null;
        if (targetMode > 0) 
        {
            //Debug.Log(targetMode);
            // set target
            if (targetMode == 1)
                rb.angularVelocity *= (1 - stabilizeMultiplier);
            if (targetMode == 2)
                target = progradeMarker;
            if (targetMode == 3)
                target = retrogradeMarker;
        }
        if (rotating)
            target = forwardDeltaMarker;
        if(target != null)
        {
            // make rotation
            Quaternion oldRotation = transform.rotation;
            Vector3 localTargetPos = (target.position - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(localTargetPos, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                0.1f);
            Debug.Log(transform.rotation.eulerAngles + " _ " + targetRotation.eulerAngles);
            // subtract fuel
            float rotationMagnitude = (transform.eulerAngles - oldRotation.eulerAngles).normalized.magnitude;
            resources.fuel -= rotationMagnitude * torqueFuelUsage;
        }



        //transform.rotation = Quaternion.Slerp(
        //    transform.rotation,
        //    targetRot,
        //    rotationSpeed);
        //Debug.Log(targetRot);
        //if(forwardDeltaMarker.localPosition.magnitude > 0.1f)
        //{
        //    Quaternion rot = Quaternion.Euler(
        //        (forwardDeltaMarker.position - transform.position)
        //        + transform.rotation.eulerAngles);

        //    transform.rotation = Quaternion.Slerp(
        //        transform.rotation,
        //        rot,
        //        1);
        //}
            
        //Debug.Log("1currentRot: " + transform.rotation.eulerAngles + "targetRot: " + targetRot.eulerAngles);
        //// slerp towards targetRot, allow when in all target modes
        //if(targetRot != transform.rotation)
        //transform.rotation = Quaternion.Slerp(
        //    transform.rotation,
        //    targetRot,
        //    1);
        //Debug.Log("2currentRot: " + transform.rotation.eulerAngles + "targetRot: " + targetRot.eulerAngles);


        // disable controls when fuel reaches 0
        if (resources.fuel == 0)
        {
            canvas.throttleSlider.value = 0;
            canvas.torqueKnob.anchoredPosition = Vector2.zero;
            canvas.stabilizeButtonState = false;
        }
        // ensure that fuel is never negative
        if (resources.fuel < 0f)
            resources.fuel = 0f;

    }

    // move rotation
    Quaternion targetRot = Quaternion.identity;
    bool rotating = false;
    public void BeginDrag()
    {
        // disable camera rotation when rotating player ship
        Camera.main.GetComponent<CameraController>().canRotate = false;

        rotating = true;
    }
    public void DuringDrag(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;

        // screen position of the marker
        Vector2 markerPos = Camera.main.WorldToScreenPoint(
            forwardMarker.position);

        // screen position of the pointer
        Vector2 pointerPos = pointerData.position;

        // vector from marker to pointer
        Vector2 pointerVector = (pointerPos - markerPos).normalized;
        Vector2 adjustedVector = pointerVector;
        adjustedVector.x = -adjustedVector.x;
        adjustedVector *= 0.3f;

        // move the forwardDeltaMarker as the player drags
        forwardDeltaMarker.localPosition = adjustedVector;

        // set the player rotation based on forwardMarker, forwardDeltaMarker, and player pos
        Vector3 actualRot = transform.rotation.eulerAngles;
        Vector3 calcRot = (forwardMarker.position - transform.position).normalized * 360f;
        //Debug.Log(actualRot + " " + calcRot);
        //Debug.Log(rotating);
    }
    public void EndDrag()
    {
        // re-enable after drag is finished
        Camera.main.GetComponent<CameraController>().canRotate = true;
        rotating = false;
        // reset forward marker's positions
        StartCoroutine("ResetForwardMarker");

        // set target mode to stabilize
        targetMode = 0;
        
    }
    float markerResetSpeed = 0.3f;
    IEnumerator ResetForwardMarker()
    {
        while (forwardDeltaMarker.localPosition != Vector3.zero)
        {
            yield return new WaitForSeconds(0.02f);
            forwardDeltaMarker.localPosition = Vector3.Lerp(
                forwardDeltaMarker.localPosition,
                Vector3.zero,
                markerResetSpeed);
        }
            
        
    }


    // die on collision
    void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > maxVelocityOnCollision)
            StartCoroutine("Die");
        
    }

    public float maxVelocityOnCollision = 2;
    public GameObject explosionPrefab;
    IEnumerator Die()
    {
        explosionPrefab = Instantiate(explosionPrefab, transform);
        transform.Find("Model").gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
