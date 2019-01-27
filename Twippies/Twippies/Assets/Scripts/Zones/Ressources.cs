using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource {

    public enum RessourceType
    {
        Food,
        Drink
    }
    public RessourceType ressourceType;
    public IConsumable consumableObject;
    public float quantity;

    public Ressource(RessourceType rType, IConsumable cObject, float q)
    {
        ressourceType = rType;
        consumableObject = cObject;
        quantity = q;
    }
    public Ressource(RessourceType rType)
    {
        ressourceType = rType;
        consumableObject = null;
    }


}
