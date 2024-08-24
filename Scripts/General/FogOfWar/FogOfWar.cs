using System.Collections.Generic;
using System.IO;
using Godot;

namespace DungeonRPG.Scripts.General.FogOfWar;

[Tool]
public partial class FogOfWar : Area3D
{
    [ExportCategory("CONFIGURATION:")] 
    // [Export] private float _fogAltitude = 10f;
    // [Export] private Vector2 _fogSize;
    [Export] private Color _fogColor;

    /// <summary>
    /// The fog is placed inside 3D world, as a layer over battleground, only
    /// visible for minimap. This vectors allows to place the fog from the aerial
    /// minimap point of view.
    /// </summary>
    // [Export] private Vector2 _fogTextureUpperLeft = new Vector2(0, 0);

    [Export] private Vector2I _fogTextureResolution = new Vector2I(1920, 1080);
    /// <summary>
    /// <p>Defines the used fog texture according to:</p>
    ///     <p>texture-size = fog_size * fog_texture_scale.</p>
    /// <p> The scale is expected to be &lt;= 1.0. A smaller value gives
    /// a coarser result.</p>
    /// </summary>
    [Export] private Vector2 _fogTextureScale= new Vector2(1, 1);
    [Export] private Resource _clearTexture;
    [Export] private Vector2 _clearTextureScale = new Vector2(1, 1);

    [ExportGroup("WIRING:")] 
    [Export] private SubViewport _viewport;
    [Export] private Fog _fog;
    [Export] private Sprite3D _fogSprite;

    private CollisionShape3D _fogCollisionShape;
    private Shape3D _currentShape;
    private Vector2 _fogSize;
    private float _fogAltitude;
    private float _pixelSize;

    public override void _Ready()
    {
        base._Ready();
        _fogCollisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        UpdateConfigurationWarnings();
        // Position = new Vector3(
        //     _fogTextureUpperLeft.X + _fogSize.X/2, 
        //     _fogAltitude, 
        //     _fogTextureUpperLeft.Y + _fogSize.Y/2);
        // Scale = new Vector3(
        //     1.0f / _fogTextureScale.X, 
        //     1, 
        //     1.0f / _fogTextureScale.Y);
        Vector3 shapeSize = ((BoxShape3D)_fogCollisionShape.Shape).Size;
        _fogSize = new Vector2(shapeSize.X, shapeSize.Z);
        _pixelSize = _fogSize.X / _fogTextureResolution.X;
        _fogAltitude = _fogCollisionShape.GlobalPosition.Y;
        InitViewport();
        _fog.InitFog(
            // _fogSize,
            _fogTextureResolution,
            _pixelSize,
            _fogTextureScale,
            _fogColor,
            _fogAltitude,
            _clearTexture, 
            _clearTextureScale);
        UpdateSpriteSize();
    }

    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new();
        if (_fogCollisionShape == null)
        {
            warnings.Add("[FogOfWar] Fog collision shape not found.");
        }
        else if (_fogCollisionShape.Shape is not BoxShape3D)
        {
            warnings.Add("[FogOfWar] Fog collision shape should be a BoxShape3D.");
        }
        return warnings.ToArray();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Engine.IsEditorHint())
        {
            if (_fogCollisionShape == null) return;
            if (_currentShape != _fogCollisionShape.Shape)
            {
                UpdateSpriteSize();
            }
        }
    }

    private void UpdateSpriteSize()
    {
        if (_fogCollisionShape == null) return;
        _currentShape = _fogCollisionShape.Shape;
        Vector3 shapeSize = Vector3.Zero;
        if (_currentShape is BoxShape3D boxShape)
        {
            shapeSize = boxShape.Size;
        }
        else
        {
            GD.PrintErr("[FogOfWar] Collision shape is not a BoxShape3D.");
            return;
        }
        Texture2D spriteTexture = _fogSprite.Texture;
        if (spriteTexture == null)
        {
            GD.PrintErr("[FogOfWar] Sprite texture not found.");
            return;
        }

        // Vector2 textureSize = spriteTexture.GetSize();
        // // Set the sprite scale to match the collision shape size.
        // // Assuming the sprite should match the shape's size in its XZ plane.
        // _fogSprite.Scale = new Vector3(
        //     shapeSize.X / textureSize.X, 
        //     1.0f, 
        //     shapeSize.Z / textureSize.Y);
    }

    private void InitViewport()
    {
        // _viewport.Size = (Vector2I) ((_fogSize / _pixelSize) * _fogTextureScale);
        _viewport.Size = (Vector2I) (_fogTextureResolution * _fogTextureScale);
        _viewport.TransparentBg = true;
    }

    /// <summary>
    /// Gives the location (inside the XZ plane) where the fog is dissolved.
    /// </summary>
    /// <param name="newClearPosition">Position where the fog must be
    /// dissolved.</param>
    public void ClearPosition(Vector2 newClearPosition)
    {
        _fog.ClearPosition(newClearPosition);
    }
}