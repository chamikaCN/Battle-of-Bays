using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    int MeshHeight = 20, MapWidth = 100, MapLength = 100, Octaves = 4;
    float Persistance = 0.5f, Lacunarity = 2;
    public int Seed;
    public float Scale = 20;
    public bool AutoUpdate;
    public int FallOffSpread;
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public enum TreeCount { None, Low, Medium, High };
    public TreeCount treeCount;
    Hashtable treeCountNumber = new Hashtable()
                {{ TreeCount.None, 0 }, { TreeCount.Low, 40 }, { TreeCount.Medium, 90 }, { TreeCount.High, 200 }};

    public GameObject[] TreePrefabs;
    public enum OthersCount { None, Low, Medium, High };
    public OthersCount otherCount;
    Hashtable otherCountNumber = new Hashtable()
                {{ OthersCount.None, 0 }, { OthersCount.Low, 10 }, { OthersCount.Medium, 22 }, { OthersCount.High, 50 }};
    public GameObject[] OtherPrefabs;
    [Header("Region Colors")]
    public TerrainType[] regions;
    MeshData mapMeshData;
    NavMeshSurface surface;
    System.Random commonRandom;

    [System.Serializable]
    public struct TerrainType
    {
        public string Name;
        public float Height;
        public Color Color;
        public bool BlendColors;

    }


    public void GenerateMap(int gameSeed)
    {
        Seed = gameSeed;
        surface = GetComponent<NavMeshSurface>();
        clearPlacedObjects();
        commonRandom = new System.Random();

        float[,] noiseMap = generateNoiseMap();
        AddFalloffEfect(noiseMap);

        Texture2D texture = colorMapToTexture(noiseMap, MapWidth, MapLength, regions, MeshHeight);
        drawTexture(texture);
        mapMeshData = GenerateTerrainMesh(noiseMap, MeshHeight);
        drawMesh(mapMeshData);
        placeObjects(mapMeshData);
        surface.BuildNavMesh();

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

    Texture2D heightMapToTexture(float[,] heightMap, int width, int length)
    {
        Texture2D texture = new Texture2D(width, length);

        Color[] colorMap = new Color[length * width];

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[z * width + x] = Color.Lerp(Color.white, Color.black, heightMap[x, z]);
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        return createTexture(colorMap, width, length);
    }

    Texture2D colorMapToTexture(float[,] noiseMap, int width, int length, MapGenerator.TerrainType[] regions, int meshHeight)
    {
        Texture2D texture = new Texture2D(width, length);

        Color[] colorMap = new Color[length * width];

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, z];
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

    void drawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(-texture.width, 1, texture.height);
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

    void OnValidate()
    {
        if (MapWidth < 1) { MapWidth = 1; }
        if (MapLength < 1) { MapLength = 1; }
        if (Lacunarity < 1) { Lacunarity = 1; }
        if (Octaves < 0) { Octaves = 0; }
        if (MapWidth < 1) { MapWidth = 1; }
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

    public List<GameObject> PlaceShips(GameObject model, int count)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int r = 0; r < count; r++)
        {
            Vector3 place = getValidShipPlacement(commonRandom);
            GameObject go = Instantiate(model, place, Quaternion.identity);
            go.transform.localScale = go.transform.localScale * 0.5f;
            gameObjects.Add(go);
        }
        return gameObjects;
    }

    Vector3 getValidShipPlacement(System.Random random)
    {
        int val = random.Next(MapLength * MapWidth);
        float x = mapMeshData.vertices[val].x;
        float z = mapMeshData.vertices[val].z;
        float y = mapMeshData.vertices[val].y;

        float originalX = 5 * (x);
        float originalZ = 5 * (z);

        if (y < 10 && getNeighbourSeaPlaneCount(mapMeshData, val) > 7)
        {
            return new Vector3(originalX, 10, originalZ);
        }
        else
        {
            return getValidShipPlacement(random);
        }
    }

    public List<GameObject> PlaceDocks(GameObject model, int count)
    {
        System.Random rand = new System.Random();
        List<Vector3> dockplaces = detectSeaPlane(mapMeshData);
        int totalCount = dockplaces.Count;
        List<GameObject> gameObjects = new List<GameObject>();
        for (int r = 0; r < count; r++)
        {
            GameObject go = Instantiate(model, dockplaces[(totalCount / count) * r], Quaternion.identity);
            go.transform.localScale = go.transform.localScale * 0.5f;
            gameObjects.Add(go);
        }
        return gameObjects;
    }



    List<Vector3> detectSeaPlane(MeshData md)
    {
        List<Vector3> dockplaces = new List<Vector3>();
        for (int g = 0; g < MapLength * MapWidth; g++)
        {
            float x = md.vertices[g].x;
            float z = md.vertices[g].z;
            float y = md.vertices[g].y;

            float originalX = 5 * (x);
            float originalZ = 5 * (z);

            if (y > 10 && getNeighbourSeaPlaneCount(md, g) > 4)
            {
                dockplaces.Add(new Vector3(originalX, 2.5f * y, originalZ));
            }
        }
        return dockplaces;
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
