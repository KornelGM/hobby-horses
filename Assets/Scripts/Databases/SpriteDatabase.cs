using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Sprite Database", menuName = "ScriptableObjects/Icons/Sprite Database ")]
public class SpriteDatabase : DatabaseElement<Sprite>
{
    public override List<string> GetPath()
    {
        throw new System.NotImplementedException();
    }
}
