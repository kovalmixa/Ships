using Assets.Entity.StatMods;
using UnityEngine;

//public class ItemProperty
//{
//    public string name;
//    public string value;
//    public Color textColor = Color.white;
//}

//public static class ItemPropertiesHandler
//{
//    public static ItemProperty CreatePropertyFromStat(StatType type, float value)
//    {
//        var itemProperty = new ItemProperty();
//        itemProperty.name = type.ToString();
//        itemProperty.value = value.ToString();
//        // special property for color if its 
//    }
//}

public class ItemData
{
    public string id;
    public string name;
    public Texture icon;
    public string[] modIds;
}
