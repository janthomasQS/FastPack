using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FastPack.Lib.Compression;

public interface IFileCompressor
{
	public Task DecompressFile(Stream sourceStream, Stream targetStream);
	public Task DecompressFile(Stream sourceStream, int chunkSize, Func<ReadOnlyMemory<byte>, ValueTask> callback);
	public Task<Stream> CompressFile(string inputDirectory, string inputFile, ushort compressionLevel, Stream targetStream = null);
	public ushort GetDefaultCompressionLevel();
	public IDictionary<ushort, string> GetCompressionLevelValuesWithNames();
}