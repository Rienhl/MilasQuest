using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MilasQuest.UI
{
    public class LevelPassedPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject holder;
        [SerializeField] private Image img_background;
        [SerializeField] private TMP_Text txt_title;
        [SerializeField] private Button btn_continue;

        [Space]
        [Header("Anim Values")]
        [Range(0, 5)]
        [SerializeField] private float backgroundFadeInTime = 0.4f;
        [SerializeField] private Ease backgroundFadeEaseType = Ease.OutCubic;
        [Range(0, 5)]
        [SerializeField] private float titleFadeInTime = 0.4f;
        [SerializeField] private Ease titleFadeEaseType = Ease.OutCubic;
        [Range(0, 5)]
        [SerializeField] private float continueButtonAnimTime = 0.3f;
        [Range(0, 5)]
        [SerializeField] private float continueButtonAnimElasticity = 0.1f;
        [SerializeField] private Ease continueButtonAnimEaseType = Ease.OutBack;

        private RectTransform _rt_btn_continue;
        private Sequence _sequence;

        public Action OnContinue;

        private void Awake()
        {
            _rt_btn_continue = btn_continue.GetComponent<RectTransform>();
        }

        public void Show()
        {
            holder.SetActive(true);
            btn_continue.onClick.AddListener(HandleOnContinue);

            if (_sequence == null)
            {
                _sequence = DOTween.Sequence();
                _sequence.Append(img_background.DOFade(0, backgroundFadeInTime).From().SetEase(backgroundFadeEaseType));
                _sequence.Append(txt_title.DOFade(0, titleFadeInTime).From().SetEase(titleFadeEaseType));
                _sequence.Append(_rt_btn_continue.DOMoveY(-20, continueButtonAnimTime).From().SetEase(continueButtonAnimEaseType, continueButtonAnimElasticity));
                _sequence.SetAutoKill(false); // we don't want to kill the sequence since we'll continue using it like a PingPong effect betwen show and hide
                _sequence.Play();
            }
            else
                _sequence.Restart();
            _sequence.Play();
        }

        private void HandleOnContinue()
        {
            btn_continue.onClick.RemoveListener(HandleOnContinue);
            Hide();
            OnContinue?.Invoke();
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
    }
}