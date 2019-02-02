using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private Animation anim;

    [SerializeField] private GameObject flagMainObj;
    [SerializeField] private Material[] flagMaterials = new Material[2];

    private float retryTimer = 0.1f;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }

    private void PlaySpawnAnimation(int _controllerId)
    {
        ChangeFlagColor(_controllerId);
        anim.Play("flag_Spawn");
    }

    private IEnumerator RetryPlaySpawnAnimation(int _controllerId)
    {
        yield return new WaitForSeconds(retryTimer);

        while (true)
        {
            if (anim.IsPlaying("flag_Destroy"))
            {
                yield return new WaitForSeconds(retryTimer);
            }
            else
            {
                PlaySpawnAnimation(_controllerId);
                break;
            }
        }

        yield return null;
    }

    private void PlayDestroyAnimation()
    {
        anim.Play("flag_Destroy");
    }

    private void ChangeFlagColor(int _controllerId)
    {
        flagMainObj.GetComponent<SkinnedMeshRenderer>().material = flagMaterials[_controllerId];
    }

    public void OnContested()
    {
        PlayDestroyAnimation();
    }

    public void OnCapture(int _controllerId)
    {
        if (anim.IsPlaying("flag_Destroy"))
        {
            RetryPlaySpawnAnimation(_controllerId);
        }
        else
        {
            PlaySpawnAnimation(_controllerId);
        }
    }
}
