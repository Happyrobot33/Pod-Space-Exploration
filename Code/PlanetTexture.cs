using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlanetTexture : MonoBehaviour {

    public int textureSize = 100;

    int n_lands = 1;
    int n_land_extensions = 3;
    float landSize = 50f;
    int fractIterations = 1;


    public float landRandomness = 1;

    Color[,] textureData;

    public List<int> growData = new List<int>();
    public List<int> pruneData = new List<int>();

    int iceSize = 15;
    float iceThickness = 9f;


    int width;
    int height;
	void Start ()
    {
        width = 2 * textureSize;
        height = textureSize;

        iceSize = Mathf.RoundToInt(height / 8f);
        iceThickness = Mathf.RoundToInt(height / 16f);

        textureData = new Color[width, height];
        // fill with ocean
        for(int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                textureData[j, i] = new Color(0.443f, 0.776f, 0.878f);
            }
        }

        //generate land
        int[,] landData = new int[width, height];
        // fill with random
        for (int i = 0; i < height; i++)
        {
            float landness = 1;
            for (int j = 0; j < width; j++)
            {
                float noise = landness * Mathf.PerlinNoise(
                    0.05f * (j + (landRandomness * Random.value)),
                    0.05f * (i + (landRandomness * Random.value)) );
                landness += Mathf.Round(0.1f * noise);
                landData[j, i] = Mathf.RoundToInt(noise);
            }
        }

        landData = Procedurize(landData, pruneData, 0, CompareMode.LessThan);
        landData = Procedurize(landData, growData, 1, CompareMode.GreaterThan);
        landData = Procedurize(landData, pruneData, 0, CompareMode.LessThan);


        // convert landData
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (landData[j, i] == 1)
                    textureData[j, i] = new Color(0.262f, 0.623f, 0.27f);

                bool withinIceLimit = i < iceSize || height - i < iceSize;

                int distanceFromEdge;
                if (i < height / 2)
                    distanceFromEdge = i + 1;
                else
                    distanceFromEdge = height - i;

                if ( (iceThickness / distanceFromEdge) > Random.value && withinIceLimit)
                    textureData[j, i] = Color.white;
            }
        }

        // convert textureData
        List<Color> data = new List<Color>();
        for (int i = 0; i < textureSize; i++)
            for (int j = 0; j < 2*textureSize; j++)
                data.Add(textureData[j, i]);

        // apply textureData to the planet
        Texture2D texture = new Texture2D(2 * textureSize, textureSize);
        texture.SetPixels(data.ToArray());
        texture.Apply();
        GetComponent<MeshRenderer>().material.mainTexture = texture;

        // save texture
        //System.IO.File.WriteAllBytes(Application.persistentDataPath + "texture.png", texture.EncodeToPNG());
    }

    enum CompareMode
    {
        LessThan,
        GreaterThan,
        EqualTo
    }
    int[,] Procedurize(int[,] landData, List<int> thresholdData, 
        int outcome, CompareMode procedureMode)
    {
        foreach (int neighborThreshold in thresholdData)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    // set up vars
                    int x = j;
                    int prevX = x - 1;
                    if (prevX < 0)
                        prevX = width + prevX;
                    int nextX = x + 1;
                    if (nextX >= width)
                        nextX -= width;

                    int y = i;
                    int prevY = y - 1;
                    if (prevY < 0)
                        prevY = height + prevY;
                    int nextY = y + 1;
                    if (nextY >= height)
                        nextY -= height;

                    // set up neighbors
                    int[] neighbors = new int[8];

                    neighbors[0] = landData[prevX, prevY];
                    neighbors[1] = landData[x, prevY];
                    neighbors[2] = landData[nextX, prevY];

                    neighbors[3] = landData[prevX, y];
                    neighbors[4] = landData[nextX, y];

                    neighbors[5] = landData[prevX, nextY];
                    neighbors[6] = landData[x, nextY];
                    neighbors[7] = landData[nextX, nextY];

                    // calculate how many neighbors
                    int n_neighbors = 0;
                    foreach (int neighbor in neighbors)
                        if (neighbor == 1)
                            n_neighbors++;

                    // choose based on neighbors
                    if (procedureMode == CompareMode.LessThan &&
                        n_neighbors < neighborThreshold)
                        landData[x, y] = outcome;
                    if (procedureMode == CompareMode.GreaterThan &&
                        n_neighbors > neighborThreshold)
                        landData[x, y] = outcome;
                    if (procedureMode == CompareMode.EqualTo &&
                        n_neighbors == neighborThreshold)
                        landData[x, y] = outcome;
                }
            }
        }
        return landData;
    }

    void GenerateFractalLand()
    {
        for (int i = 0; i < n_lands; i++)
        {
            Vector2 landOrigin = new Vector2(
                Random.Range(0, (2 * textureSize) - 1),
                Random.Range(0, textureSize - 1));
            landOrigin = new Vector2(textureSize, 0.5f * textureSize);
            //generate primitive shape of land
            List<Vector2> landVerts = new List<Vector2>();
            for (int j = 0; j < n_land_extensions; j++)
            {
                float angle = Random.Range(0, 2f * Mathf.PI);
                Vector2 vert = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                //Debug.Log(vert);
                vert = (landSize * vert) + landOrigin;
                //Debug.Log(vert);
                landVerts.Add(vert);
            }

            //Debug.Log("a" + landOrigin);



            // fractalize the land
            for (int j = 0; j < fractIterations; j++)
            {
                landVerts = FractalizeLand(landVerts, 0.5f, 1f);
            }

            // draw vertz
            for (int j = 0; j < landVerts.Count; j++)
            {
                textureData[Mathf.RoundToInt(landVerts[j].x),
                    Mathf.RoundToInt(landVerts[j].y)] = Color.red;
            }

            // draw land, loop through all points
            for (int j = 0; j < 2 * textureSize; j++)
            {
                for (int k = 0; k < textureSize; k++)
                {
                    int n_intersections = 0;
                    for (int l = 0; l < landVerts.Count; l++)
                    {
                        //// solve the system, to find intersection
                        // y = k
                        // y = m * x + b

                        // k = m * x + b
                        // x = (k-b) / m
                        Vector2 currentPt = landVerts[l];
                        Vector2 nextPt;
                        if (l + 1 == landVerts.Count)
                            nextPt = landVerts[0];
                        else
                            nextPt = landVerts[l + 1];

                        // find equation for line between current and next
                        Vector2 delta = nextPt - currentPt;
                        float m = delta.y / delta.x;
                        float b = currentPt.y - (m * currentPt.x);

                        // lines cannot be parallel
                        if (m == 0)
                            continue;
                        float intersect_x = (k - b) / m;
                        Vector2 intersect = new Vector2(intersect_x, k);

                        // intersection must be on the segment
                        if (currentPt.magnitude <= intersect.magnitude &&
                           intersect.magnitude <= nextPt.magnitude)
                            n_intersections++;
                    }
                    // if odd number of intersections
                    if (n_intersections % 2 == 1)
                        textureData[j, k] = Color.green;
                }
            }

        }
    }

    List<Vector2> FractalizeLand(List<Vector2> landVerts,
        float hRandRange, float vRandRange)
    {
        List<Vector2> verts = new List<Vector2>();
        for (int j = 0; j < landVerts.Count; j++)
        {
            Vector2 currentPt = landVerts[j];
            Vector2 nextPt;
            if (j + 1 == landVerts.Count)
                nextPt = landVerts[0];
            else
                nextPt = landVerts[j + 1];

            // 0.5 +- h, between 0 and 1
            float horizontalRand = 0.5f + Random.Range(-hRandRange, hRandRange);
            Vector2 delta = nextPt - currentPt;
            Vector2 midPt = currentPt + (horizontalRand * delta);

            float verticalRand = Random.Range(-vRandRange, vRandRange);
            Vector2 perpDirection = delta.normalized;
            perpDirection = new Vector2(perpDirection.y, perpDirection.x);
            Vector2 fractPt = midPt + (verticalRand * perpDirection);


            verts.Add(currentPt);
            verts.Add(fractPt);
        }
        return verts;
    }

    // fractal land that didn't work



}
