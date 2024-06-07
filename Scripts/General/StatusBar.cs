using Godot;

namespace DungeonRPG.Scripts.General;

public partial class StatusBar : Sprite3D
{
    [ExportCategory("CONFIGURATION:")] 
    private float _maxValue;
    [Export] public float MaxValue
    {
        get => _maxValue;
        set
        {
            if (_maxValue != value)
            {
                _maxValue = value;
                _progressBar.MaxValue = _maxValue;
            }    
        }
    }

    private float _minValue;
    [Export] public float MinValue
    {
        get => _minValue;
        set
        {
            if (!Mathf.IsEqualApprox(_minValue,value))
            {
                _minValue = value;
                _progressBar.MinValue = _minValue;
            }    
        }
    }
    
    private float _currentValue;
    [Export] public float CurrentValue
    {
        get => _currentValue;
        set
        {
            if (!Mathf.IsEqualApprox(_currentValue, value))
            {
                _currentValue = value;
                if (_progressBar != null) _progressBar.Value = _currentValue;
            }
        }
    }
    
    private Color _barColor;
    [Export] public Color BarColor
    {
        get => _barColor;
        private set
        {
            if (_barColor != value)
            {
                _barColor = value;
                _fillStyle.BgColor = _barColor;
                // _progressBar.AddThemeStyleboxOverride("fill", _fillStyle);
            }    
        }
    }

    private Color _backgroundColor;
    [Export] public Color BackgroundColor
    {
        get => _backgroundColor;
        private set
        {
            if (_backgroundColor != value)
            {
                _backgroundColor = value;
                _backgroundStyle.BgColor = _backgroundColor;
                // _progressBar.AddThemeStyleboxOverride("background", _backgroundStyle);
            }    
        }
    }
    
    [ExportCategory("WIRING:")] 
    [Export] private ProgressBar _progressBar;

    private StyleBoxFlat _fillStyle = new();
    private StyleBoxFlat _backgroundStyle = new();
    
    public override void _Ready()
    {
        _progressBar.MaxValue = MaxValue;
        _progressBar.MinValue = MinValue;
        _progressBar.Value = CurrentValue;
    }
    
    
    /// <summary>
    /// Event handler when tracked value changes.
    /// </summary>
    /// <param name="deltaValue">Amount of change in value.</param>
    public void OnCurrentValueChanged(float deltaValue)
    {
        CurrentValue += deltaValue;
    }
}