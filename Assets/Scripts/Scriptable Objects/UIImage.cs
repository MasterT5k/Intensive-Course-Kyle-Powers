using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new UISprites", menuName = "Scriptable Objects/UI/Sprites")]
public class UIImage : ScriptableObject
{
    public Sprite normal;
    public Sprite caution;
    public Sprite warning;
}
