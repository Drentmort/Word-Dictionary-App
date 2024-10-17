using System.Text;
using WordNameCounter.Factory;
using WordNameCounterFacade.DTO;
using static System.Collections.Specialized.BitVector32;

namespace WordNameCounterTestProject
{
	public class WordDataComponentTests
	{
		[Theory]
		[InlineData("aaaa ddddd", 5)]
		[InlineData($"aaaa\r\nddddd", 6)]
		[InlineData($"aaaa\nddddd", 5)]
		[InlineData($"aaaa\rddddd", 5)]
		[InlineData($"aaaa5674ddddd", 8)]
		public void Rewinder1(string data, int nextStreamPos)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream);
			Assert.Equal(0, rewind1.FromPosition);
			Assert.Equal(4, rewind1.ToPosition);
			Assert.Equal(nextStreamPos, stream.Position);
		}

		[Theory]
		[InlineData("aaaa ddddd", 5, 10)]
		[InlineData($"aaaa\r\nddddd", 6, 11)]
		[InlineData($"aaaa\r\n \r \nddddd", 10, 15)]
		[InlineData($"aaaa\nddddd", 5, 10)]
		[InlineData($"aaaa\rddddd", 5, 10)]
		[InlineData($"aaaa5674ddddd", 8, 13)]
		public void Rewinder2(string data, int from, int to)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream);
			var rewind2 = rewinder.Rewind(stream);
			Assert.Equal(from, rewind2.FromPosition);
			Assert.Equal(to, rewind2.ToPosition);
		}

		[Theory]
		[InlineData("aaaadddddsdfsbvdbsfbfgdbdfgbdgfbfgdbdfgbdfgbdfgbdfgbdfgbdfgbdfgbfdgbdfg")]
		public void Rewinder3(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream);
			Assert.Equal(0, rewind1.FromPosition);
			Assert.Equal(stream.Length, rewind1.ToPosition);
		}

		[Theory]
		[InlineData("aaaadddddsdfsbvdbsfbfgdbdfgbdgfbfgdbdfgbdfgbdfgbdfgbdfgbdfgbdfgbfdgbdfg")]
		public void Rewinder4(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream);
			var rewind2 = rewinder.Rewind(stream);
			Assert.Equal(stream.Length, rewind2.FromPosition);
			Assert.Equal(stream.Length, rewind2.ToPosition);
		}

		[Theory]
		[InlineData("\r\n \r \n aaaaa")]
		public void Rewinder5(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind = default(DataIntervalInfo);
			for (int i = 0; i < 1; i++)
			{
				rewind = rewinder.Rewind(stream);
			}
			Assert.Equal(7, rewind.FromPosition);
			Assert.Equal(stream.Length, rewind.ToPosition);
		}

		[Theory]
		[InlineData("\r\n \r \n aaaaa\r\n\r \r\r\r")]
		public void Rewinder6(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind = default(DataIntervalInfo);
			for(int i = 0; i < 1000; i++)
			{
				rewind = rewinder.Rewind(stream);
			}
			Assert.Equal(stream.Length, rewind.FromPosition);
			Assert.Equal(stream.Length, rewind.ToPosition);
		}

		[Theory]
		[InlineData("\r\n \r \n aaaaa\r\n\r \r\r\r")]
		public void Rewinder7(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind = rewinder.Rewind(stream);

			var buffer = new byte[rewind.ToPosition - rewind.FromPosition];
			stream.Position = rewind.FromPosition;
			stream.Read(buffer, 0, buffer.Length);
			var res = encoding.GetString(buffer, 0, buffer.Length);
			Assert.Equal("aaaaa", res);
		}


		[Theory]
		[InlineData("aaaadddddsdfsbvdbsfbfgdbdfgbdgfbfgdbdfgbdfgbdfgbdfgbdfgbdfgbdfgbfdgbdfg")]
		public void RewinderWithRead1(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream, out var rewindedData);
			Assert.True(rewindedData.SequenceEqual(stream.ToArray()));
		}

		[Theory]
		[InlineData("aaaa ddddd")]
		[InlineData($"aaaa\r\nddddd")]
		[InlineData($"aaaa\nddddd")]
		[InlineData($"aaaa\rddddd")]
		[InlineData($"aaaa5674ddddd")]
		public void RewinderWithRead2(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream);
			var rewind2 = rewinder.Rewind(stream, out var rewindedData);
			Assert.True(rewindedData.SequenceEqual(encoding.GetBytes("ddddd")));
		}

		[Theory]
		[InlineData("aaaa ddddd")]
		[InlineData($"aaaa\r\nddddd")]
		[InlineData($"aaaa\nddddd")]
		[InlineData($"aaaa\rddddd")]
		[InlineData($"aaaa5674ddddd")]
		public void RewinderWithRead3(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var rewind1 = rewinder.Rewind(stream, out var rewindedData);
			var rewind2 = rewinder.Rewind(stream);
			Assert.True(rewindedData.SequenceEqual(encoding.GetBytes("aaaa")));
		}

		[Theory]
		[InlineData("aaaaaaaaaaaa", 1)]
		[InlineData("aaaa aaaa aaaa", 2)]
		[InlineData("aaab bbaaa aaaa baa aa aab baa fff fffffffffff  ffffffff", 3)]
		[InlineData("aaaa ddddd sdadfsdaf\r\n dfsdfsdfsdf \r  fdsdfsfgdfghdghasrdfasdfasdfasdfgasdfdgaddfgafasdfgadfg fad fda dfdf sdfsdfsdf sdfsdfsdf dsaa", 2)]
		public void Splitter1(string data, int sectionCount)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var splitter = WordDataComponentsFactory.CreateWordDataStreamSplitter(3, rewinder);
			var sections = splitter.Split(stream);
			Assert.Equal(sectionCount, sections.Count());
		}

		[Theory]
		[InlineData("\r\n\r\nbbb\r\n\r\n\r\n aaa\r \n \r\n bbb\rvvv\n vvv\r\n aaaa\n bbb \r \n  vvv\r\n \r\n \r\naaa \r\n \r\n \r\n ")]
		public void Scanner1(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var counter = WordDataComponentsFactory.CreateWordDataCounter();
			var scanner = WordDataComponentsFactory.CreateWordDataScanner(stream, rewinder, encoding, counter);
			scanner.Scan(new DataIntervalInfo { FromPosition = 0, ToPosition = stream.Length });
			Assert.Equal(2, counter.GetObjectCount("aaa"));
			Assert.Equal(3, counter.GetObjectCount("bbb"));
			Assert.Equal(3, counter.GetObjectCount("vvv"));
			Assert.Equal(1, counter.GetObjectCount("aaaa"));
		}

		[Theory]
		[InlineData("\r\n\r\nbbb\r\n\r\n\r\n aaa\r \n \r\n bbb\rvvv\n vvv\r\n aaaa\n bbb \r \n  vvv\r\n \raaa\n \r\naaa \rvvv\n \r\nvvv \r\n ")]
		public void Scanner2(string data)
		{
			var encoding = Encoding.UTF8;
			using var stream = new MemoryStream(encoding.GetBytes(data));

			var rewinder = WordDataComponentsFactory.CreateWordRewinder(new List<string> { " ", "\r\n", "\n", "\r", "5674" }, encoding);
			var counter = WordDataComponentsFactory.CreateWordDataCounter();
			var scanner = WordDataComponentsFactory.CreateWordDataScanner(stream, rewinder, encoding, counter);
			scanner.Scan(new DataIntervalInfo { FromPosition = 9, ToPosition = stream.Length });
			Assert.Equal(3, counter.GetObjectCount("aaa"));
			Assert.Equal(2, counter.GetObjectCount("bbb"));
			Assert.Equal(5, counter.GetObjectCount("vvv"));
			Assert.Equal(1, counter.GetObjectCount("aaaa"));
		}		
	}
}