using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicElement
{
    public string SpriteAtlasPath { get; set; }
    public string Filename { get; set; }
    public Dimensions Frame { get; set; }
    public int FPS { get; set; } = 12;
    public int Quantity { get; set; } = 1;

    public float Time
    {
        get => (float)Quantity / FPS;
        set { }
    }
    public float Delay
    {
        get => 1f / Time;
        set { }
    }
}