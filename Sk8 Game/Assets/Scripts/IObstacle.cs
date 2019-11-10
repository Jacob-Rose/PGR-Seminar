using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObstacle : Obstacle
{
    public bool m_CanBeInteractedWith = true;
    public int scoreIncreaseOnInteract = 5;
    public Sprite interactedSprite = null;
    public abstract void HandleInteraction(Player p);
    public abstract void InteractedWith(Player p);
}

