public class GameManager : Scene.Object {
	public override void OnCreate() {
		Scene.Active.MainCamera = new Scene.Camera.Perspective();
		Model.Load("models/characters/human_base.bmdl");
		base.OnCreate();
	}

	public Material Material;
	public override void Render() {
		Material ??= Material.From("shaders/vs_model.glsl", "shaders/fs_util_uv.glsl");
		Material.Bind();
		Mesh.Sprite.Draw(Transform.Indentity);
		base.Render();
	}
}