using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] RectTransform _fadeInCircle;
    [SerializeField] GameObject _fadeBG;
    [SerializeField] float _fadeInTime;
    [SerializeField] float _fadeInCircleTime;
    [SerializeField] float _fadeOutTime;
    private CanvasGroup _fadeOutBG;
    private Canvas _canvas;
    private Player _player;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _fadeOutBG = GetComponentInChildren<CanvasGroup>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INIT_PLAYER, CachePlayer);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INIT_PLAYER, CachePlayer);
    }

    public void CachePlayer(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player has not been assigned to Fader");
        }

        _player = (Player)data;
    }

    public IEnumerator NormalFadeOut()
    {
        LTDescr anim = LeanTween.alphaCanvas(_fadeOutBG, 1f, _fadeOutTime).setFrom(0f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }

    public IEnumerator NormalFadeIn()
    {
        LTDescr anim = LeanTween.alphaCanvas(_fadeOutBG, 0f, _fadeOutTime).setFrom(1f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }

    public IEnumerator CircleFadeIn()
    {
        //_test = true;
        _fadeInCircle.gameObject.SetActive(true);
        _fadeInCircle.position = WorldToUI(_player.transform.position);
        LTDescr anim = LeanTween.scale(_fadeInCircle as RectTransform, new Vector3(10, 10, 10), _fadeInCircleTime).setFrom(Vector3.zero).setEase(LeanTweenType.easeInSine).setOnComplete(DisableCircle);
        
        while (LeanTween.isTweening(_fadeInCircle.gameObject))
        {
            yield return null;
        }
    }

    public void DisableCircle()
    {
        _fadeInCircle.gameObject.SetActive(false);
        _fadeOutBG.alpha = 0;
    }

    private Vector3 WorldToUI(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 circlePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _canvas.worldCamera, out circlePos);

        return _canvas.transform.TransformPoint(circlePos);
    }
}
