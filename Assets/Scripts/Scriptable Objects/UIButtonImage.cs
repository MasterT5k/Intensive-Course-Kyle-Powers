using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UIButtonSprites", menuName = "Scriptable Objects/UI/Button Sprites")]
public class UIButtonImage : ScriptableObject
{
    public Sprite normal;
    public Sprite pressed;
}
