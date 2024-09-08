using Godot;

namespace DungeonRPG.addons.area_rect;

[Tool]
public partial class AreaRectGizmo : EditorNode3DGizmoPlugin
{
    private const string AREA_RECT_MATERIAL_NAME = "AreaRectMaterial";
    private const string AREA_RECT_HANDLE_MATERIAL_NAME = "AreaRectHandleMaterial";
    private const int MAIN_POINT_HANDLE_ID = 0;
    
    private StandardMaterial3D _rectMaterial;
    private StandardMaterial3D _handleMaterial;
    private EditorUndoRedoManager _undoRedo;
    
    public AreaRectGizmo(EditorUndoRedoManager undoRedo)
    {
        CreateMaterial(AREA_RECT_MATERIAL_NAME, new Color(1,0,0));
        CreateHandleMaterial(AREA_RECT_HANDLE_MATERIAL_NAME);
        _rectMaterial = GetMaterial(AREA_RECT_MATERIAL_NAME);
        _handleMaterial = GetMaterial(AREA_RECT_HANDLE_MATERIAL_NAME);
        _undoRedo = undoRedo;
    }

    public override string _GetGizmoName()
    {
        return "AreaRect gizmo";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is AreaRect;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        base._Redraw(gizmo);
        gizmo.Clear();
        var areaRect = (AreaRect) gizmo.GetNode3D();
        _rectMaterial.AlbedoColor = areaRect.GizmoColor;
        _handleMaterial.AlbedoColor = areaRect.GizmoColor;
        
        var lines = new[]
        {
            // Line from top right to bottom right.
            areaRect.Right * areaRect.Width/2 + areaRect.Up * -areaRect.Height/2,
            areaRect.Right * areaRect.Width/2 + areaRect.Up * areaRect.Height/2,
            // Line from bottom right to bottom left.
            areaRect.Right * areaRect.Width/2 + areaRect.Up * areaRect.Height/2,
            areaRect.Right * -areaRect.Width/2 + areaRect.Up * areaRect.Height/2,
            // Line from bottom left to top left.
            areaRect.Right * -areaRect.Width/2 + areaRect.Up * areaRect.Height/2,
            areaRect.Right * -areaRect.Width/2 + areaRect.Up * -areaRect.Height/2,
            // Line from top left to top right.
            areaRect.Right * -areaRect.Width/2 + areaRect.Up * -areaRect.Height/2,
            areaRect.Right * areaRect.Width/2 + areaRect.Up * -areaRect.Height/2,
        };

        var handles = new[]
        {
            areaRect.Right * areaRect.Width/2 + areaRect.Up * -areaRect.Height/2,
        };
        
        gizmo.AddLines(lines, _rectMaterial);
        gizmo.AddHandles(
            handles, 
            _handleMaterial, 
            new[] {MAIN_POINT_HANDLE_ID}
            );
    }

    public override string _GetHandleName(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        switch (handleId)
        {
            case MAIN_POINT_HANDLE_ID:
                return "AreaRect width and height";
            default:
                return base._GetHandleName(gizmo, handleId, secondary);
        }
    }

    public override Variant _GetHandleValue(EditorNode3DGizmo gizmo, int handleId, bool secondary)
    {
        var areaRect = (AreaRect) gizmo.GetNode3D();
        switch (handleId)
        {
            case MAIN_POINT_HANDLE_ID:
                return new Vector3(areaRect.Width, areaRect.Height, 0);
            default:
                return base._GetHandleValue(gizmo, handleId, secondary);
        }
    }

    public override void _SetHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary,
        Camera3D camera, Vector2 screenPos)
    {
        var areaRect = (AreaRect) gizmo.GetNode3D();
        switch (handleId)
        {
            case MAIN_POINT_HANDLE_ID:
                Vector3 handlePosition = camera.ProjectPosition(
                    screenPos,
                    GetZDepth(
                        camera, 
                        areaRect.GlobalPosition + areaRect.Right * areaRect.Width/2 + areaRect.Up * -areaRect.Height/2
                        )
                );
                areaRect.Width = ((handlePosition - areaRect.GlobalPosition) * areaRect.Right).Length() * 2;
                areaRect.Height = ((handlePosition - areaRect.GlobalPosition) * areaRect.Up).Length() * 2;;
                break;
            default:
                base._SetHandle(gizmo, handleId, secondary, camera, screenPos);
                break;
        }
    }

    public override void _CommitHandle(EditorNode3DGizmo gizmo, int handleId, bool secondary,
        Variant restore, bool cancel)
    {
        var areaRect = (AreaRect) gizmo.GetNode3D();
        _undoRedo.CreateAction("Change AreaRect size");
        switch (handleId)
        {
            case MAIN_POINT_HANDLE_ID:
                _undoRedo.AddDoProperty(
                    areaRect,
                    AreaRect.PropertyName.Width,
                    areaRect.Width
                    );
                _undoRedo.AddUndoProperty(
                    areaRect,
                    AreaRect.PropertyName.Width,
                    restore);
                _undoRedo.AddDoProperty(
                    areaRect,
                    AreaRect.PropertyName.Height,
                    areaRect.Height
                    );
                _undoRedo.AddUndoProperty(
                    areaRect,
                    AreaRect.PropertyName.Height,
                    restore);
                _undoRedo.AddDoProperty(
                    areaRect,
                    AreaRect.PropertyName.AspectRatio,
                    areaRect.AspectRatio);
                _undoRedo.AddUndoProperty(
                    areaRect, 
                    AreaRect.PropertyName.AspectRatio, 
                    restore);
                _undoRedo.AddDoProperty(
                    areaRect, 
                    AreaRect.PropertyName.AspectRatioEnabled, 
                    areaRect.AspectRatioEnabled);
                _undoRedo.AddUndoProperty(
                    areaRect, 
                    AreaRect.PropertyName.AspectRatioEnabled, 
                    restore);
                break;
            default:
                base._CommitHandle(gizmo, handleId, secondary, restore, cancel);
                break;
        }

        if (cancel)
        {
            switch (handleId)
            {
                case MAIN_POINT_HANDLE_ID:
                    areaRect.Width = (float) restore;
                    areaRect.Height = (float) restore;
                    areaRect.AspectRatioEnabled = (bool) restore;
                    areaRect.AspectRatio = (float) restore;
                    break;
                default:
                    base._CommitHandle(gizmo, handleId, secondary, restore, cancel);
                    break;
            }
        }
        _undoRedo.CommitAction();
    }
    
    private float GetZDepth(Camera3D camera, Vector3 position)
    {
        Vector3 cameraPosition = camera.GlobalPosition;
        // Remember Camera3D looks towards its -Z local axis.
        Vector3 cameraForwardVector = -camera.GlobalTransform.Basis.Z;
		
        Vector3 vectorToPosition = position - cameraPosition;
        float zDepth = vectorToPosition.Dot(cameraForwardVector);
        return zDepth;
    }
}