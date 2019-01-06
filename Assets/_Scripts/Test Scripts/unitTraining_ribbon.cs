using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class unitTraining_ribbon : MonoBehaviour
{
    private unitTraining_test utm;
    private Animation anim;

    public Image unitImage;
    public TextMeshProUGUI unitText;

    private Animation unitImageAnim;
    private Animation unitTextAnim;

    public void SetupRibbon(string _unitName, Sprite _unitSprite, Color _classColor, float _yPos)
    {
        utm = unitTraining_test.singleton;
        anim = GetComponent<Animation>();

        unitImageAnim = unitImage.GetComponent<Animation>();
        unitTextAnim = unitText.GetComponent<Animation>();

        unitImage.sprite = _unitSprite;
        unitText.text = _unitName;

        Image ribbonImage = GetComponent<Image>();
        ribbonImage.color = _classColor;

        Vector3 pos = new Vector3(-390, _yPos, 0);
        transform.localPosition = pos;

        PlaySpawnAnimation();
    }

    public void CancelTrainingUnit()
    {
        PlayFadeAnimation();
    }

    public IEnumerator MoveRibbon(float _yPos, float _moveSpeed)
    {
        Vector3 pos = transform.localPosition;

        while (pos.y < _yPos)
        {
            pos.y += _moveSpeed;

            if (pos.y > _yPos)
                pos.y = _yPos;

            transform.localPosition = pos;

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    private void PlaySpawnAnimation()
    {
        anim.Play("ribbonSpawn");
    }

    private void PlayFadeAnimation()
    {
        anim.Play("ribbonCancel");
        unitImageAnim.Play("ribbonCancel");
        unitTextAnim.Play("ribbonTextCancel");

        float timer = anim.GetClip("ribbonCancel").length;

        Invoke("OnFinishedFading", timer);
    }

    private void OnFinishedFading()
    {
        utm.CancelTrainingUnit(this);
    }
}
