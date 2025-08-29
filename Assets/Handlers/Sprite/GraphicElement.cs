using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicElement
{
    public string SpriteAtlasPath { get; set; }
    public string Filename { get; set; }
    public Dimensions Frame { get; set; }
    public uint FPS { get; set; } = 12;
    public uint Quantity { get; set; } = 1;
    public float Time
    {
        get => (float)Quantity / FPS;
        set{}
    }
}