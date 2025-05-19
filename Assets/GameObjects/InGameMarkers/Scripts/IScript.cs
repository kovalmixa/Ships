using System.Collections;
using System.Collections.Generic;
using Assets.Entity;
using UnityEngine;

public interface IScript
{
    bool Execute(Entity entity);
}