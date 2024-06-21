using System.Text;

namespace PathReader
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			String file = args[0];
			BinaryReader reader = ReadElements(file);
			PathData PathData = new PathData();
			PathData.Timestamp = BitConverter.ToInt32(reader.ReadBytes(4));
			PathData.TotalPaths = BitConverter.ToInt32(reader.ReadBytes(4));

			Console.WriteLine("Timestamp: " + PathData.Timestamp);
			Console.WriteLine("Total Paths: " + PathData.TotalPaths);

			ESOPath[] ESOPaths = new ESOPath[PathData.TotalPaths];
			for (Int32 runs = 0; runs < PathData.TotalPaths; runs++)
			{
				
				ESOPath ESOPath = new ESOPath();
				byte[] data = reader.ReadBytes(4);
				ESOPath.ID = BitConverter.ToInt32(data);
				data = reader.ReadBytes(4);
				ESOPath.PathLength = BitConverter.ToInt32(data);
				data = reader.ReadBytes(ESOPath.PathLength);
				ESOPath.FilePath = Encoding.GetEncoding(936).GetString(data);
				
				ESOPaths[runs] = ESOPath;
			}
			PathData.ESOPaths = ESOPaths;

			Directory.CreateDirectory("./out");
			String docPath = "./out/";
			string doc = $"{PathData.Timestamp}-paths.csv";

			using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, doc), true))
			{
				outputFile.WriteLine("ID,Path");
				foreach (ESOPath gamepath in ESOPaths)
				{
					outputFile.WriteLine($"{gamepath.ID},{gamepath.FilePath}");
				}
			}
		}

		static BinaryReader ReadElements(string file)
		{
			BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read));
			return reader;
		}

		public class PathData
		{
			public Int32 Timestamp { get; set; }
			public Int32 TotalPaths { get; set; }
			public ESOPath[] ESOPaths { get; set; }
		}

		public class ESOPath : PathData
		{
			public Int32 ID { get; set; }
			public Int32 PathLength {get; set;}
			public String? FilePath { get; set; }
		}
	}
}