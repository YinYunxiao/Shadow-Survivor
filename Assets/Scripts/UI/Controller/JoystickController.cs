using FairyGUI;
using UnityEngine;

public class JoystickController
{
    private All.Joystick _view;
    private Vector2 _inputDir;
    private float _radius;
    private bool _isDragging;

    private Vector2 _mouseStartPos;
    private Vector2 _objStartPos;

    public Vector2 InputDir => _inputDir;
    public bool IsActive => _isDragging;

    public JoystickController(All.Joystick view)
    {
        _view = view;
        _radius = _view.back.height * 0.5f;
        _objStartPos = _view.stick.position;
        _inputDir = Vector2.zero;
        _isDragging = false;

        _view.stick.draggable = true;
        BindEvents();
    }

    private void BindEvents()
    {
        _view.stick.onDragStart.Add(OnDragStart);
        _view.stick.onDragMove.Add(OnDragMove);
        _view.stick.onDragEnd.Add(OnDragEnd);
    }

    private void OnDragStart(EventContext context)
    {
        _isDragging = true;
        _mouseStartPos = _view.GlobalToLocal(context.inputEvent.position);
        context.CaptureTouch();
    }

    private void OnDragMove(EventContext context)
    {
        Vector2 mousePos = _view.GlobalToLocal(context.inputEvent.position);
        Vector2 offset = mousePos - _mouseStartPos;

        Vector2 newPos;
        if (offset.magnitude > _radius)
            newPos = _objStartPos + offset.normalized * _radius;
        else
            newPos = _objStartPos + offset;

        _view.stick.SetPosition(newPos.x, newPos.y, 0);

        Vector2 stickOffset = newPos - _objStartPos;
        _inputDir = new Vector2(stickOffset.x, -stickOffset.y) / _radius;

        if (_inputDir.magnitude < 0.2f)
            _inputDir = Vector2.zero;
    }

    private void OnDragEnd()
    {
        _isDragging = false;
        _inputDir = Vector2.zero;
        _view.stick.SetPosition(_objStartPos.x, _objStartPos.y, 0);
    }

    public void Dispose()
    {
        _view.stick.onDragStart.Remove(OnDragStart);
        _view.stick.onDragMove.Remove(OnDragMove);
        _view.stick.onDragEnd.Remove(OnDragEnd);
    }
}
