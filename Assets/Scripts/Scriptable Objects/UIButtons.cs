using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new UIButtons", menuName = "Scriptable Objects/UI/Buttons")]
public class UIButtons : ScriptableObject
{
    public UIButtonImage play;
    public UIButtonImage pause;
    public UIButtonImage fastForward;
    public UIButtonImage restart;
}
