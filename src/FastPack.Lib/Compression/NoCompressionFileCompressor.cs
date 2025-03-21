using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FastPack.Lib.Compression;

public class NoCompressionFileCompressor : IFileCompressor
{
	public async Task DecompressFile(Stream sourceStream, Stream targetStream)
	{
		await sourceStream.CopyToAsync(targetStream);
	}

	public async Task DecompressFile(Stream sourceStream, int chunkSize, Func<ReadOnlyMemory<byte>, ValueTask> callback)
	{
		using var memoryOwner = MemoryPool<byte>.Shared.Rent(chunkSize);
		var slicedMemory = memoryOwner.Memory[..chunkSize];

		while (true)
		{
			var readCount = await sourceStream.ReadAsync(slicedMemory);
			if(readCount is 0) break;
			await callback(slicedMemory[..readCount]);
		}
	}

	public async Task<Stream> CompressFile(string inputDirectory, string inputFile, ushort compressionLevel, Stream targetStream = null)
	{
		bool inMemory = targetStream == null;
		targetStream ??= new MemoryStream();

		await using (Stream inputStream = new FileStream(Path.Combine(inputDirectory, inputFile), FileMode.Open, FileAccess.Read, FileShare.Read, Constants.BufferSize, Constants.OpenFileStreamsAsync))
			await inputStream.CopyToAsync(targetStream);

		return inMemory ? targetStream : null;
	}

	public ushort GetDefaultCompressionLevel()
	{
		return 0;
	}

	public IDictionary<ushort, string> GetCompressionLevelValuesWithNames()
	{
		return new Dictionary<ushort, string> {{0, "NoCompression"}};
	}
}