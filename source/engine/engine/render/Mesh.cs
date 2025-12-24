using Silk.NET.OpenGL;

public class Mesh {
	public Guid Id = Guid.NewGuid();
	public uint Handle;
	public uint Count;

	public unsafe void Render() { //calling render here should add to a per frame queue for instance rendering, hash by current program and mesh id
		Graphics.Instance.BindVertexArray(Handle);
		Graphics.Instance.DrawElements(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0);
	}

	public static Mesh Sprite {get; set;}
	public static void Init() {
		Sprite = From([1,1,0,1,0,1,-1,0,1,1,-1,-1,0,0,1,-1,1,0,0,0], [0,1,3,1,2,3]);
	}
	public unsafe static Mesh From(float[] verticies, uint[] indicies) {
		var m = new Mesh {
			Handle = Graphics.Instance.GenVertexArray(),
			Count = (uint)indicies.Length
		};
		Graphics.Instance.BindVertexArray(m.Handle);

		var vbo = Graphics.Instance.GenBuffer();
		Graphics.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, m.Handle);
		fixed (float* buf = verticies)
			Graphics.Instance.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticies.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

		var ebo = Graphics.Instance.GenBuffer();
		Graphics.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
		fixed (uint* buf = indicies)
			Graphics.Instance.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicies.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

		Graphics.Instance.EnableVertexAttribArray(0);
		Graphics.Instance.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
		Graphics.Instance.EnableVertexAttribArray(1);
		Graphics.Instance.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));

		Graphics.Instance.BindVertexArray(0);
		Graphics.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

		return m;
	}
}