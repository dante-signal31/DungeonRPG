#if TOOLS
using Godot;

namespace DungeonRPG.addons.area_rect;

[Tool]
public partial class AreaRectRegister : EditorPlugin
{
	private AreaRectGizmo _areaRectGizmo;
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		var script = GD.Load<Script>("res://addons/area_rect/AreaRect.cs");
		var icon = GD.Load<Texture2D>(
			"res://addons/area_rect/RectangleShape2D.svg"
			);
		AddCustomType("AreaRect", "Node3D", script, icon);

		_areaRectGizmo = (AreaRectGizmo)GD
			.Load<CSharpScript>("res://addons/area_rect/AreaRectGizmo.cs")
			.New(GetUndoRedo());
		AddNode3DGizmoPlugin(_areaRectGizmo);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("AreaRect");
		RemoveNode3DGizmoPlugin(_areaRectGizmo);
	}
}
#endif
