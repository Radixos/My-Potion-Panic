using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class sfxSpells : MonoBehaviour
{
    [SerializeField]
    private EventReference arrowPath, arrowHitPath, flamePath, icePath, iceActivatePath;

    void FlameSpell()
    {

    }

    void ArrowSpell()
    {
        RuntimeManager.PlayOneShotAttached(arrowPath.Path, gameObject);
    }

    void IceSpellPlant()
    {

    }

    void IceSpellActivate()
    {

    }
}
