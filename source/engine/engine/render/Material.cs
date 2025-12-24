using Silk.NET.OpenGL;

public class Material {
	public Guid Id = Guid.NewGuid();
	public int Hash;
	public uint Handle;
	private readonly Dictionary<string,object> Attributes = [];
	
	//TODO we should also be able to load these from files, json makes sense i think
	public void Activate() => Graphics.UseProgram(this);
	public void Dispose() {
		Attributes.Clear();
		Graphics.Instance.DeleteProgram(Handle);
		Resident.Remove(Hash);
	}
	private bool Check<T>(string property, T value) => Attributes.TryGetValue(property, out var a) && a.GetType() == typeof(T) && value.Equals((T)a);
	public void Set(string property, float value) {
		if (Check(property, value))
			return;
		Attributes[property] = value;
		Graphics.UseProgram(this); //TODO this stuff shouldnt actually get set until rendering starts, we dont actually own the handle anymore. Maybe seperate internal attributes list so we dont have to keep setting the same shit
		Graphics.Instance.Uniform1(Graphics.Instance.GetUniformLocation(Handle, property), value);
	}
	public void Set(string property, Vector2 value) {
		if (Check(property, value))
			return;
		Attributes[property] = value;
		Graphics.UseProgram(this);
		Graphics.Instance.Uniform2(Graphics.Instance.GetUniformLocation(Handle, property), value);
	}
	public void Set(string property, Texture value) {
		if (Check(property, value))
			return;
		Attributes[property] = value;
		Graphics.UseProgram(this);
		var loc = Graphics.Instance.GetUniformLocation(Handle, property);
		Graphics.Instance.BindTextureUnit((uint)loc, value.Handle);
		Graphics.Instance.Uniform1(loc, loc);
	}

	private readonly static Dictionary<int,Material> Resident = [];
	public static Material From(Resources system, string vert, string frag) {
		var v = Shader.Get(system, vert, ShaderType.VertexShader);
		var f = Shader.Get(system, frag, ShaderType.FragmentShader);
		return From(v, f);
	}
	public static Material From(Shader vert, Shader frag) {
		var hc = HashCode.Combine(vert.Hash, frag.Hash);
		if (Resident.TryGetValue(hc, out var ep))
			return ep; //TODO this should be a new instance, just using the same handle
		var p = new Material {
			Handle = Graphics.Instance.CreateProgram(),
			Hash = hc,
		};
		Graphics.Instance.AttachShader(p.Handle, vert.Handle);
		Graphics.Instance.AttachShader(p.Handle, frag.Handle);
		Graphics.Instance.LinkProgram(p.Handle);
		Graphics.Instance.GetProgram(p.Handle, ProgramPropertyARB.LinkStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"program failed to link {Graphics.Instance.GetProgramInfoLog(p.Handle)}");
		Graphics.Instance.DetachShader(p.Handle, vert.Handle);
		Graphics.Instance.DetachShader(p.Handle, frag.Handle);
		Resident.Add(hc, p);
		return p;
	}
	public static void FlushAll() {
		foreach (var program in Resident.Values)
			program.Dispose();
		Resident.Clear();
	}
}