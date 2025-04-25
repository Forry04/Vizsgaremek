using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
[CreateAssetMenu(menuName = "Skin System/Skin Data")]
public class Skindata : ScriptableObject
{

    public int skinId;
    public string skinName;
    public Sprite previewImage;
    public Rarity rarity;
    public Material skinMaterial;

}
