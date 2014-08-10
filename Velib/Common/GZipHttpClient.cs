using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Velib.Common
{
    public class GZipHttpClient : HttpClient
	{
		private static readonly StringWithQualityHeaderValue gzip = StringWithQualityHeaderValue.Parse("gzip");
		private static readonly StringWithQualityHeaderValue deflate = StringWithQualityHeaderValue.Parse("deflate");

		public GZipHttpClient() : base(new CompressedHttpMessageHandler())
		{
		}
		public GZipHttpClient(HttpMessageHandler handler)
		{
			throw new NotSupportedException();
		}

		private sealed class CompressedHttpMessageHandler : System.Net.Http.HttpClientHandler
		{
			public override bool SupportsAutomaticDecompression
			{
				get { return true; }
			}
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				if (!request.Headers.Contains(gzip.Value))
					request.Headers.AcceptEncoding.Add(gzip);
				if(!request.Headers.Contains(deflate.Value))
					request.Headers.AcceptEncoding.Add(deflate);
 				var result = await base.SendAsync(request, cancellationToken);
				if(result.Content.Headers.ContentEncoding.Contains(gzip.Value) ||
					result.Content.Headers.ContentEncoding.Contains(deflate.Value))
				{
					result.Content = new CompressedHttpContent(result.Content);
				}
				return result;
			}
		}
		private sealed class CompressedHttpContent : HttpContent
		{
			HttpContent _content;
			public CompressedHttpContent(HttpContent content)
			{
				_content = content;

			}
			private async Task<System.IO.Stream> CreateDeflateStream()
			{
				var stream = await _content.ReadAsStreamAsync();
				if (_content.Headers.ContentEncoding.Contains(gzip.Value))
					return new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress);
				else if (_content.Headers.ContentEncoding.Contains(deflate.Value))
					return new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress);
				else
					throw new NotSupportedException("Compression type not supported or stream isn't compressed");
			}
			protected override Task<System.IO.Stream> CreateContentReadStreamAsync()
			{
				return CreateDeflateStream();
			}
			protected override async Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context)
			{
				var deflateStream = await CreateDeflateStream();
				deflateStream.CopyTo(stream);
			}
			protected override void Dispose(bool disposing)
			{
				_content.Dispose();
			}
			protected override bool TryComputeLength(out long length)
			{
				length = 0;
				return false;
			}
		}
	}
}