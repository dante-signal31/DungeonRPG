using System;
using Godot;

namespace DungeonRPG.Scripts.General.FogOfWar;

[Tool]
public partial class Fog : Sprite2D
{
    public const string LastTextureParameterName = "last_texture";
    public const string ClearTextureParameterName = "clear_texture";
    public const string ClearPositionParameterName = "clear_position";
    public const string ClearSizeParameterName = "clear_size";
    
    [ExportCategory("CONFIGURATION:")]
    
    [ExportGroup("WIRING:")] 
    [Export] private Sprite3D _fogTexture;

    public Color FogColor { get; private set; }

    private Vector2 _fogTexturePixelDimensions;
    private Vector2 _fogTextureScale;
    
    private string _clearTexturePath;
    private Vector2 _clearSize;
    private Vector2 _clearPositionOffset;
    private Vector2 _clearSizeShader;
    private Vector2 _clearPositionShader;
    private Texture2D _clearTexture;
    private Vector2 _clearTextureScale;
    private Vector2 _clearPosition;
    private Vector2? _clearPositionOld;

    private Image _imageTexture;
    private ShaderMaterial _shaderMaterial;
    
    private bool _initialized = false;

    public override void _Ready()
    {
        base._Ready();
        _shaderMaterial = (ShaderMaterial) Material;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        SetShaderUniforms();
    }

    private void SetShaderUniforms()
    {
        if (_clearPosition == _clearPositionOld) return;
        _clearPositionOld = _clearPosition;
        if (!_initialized)
            _imageTexture.SetData(
                GetParent<SubViewport>().Size.X,
                GetParent<SubViewport>().Size.Y,
                false,
                Image.Format.Rgba8,
                GetParent<SubViewport>().GetTexture().GetImage().GetData()
                );

        // Real world positions are at center of sprites, whereas shader
        // positions are in the top left corner of the sprite.
        _clearPositionShader = _clearPosition + _clearPositionOffset;
        // Now to texture coordinates.
        _clearPositionShader -= new Vector2(Position.X, Position.Y);
        // Normalize to shader values in range [0.0, 1.0]
        _clearPositionShader /= _fogTexturePixelDimensions;
        
        _shaderMaterial.SetShaderParameter(
            LastTextureParameterName, 
            _imageTexture);
        _shaderMaterial.SetShaderParameter(
            ClearTextureParameterName, 
            _clearTexture);
        _shaderMaterial.SetShaderParameter(
            ClearPositionParameterName, 
            _clearPositionShader);
        _shaderMaterial.SetShaderParameter(
            ClearSizeParameterName, 
            _clearSizeShader);
    }

    public void SetClearTexturePath(
        Resource clearTextureResource, 
        Vector2 clearImageScale)
    {
        _clearTexturePath = clearTextureResource.ResourcePath;
        _clearTextureScale = clearImageScale;
    }
    
    /// <summary>
    /// Start making a (new) rectangular fog texture filled with the fog color.
    /// </summary>
    /// <param name="fogTextureSize">The fog gets a size in world unitsgiven with this
    /// parameter.</param>
    /// <param name="fogTextureScale"></param>
    /// <param name="fogColor"></param>
    /// <param name="fogAltitude"></param>
    /// <param name="clearTextureResource"></param>
    /// <param name="clearImageScale"></param>
    public void InitFog(
        // Vector2 fogTextureSize,
        Vector2 fogTexturePixelDimensions,
        float fogTexturePixelSize,
        Vector2 fogTextureScale,
        Color fogColor,
        float fogAltitude,
        Resource clearTextureResource, 
        Vector2 clearImageScale
        )
    {
        SetClearTexturePath(clearTextureResource, clearImageScale);
        if (_clearTexturePath == String.Empty) return;
        
        _fogTexture.PixelSize = fogTexturePixelSize;
        // _fogTexturePixelDimensions = fogTextureSize / _fogTexture.PixelSize;
        _fogTexturePixelDimensions = fogTexturePixelDimensions;
        _fogTextureScale = fogTextureScale;
        Vector2 scaledFogTextureSize = _fogTexturePixelDimensions * _fogTextureScale;
        FogColor = fogColor;
        
        _clearTexture = GD.Load<Texture2D>(_clearTexturePath);

        _fogTexture.GlobalPosition = _fogTexture.GlobalPosition with 
            {Y = fogAltitude};
        _fogTexture.Centered = true;
        _fogTexture.FlipV = false;

        InitFogVisualAppearance((Vector2I)scaledFogTextureSize);
        InitClear();
        _initialized = true;
    }

    private void InitClear()
    {
        _clearSize = _clearTexture.GetSize();
        _clearSize *= _clearTextureScale;
        _clearSize *= _fogTextureScale;
        _clearSizeShader = _clearSize / _fogTexturePixelDimensions;
        _clearPositionOffset = new Vector2(-_clearSize.X / 2, -_clearSize.Y / 2);
        _clearPosition = _clearPositionOffset;
        _clearPositionOld = null;
    }

    private void InitFogVisualAppearance(Vector2I scaledFogTextureSize)
    {
        _imageTexture = Image.Create(
            scaledFogTextureSize.X, 
            scaledFogTextureSize.Y, 
            false, 
            Image.Format.Rgba8);
        _imageTexture.Fill(FogColor);
        Texture = ImageTexture.CreateFromImage(_imageTexture);
    }

    /// <summary>
    /// Gives the location (inside the XZ plane) where the fog is dissolved.
    /// </summary>
    /// <param name="newClearPosition">Position where the fog must be
    /// dissolved.</param>
    public void ClearPosition(Vector2 newClearPosition)
    {
        _clearPosition = newClearPosition;
    }
}