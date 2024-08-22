using System.Collections.Generic;
using Godot;

namespace DungeonRPG.Scripts.General;


[Tool]
public partial class Map : SubViewportContainer
{
    [Signal] public delegate void MapShownEventHandler();
    [Signal] public delegate void MapHiddenEventHandler();
    
    [ExportCategory("CONFIGURATION")]
    [Export(PropertyHint.Range, "0, 100, 1")] private float _cameraHeight = 10f;
    [Export] private Camera3D.KeepAspectEnum _keepAspect = Camera3D.KeepAspectEnum.Height;
    [Export(PropertyHint.Range, "0, 100, 1")] private float _size;
    
    [ExportGroup("WIRING:")] 
    [Export] private Camera3D _camera;

    private Marker3D _cameraPosition;
    
    public override void _Ready()
    {
        base._Ready();
        GetCameraPositionMarker();
        if (_cameraPosition == null) return;
        UpdateCameraConfiguration();
    }

    private void GetCameraPositionMarker()
    {
        _cameraPosition = GetNode<Marker3D>("Marker3D");
        UpdateConfigurationWarnings();
    }

    private void UpdateCameraConfiguration()
    {
        _camera.GlobalPosition = _cameraPosition.GlobalPosition with {Y = _cameraHeight};
        _camera.KeepAspect = _keepAspect;
        _camera.Size = _size;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Engine.IsEditorHint())
        {
            if (_cameraPosition == null)
            {
                GetCameraPositionMarker();
                if (_cameraPosition == null) return;
            };
            UpdateCameraConfiguration();
        }
    }
    
    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new();
        if (_cameraPosition == null)
        {
            warnings.Add("[Map] Marker3D for map camera position not found.");
        }
        return warnings.ToArray();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.M)
            {
                if (Visible != keyEvent.Pressed)
                {
                    Visible = keyEvent.Pressed;
                    if (Visible)
                    {
                        EmitSignal(SignalName.MapShown);
                    }
                    else
                    {
                        EmitSignal(SignalName.MapHidden);
                    }
                }
            }
        }
    }
}