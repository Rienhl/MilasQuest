using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class LevelFailedPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject holder;
        [SerializeField] private Image img_background;
        [SerializeField] private TMP_Text txt_levelFailed;
        [SerializeField] private TMP_Text txt_retry;
        [SerializeField] private Button btn_yes;
        [SerializeField] private Button btn_no;

        [Space]
        [Header("Anim Values")]
        [Range(0, 5)]
        [SerializeField] private float backgroundFadeInTime = 0.7f;
        [SerializeField] private Ease backgroundFadeEaseType = Ease.OutCubic;
        [Range(0, 5)]
        [SerializeField] private float titleFadeInTime = 0.7f;
        [SerializeField] private Ease titleFadeEaseType = Ease.OutCubic;
        [Range(0, 5)]
        [SerializeField] private float retryTextFadeInTime = 0.7f;
        [SerializeField] private Ease retryTextFadeEaseType = Ease.OutCubic;
        [Range(0, 5)]
        [SerializeField] private float yesButtonAnimTime = 0.7f;
        [Range(0, 5)]
        [SerializeField] private float yesButtonAnimElasticity = 0.5f;
        [SerializeField] private Ease yesButtonAnimEaseType = Ease.OutBack;
        [Range(0, 5)]
        [SerializeField] private float noButtonAnimTime = 0.7f;
        [Range(0, 5)]
        [SerializeField] private float noButtonAnimElasticity = 0.5f;
        [SerializeField] private Ease noButtonAnimEaseType = Ease.OutBack;

        private RectTransform _rt_btn_yes;
        private RectTransform _rt_btn_no;
        private Sequence _sequence;

        public Action OnRetryLevel;
        public Action OnExitLevel;

        private void Awake()
        {
            _rt_btn_yes = btn_yes.GetComponent<RectTransform>();
            _rt_btn_no = btn_no.GetComponent<RectTransform>();
        }

        public void Show()
        {
            holder.SetActive(true);
            btn_yes.onClick.AddListener(HandleOnRetry);
            btn_no.onClick.AddListener(HandleOnExit);

            if (_sequence == null)
            {
                _sequence = DOTween.Sequence();
                _sequence.Append(img_background.DOFade(0, backgroundFadeInTime).From().SetEase(backgroundFadeEaseType));
                _sequence.Append(txt_levelFailed.DOFade(0, titleFadeInTime).From().SetEase(titleFadeEaseType));
                _sequence.Append(txt_retry.DOFade(0, retryTextFadeInTime).From().SetEase(retryTextFadeEaseType));
                _sequence.Append(_rt_btn_yes.DOMoveY(-20, yesButtonAnimTime).From().SetEase(yesButtonAnimEaseType, yesButtonAnimElasticity));
                _sequence.Append(_rt_btn_no.DOMoveY(-20, noButtonAnimTime).From().SetEase(noButtonAnimEaseType, noButtonAnimElasticity));
                _sequence.SetAutoKill(false); // we don't want to kill the sequence since we'll continue using it like a PingPong effect betwen show and hide
                _sequence.Play();
            }
            else
                _sequence.Restart();
                _sequence.Play();
        }

        private void HandleOnRetry()
        {
            RemoveButtonListeners();
            Hide();
            OnRetryLevel?.Invoke();
        }

        private void HandleOnExit()
        {
            RemoveButtonListeners();
            Hide();
            OnExitLevel?.Invoke();
        }

        private void Hide()
        {
            _sequence.OnRewind(Reset);
            _sequence.PlayBackwards();
        }

        private void Reset()
        {
            holder.SetActive(false);
        }

        private void RemoveButtonListeners()
        {
            btn_yes.onClick.RemoveListener(HandleOnRetry);
            btn_no.onClick.RemoveListener(HandleOnExit);
        }

    }
}