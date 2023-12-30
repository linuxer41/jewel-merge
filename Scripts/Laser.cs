using Godot;
using Godot.Collections;

public partial class Laser : RayCast3D
{
   public Color color {get; set; } = Colors.Red;
   public LaserVFX endVFX {get; set; }
   MeshInstance3D mesh;
   public override void _Ready(){
            // Crea el Mesh
        StandardMaterial3D material = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            AlbedoColor = new Color(color.R, color.G, color.B, 0.25f),
            EmissionEnabled = true,
            EmissionEnergyMultiplier = 10f,
            Emission = color,
        };

        mesh = new MeshInstance3D() {
            Mesh = new CylinderMesh(){
                Height = 0.05f,
                TopRadius = 0.05f,
                BottomRadius = 0.05f,
                Material = material
                
            },
        };
        TargetPosition = new Vector3(0f, -50f, 0f);
        endVFX = new LaserVFX(){
            color = color,
        };
        SetDisableScale(true);
        AddChild(mesh);
        AddChild(endVFX);
   }

   public override void _Process(double delta){
       Vector3 point;
       ForceRaycastUpdate();
       if(IsColliding()){
           point = ToLocal(GetCollisionPoint());
           ((CylinderMesh)mesh.Mesh).Height = point.Y;
           mesh.Position = new Vector3(0f, point.Y/2, 0f);
           endVFX.Position = new Vector3(0f, point.Y, 0f);
       }
   }
}