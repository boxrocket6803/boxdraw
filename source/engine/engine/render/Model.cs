using System.IO;
using System.Text;

public class Model {
	//TODO skeleton
	private struct MeshChunk {
		public Material Material {get; set;}
		public Mesh Mesh {get; set;}
	}
	private List<MeshChunk> Meshes {get; set;} = [];

	public static Model Load(string path) {
		//TODO cache
		//TODO load material
		var f = new BinaryReader(Assets.GetStream(path));
		if (f is null)
			return null;
		var bonecount = f.ReadInt16();
		for (int i = 0; i < bonecount; i++) {
			Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte())); //name
			f.ReadSingle(); f.ReadSingle(); f.ReadSingle(); //pos
			f.ReadSingle(); f.ReadSingle(); f.ReadSingle(); //ang
		}
		var meshcount = f.ReadInt16();
		for (int i = 0; i < meshcount; i++) {
			//Log.Info(Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte()))); //mesh name
			//Log.Info(Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte()))); //material name
			f.ReadByte(); //uv channel count
			f.ReadByte(); //vertex color channel count

		}
		f.Close();
		return default;
	}
}