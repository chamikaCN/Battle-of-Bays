﻿using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    int MeshHeight = 20, MapWidth = 100, MapLength = 100, Octaves = 4;
    float Persistance = 0.5f, Lacunarity = 2;
    public int Seed;
    public float Scale = 20;
    public int FallOffSpread;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public enum TreeCount { None, Low, Medium, High };
    public TreeCount treeCount;
    Hashtable treeCountNumber = new Hashtable()
                {{ TreeCount.None, 0 }, { TreeCount.Low, 40 }, { TreeCount.Medium, 150 }, { TreeCount.High, 400 }};
    public GameObject[] TreePrefabs;
    public enum OthersCount { None, Low, Medium, High };
    public OthersCount otherCount;
    Hashtable otherCountNumber = new Hashtable()
                {{ OthersCount.None, 0 }, { OthersCount.Low, 10 }, { OthersCount.Medium, 22 }, { OthersCount.High, 50 }};
    public GameObject[] OtherPrefabs;
    [Header("Region Colors")]
    public TerrainType[] regions;
    public RawImage image;
    MeshData mapMeshData;
    float[,] noiseMap;
    List<Placement> selectedDockPlacements;
    List<int> selectedPlacementIntegers;
    List<Vector3> selectedPlacementVectors;
    NavMeshSurface surface;
    System.Random commonRandom;
    int playerHQPos, enemyHQPos;

    [System.Serializable]
    public struct TerrainType
    {
        public string Name;
        public float Height;
        public Color Color;
        public bool BlendColors;
        public TerrainType(string name, float height, Color color, bool Blend)
        {
            this.Name = name;
            this.Height = height;
            this.Color = color;
            this.BlendColors = Blend;
        }

    }

    [System.Serializable]
    public class Placement
    {
        public Vector3 Vector { get; set; }
        public int integer { get; set; }
    }


    public void GenerateMap(int gameSeed)
    {
        Seed = gameSeed;
        surface = GetComponent<NavMeshSurface>();
        clearPlacedObjects();
        commonRandom = new System.Random();

        noiseMap = generateNoiseMap();
        AddFalloffEfect(noiseMap);

        mapMeshData = GenerateTerrainMesh(noiseMap, MeshHeight);
        drawMesh(mapMeshData);
        placeObjects(mapMeshData);
    }

    void clearPlacedObjects()
    {
        GameObject veg = GameObject.Find("Vegitation");
        int childCount = veg.transform.childCount;
        for (int m = 0; m < childCount; m++)
        {
            DestroyImmediate(veg.transform.GetChild(0).gameObject);
        }
        GameObject oth = GameObject.Find("OtherObjects");
        int childObCount = oth.transform.childCount;
        for (int m = 0; m < childObCount; m++)
        {
            DestroyImmediate(oth.transform.GetChild(0).gameObject);
        }
        GameObject shi = GameObject.Find("ShipWrecks");
        int childShCount = shi.transform.childCount;
        for (int m = 0; m < childShCount; m++)
        {
            DestroyImmediate(shi.transform.GetChild(0).gameObject);
        }
    }

    float[,] generateNoiseMap()
    {
        float[,] noiseMap = new float[MapWidth, MapLength];

        System.Random ran = new System.Random(Seed);
        Vector2[] octaveOffsets = new Vector2[Octaves];

        for (int i = 0; i < Octaves; i++)
        {
            float offsetX = ran.Next(-1000000, 1000000);
            float offsetZ = ran.Next(-1000000, 1000000);
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        float halfWidth = MapWidth / 2;
        float halfLength = MapLength / 2;

        if (Scale < 0.0001f) { Scale = 0.0001f; }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int z = 0; z < MapLength; z++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < Octaves; i++)
                {
                    float sampleX = (x - halfWidth) / Scale * frequency + octaveOffsets[i].x;
                    float sampleZ = (z - halfLength) / Scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= Persistance;
                    frequency *= Lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, z] = noiseHeight;
            }
        }

        for (int z = 0; z < MapLength; z++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }
        return noiseMap;
    }

    void AddFalloffEfect(float[,] nMap)
    {
        int length = nMap.GetLength(0);
        int width = nMap.GetLength(1);
        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float i = 1 - ((2 * x) / (float)width);
                float j = 1 - ((2 * z) / (float)length);

                float val = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
                float FOval = getFalloffValue(val);
                nMap[x, z] = Mathf.Clamp01(nMap[x, z] - FOval);
            }
        }

    }

    float getFalloffValue(float value)
    {
        float a = 3;
        float b = FallOffSpread;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    MeshData GenerateTerrainMesh(float[,] heightMap, int MeshHeight)
    {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;

        MeshData meshData = new MeshData(width, length);
        int vertexIndex = 0;

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, MeshHeight * heightMap[x, z], topLeftZ - z);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, z / (float)length);
                if (x < width - 1 && z < length - 1)
                {
                    meshData.addTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.addTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }

    void drawMesh(MeshData meshData)
    {
        Mesh m = meshData.CreateMesh();
        meshFilter.sharedMesh = m;
        meshColliderGenerator.instance.setMesh(m);
    }

    void placeObjects(MeshData md)
    {
        System.Random rand = new System.Random();
        int m = 0, n = 0;
        while (m < (int)(treeCountNumber[treeCount]) || n < (int)(otherCountNumber[otherCount]))
        {
            int val = rand.Next(MapLength * MapWidth);
            float x = md.vertices[val].x;
            float z = md.vertices[val].z;
            float y = md.vertices[val].y;

            float originalX = 5 * (x);
            float originalZ = 5 * (z);

            if (y > 11 && m < (int)(treeCountNumber[treeCount]))
            {
                int vegIndex = rand.Next(TreePrefabs.Length);
                GameObject go = Instantiate(TreePrefabs[vegIndex], new Vector3(originalX, (2.5f * y) + 2, originalZ), Quaternion.identity);
                go.transform.localScale = go.transform.localScale * 0.3f;
                go.transform.parent = GameObject.Find("Vegitation").transform;
                m++;
            }
            else if ((y < 10) && (x > (-MapWidth / 2) + 8) && (x < (MapWidth / 2) - 8) && (z > (-MapLength / 2) + 8) && (z < (MapLength / 2) - 8) && n < (int)(otherCountNumber[otherCount]))
            {
                int otherIndex = rand.Next(OtherPrefabs.Length);
                GameObject go = Instantiate(OtherPrefabs[otherIndex], new Vector3(originalX, (2.5f * 10) - 5, originalZ), Quaternion.identity);
                go.transform.localScale = go.transform.localScale * 0.5f;
                go.transform.parent = GameObject.Find("OtherObjects").transform;
                n++;
            }

        }
    }

    public void drawTexture()
    {
        Texture2D texture = colorMapToTexture(noiseMap, MapWidth, MapLength, regions, MeshHeight);
        image.texture = texture;
    }

    Texture2D colorMapToTexture(float[,] noiseMap, int width, int length, MapGenerator.TerrainType[] regions, int meshHeight)
    {
        Texture2D texture = new Texture2D(width, length);
        int buttonIndex = 0;

        Color[] colorMap = new Color[length * width];

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, z];
                if (!selectedPlacementIntegers.Contains(z * width + x))
                {
                    for (int i = 0; i < regions.Length; i++)
                    {

                        if (currentHeight <= (regions[i].Height) * 20 / meshHeight)
                        {
                            if (regions[i].BlendColors && i < regions.Length - 1)
                            {
                                colorMap[z * width + x] = Color.Lerp(regions[i].Color, regions[i + 1].Color, noiseMap[x, z]);
                            }
                            else
                            {
                                colorMap[z * width + x] = regions[i].Color;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    HUDManager.instance.setupButtons(x, z, MapWidth, MapLength, buttonIndex, selectedPlacementVectors[selectedPlacementIntegers.IndexOf(z * width + x)]);
                    colorMap[z * width + x] = Color.red;
                    buttonIndex++;
                }
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        return createTexture(colorMap, width, length);
    }

    Texture2D createTexture(Color[] colormap, int width, int length)
    {
        Texture2D texture = new Texture2D(width, length);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colormap);
        texture.Apply();
        return texture;
    }

    public void calculateDockPlacements(int count)
    {
        int loopCount = 0;

        int requiredPlacementCount = count;
        System.Random rand = new System.Random();
        List<Placement> allSuitableDockPlacemets = detectSeaPlane(mapMeshData);
        selectedDockPlacements = new List<Placement>();
        while (requiredPlacementCount > 0)
        {
            int randIndex = rand.Next(allSuitableDockPlacemets.Count);
            Placement randPlacement = allSuitableDockPlacemets[randIndex];
            bool isSuitable = true;
            loopCount += 1;
            foreach (Placement item in selectedDockPlacements)
            {
                float dist = getDistanceBetweenMapIntergers(randPlacement.integer, item.integer, MapWidth);
                if (dist < (1.414f * MapWidth / count))
                {
                    isSuitable = false;
                    allSuitableDockPlacemets.RemoveAt(randIndex);
                    break;
                }
            }
            if (isSuitable)
            {
                selectedDockPlacements.Add(randPlacement);
                allSuitableDockPlacemets.RemoveAt(randIndex);
                requiredPlacementCount -= 1;
            }
        }
        selectedDockPlacements.Sort((x, y) => x.integer.CompareTo(y.integer));
        selectedPlacementIntegers = selectedDockPlacements.Select(p => p.integer).ToList();
        selectedPlacementVectors = selectedDockPlacements.Select(p => p.Vector).ToList();
    }

    List<Placement> detectSeaPlane(MeshData md)
    {
        List<Placement> pl = new List<Placement>();
        for (int g = 0; g < MapLength * MapWidth; g++)
        {
            float x = md.vertices[g].x;
            float z = md.vertices[g].z;
            float y = md.vertices[g].y;

            float originalX = 5 * (x);
            float originalZ = 5 * (z);

            if (y > 10 && getNeighbourSeaPlaneCount(md, g) > 4)
            {
                Placement p = new Placement();
                p.Vector = new Vector3(originalX, 2.5f * y, originalZ);
                p.integer = g;
                pl.Add(p);
            }
        }
        return pl;
    }

    int getNeighbourSeaPlaneCount(MeshData md, int index)
    {
        int count = 0;
        if (index % MapWidth != 0 && index % MapWidth != MapWidth - 1 && index > MapWidth && index < (MapLength - 1) * MapWidth)
        {
            if (md.vertices[index - MapWidth - 1].y < 10) { count++; }
            if (md.vertices[index - MapWidth].y < 10) { count++; }
            if (md.vertices[index - MapWidth + 1].y < 10) { count++; }
            if (md.vertices[index - 1].y < 10) { count++; }
            if (md.vertices[index + 1].y < 10) { count++; }
            if (md.vertices[index + MapWidth - 1].y < 10) { count++; }
            if (md.vertices[index + MapWidth].y < 10) { count++; }
            if (md.vertices[index + MapWidth + 1].y < 10) { count++; }
        }
        return count;
    }

    public void BuildNavmesh()
    {
        surface.BuildNavMesh();
    }

    public List<GameObject> PlaceShips(GameObject model, int count, Vector3 hqLocation, bool isPlayerHQ)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int r = 0; r < count; r++)
        {
            Vector3 place = getValidShipPlacement(commonRandom, hqLocation, isPlayerHQ ? playerHQPos : enemyHQPos);
            GameObject go = Instantiate(model, place, Quaternion.identity);
            go.transform.localScale = go.transform.localScale * 0.3f;
            gameObjects.Add(go);
        }
        return gameObjects;
    }


    Vector3 getValidShipPlacement(System.Random random, Vector3 hqLocation, int hq)
    {
        int val = random.Next(hq - (5 * MapWidth), hq + (5 * MapWidth));
        float x = mapMeshData.vertices[val].x;
        float z = mapMeshData.vertices[val].z;
        float y = mapMeshData.vertices[val].y;

        float originalX = 5 * (x);
        float originalZ = 5 * (z);
        float distanceSquared = Mathf.Pow((originalX - hqLocation.x), 2) + Mathf.Pow((originalZ - hqLocation.z), 2);
        if (y < 10 && getNeighbourSeaPlaneCount(mapMeshData, val) > 7 && distanceSquared < 10000)
        {
            return new Vector3(originalX, 10, originalZ);
        }
        else
        {
            return getValidShipPlacement(random, hqLocation, hq);
        }
    }

    public GameObject placePlayerHQ(GameObject model, int index)
    {
        playerHQPos = selectedPlacementIntegers[index];
        GameObject go = Instantiate(model, selectedPlacementVectors[index], Quaternion.identity);
        go.transform.localScale = go.transform.localScale * 0.5f;
        selectedDockPlacements.RemoveAt(index);
        selectedPlacementIntegers.RemoveAt(index);
        selectedPlacementVectors.RemoveAt(index);
        return go;
    }

    public GameObject placeEnemyHQ(GameObject model)
    {
        float maxdist = 0; 
        int maxIndex = 0;
        for (int h = 0; h < selectedDockPlacements.Count; h++)
        {
            float dist = getDistanceBetweenMapIntergers(playerHQPos, selectedDockPlacements[h].integer, MapWidth);
            if ( dist > maxdist)
            {
                maxdist = dist;
                maxIndex = h;
            }
        }
        enemyHQPos = selectedPlacementIntegers[maxIndex];
        GameObject go = Instantiate(model, selectedPlacementVectors[maxIndex], Quaternion.identity);
        go.transform.localScale = go.transform.localScale * 0.5f;
        selectedDockPlacements.RemoveAt(maxIndex);
        selectedPlacementIntegers.RemoveAt(maxIndex);
        selectedPlacementVectors.RemoveAt(maxIndex);
        return go;
    }

    public List<GameObject> PlaceDocks(GameObject model)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Vector3 vec in selectedPlacementVectors)
        {
            GameObject go = Instantiate(model, vec, Quaternion.identity);
            go.transform.localScale = go.transform.localScale * 0.5f;
            gameObjects.Add(go);
        }
        return gameObjects;
    }

    public float getDistanceBetweenMapIntergers(int L1, int L2, int width)
    {
        int horizontalDistance = Mathf.Abs((L1 % width) - (L2 % width));
        int verticalDistance = Mathf.Abs((L1 / width) - (L2 / width));
        float squaredDistance = Mathf.Pow(horizontalDistance, 2) + Mathf.Pow(verticalDistance, 2);
        return Mathf.Sqrt(squaredDistance);
    }
}


public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] UVs;

    int triangleIndex = 0;

    public MeshData(int width, int length)
    {
        vertices = new Vector3[width * length];
        UVs = new Vector2[width * length];
        triangles = new int[(width - 1) * (length - 1) * 6];
    }

    public void addTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
