using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NornikelTestApp
{
	internal static class InputParser
	{
		public static InputData GetInputData(string[] args)
		{
			if(args.Length != 3)
			{
				throw new ArgumentException("Неверное количество входных параметров");
			}
			var sourceFile = args[1];
			if(!File.Exists(sourceFile)) 
			{
				throw new ArgumentException($"Файл {sourceFile} должен существовать!");
			}
			var destonationFile = args[2];
			var destonationFileDir = Path.GetDirectoryName(sourceFile);
			if(!Directory.Exists(destonationFileDir)) 
			{
				throw new ArgumentException($"Директория для файла {destonationFile} должна сужуствовать!");
			}
			if(File.Exists(destonationFileDir))
			{
				var currentColor = Console.BackgroundColor;
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine($"Файл {destonationFileDir} уже имеется! Он будет перезаписан");
				Console.BackgroundColor = currentColor;
			}

			return new InputData
			{
				FileSource = sourceFile,
				FileDestonation = destonationFile,
			};
		}
	}
}
