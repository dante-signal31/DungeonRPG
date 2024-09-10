using System.Collections.Generic;
using System.Diagnostics;
using DungeonRPG.addons.area_rect;
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
    [Export] private Color _mapCameraGizmoColor = new Color(0, 0, 0);
    
    [ExportGroup("WIRING:")] 
    [Export] private Camera3D _mapCamera;
    [Export] private Camera3D _shapeCamera;
    [Export] private Camera3D _staticMapCamera;
    [Export] private ColorRect _maskShaderTexture;
    [Export] private TextureRect _mapShaderTexture;
    
    [Export] private ColorRect _alphaMaskShaderTexture;
    [Export] private Decal _fogOfWarDecal;
    [Export] private AreaRect _mapCameraAreaRect;
    [Export] private SubViewport _mapSubviewPort;

    private Marker3D _cameraPosition;
    private ShaderMaterial _mapMaterial;
    private ShaderMaterial _alphaMaskMaterial;
    
    public override void _Ready()
    {
        base._Ready();
        ConfigureMap();
        GetCameraPositionMarker();
        UpdateCamerasConfiguration();
        // ConfigureDecal();
    }

    private void ConfigureDecal()
    {
        if (_alphaMaskMaterial == null) 
            _alphaMaskMaterial = (ShaderMaterial) _alphaMaskShaderTexture.Material;
        _alphaMaskMaterial?.SetShaderParameter("fogColor", _fogColor);
        
        if (_cameraPosition == null) return;
        _fogOfWarDecal.GlobalPosition = _cameraPosition.GlobalPosition;
        _fogOfWarDecal.Size = new Vector3(
            _mapCameraAreaRect.Width, 
            _mapCameraAreaRect.Height, 
            _cameraHeight);
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
        UpdateCameraConfiguration(_staticMapCamera);
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
            UpdateCameraAreaRect();
        }
    }

    private void UpdateCameraAreaRect()
    {
        _mapCameraAreaRect.GlobalPosition = _mapCamera.GlobalPosition;
        _mapCameraAreaRect.AspectRatioEnabled = true;
        _mapCameraAreaRect.AspectType = _mapCamera.KeepAspect;
        _mapCameraAreaRect.AspectRatio = (float) _mapSubviewPort.Size.X / _mapSubviewPort.Size.Y;
        switch (_mapCameraAreaRect.AspectType)
        {
            case Camera3D.KeepAspectEnum.Height:
                _mapCameraAreaRect.Width = _mapCamera.Size;
                break;
            case Camera3D.KeepAspectEnum.Width:
                _mapCameraAreaRect.Height = _mapCamera.Size;
                break;
        }
        _mapCameraAreaRect.Right = _mapCamera.Basis.X;
        _mapCameraAreaRect.Up = _mapCamera.Basis.Y;
        _mapCameraAreaRect.GizmoColor = _mapCameraGizmoColor;
    }

    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new();
        if (_cameraPosition == null)
        {
            warnings.Add("[Map] Marker3D for map camera position not found. " +
                         "Add a Marker3D to position map camera.");
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