using Silk.NET.Windowing;
using System.IO;

public class Game {
	static void Main() {
		Game g = new();
		g.Init();
		g.Destroy();
	}

	public IWindow Window {get; set;}
	public string Directory {get; set;}
	public Resources Core {get; set;}
	public Resources Assets {get; set;}
	public Graphics Graphics {get; set;}
	public Audio Audio {get; set;}
	public Input Input {get; set;}

	public Scene Scene {get; set;}

	private void Init() {
		Directory = Path.GetDirectoryName(Environment.ProcessPath);
		var test = Directory == "E:\\engine\\source\\game\\bin";
		if (test)
			Directory = "E:\\engine";
		Time.Update();

		Core = new(this, "core");
		Core.Init();
		Assets = new(this, "assets");
		Assets.Init();
		Window = Silk.NET.Windowing.Window.Create(WindowOptions.Default with {
			Size = new(300, 1),
			Title = "BOXDRAW",
			VSync = false
		});
		Window.Initialize();
		Graphics = new(this);
		Graphics.Init(Window);
		Audio = new(this);
		Audio.Init();
		Input = new();
		Input.Init(Window);
		Scene.Active = Scene = new(this);

		Window.Update += Update;
		Window.FramebufferResize += (size) => Graphics.Instance?.Viewport(size);
		Window.Render += (d) => Graphics.Render();
		Window.Size = new(640, 480);
		if (!test)
			Window.WindowState = WindowState.Maximized;
		Time.Update();
		Window.Run();
	}

	private void Update(double delta) {
		Core.Update();
		Assets.Update();
		Time.Update();
		Input.Update();
		Scene.UpdateActive();
		Audio.Update();
	}

	private void Destroy()  => Window.Dispose();
}
