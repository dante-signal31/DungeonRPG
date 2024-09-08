using System.Diagnostics;
using Godot;

namespace DungeonRPG.addons.area_rect;

[Tool]
public partial class AreaRect : Node3D
{
    [ExportCategory("CONFIGURATION")] 
    /// <summary>
    /// <p>Whether width and height should be calculated automatically
    /// depending on an apect ratio.</p>
    /// </summary> 
    [Export] public bool AspectRatioEnabled { get; set; } = true;
    
    /// <summary> This is the width divided by the height. </summary> 
    [Export] public float AspectRatio { get; set; }= 1.0f;

    private float _width;
    /// <summary>
    /// <p>Width of the area.</p>
    /// <p>If aspect ratio is enabled, this is the only field you have to set,
    /// because height is calculated automatically.</p>
    /// </summary>
    [Export]
    public float Width
    {
        get => _width;
        set
        {
            if (!Mathf.IsEqualApprox(_width,value))
            {
                _width = value;
                if (AspectRatioEnabled)
                {
                    _height = Width / AspectRatio;
                }
                UpdateGizmos();
            }
        }
    }
    
    private float _height;
    /// <summary>
    /// <p>Height of the area.</p>
    /// <p>This field is only used if aspect ratio is not enabled.</p>
    /// <p>If aspect ratio is enabled, this field value is calculated
    /// automatically depending on the aspect ratio and the set width.</p>
    /// </summary>
    [Export] public float Height {
        get => _height;
        set
        {
            if (!Mathf.IsEqualApprox(_height,value))
            {
                if (AspectRatioEnabled) return;
                _height = value;
                UpdateGizmos();
            }
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
                UpdateGizmos();
            }
        }
    }
    
    /// <summary>
    /// Color for this node Gizmo.
    /// </summary>
    [Export] public Color GizmoColor { get; set; } = new Color(1,0,0,1);
}