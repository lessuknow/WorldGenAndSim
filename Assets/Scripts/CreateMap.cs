using System.Collections.Generic;
using UnityEngine;
using static OverworldGenerator;

[System.Serializable]
public struct Terrain
{
    public string Name;
    public float Height;
    public Color color;
}

public class CreateMap : MonoBehaviour
{
    public GameObject template;

    //TODO :Split this into two things.


    public int Seed;
    public float Scale;
    public int Octaves;
    public float Persistance;
    public float Lacunarity;
    public Vector2 Offset;
    public float heightMultiplier;

    public Terrain[] Regions;
    public Color borderColor;
    public int borderSize;
    public bool ignoreBorder;

    public bool useFlatShading;
    
    // NOTE: Sizes = num_cubes_in_axis - 1 (they represent the number of verts in each axis)
    public static int x_size { get; set; }
    public static int z_size { get; set; }
    public static MapTile[] tileMap;
    public static float[,] heightMap { get; set; }
    public AnimationCurve meshHeightCurve;

    public GameObject go;
    public GameObject chaseObj;

    // square tiles for now; maybe not even have them in future.
    public class MapTile {
        public float height;
        // more memory used, less calls made although.
        public float normal;
    }

    public void GenerateMapTiles()
    {
        tileMap = new MapTile[x_size * z_size];
        for(int z = 0;z < z_size; z++)
        {
            for(int x = 0; x < x_size; x++)
            {
                MapTile tile = new MapTile();
                tile.height = heightMap[x, z];
                tileMap[x + z * x_size] = tile;
            }
        }
    }

    public void Start()
    {
        TestingValues();
        heightMap = Noise.MakeNoiseMap(x_size, z_size, Seed, Scale, Octaves, Persistance, Lacunarity, Offset);
        GenerateMapMesh();
        GenerateMapTiles();
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        for(int i = 0;i < 10; i++)
        {
            // PRob switch go with actual objects.
            GameObject obj = Instantiate(chaseObj);
            int randX = Random.Range(0, x_size - 1);
            int randY = Random.Range(0, z_size - 1);
            obj.transform.position = new Vector3(randX + 0.5f, 1.35f, randY + 0.5f);
        }
    }

    private void TestingValues()
    {
        x_size = 30;
        z_size = 30;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            Seed += Random.Range(1, 1000);
            Start();
        }
    }

    // Move this stuff to a separate class. One class should be for mesh rendering / visual elements;
    // one for more overaching tiles
    public void GenerateMapMesh()
    {
        MeshFilter meshFilter = template.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = template.GetComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        Texture2D texture =
           GenerateTexture(GenerateColorMap(heightMap, x_size, z_size, Regions), borderColor, ignoreBorder, borderSize, x_size, z_size);
        MeshData meshData =
           GenerateMeshData(heightMap, x_size, z_size, heightMultiplier, meshHeightCurve);
       
        if (useFlatShading)
            meshData = FlatShading(meshData);

        mesh.name = "New Mesh";
        mesh.vertices = meshData.verticies.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.uv = meshData.uvs.ToArray();

        // Implement own normal generation
        mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public MeshData FlatShading(MeshData meshData)
    {
        Vector3[] flatShadedVerts = new Vector3[meshData.triangles.Count];
        Vector2[] flatShadedUVs = new Vector2[meshData.triangles.Count];

        for(int i=0;i < meshData.triangles.Count;i++)
        {
            flatShadedVerts[i] = meshData.verticies[meshData.triangles[i]];
            flatShadedUVs[i] = meshData.uvs[meshData.triangles[i]];
            meshData.triangles[i] = i;
        }
        meshData.verticies = new List<Vector3> (flatShadedVerts);
        meshData.uvs = new List<Vector2>(flatShadedUVs);
        return meshData;
    }
    

}
