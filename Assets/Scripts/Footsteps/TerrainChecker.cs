/**
 * Description: Tool to extract the texture name from the "ground" object that is colliding with FootstepCollider
 * Authors: Tomek, Michał
 * Copyright: © 2022 Animal Shelter. All rights reserved.
 **/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class TerrainChecker
{
    private float[] GetTextureMix(Vector3 position, Terrain terrain)
    {
        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;
        int x = Mathf.RoundToInt((position.x - terrainPosition.x) / terrainData.size.x * terrainData.alphamapWidth);
        int z = Mathf.RoundToInt((position.z - terrainPosition.z) / terrainData.size.z * terrainData.alphamapHeight);
        float[,,] splatMapData = terrainData.GetAlphamaps(x, z, 1, 1);

        float[] mix = new float[splatMapData.GetUpperBound(2) + 1];

        for (int i = 0; i < mix.Length; i++)
        {
            mix[i] = splatMapData[0, 0, i];
        }

        return mix;
    }

    public string GetTerrainName(Vector3 position, Terrain terrain, bool debugging = false)
    {
        float[] mix = GetTextureMix(position, terrain);
        float strongestTextureInTheMix = 0;
        int index = 0;

        for (int i = 0; i < mix.Length; i++)
        {
            if (mix[i] > strongestTextureInTheMix)
            {
                index = i;
                strongestTextureInTheMix = mix[i];
            }
        }

        return terrain.terrainData.terrainLayers[index].name;
    }
}
