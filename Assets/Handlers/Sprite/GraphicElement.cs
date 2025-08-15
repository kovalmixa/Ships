using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicElement
{
    public string Filename { get; set; }
    public Dimensions Frame { get; set; }
    public uint FPS { get; set; } = 0;
    public uint Quantity { get; set; } = 1;
}