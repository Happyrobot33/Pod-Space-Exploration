using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extensions {

	public static Transform extFind (this Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
            return result;
        foreach (Transform child in parent)
        {
            result = child.extFind(name);
            if (result != null)
                return result;
        }
        return null;
    }
    public static List<Transform> extFindAll(this Transform parent, string name)
    {
        List<Transform> results = new List<Transform>();
        
        foreach (Transform child in parent)
        {
            if (child.name == name)
                results.Add(child);
            List<Transform> tmp = child.extFindAll(name);
            if(tmp != null)
                results.AddRange(tmp);
        }
        if (results.Count != 0)
            return results;
        return null;
    }

    public static Vector2 extScreenPosition (this RectTransform transform)
    {
        Vector3 worldPos = transform.position;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        return screenPos;
    }


    // mass needs to be between 10^-7 and 10^9
    // // sun mass: 10^8 ... actual: 10^30
    // // player mass: 10^-6 ... actual: 10^3

    ///// unity mass >>> actual mass
    //////////// y = 10^22 x - 10^16 + 10^3
    ///// actual mass >>> unity mass
    //////////// y = 10^-22 x + 10^-6 - 10^-19

    public const float gravityConstant = 0.0000000000667f;
    public const float lightSpeed = 299792458f; // speed of light, m/s

    public static float ToUnityMass(double actualMass)
    {
        double a = 3.7449021648810784320030912401521619359464240388E-8;
        double b = 0.3856639880926862489769230312191934905431038042013;
        double unityMass = a * Math.Pow(actualMass, b);
        return (float) unityMass;
    }
    public static double ToActualMass(float unityMass)
    {
        double a = 18053203938204973569.08775292159009917147267492461;
        double b = 2.592930713975998656800981349623178073340584991533;
        double actualMass = a * Math.Pow(unityMass, b);
        return Math.Round(actualMass, 0);
    }

    ////// mass mappings
    //// 1e-6 Ug = 5000 kg
    //// 1e+8 Ug = 1e+40 kg

    // this uses an exponential model, which is okay because
    //// nothing uses the unity mass

    // also, unityMass is always float, and actualMass is always double





}
