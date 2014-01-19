using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HEPacker
{
	public class Packer
	{
		public static void Pack(String FolderPath, String OutFile)
		{
			string[] he = Directory.GetFiles(FolderPath, "*.he*");
			string[] a = Directory.GetFiles(FolderPath, "*.(a)");
			string[] b = Directory.GetFiles(FolderPath, "*.(b)");
			MD5Entry game = new MD5Entry();
			foreach (string s in he)
			{
				if (s.ToLower().EndsWith("he0"))
				{
					byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(s));
					string hashs = BitConverter.ToString(hash).Replace("-", "").ToLower();
					MD5Entry[] e = MD5Table.Where(ss => ss.hash == hashs).ToArray();
					if (e.Length != 0)
					{
						game = e[0];
						break;
					}
					else
					{
						System.Windows.Forms.MessageBox.Show("Unknown Game!\n" + hashs);
						game = new MD5Entry(hashs, "Unknown Game", 0);
						break;
					}
					return;
				}
			}
			if (game.name == null) return;
			//System.Windows.Forms.MessageBox.Show(String.Format("{0}\nHE {1}", game.name, game.version));
			//-- Header (0xC) --
			//0x00 - HEPA
			//0x04 - FileSize (BE!)
			//0x08 - HE Version
			//0x0A - Nr Files
			//-- File Index Block (0xC) --
			//0x00 - File Id (he0 = 0, he1 = 1, he2 = 2, (a) = 1, (b) = 0xB)
			//0x04 - File Offset
			//0x08 - File Length
			//MemoryStream m = new MemoryStream();
			EndianBinaryWriter er = new EndianBinaryWriter(/*m*/File.Create(OutFile), Endianness.LittleEndian);
			er.Write("HEPA", Encoding.ASCII, false);
			er.Write((UInt32)0);
			er.Write((UInt16)(game.version * 10));
			er.Write((UInt16)(he.Length + a.Length + b.Length));
			Dictionary<int, String> Files = new Dictionary<int, String>();
			for (int i = 0; i < he.Length; i++)
			{
				char idx = he[i][he[i].Length - 1];
				Files.Add((int)char.GetNumericValue(idx), he[i]);
			}
			if (a.Length != 0) Files.Add(1, a[0]);
			if (b.Length != 0) Files.Add(0xB, b[0]);
			int[] files = Files.Keys.ToArray();
			Array.Sort(files);
			for (int i = 0; i < files.Length; i++)
			{
				er.Write((UInt32)(files[i]));
				er.Write((UInt32)0);
				er.Write((UInt32)0);
			}
			for (int i = 0; i < files.Length; i++)
			{
				byte[] d = File.ReadAllBytes(Files[files[i]]);
				if (files[i] == 0 || files[i] == 1)
				{
					for (int q = 0; q < d.Length; q++)
					{
						d[q] ^= 0x69;
					}
				}
				long curpos = er.BaseStream.Position;
				er.BaseStream.Position = 12 + i * 12 + 4;
				er.Write((UInt32)(curpos));
				er.Write((UInt32)(d.Length));
				er.BaseStream.Position = curpos;
				er.Write(d, 0, d.Length);
			}
			er.BaseStream.Position = 4;
			er.Endianness = Endianness.BigEndian;
			er.Write((UInt32)(er.BaseStream.Length));
			//byte[] data = m.GetBuffer();
			er.Close();
			//File.Create(OutFile).Close();
			//File.WriteAllBytes(OutFile, data);
		}

		struct MD5Entry
		{
			public MD5Entry(string hash, string name, float version)
			{
				this.hash = hash;
				this.name = name;
				this.version = version;
			}
			public string hash;
			public string name;
			public float version;
		};

		private static MD5Entry[] MD5Table =
		{
			new MD5Entry("1c792d28376d45e145cb916bca0400a2", "spyfox2-demo-win-nl", 99.0f),
			new MD5Entry("30ba1e825d4ad2b448143ae8df18482a", "pajama2-demo-win-nl", 98.5f),
			new MD5Entry("499c958affc394f2a3868f1eb568c3ee", "freddi4-demo-win-nl", 99.0f),
			new MD5Entry("4edbf9d03550f7ba01e7f34d69b678dd", "spyfox-demo-win-nl", 98.5f),
			new MD5Entry("cd424f143a141bc59226ad83a6e40f51", "maze-win-nl", 98.5f),
			new MD5Entry("f08145577e4f13584cc90b3d6e9caa55", "pajama3-demo-win-nl", 99.0f)
		};
	}
}
