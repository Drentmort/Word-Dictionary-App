﻿using NornikelTestApp;
using System.Diagnostics;
using System.Text;
using WordNameCounter.Factory;

Console.WriteLine("Тестовое приложение. Андряков А.А.");
var input = InputParser.GetInputData(Environment.GetCommandLineArgs());

Console.WriteLine($"{DateTime.Now}: Начинаем анализ файла {input.FileSource}...");
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var scanManager = WordDataComponentsFactory.CreateSimpleWordDataCountScanManager(encoding: Encoding.GetEncoding("windows-1251"));
scanManager.ScanFile(input.FileSource, input.FileDestonation);

Console.WriteLine($"{DateTime.Now}: Анализ файла {input.FileSource} окончен. Результат в {input.FileDestonation}");
