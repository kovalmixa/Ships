using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScript
{
    bool Execute(Entity entity);
}