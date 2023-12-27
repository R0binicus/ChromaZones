using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] CanvasGroup _fadeOutImage;
    [SerializeField] RectTransform _fadeInCircle;
    [SerializeField] GameObject _fadeInBG;
    private Canvas _canvas;
    private Player _player;

    //bool _test = false;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_PLAYER, CachePlayer);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_PLAYER, CachePlayer);
    }

    private void Update()
    {   
        // while (!_test)
        // {
        //     StartCoroutine(CircleFadeIn(Time.time));
        // }
        _fadeInCircle.position = WorldToUI(_player.transform.position);
    }

    public void CachePlayer(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player has not been assigned to Fader");
        }

        _player = (Player)data;
    }

    // IEnumerator NormalFadeOut(AnimationCurve fadeCurve, float startTime)
    // {
    //     //LTDescr anim = LeanTween.value(0, 1, 1f).setOnUpdate(UpdateAlphaFade(anim.va));

    //     while (Time.time - startTime < fadeCurve.keys[fadeCurve.length - 1].time)
    //     {
    //         _fadeInBG.alpha = Mathf.Lerp
    //         (
    //             fadeCurve.keys[0].time,
    //             fadeCurve.keys[fadeCurve.length - 1].time,
    //             fadeCurve.Evaluate(Time.time - startTime)
    //         );
    //         yield return null;
    //     }
    //     _fadePanel.alpha = fadeCurve.keys[fadeCurve.length - 1].value;
    // }

    IEnumerator CircleFadeIn(float startTime)
    {
        //_test = true;
        LTDescr anim = LeanTween.scale(_fadeInCircle as RectTransform, new Vector3(10, 10, 10), 1f).setOnComplete(DisableCircle);
        
        while (anim.time - anim.passed > 0)
        {
            yield return null;
        }
    }

    public void UpdateAlphaFade(float value)
    {
        _fadeOutImage.alpha = value;
    }

    public void DisableCircle()
    {
        _fadeInCircle.gameObject.SetActive(false);
        _fadeInBG.SetActive(false);
    }

    private Vector3 WorldToUI(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 circlePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _canvas.worldCamera, out circlePos);

        return _canvas.transform.TransformPoint(circlePos);
    }
}
