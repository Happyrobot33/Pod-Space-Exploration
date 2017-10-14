using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour {

    public float fuel = 5f;
    public float maxFuel = 10f;

    //public float shipHardness = 1f;
    //public float maxResourceVolume = 100f;

    //public class Resource
    //{
    //    public string name;
    //    public Material material;
    //    public float amount = 0f;
    //    public Resource(string n, Material m)
    //    {
    //        name = n;
    //        material = m;
    //    }
    //}
    //public List<Material> resourceMaterials = new List<Material>();
    //List<Resource> resources = new List<Resource>();
    //void OnEnable()
    //{
    //    resources.Clear();
    //    resources.Add(new Resource("hydrogen", resourceMaterials[0])); // fuel
    //    resources.Add(new Resource("oxygen", resourceMaterials[1])); // fuel
    //    resources.Add(new Resource("titanium", resourceMaterials[2])); // ship-shell
    //}

    //public float collectionSpeed = 1f;
    //float totalResources = 0f;
    //public float maxResources = 100f;
    ////void OnCollisionStay(Collision collision)
    ////{
    ////    Material mat = collision.gameObject.GetComponent<MeshRenderer>().material;
    ////    if(mat != null && totalResources <= maxResources)
    ////    {
    ////        Resource r = resources[resourceMaterials.IndexOf(mat)];
    ////        r.amount += collectionSpeed;
    ////    }
    ////}

    //void Update()
    //{
    //    foreach (Resource r in resources)
    //        totalResources += r.amount;
    //}

}
