using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverworldGenerator
{

    public class MeshData
    {
        public List<Vector3> verticies { get; set; }
        public List<Vector2> uvs { get; set; }
        public List<int> triangles { get; set; }

        public MeshData()
        {
            verticies = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();
        }
    }

    public static Color[] GenerateColorMap(float[,] heightMap, int x_size, int z_size, Terrain[] Regions)
    {
        // create map
        Color[] colorMap = new Color[x_size * z_size];
        for (int z = 0; z < z_size; z++)
        {
            for (int x = 0; x < x_size; x++)
            {
                float currentHeight = heightMap[x, z];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight <= Regions[i].Height)
                    {
                        colorMap[z * x_size + x] = Regions[i].color;
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

    public static Texture2D GenerateTexture(Color[] colorMap, Color borderColor, bool ignoreBorder, int borderSize, int x_size, int z_size) {
        // generate texture from created map and applya border.
        // TODO: MAke bordercolor transparent; layer colors.
            
        Texture2D texture = new Texture2D(x_size * borderSize, z_size * borderSize);
        for (int mapZ = 0; mapZ < z_size; mapZ++)
        {
            for (int mapX = 0; mapX < x_size; mapX++)
            {
                for (int z = 0; z < borderSize; z++)
                {
                    for (int x = 0; x < borderSize; x++)
                    {
                        if (!ignoreBorder && (x == 0 || x == borderSize - 1 || z == 0 || z == borderSize - 1))
                            texture.SetPixel(mapX * borderSize + x, mapZ * borderSize + z, borderColor);
                        else
                            texture.SetPixel(mapX * borderSize + x, mapZ * borderSize + z, colorMap[mapX + mapZ * x_size]);
                    }
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }

    public static MeshData GenerateMeshData(float[,] heightMap, int x_size, int z_size, float heightMultiplier, AnimationCurve heightCurve)
    {
        MeshData data = new MeshData();

        //Create Verts
        int cur_x = 0, cur_z = 0;
        // replace 1 w 
            // heightCurve.Evaluate(heightMap[x, z])* heightMultiplier
        //Generate verts, first by x then by z
        for (int z = 0; z < z_size; z++)
        {
            for (int x = 0; x < x_size; x++)
            {
                data.verticies.Add(new Vector3(cur_x, 1, cur_z));
                data.uvs.Add(new Vector2(cur_x / (float)x_size, cur_z / (float)z_size));
                cur_x++;
            }
            cur_x = 0;
            cur_z++;
        }

        // Create trianges
        for (int x = 1; x < x_size; x++)
        {
            for (int z = 1; z < z_size; z++)
            {
                // Add a square to the mesh.
                // We make sure that we're at a square's bottom-right vert, and then find that square's upper-left vert and construct it from there.

                // this is the upper-left position of the cube in regards to verts.
                int vertUpperLeft = (x - 1) + (z - 1) * x_size;

                int vertUpperRight = vertUpperLeft + 1;
                int vertLowerLeft = vertUpperLeft + x_size;
                int vertLowerRight = vertUpperLeft + x_size + 1;

                data.triangles.Add(vertUpperLeft);
                data.triangles.Add(vertLowerLeft);
                data.triangles.Add(vertUpperRight);

                data.triangles.Add(vertUpperRight);
                data.triangles.Add(vertLowerLeft);
                data.triangles.Add(vertLowerRight);
            }
        }
        return data;
    }
}
