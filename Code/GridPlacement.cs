using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacement : MonoBehaviour {

    // Make a grid scale; 1==1 Unit
    public int gridScale = 1;
    // find our camera
    public Camera camera;
    //Cube Prefab
    public GameObject Prefab;
    //Ghost Cube Transparency Value
    public float GhostTransparency = 0;
    //Ghost Cube GameObject
    GameObject ghostCube;
    //Placeable Prefabs list
    public static List<GameObject> PlacablePrefabs = new List<GameObject>();

    bool capslock = false;

    // Use this for initialization
    void Start () {
        //Remove previous Ghosts
        Destroy(ghostCube);
        //Make new ghost
        ghostCube = Instantiate(Prefab, new Vector3(0, 0, 0), Quaternion.identity);
        //Set the ghost cube to transparent
        ghostCube.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, GhostTransparency);
        //Fetch the GameObject's Collider (make sure it has a Collider component)
        Collider ghost_Collider = ghostCube.GetComponent<Collider>();
        //Disable the ghostCube collider
        ghost_Collider.enabled = false;

        //Test Code
        Object[] prefabs = Resources.LoadAll("Prefabs", typeof(GameObject));

        //define a list
        //THIS IS REALLY MESSY, SUGGESTIONS NEEDED
        foreach (GameObject prefab in prefabs)
        {
            GameObject lo = (GameObject)prefab;

            if (lo.gameObject.tag == "Destroyable")
            {
                PlacablePrefabs.Add(lo);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            capslock = !capslock;
        }

        if (capslock == true)
        {
            PlaceObject();
        }
    }

    void PlaceObject () {
        // Camera Raycast
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        //camera rotation snapped
        Vector3 rotation = camera.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        rotation.y = Mathf.Round(rotation.y / 90) * 90;
        Quaternion rotationEuler = Quaternion.Euler(rotation);

        if (Physics.Raycast(ray, out hit))
        {
            //Selection list based on user input
            if (Input.GetKeyDown("1"))
            {
                Prefab = PlacablePrefabs[1];
                Start();
            }
            else if (Input.GetKeyDown("2"))
            {
                Prefab = PlacablePrefabs[2];
                Start();
            }
            else if (Input.GetKeyDown("3"))
            {
                Prefab = PlacablePrefabs[3];
                Start();
            }
            else if (Input.GetKeyDown("4"))
            {
                Prefab = PlacablePrefabs[4];
                Start();
            }

            //Show the ghost object
            ghostCube.GetComponent<Renderer>().enabled = true;

            //Get our hit point
            Vector3 position = hit.point;

            // Round Raycast Hit position to our grid
            position /= gridScale;
            position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
            position *= gridScale;

            if (Input.GetButtonDown("Fire2"))
            {
                GameObject placed = Instantiate(Prefab, position, Quaternion.identity);
                placed.transform.rotation = rotationEuler;
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                if (hit.transform.gameObject.tag == "Destroyable")
                {
                    Destroy(hit.transform.gameObject);
                }
            }
            else
            {
                //Ghost Block Code
                ghostCube.transform.position = position;
                ghostCube.transform.rotation = rotationEuler;
            }
        }
        else
        {
            //Hide the ghost object
            ghostCube.GetComponent<Renderer>().enabled = false;
        }
    }
}
