using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static System.Text.Encoding;

namespace TioSharp
{
	public class TioApi
	{
		public string Backend { get; }
		public string Json { get; }
		public List<string> Languages { get; }

		public TioApi(string backend = "https://tio.run/cgi-bin/run/api/", string json = "https://tio.run/languages.json")
		{
			Backend = backend;
			Json = json;
			Languages = new List<string>();
			RefreshLanguages();
		}

		public void RefreshLanguages()
		{
			JObject file = JObject.Parse(new WebClient().DownloadString(Json));

			foreach (JToken content in file.Children())
			{
				JProperty jProperty = content.ToObject<JProperty>();
				if (jProperty != null) Languages.Add(jProperty.Name);
			}
		}

		public async Task RefreshLanguagesAsync()
		{
			JObject file = JObject.Parse(await new WebClient().DownloadStringTaskAsync(new Uri(Json)));

			foreach (JToken content in file.Children())
			{
				JProperty jProperty = content.ToObject<JProperty>();
				if (jProperty != null) Languages.Add(jProperty.Name);
			}
		}

		// <summary>
		// Generates a valid TIO byte array (utf-8) for a variable or a file
		// </summary>
		private byte[] ToTioBytes(KeyValuePair<string, object> couple)
		{
			string name = couple.Key;
			object obj = couple.Value;

			switch (obj)
			{
				case null:
					return new byte[0];
				case string[] flags:
					{
						switch (flags.Length)
						{
							case 0:
								return new byte[0];
							case 1:
								return UTF8.GetBytes($"F{name}\0{UTF8.GetBytes(flags[0]).Length}\0{flags[0]}\0");
							default:
								{
									var content = new List<string> { $"V{name}", flags.Length.ToString() };
									content.AddRange(flags);
									return UTF8.GetBytes(string.Join('\0', content) + "\0");
								}
						}
					}
				default:
					return UTF8.GetBytes($"{(name == "lang" ? 'V' : 'F')}{name}\0{(name == "lang" ? 1 : UTF8.GetBytes((string)obj).Length)}\0{(string)obj}\0");
			}
		}

		// <summary>
		// Returns a DEFLATE compressed byte array ready to be sent
		// </summary>
		public byte[] CreateRequestData(string language, string code, string[] inputs = null, string[] cFlags = null, string[] options = null, string[] args = null)
		{
			Dictionary<string, object> strings = new Dictionary<string, object> {
				{ "lang", language },
				{ ".code.tio", code },
				{ ".input.tio", inputs != null? string.Join('\n',inputs) : null },// Lists of lines to give when input is asked
				{ "TIO_CFLAGS", cFlags },
				{ "TIO_OPTIONS", options },
				{ "args", args }
 			};

			List<byte> bytes = new List<byte>();

			foreach (KeyValuePair<string, object> pair in strings)
			{
				bytes.AddRange(ToTioBytes(pair));
			}
			bytes.Add((byte)'R');

			return Ionic.Zlib.DeflateStream.CompressBuffer(bytes.ToArray());
		}

		// <summary>
		// Sends given request and returns TIO output
		// </summary>
		public async Task<string> SendAsync(byte[] requestData)
		{
			HttpClient client = new HttpClient();
			var response = await client.PostAsync("https://tio.run/cgi-bin/run/api/", new ByteArrayContent(requestData));
			response.EnsureSuccessStatusCode();
			byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
			string output = Ionic.Zlib.GZipStream.UncompressString(responseBytes);
			return output.Replace(output[..16], ""); // Remove token
		}
	}
}
