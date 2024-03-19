using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;
using Color = UnityEngine.Color;

public class EffectSelector : MonoBehaviour
{
    [SerializeField][Tooltip("A game object with all the effects as child, you can find it at Assets/Prefabs/Effects/StatusParticles/StatusEffects.prefab")] 
    private GameObject effectsObject;
    private GameObject fireEffect;
    private GameObject iceEffect;
    private GameObject shockEffect;
    private GameObject poisonEffect;
    private GameObject frenzyEffect;
    [SerializeField][Tooltip("Point where the effect object should be placed")] private Vector3 offset;

    private GameObject currentEffect;

    private void Start()
    {
        //the next lines search in the child objects of effects object to assign the effects
        fireEffect = effectsObject.transform.Find("FireParticles").gameObject;
        iceEffect = effectsObject.transform.Find("IceParticles").gameObject;
        shockEffect = effectsObject.transform.Find("ShockParticles").gameObject;
        poisonEffect = effectsObject.transform.Find("PoisonParticles").gameObject;
        frenzyEffect = effectsObject.transform.Find("FrenzyParticles").gameObject;
    }
    private void EffectTemplate(GameObject effect, Color color, Vector3 effectOffset ,Quaternion rotation) {
        ClearEffect();
        currentEffect = Instantiate(effect, gameObject.transform.position + offset + effectOffset, gameObject.transform.rotation * rotation);
        currentEffect.transform.parent = transform;
        currentEffect.transform.localScale = Vector3.one;
        GetComponent<SpriteRenderer>().color = color;

    }
    public void Fire() {
        EffectTemplate(fireEffect, new Color(255 / 255f, 155 / 255f, 85 / 255f), Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
    }
    public void Ice() {
        EffectTemplate(iceEffect, new Color(76 / 255f, 249 / 255f, 255 / 255f), new Vector3(0, 0, -3), Quaternion.Euler(-90f, 0f, 0f));
    }
    public void Shock() {
        EffectTemplate(shockEffect, new Color(255 / 255f, 239 / 255f, 0 / 255f), new Vector3(0, 0, -3), Quaternion.Euler(0, 0f, 0f));
    }
    public void Poison() {
        EffectTemplate(poisonEffect, new Color(212 / 255f, 158 / 255f, 255 / 255f), new Vector3(0, 0, -3), Quaternion.Euler(0f, 0f, 0f));
    }
    public void Frenzy() {
        EffectTemplate(frenzyEffect, new Color(255 / 255f, 66 / 255f, 229 / 255f), Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
    }

    public void ClearEffect() {
        Destroy(currentEffect);
        GetComponent<SpriteRenderer>().color = new Color(1,1,1);
    }
}
