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
                if (_progressBar != null) _progressBar.MaxValue = _maxValue;
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
                if (_progressBar != null) _progressBar.MinValue = _minValue;
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
    
    [ExportCategory("WIRING:")] 
    [Export] private ProgressBar _progressBar;
    
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