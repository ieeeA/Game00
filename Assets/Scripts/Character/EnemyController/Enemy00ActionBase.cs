using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy00ActionBase
{
    public abstract void Init(Enemy00 owner);

    public abstract void Update(Enemy00 owner);

    public abstract void Finish(Enemy00 owner);

    public abstract void Cancel(Enemy00 owner);
}