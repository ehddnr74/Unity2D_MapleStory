using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkillEffectManager : MonoBehaviour
{
    public EffectPool objectPool;

    public void ShowEffect(string effectName, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float lifetime)
    {
        objectPool.GetObject(effectName, parent, localPosition, localRotation, localScale, lifetime);
    }
}
