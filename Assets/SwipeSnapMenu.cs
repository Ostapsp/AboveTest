using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SwipeSnapMenu : MonoBehaviour, IBeginDragHandler, IEndDragHandler {

    [SerializeField] private RectTransform _contentContainer;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private float _snapSpeed = 15;
    public int SelectedTabIndex => _selectedTabIndex;

    private bool _isBreak;
    private bool _isDragging;
    private bool _isSnapping;
    private float _targetScrollBarValue = 0f;
    private readonly List<float> _itemPositions = new List<float>() { 0.0f, 0.5f, 1.0f };
    private float _itemSize = 0.5f;
    private int _selectedTabIndex;
    private float _lastSnappTime;
    private int _lasNdx = 0, _curNdx = 1;

    void Start() {
        _lastSnappTime = Time.time;
        _scrollbar.value = _curNdx;
        _itemSize = 1f / (_itemPositions.Count -1);
        StartCoroutine(AutoSnapping());
    }

    public void OnBeginDrag(PointerEventData eventData) {
        _isDragging = true;
        _isSnapping = false;
    }

    public void OnEndDrag(PointerEventData eventData) {
        _targetScrollBarValue = _scrollbar.value;
        _isDragging = false;
        FindSnappingTabAndStartSnapping();
    }

    void Update() {
        if (_isDragging) return;
        if (_isSnapping) SnapContent();
    }
    private void FindSnappingTabAndStartSnapping() {
        for (var i = 0; i < _itemPositions.Count; i++) {
            var position = _itemPositions[i];
            if (_targetScrollBarValue > position - _itemSize / 2f && _targetScrollBarValue < position + _itemSize / 2f) {
                selectTab(i);
                break;
            }
        }
    }

    private void selectTab(int tabIndex) {
        if (tabIndex < 0 || tabIndex > _itemPositions.Count) { tabIndex = 0; }
        _selectedTabIndex = tabIndex;
        //_targetScrollBarValue = _itemPositions[tabIndex];
        _isSnapping = true;
        if (_curNdx != tabIndex) {
            _lasNdx = _curNdx;
            _curNdx = tabIndex;
        }
    }

    private void SnapContent() {
        var targetPos = _itemPositions[_selectedTabIndex];
        _scrollbar.value = Mathf.Lerp(_scrollbar.value, targetPos, Time.deltaTime * _snapSpeed);
        if (Mathf.Abs(_scrollbar.value - targetPos) <= 0.0001f) {
            _isSnapping = false;
            _lastSnappTime = Time.time;
            _isBreak = true;
        }
    }

    IEnumerator AutoSnapping() {
        while (true) {
            _isBreak = false;
            yield return new WaitForSeconds(5f);// Задержка на 5 секунд
            if (_isBreak) {                     // во время ожидания (yield) случился Snapping
                float dt = Time.time - _lastSnappTime;
                if (dt > .3f)
                    yield return new WaitForSeconds(5.0f - dt); // ждем до истечения 5 секунд
            }

            int i = _selectedTabIndex + ((_lasNdx < _curNdx) ? 1 : -1);     // следующий по ходу движения
            if (i >= _itemPositions.Count) i = _itemPositions.Count - 2;    // разворот
            else if (i < 0) i = 1;                                          // разворот
            _selectedTabIndex = i;
            _isSnapping = true;
            if (_curNdx != i) {
                _lasNdx = _curNdx;
                _curNdx = i;
            }
            _lastSnappTime = Time.time;
        }
    }
}
