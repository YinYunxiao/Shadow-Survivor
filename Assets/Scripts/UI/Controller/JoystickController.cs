using FairyGUI;
using UnityEngine;

public class JoystickController
{
    private All.Joystick _view;
    private Vector2 _inputDir;
    private float _radius;
    private bool _isDragging;

    private Vector2 _centerPos;

    public Vector2 InputDir => _inputDir;
    public bool IsActive => _isDragging;

    public JoystickController(GComponent parent)
    {
        _view = All.Joystick.CreateInstance();
        parent.AddChild(_view);

        _radius = _view.back.height * 0.5f;
        _inputDir = Vector2.zero;
        _isDragging = false;

        _view.show.SetSelectedIndex(0);

        GRoot.inst.onTouchBegin.Add(OnTouchBegin);
        GRoot.inst.onTouchMove.Add(OnTouchMove);
        GRoot.inst.onTouchEnd.Add(OnTouchEnd);
    }

    private void OnTouchBegin(EventContext context)
    {
        Vector2 pos = GRoot.inst.GlobalToLocal(context.inputEvent.position);

        // 只允许下半屏触摸
        if (pos.y < GRoot.inst.height * 0.5f) return;

        _centerPos = pos;
        _view.SetPosition(pos.x, pos.y, 0);
        _view.stick.SetPosition(_radius, _radius, 0);
        _view.show.SetSelectedIndex(1);
        _isDragging = true;
        _inputDir = Vector2.zero;

        context.CaptureTouch();
    }

    private void OnTouchMove(EventContext context)
    {
        if (!_isDragging) return;

        Vector2 pos = GRoot.inst.GlobalToLocal(context.inputEvent.position);
        Vector2 offset = pos - _centerPos;

        if (offset.magnitude > _radius)
            offset = offset.normalized * _radius;

        _view.stick.SetPosition(offset.x + _radius, offset.y + _radius, 0);

        _inputDir = new Vector2(offset.x, -offset.y) / _radius;

        if (_inputDir.magnitude < 0.2f)
            _inputDir = Vector2.zero;
    }

    private void OnTouchEnd(EventContext context)
    {
        if (!_isDragging) return;

        _isDragging = false;
        _inputDir = Vector2.zero;
        _view.stick.SetPosition(0, 0, 0);
        _view.show.SetSelectedIndex(0);
    }

    public void Dispose()
    {
        GRoot.inst.onTouchBegin.Remove(OnTouchBegin);
        GRoot.inst.onTouchMove.Remove(OnTouchMove);
        GRoot.inst.onTouchEnd.Remove(OnTouchEnd);

        if (_view.parent != null)
            _view.parent.RemoveChild(_view);
        _view.Dispose();
    }
}
