public class GameManager : Scene.Object {
	public override void OnCreate() {
		Scene.Active.MainCamera = new SceneCamera.Flat();
		base.OnCreate();
	}
}