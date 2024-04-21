using Godot;

public abstract partial class PlayerState : Node
{
    protected Player _characterNode;

    public override void _Ready()
    {
        _characterNode = GetOwner<Player>();
        SetPhysicsProcess(false);
        SetProcessInput(false);
    }
    
    public void Notify(int what, NotificationArgs args=null)
    {
        base._Notification(what);

        switch (what)
        {
            // Code sent to make the state as the current one. This state processing
            // is activated because it is who decides when to transition to another state.
            case 5001:
                // _characterNode.AnimationPlayer.Play(_stateAnimation);
                EnterState();
                SetPhysicsProcess(true);
                SetProcessInput(true);
                break;
            // Code sent to disable the state processing. This happens when the state 
            // no longer is going to be the current one. So, its processing is disabled.
            case 5002:
                SetPhysicsProcess(false);
                SetProcessInput(false);
                break;
        }
    }

    /// <summary>
    /// Actions taken when this state is activated as the current one.
    /// </summary>
    protected abstract void EnterState();
}