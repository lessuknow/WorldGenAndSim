using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] MakeNoiseMap(int mapXSize, int mapZSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] NoiseMap = new float[mapXSize, mapZSize];

        System.Random pseudorng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float halfWidth = mapXSize / 2f;
        float halfHeight = mapXSize / 2f;

        for (int i=0;i < octaves; i++)
        {
            float offsetX = pseudorng.Next(-10000, 10000) + offset.x;
            float offsetY = pseudorng.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for(int y = 0;y < mapZSize; y++)
        {
            for(int x=0;x<mapXSize;x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i=0;i < octaves;i++) { 
                    float sampleX = (x- - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;
                NoiseMap[x, y] = noiseHeight;

            }
        }
        for (int y = 0; y < mapZSize; y++)
        {
            for (int x = 0; x < mapXSize; x++)
            {
                // Noramlize
                NoiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, NoiseMap[x, y]);

                // After doing all that, round to .1's. 
                NoiseMap[x, y] = (int)(NoiseMap[x, y] * 10);
                NoiseMap[x, y] /= 10;
            }
        }
        return NoiseMap;
    }
}
