﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс-описатель программы
	/// </summary>
	public static class GeomagDataDrawerProgram
		{
		/// <summary>
		/// Главная точка входа для приложения
		/// </summary>
		[STAThread]
		public static void Main (string[] args)
			{
			// Загрузка конфигурации
			ConfigAccessor ca = new ConfigAccessor ();

			// Проверка запуска единственной копии
			bool result;
			Mutex instance = new Mutex (true, ProgramDescription.AssemblyTitle, out result);
			if (!result)
				{
				MessageBox.Show (string.Format (Localization.GetText ("ProgramLaunchedError", ca.InterfaceLanguage),
					ProgramDescription.AssemblyTitle), ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Запуск программы в случае уникальности
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Отображение справки и запроса на принятие Политики
			if (!ProgramDescription.AcceptEULA ())
				return;
			ProgramDescription.ShowAbout (true);

			// Передача параметра и предполагаемого типа файла
			if (args.Length > 0)
				{
				// Справка по командной строке
				if (args[0].Contains ("?"))
					{
					MessageBox.Show (Localization.GetText ("CommandLineHelp", ca.InterfaceLanguage),
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
					}

				// Входной файл
				DataInputTypes inputType = DataInputTypes.Unknown;  // Извлечение по умолчанию

				if (args[0].Length >= 5)        // Расширение (не менее 3-х символов) + '.' + имя (не менее одного символа)
					{
					switch (args[0].Substring (args[0].Length - 4).ToLower ())
						{
						case "." + ProgramDescription.AppDataExtension:
							inputType = DataInputTypes.GDD;
							break;

						case "xlsx":
						case ".xls":
							inputType = DataInputTypes.XLS;
							break;

						case ".csv":
							inputType = DataInputTypes.CSV;
							break;
						}
					}

				// Только открытие файла
				if (args.Length == 1)
					{
					Application.Run (new GeomagDataDrawerForm (args[0], inputType));
					return;
					}

				// Определение параметров для консольной обработки
				uint skippedLinesCount = ca.SkippedLinesCount;
				uint expectedColumnsCount = ca.ExpectedColumnsCount;

				if (args.Length > 2)
					{
					uint.TryParse (args[2], out skippedLinesCount);
					if (skippedLinesCount > ConfigAccessor.MaxSkippedLinesCount)
						{
						skippedLinesCount = ConfigAccessor.MaxSkippedLinesCount;
						}

					if (args.Length > 3)
						{
						uint.TryParse (args[3], out expectedColumnsCount);
						if (expectedColumnsCount > ConfigAccessor.MaxExpectedColumnsCount)
							{
							expectedColumnsCount = ConfigAccessor.MaxExpectedColumnsCount;
							}
						if (expectedColumnsCount < 2)
							{
							expectedColumnsCount = 2;
							}
						}
					}

				// Выходной файл
				int outputType = (int)DataOutputTypes.ANY;  // По умолчанию - файл свободной топологии

				if (args[1].Length >= 5)
					{
					switch (args[1].Substring (args[1].Length - 4).ToLower ())
						{
						// Файлы данных
						case "." + ProgramDescription.AppDataExtension:
							outputType = (int)DataOutputTypes.GDD;
							break;

						/*case "xlsx":
						case ".xls":
							outputType = DataOutputTypes.XLS;
							break;*/

						case ".csv":
							outputType = (int)DataOutputTypes.CSV;
							break;

						// Файлы изображений
						case ".png":
							outputType = (int)ImageOutputTypes.PNG;
							break;

						case ".svg":
							outputType = (int)ImageOutputTypes.SVG;
							break;

							/*case ".emf":
								outputType = (int)ImageOutputTypes.EMF;
								break;*/
						}
					}

				// Запуск интерпретации
				DiagramData dd;
				if (inputType == DataInputTypes.Unknown)
					{
					dd = new DiagramData (args[0], expectedColumnsCount, skippedLinesCount);
					}
				else
					{
					dd = new DiagramData (args[0], inputType, skippedLinesCount);
					}

				// Контроль результата
				if (dd.InitResult != DiagramDataInitResults.Ok)
					{
					MessageBox.Show (string.Format (Localization.GetText ("DataFileLoadError", ca.InterfaceLanguage), args[0],
						DiagramDataInitResultsMessage.ErrorMessage (dd.InitResult, ca.InterfaceLanguage)),
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
					}

				// Запись файла данных
				if (outputType < 4)
					{
					if (dd.SaveDataFile (args[1], (DataOutputTypes)outputType, true) < 0)
						{
						MessageBox.Show (Localization.GetText ("DataFileSaveError", ca.InterfaceLanguage),
							ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
						}
					}
				// Запись изображения
				else
					{
					// Применение шаблона отображения
					ColumnsAdderCmd cad = new ColumnsAdderCmd (dd.DataColumnsCount, true, ca.InterfaceLanguage);
					if (!cad.LoadParametersFile (AboutForm.AppStartupPath + ConfigAccessor.LineParametersFileName))
						{
						if (!cad.CreateParametersFile (AboutForm.AppStartupPath + ConfigAccessor.LineParametersFileName))
							{
							return;
							}

						cad.LoadParametersFile (AboutForm.AppStartupPath + ConfigAccessor.LineParametersFileName);
						}

					// Добавление кривых
					for (int i = 0; i < cad.XColumnNumber.Count; i++)
						{
						int res = dd.AddDiagram (cad.XColumnNumber[i], cad.YColumnNumber[i]);
						if (res < 0)
							{
							continue;
							}

						dd.GetStyle (i).DiagramImageWidth = cad.ImageWidth[i];
						dd.GetStyle (i).DiagramImageHeight = cad.ImageHeight[i];
						dd.GetStyle (i).DiagramImageLeftOffset = cad.ImageLeft[i];
						dd.GetStyle (i).DiagramImageTopOffset = cad.ImageTop[i];
						if (!cad.AutoNameOffset[i])
							{
							dd.GetStyle (i).LineName = cad.LineName[i];
							dd.GetStyle (i).LineNameLeftOffset = cad.LineNameLeftOffset[i];
							dd.GetStyle (i).LineNameTopOffset = cad.LineNameTopOffset[i];
							}
						}

					// Сохранение изображения
					SavePicture sp = new SavePicture (dd, ca.InterfaceLanguage, true);
					sp.SaveImage (args[1], (ImageOutputTypes)outputType);
					}

				// Завершено
				}
			else
				{
				// Случай загрузки backup-файла
				Application.Run (new GeomagDataDrawerForm ("", DataInputTypes.Unspecified));
				}
			}
		}
	}
