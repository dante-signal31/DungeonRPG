using System.Collections.Generic;
using Godot;

namespace DungeonRPG.Scripts.General;


[Tool]
public partial class Map : Control
{
    [Signal] public delegate void MapShownEventHandler();
    [Signal] public delegate void MapHiddenEventHandler();
    
    [ExportCategory("CONFIGURATION")]
    [Export(PropertyHint.Range, "0, 100, 1")] private float _cameraHeight = 10f;
    [Export] private Camera3D.KeepAspectEnum _keepAspect = Camera3D.KeepAspectEnum.Height;
    [Export(PropertyHint.Range, "0, 100, 1")] private float _size;
    [Export] private Color _fogColor = new Color(0, 0, 0);
    
    [ExportGroup("WIRING:")] 
    [Export] private Camera3D _mapCamera;
    [Export] private Camera3D _shapeCamera;
    [Export] private ColorRect _maskShaderTexture;
    [Export] private TextureRect _mapShaderTexture;

    private Marker3D _cameraPosition;
    private ShaderMaterial _maskMaterial;
    private ShaderMaterial _mapMaterial;
    
    public override void _Ready()
    {
        base._Ready();
        ConfigureMap();
        GetCameraPositionMarker();
        UpdateCamerasConfiguration();
    }

    private void ConfigureMap()
    {
        if (_mapMaterial == null) 
            _mapMaterial = (ShaderMaterial) _mapShaderTexture.Material;
        UpdateMapShader();
    }

    private void UpdateMapShader()
    {
        if (_mapMaterial == null) return;
            _mapMaterial.SetShaderParameter("fogColor", _fogColor);
    }

    private void UpdateCamerasConfiguration()
    {
        UpdateCameraConfiguration(_mapCamera);
        UpdateCameraConfiguration(_shapeCamera);
    }

    private void GetCameraPositionMarker()
    {
        _cameraPosition = GetNodeOrNull<Marker3D>("Marker3D");
        UpdateConfigurationWarnings();
    }

    private void UpdateCameraConfiguration(Camera3D _camera)
    {
        if (_cameraPosition == null) return;
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
            UpdateCamerasConfiguration();
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