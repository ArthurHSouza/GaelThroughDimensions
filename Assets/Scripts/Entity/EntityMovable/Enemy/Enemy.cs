using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : EntityMovable 
{
    protected byte moneyDropped;
    //protected Potion potionDropped;
    override protected void onStart()
    {
        base.onStart();
        //Only a example
        //moneyDropped = Damage * Life * DifficultyOfTheGame * Attacks.size()/2 
    }
}
