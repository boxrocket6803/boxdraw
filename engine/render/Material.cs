using Silk.NET.OpenGL;
using System.Reflection.Metadata;

public class Material {
	public class Resource : global::Resource {
		public string Vertex {get; set;} = "shaders/vs_model.glsl";
		public string Depth {get; set;} = "shaders/ds_opaque.glsl";
		public string Fragment {get; set;}
		public Dictionary<string, Texture> Textures {get; set;} = [];
		public Material GetMaterial() {
			var m = From(Vertex, Fragment, Depth);
			if (m is null)
				return null;
			foreach (var texture in Textures)
				m.Set(texture.Key, texture.Value);
			return m;
		}
	}

	public Guid Id = Guid.NewGuid();
	public uint DepthHandle;
	public uint ColorHandle;
	public uint Handle => Graphics.Stage == Graphics.RenderStage.Depth ? DepthHandle : ColorHandle;
	private readonly Dictionary<string,object> Attributes = [];
	
	public void Set(string property, float value) => Set(property, (object)value);
	public void Set(string property, Vector2 value) => Set(property, (object)value);
	public void Set(string property, Texture value) => Set(property, (object)value);
	public void Set(string property, Matrix4x4 value) => Set(property, (object)value);
	private void Set(string property, object value) {
		Attributes[property] = value;
		if (Graphics.Stage == Graphics.RenderStage.Idle || Graphics.Stage == Graphics.RenderStage.Submit)
			return;
		SetUniform(Handle, property, value);
	}
	public void Bind() {
		if (Active != Handle)
			Graphics.Instance.UseProgram(Handle);
		Active = Handle;
		foreach (var attribute in Attributes)
			SetUniform(Handle, attribute.Key, attribute.Value);
		Scene.Active.MainCamera.Update(this);
	}
	private unsafe void SetUniform(uint handle, string property, object value) {
		var hc = value.GetHashCode();
		if (ProgState[handle].GetValueOrDefault(property) == hc)
			return;
		ProgState[handle][property] = hc;
		var location = Graphics.Instance.GetUniformLocation(handle, property);
		if (value is float flval) {
			Graphics.Instance.Uniform1(location, flval);
			return;
		}
		if (value is Vector2 v2val) {
			Graphics.Instance.Uniform2(location, v2val);
			return;
		}
		if (value is Texture tval) {
			Graphics.Instance.BindTextureUnit((uint)location, tval.Handle);
			Graphics.Instance.Uniform1(location, location);
			return;
		}
		if (value is Matrix4x4 m3val) {
			Graphics.Instance.UniformMatrix4(location, 1, false, (float*)&m3val);
			return;
		}
	}

	private readonly static Dictionary<int,Material> Resident = [];
	private readonly static Dictionary<uint,Dictionary<string, int>> ProgState = [];
	private static uint Active {get; set;}
	public static Material From(string file) => From(global::Resource.Load<Resource>(file));
	public static Material From(Resource r) => r?.GetMaterial() ?? null;
	public static Material From(string vert, string frag, string depth) {
		if (vert is null || frag is null || depth is null)
			return null;
		var v = Shader.Get(vert, ShaderType.VertexShader);
		var f = Shader.Get(frag, ShaderType.FragmentShader);
		var d = Shader.Get(depth, ShaderType.FragmentShader);
		return From(v, f, d);
	}
	public static Material From(Shader vert, Shader frag, Shader depth) {
		if (vert is null || frag is null || depth is null)
			return null;
		var hc = HashCode.Combine(vert.Hash, frag.Hash, depth.Hash);
		if (Resident.TryGetValue(hc, out var ep))
			return new() {ColorHandle = ep.ColorHandle};
		var p = new Material {
			ColorHandle = Graphics.Instance.CreateProgram(),
			DepthHandle = Graphics.Instance.CreateProgram()
		};
		Link(p.ColorHandle, vert.Handle, frag.Handle);
		Link(p.DepthHandle, vert.Handle, depth.Handle);
		Resident.Add(hc, p);
		ProgState.Add(p.ColorHandle, []);
		ProgState.Add(p.DepthHandle, []);
		return p;
	}
	private static void Link(uint handle, uint vert, uint frag) {
		Graphics.Instance.AttachShader(handle, vert);
		Graphics.Instance.AttachShader(handle, frag);
		Graphics.Instance.LinkProgram(handle);
		Graphics.Instance.GetProgram(handle, ProgramPropertyARB.LinkStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"program failed to link {Graphics.Instance.GetProgramInfoLog(handle)}");
		Graphics.Instance.DetachShader(handle, vert);
		Graphics.Instance.DetachShader(handle, frag);
	}
	public static void FlushAll() { //TODO should be per shader, which is of course really annoying
		HashSet<uint> handles = [];
		foreach (var program in Resident.Values)
			handles.Add(program.ColorHandle);
		foreach (var handle in handles)
			Graphics.Instance.DeleteProgram(handle);
		ProgState.Clear();
		Resident.Clear();
	}
}