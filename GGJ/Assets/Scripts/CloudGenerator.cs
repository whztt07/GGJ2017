﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour {

    public float MinRangeX = -10.0f;
    public float MaxRangeX = 10.0f;
    public float MinRangeY = 3.0f;
    public float MaxRangeY = 5.0f;

    public float NewCloudXOffset = -15.0f;

    /// <summary>
    /// Reference to the current boat
    /// </summary>
    public Transform Boat;

    /// <summary>
    /// could prefab
    /// </summary>
    public Transform CloudTransform;

    /// <summary>
    ///  Initial Number of clouds
    /// </summary>
    public int InitialClouds = 10;

    public float MinimalDeltaTime = 2.0f;

    // Use this for initialization
    void Start () {
        for(int c=0; c< InitialClouds; c++)
        {
            GenerateCloud(Boat.position);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if(CheckTIme())
        {
            GenerateCloud(new Vector3(Boat.position.x + NewCloudXOffset, Boat.position.y, Boat.position.z));
        }
    }


    private float deltaSinceLastCloudGenerated = 0;
    bool CheckTIme ()
    {
        if(deltaSinceLastCloudGenerated > MinimalDeltaTime)
        {
            deltaSinceLastCloudGenerated = 0.0f;
            return true;
        }

        deltaSinceLastCloudGenerated += Time.deltaTime;

        return false;
    }

    /// <summary>
    /// Generate a new CloudInstance
    /// </summary>
    private void GenerateCloud(Vector3 position)
    {
        Vector3 pos = new Vector3(position.x, position.y, position.z);

        pos.x += Random.Range(MinRangeX, MaxRangeX);
        pos.y += Random.Range(MinRangeY, MaxRangeY);

        Transform cloud = Instantiate(CloudTransform, pos, Quaternion.identity);
        float scale = Random.Range(CloudInstance.MinScale, CloudInstance.MaxScale);
        cloud.localScale = new Vector3(cloud.localScale.x * scale, cloud.localScale.y * scale, cloud.localScale.z);

        cloud.SetParent(transform);
    }
}
