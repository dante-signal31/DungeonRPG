using System.Diagnostics;
using Godot;

namespace DungeonRPG.addons.area_rect;

[Tool]
public partial class AreaRect : Node3D
{
    /// <summary>
    /// Emitted when the area is resized.
    /// </summary>
    [Signal] public delegate void AreaRectUpdatedEventHandler(AreaRect sourceAreaRect);
    
    [ExportCategory("CONFIGURATION")] 
    /// <summary>
    /// <p>Whether width and height should be calculated automatically
    /// depending on an apect ratio.</p>
    /// </summary> 
    [Export] public bool AspectRatioEnabled { get; set; } = true;

    /// <summary>
    /// Whether the width or the height condition the other one.
    /// </summary>
    [Export] public Camera3D.KeepAspectEnum AspectType { get; set; }= Camera3D.KeepAspectEnum.Width;
    
    /// <summary> This is the width divided by the heigh. </summary> 
    [Export] public float AspectRatio { get; set; }= 1.0f;

    private float _width;
    /// <summary>
    /// <p>Width of the area.</p>
    /// <p>If aspect ratio Height is enabled, this is the only field you have to set,
    /// because height is calculated automatically.</p>
    /// </summary>
    [Export]
    public float Width
    {
        get => _width;
        set
        {
            if (Mathf.IsEqualApprox(_width, value)) return;
            if (!AspectRatioEnabled) 
                _width = value;
            if (AspectRatioEnabled && AspectType == Camera3D.KeepAspectEnum.Height)
            {
                _width = value;
                _height = _width / AspectRatio;
            }
            EmitSignal(SignalName.AreaRectUpdated, this);
            UpdateGizmos();
        }
    }
    
    private float _height;
    /// <summary>
    /// <p>Height of the area.</p>
    /// <p>This field is only used if aspect ratio is not enabled.</p>
    /// <p>If aspect ratio Width is enabled, this is the only field you have to set,
    /// because width is calculated automatically.</p>
    /// </summary>
    [Export] public float Height {
        get => _height;
        set
        {
            if (Mathf.IsEqualApprox(_height, value)) return;
            if (!AspectRatioEnabled) 
                _height = value;
            if (AspectRatioEnabled && AspectType == Camera3D.KeepAspectEnum.Width)
            {
                _height = value;
                _width = _height * AspectRatio;
            }
            EmitSignal(SignalName.AreaRectUpdated, this);
            UpdateGizmos();
        }
    }

    private Vector3 _up = Vector3.Up;
    /// <summary>
    /// AreaRect plane vector defining height direction.
    /// </summary>
    [Export] public Vector3 Up {
        get => _up;
        set
        {
            if (_up != value)
            {
                _up = value;
                EmitSignal(SignalName.AreaRectUpdated, this);
                UpdateGizmos();
            }
        }
    }
    
    private Vector3 _right = Vector3.Right;
    /// <summary>
    /// AreaRect plane vector defining width direction.
    /// </summary>
    [Export] public Vector3 Right {
        get => _right;
        set
        {
            if (_right != value)
            {
                _right = value;
                EmitSignal(SignalName.AreaRectUpdated, this);
                UpdateGizmos();
            }
        }
    }
    
    /// <summary>
    /// Color for this node Gizmo.
    /// </summary>
    [Export] public Color GizmoColor { get; set; } = new Color(1,0,0,1);
}