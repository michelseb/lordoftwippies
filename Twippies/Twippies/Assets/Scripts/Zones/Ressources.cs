using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressources {

    public enum RessourceType
    {
        Food,
        Drink,
        None
    }
    public RessourceType ressourceType;
    public IConsumable consumableObject;
    public float quantity;

    public Ressources(RessourceType rType, IConsumable cObject, float q)
    {
        ressourceType = rType;
        consumableObject = cObject;
        quantity = q;
    }

}
