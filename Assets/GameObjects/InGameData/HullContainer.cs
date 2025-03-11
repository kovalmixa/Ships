using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullContainer
{
    public string id;
    public General general = new General();
    public PhysicsData physics = new PhysicsData();
    public Graphics graphics = new Graphics();
    public List<HullWeaponProperties> weapons = new List<HullWeaponProperties>();
}
