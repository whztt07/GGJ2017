﻿using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour {

    GameFlowManager gameFlowManager;

    class WaveItem
    {
        public Func<float, float> Function;
        public float X = 0.0f;

        public float Update(float step)
        {
            X += step;
            return Function(X);
        }
    }

    List<WaveItem> waves = new List<WaveItem>();

    public Vector2 Margin = new Vector2(-4.0f, 4.0f);

    public void AddWave(Func<float, float> wave, float phase = 0.0f)
    {
        waves.Add(new WaveItem() { Function = wave, X = phase });
    }

    float WaveFunction(float x)
    {
        return Mathf.Sin(20.0f * Mathf.PI * x);
    }

    EdgeCollider2D waveCollider;

    public Vector2 WaveScale = new Vector2(1, 1);
    public float Speed = 1.0f;

    float GetSpeed()
    {
        return Speed * gameFlowManager.Speed;
    }

    List<Vector2> points = new List<Vector2>();

    WaveCollection WaveCollection = new WaveCollection();

    public GameObject BrickScheduler;
    public GameObject BrickGenerator;


    WaveItem ambientWave = new WaveItem() { Function = WaveCollection.StandardWave, X = -2000.0f };

    public int MaxPoints = 256;
	public List<Vector2> Points;

    // Use this for initialization
    void Awake () { 
        waveCollider = GetComponent<EdgeCollider2D>();
        points.Add(new Vector2(60.0f, 0.0f));
    }

    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("MainLoop");
        gameFlowManager = obj.GetComponent<GameFlowManager>();
    }

    public float Height = 0.0f;
    public float Length = 0.0f;

    float GetDificultyMultiplier()
    {
        if (gameFlowManager.GetGameState() != GameFlowManager.GameState.GS_Play)
            return 1.0f;

        return 1.0f + gameFlowManager.m_score / 20.0f;
    }

    void UpdateWaves(float time)
    {
        float step = time / WaveScale.x;
        float value = 0.0f;

        List<WaveItem> toRemove = new List<WaveItem>();

        waves.ForEach(wave =>
        {
            value += wave.Update(step);

            if (wave.X > 1.0f)
            {
                toRemove.Add(wave);
            }
        });

        toRemove.ForEach(wave => waves.Remove(wave));

        value = Mathf.Clamp(value, Margin.x, Margin.y);

        value += ambientWave.Update(step);


        Height = value * WaveScale.y * GetDificultyMultiplier();
        Length += time * GetSpeed();

        if (points.Count > MaxPoints)
            points.RemoveAt(0);

        points.Add(new Vector2(-Length, Height));
    }

    System.Random rand = new System.Random();

    public void AddWaveButtonClick()
    {
        int index = rand.Next() % WaveCollection.Waves.Count;
        Func<float, float> func = WaveCollection.Waves[index];
        AddWave(func);

        if(BrickGenerator!=null)
        {
            BrickGenerator bg = BrickGenerator.GetComponent<BrickGenerator>();
            if(bg!=null)
            {
                //bg.Generate(index);
                bg.GenerateBrickInCanvas(index);
            }
        }
    }

	// Update is called once per frame
	void FixedUpdate () {

        UpdateWaves(Time.deltaTime);
        
		waveCollider.points = points.ToArray();
		Points = points;

		transform.position = transform.position + new Vector3(Time.deltaTime * GetSpeed(), 0.0f, 0.0f);

        if(null != BrickScheduler )
        {
            BrickScheduler scheduler = BrickScheduler.GetComponent<BrickScheduler>();
            if (scheduler != null)
            {
                if (scheduler.GetNext() <= Time.realtimeSinceStartup)
                {
                    scheduler.Consume();
                    AddWaveButtonClick();
                }
            }
        }

    }
}
