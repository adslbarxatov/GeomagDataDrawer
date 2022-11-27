using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает схему получения данных для построения изображения SVG
	/// </summary>
	public class SVGScriptReader
		{
		// Переменные
		#region Общие параметры

		// Внутренние параметры
		private char[] splitters = new char[] { ';' };          // Массив сплиттеров строк параметров
		private CultureInfo cie = new CultureInfo ("en-us");    // Тип десятичного разделителя
		private const string tmpExtension = ".pcsc";            // Расширение вспомогательного файла
		private const uint maxIncludeDeep = 100;                // Максимальное количество подключений

		/// <summary>
		/// Максимальная абсцисса изображения
		/// </summary>
		public float MaxX
			{
			get
				{
				return maxX;
				}
			}
		private float maxX = 0;

		/// <summary>
		/// Максимальная ордината изображения
		/// </summary>
		public double MaxY
			{
			get
				{
				return maxY;
				}
			}
		private double maxY = 0;

		/// <summary>
		/// Минимальная абсцисса изображения
		/// </summary>
		public float MinX
			{
			get
				{
				return minX;
				}
			}
		private float minX = 0;

		/// <summary>
		/// Минимальная ордината изображения
		/// </summary>
		public double MinY
			{
			get
				{
				return minY;
				}
			}
		private double minY = 0;

		/// <summary>
		/// Возвращает номер строки в файле, на котором завершилась обработка
		/// (независимо от успеха)
		/// </summary>
		public uint CurrentLine
			{
			get
				{
				return currentLine;
				}
			}
		private uint currentLine = 0;

		/// <summary>
		/// Возвращает имя файла, который не удалось подключить при сборке скрипта
		/// </summary>
		public string FaliedIncludeFile
			{
			get
				{
				return faliedIncludeFile;
				}
			}
		private string faliedIncludeFile = "";

		/// <summary>
		/// Возможные результаты инициализация класса
		/// </summary>
		public enum InitResults
			{
			/// <summary>
			/// Успешно инициализирован
			/// </summary>
			Ok = 0,

			/// <summary>
			/// Файл сценария не найден или недоступен
			/// </summary>
			FileNotAvailable = -1,

			/// <summary>
			/// Не удаётся создать вспомогательный файл
			/// </summary>
			CannotCreateTMP = -2,

			/// <summary>
			/// Подключаемый файл не найден или недоступен
			/// </summary>
			CannotIncludeFile = -3,

			/// <summary>
			/// Не инициализирован
			/// </summary>
			NotInited = 1,

			/// <summary>
			/// Ошибка в описании одной из точек одной из кривых
			/// </summary>
			BrokenLinePoint = -11,

			/// <summary>
			/// Ошибка в описании цвета одной из кривых
			/// </summary>
			BrokenLineColor = -12,

			/// <summary>
			/// Ошибка в описании толщины кривой
			/// </summary>
			BrokenLineWidth = -13,

			/// <summary>
			/// Ошибка в описании одной из засечек на оси Ox
			/// </summary>
			BrokenOxNotch = -21,

			/// <summary>
			/// Ошибка в описании цвета оси Ox
			/// </summary>
			BrokenOxColor = -22,

			/// <summary>
			/// Ошибка в описании толщины оси Ox
			/// </summary>
			BrokenOxWidth = -23,

			/// <summary>
			/// Ошибка в описании смещения оси Ox
			/// </summary>
			BrokenOxOffset = -24,

			/// <summary>
			/// Ошибка в описании одной из засечек на оси Oy
			/// </summary>
			BrokenOyNotch = -31,

			/// <summary>
			/// Ошибка в описании цвета оси Oy
			/// </summary>
			BrokenOyColor = -32,

			/// <summary>
			/// Ошибка в описании толщины оси Oy
			/// </summary>
			BrokenOyWidth = -33,

			/// <summary>
			/// Ошибка в описании смещения оси Oy
			/// </summary>
			BrokenOyOffset = -34,

			/// <summary>
			/// Ошибка в описании текстовой подписи
			/// </summary>
			BrokenText = -41,

			/// <summary>
			/// Превышение максимально допустимого числа подключений файлов
			/// </summary>
			IncludeDeepOverflow = -51
			}

		/// <summary>
		/// Возвращает результат инициализации класса
		/// </summary>
		public InitResults InitResult
			{
			get
				{
				return initResult;
				}
			}
		private InitResults initResult = InitResults.NotInited;

		#endregion

		#region Параметры кривых

		/// <summary>
		/// Возвращает массив абсцисс точек кривых, полученный из файла сценария
		/// Схема: LinesX[номер кривой][номер точки]
		/// </summary>
		public List<List<float>> LinesX
			{
			get
				{
				return linesX;
				}
			}
		private List<List<float>> linesX = new List<List<float>> ();

		/// <summary>
		/// Возвращает массив ординат точек кривых, полученный из файла сценария
		/// Схема: LinesY[номер кривой][номер точки]
		/// </summary>
		public List<List<double>> LinesY
			{
			get
				{
				return linesY;
				}
			}
		private List<List<double>> linesY = new List<List<double>> ();

		/// <summary>
		/// Возвращает список цветов кривых в соответствии с их порядком в массиве данных
		/// </summary>
		public List<Color> LinesColors
			{
			get
				{
				return linesColors;
				}
			}
		private List<Color> linesColors = new List<Color> ();

		/// <summary>
		/// Возвращает список значений толщины кривых в соответствии с их порядком в массиве данных
		/// </summary>
		public List<uint> LinesWidths
			{
			get
				{
				return linesWidths;
				}
			}
		private List<uint> linesWidths = new List<uint> ();

		#endregion

		#region Параметры осей

		/// <summary>
		/// Возвращает флаг, указывающий на необходимость отрисовки оси Ox
		/// </summary>
		public bool DrawOx
			{
			get
				{
				return oxInited;
				}
			}
		private bool oxInited = false;

		/// <summary>
		/// Возвращает флаг, указывающий на необходимость отрисовки оси Oy
		/// </summary>
		public bool DrawOy
			{
			get
				{
				return oyInited;
				}
			}
		private bool oyInited = false;

		/// <summary>
		/// Возвращает массив смещений засечек на оси Ox относительно левого края изображения
		/// </summary>
		public List<float> OxNotchesOffsets
			{
			get
				{
				return oxNotchesOffsets;
				}
			}
		private List<float> oxNotchesOffsets = new List<float> ();

		/// <summary>
		/// Возвращает массив длин засечек на оси Ox
		/// </summary>
		public List<double> OxNotchesSizes
			{
			get
				{
				return oxNotchesSizes;
				}
			}
		private List<double> oxNotchesSizes = new List<double> ();

		/// <summary>
		/// Возвращает массив смещений засечек на оси Oy относительно верхнего края изображения
		/// </summary>
		public List<double> OyNotchesOffsets
			{
			get
				{
				return oyNotchesOffsets;
				}
			}
		private List<double> oyNotchesOffsets = new List<double> ();

		/// <summary>
		/// Возвращает массив длин засечек на оси Oy
		/// </summary>
		public List<float> OyNotchesSizes
			{
			get
				{
				return oyNotchesSizes;
				}
			}
		private List<float> oyNotchesSizes = new List<float> ();

		/// <summary>
		/// Возвращает толщину линии оси Ox и её засечек
		/// </summary>
		public uint OxWidth
			{
			get
				{
				return oxWidth;
				}
			}
		private uint oxWidth = 1;

		/// <summary>
		/// Возвращает цвет линии оси Ox и её засечек
		/// </summary>
		public Color OxColor
			{
			get
				{
				return oxColor;
				}
			}
		private Color oxColor = Color.FromArgb (0, 0, 0);

		/// <summary>
		/// Возвращает толщину линии оси Oy и её засечек
		/// </summary>
		public uint OyWidth
			{
			get
				{
				return oyWidth;
				}
			}
		private uint oyWidth = 1;

		/// <summary>
		/// Возвращает цвет линии оси Oy и её засечек
		/// </summary>
		public Color OyColor
			{
			get
				{
				return oyColor;
				}
			}
		private Color oyColor = Color.FromArgb (0, 0, 0);

		/// <summary>
		/// Возвращает смещение оси Ox относительно верхнего края изображения
		/// </summary>
		public double OxOffset
			{
			get
				{
				return oxOffset;
				}
			}
		private double oxOffset = 0.0;

		/// <summary>
		/// Возвращает смещение оси Oy относительно левого края изображения
		/// </summary>
		public float OyOffset
			{
			get
				{
				return oyOffset;
				}
			}
		private float oyOffset = 0.0f;

		#endregion

		#region Параметры подписей

		/// <summary>
		/// Возвращает массив смещений подписей по оси Ox
		/// </summary>
		public List<float> TextX
			{
			get
				{
				return textX;
				}
			}
		private List<float> textX = new List<float> ();

		/// <summary>
		/// Возвращает массив смещений подписей по оси Oy
		/// </summary>
		public List<double> TextY
			{
			get
				{
				return textY;
				}
			}
		private List<double> textY = new List<double> ();

		/// <summary>
		/// Возвращает массив цветов подписей
		/// </summary>
		public List<Color> TextColors
			{
			get
				{
				return textColors;
				}
			}
		private List<Color> textColors = new List<Color> ();

		/// <summary>
		/// Возвращает массив шрифтов подписей
		/// </summary>
		public List<Font> TextFonts
			{
			get
				{
				return textFonts;
				}
			}
		private List<Font> textFonts = new List<Font> ();

		/// <summary>
		/// Возвращает массив текстов подписей
		/// </summary>
		public List<string> Texts
			{
			get
				{
				return texts;
				}
			}
		private List<string> texts = new List<string> ();

		#endregion

		/// <summary>
		/// Возвращает исходный скрипт получения изображения
		/// </summary>
		public List<string> SourceScript
			{
			get
				{
				return sourceScript;
				}
			}
		private List<string> sourceScript = new List<string> ();

		/// <summary>
		/// Конструктор. Считывает файл сценария и формирует схему построения изображения
		/// </summary>
		/// <param name="FileName">Имя файла сценария</param>
		public SVGScriptReader (string FileName)
			{
			// Переменные
			bool notIncluded = true;    // Флаг, указывающий, что сборка скрипта ещё не завершена
			FileStream FSI = null, FSO = null, FSInc = null;    // Дескрипторы файлов (входной, выходной, подключаемый)
			StreamReader SR = null, SRInc = null;
			StreamWriter SW = null;
			uint step = 1;              // Номер шага сборки
			uint includeDeep = 0;       // Количество подключенных файлов (для прерывания зацикливания)
			string fileNameI = FileName,    // Имена промежуточных файлов
				fileNameO = FileName + tmpExtension + step.ToString ();

			#region Сборка скрипта
			while (notIncluded)
				{
				notIncluded = false;

				// Открытие файлов
				try
					{
					FSI = new FileStream (fileNameI, FileMode.Open);
					}
				catch
					{
					initResult = InitResults.FileNotAvailable;
					return;
					}
				SR = new StreamReader (FSI, Encoding.GetEncoding (1251));

				try
					{
					FSO = new FileStream (fileNameO, FileMode.Create);
					}
				catch
					{
					SR.Close ();
					FSI.Close ();

					initResult = InitResults.FileNotAvailable;
					return;
					}
				SW = new StreamWriter (FSO, Encoding.GetEncoding (1251));

				// Запись
				while (!SR.EndOfStream)
					{
					string line = SR.ReadLine ();

					// Если подключения нет
					if (line.Trim () != "[Include]")
						{
						SW.WriteLine (line);
						}
					// В противном случае
					else
						{
						notIncluded = true;

						// Проверка на превышение лимита
						includeDeep++;
						if (includeDeep > maxIncludeDeep)
							{
							SR.Close ();
							FSI.Close ();
							SW.Close ();
							FSO.Close ();

							initResult = InitResults.IncludeDeepOverflow;
							return;
							}

						// Открытие включаемого файла
						line = SR.ReadLine ();
						try
							{
							FSInc = new FileStream (line, FileMode.Open);
							}
						catch
							{
							SR.Close ();
							FSI.Close ();
							SW.Close ();
							FSO.Close ();

							initResult = InitResults.CannotIncludeFile;
							faliedIncludeFile = line;
							return;
							}
						SRInc = new StreamReader (FSInc, Encoding.GetEncoding (1251));

						// Получение координат вставки
						line = SR.ReadLine ();
						string[] values = line.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
						float x = 0;
						double y = 0;
						try
							{
							x = float.Parse (values[0], cie.NumberFormat);
							y = double.Parse (values[1], cie.NumberFormat);
							}
						catch
							{
							faliedIncludeFile = FSInc.Name;
							SR.Close ();
							FSI.Close ();
							SW.Close ();
							FSO.Close ();
							SRInc.Close ();
							FSInc.Close ();

							initResult = InitResults.CannotIncludeFile;
							return;
							}

						// Запись
						SW.WriteLine ();
						SW.WriteLine ("{");
						SW.WriteLine (x.ToString (cie.NumberFormat) + ";" + y.ToString (cie.NumberFormat));
						while (!SRInc.EndOfStream)
							{
							SW.WriteLine (SRInc.ReadLine ());
							}
						SW.WriteLine ();
						SW.WriteLine ("}");
						SW.WriteLine ();

						// Завершение
						SRInc.Close ();
						FSInc.Close ();
						}
					}

				// Завершение
				SR.Close ();
				FSI.Close ();
				SW.Close ();
				FSO.Close ();

				// Смена текущего файла
				if (fileNameI.Contains (tmpExtension))
					{
					try
						{
						File.Delete (fileNameI);
						}
					catch
						{
						}
					}
				fileNameI = fileNameO;
				step++;
				fileNameO = FileName + tmpExtension + step.ToString ();
				}
			#endregion

			// Попытка открытия собранного файла
			try
				{
				FSI = new FileStream (fileNameI, FileMode.Open);
				}
			catch
				{
				initResult = InitResults.FileNotAvailable;
				return;
				}
			SR = new StreamReader (FSI, Encoding.GetEncoding (1251));

			// Создание накопителей смещений
			List<float> xOffset = new List<float> ();
			List<double> yOffset = new List<double> ();

			// Чтение секций
			while (!SR.EndOfStream)
				{
				string str = SR.ReadLine ();
				currentLine++;

				switch (str)
					{
					#region Оператор смещения поля отрисовки
					case "{":
						// Добавление смещения
						sourceScript.Add (str);     // Формирование исходного скрипта
						str = SR.ReadLine ();
						sourceScript.Add (str);
						currentLine++;

						string[] values = str.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
						try
							{
							xOffset.Add (float.Parse (values[0], cie.NumberFormat));
							yOffset.Add (double.Parse (values[1], cie.NumberFormat));
							}
						catch
							{
							throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (2)");
							}
						break;
					#endregion

					#region Оператор отмены последнего смещения
					case "}":
						sourceScript.Add (str);

						try
							{
							xOffset.RemoveAt (xOffset.Count - 1);
							yOffset.RemoveAt (yOffset.Count - 1);
							}
						catch
							{
							throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (3)");
							}
						break;
					#endregion

					#region Блок описания кривой
					case "[Line]":
						sourceScript.Add (str);

						// Вычисление смещений
						float xOffsetSum = ArraySum (xOffset);
						double yOffsetSum = ArraySum (yOffset);

						// Добавление кривой во все массивы
						linesX.Add (new List<float> ());
						linesY.Add (new List<double> ());
						linesColors.Add (Color.FromArgb (0, 0, 0));
						linesWidths.Add (1);

						// Заполнение
						while (((str = SR.ReadLine ()) != null) && (str.Trim () != ""))
							{
							sourceScript.Add (str);
							currentLine++;

							switch ((str + splitters[0].ToString () + splitters[0].ToString ()).Substring (0, 2))
								{
								// Цвет кривой
								case "c=":
									try
										{
										values = str.Substring (2).Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										linesColors[linesColors.Count - 1] = Color.FromArgb (int.Parse (values[0]),
											int.Parse (values[1]), int.Parse (values[2]));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenLineColor;
										return;
										}
									break;

								// Толщина кривой
								case "w=":
									try
										{
										linesWidths[linesWidths.Count - 1] = uint.Parse (str.Substring (2));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenLineWidth;
										return;
										}
									break;

								// Точки кривой
								default:
									try
										{
										values = str.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										linesX[linesX.Count - 1].Add (float.Parse (values[0], cie.NumberFormat) + xOffsetSum);
										// Пересчёт границ необходим для того, чтобы элементы не выходили за границы изображения
										if (minX > linesX[linesX.Count - 1][linesX[linesX.Count - 1].Count - 1])
											{
											minX = linesX[linesX.Count - 1][linesX[linesX.Count - 1].Count - 1];
											}
										if (maxX < linesX[linesX.Count - 1][linesX[linesX.Count - 1].Count - 1])
											{
											maxX = linesX[linesX.Count - 1][linesX[linesX.Count - 1].Count - 1];
											}
										linesY[linesY.Count - 1].Add (double.Parse (values[1], cie.NumberFormat) + yOffsetSum);
										if (minY > linesY[linesY.Count - 1][linesY[linesY.Count - 1].Count - 1])
											{
											minY = linesY[linesY.Count - 1][linesY[linesY.Count - 1].Count - 1];
											}
										if (maxY < linesY[linesY.Count - 1][linesY[linesY.Count - 1].Count - 1])
											{
											maxY = linesY[linesY.Count - 1][linesY[linesY.Count - 1].Count - 1];
											}
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenLinePoint;
										return;
										}
									break;
								}
							}

						sourceScript.Add ("");
						break;
					#endregion

					#region Блок описания оси Ox
					case "[Ox]":
						sourceScript.Add (str);

						// Вычисление смещений
						xOffsetSum = ArraySum (xOffset);
						yOffsetSum = ArraySum (yOffset);

						// Проверка на наличие предыдущих описаний
						if (oxInited)
							{
							break;
							}
						oxInited = true;

						// Заполнение
						while (((str = SR.ReadLine ()) != null) && (str.Trim () != ""))
							{
							sourceScript.Add (str);
							currentLine++;

							switch ((str + splitters[0].ToString () + splitters[0].ToString ()).Substring (0, 2))
								{
								// Смещение оси
								case "o=":
									try
										{
										oxOffset = uint.Parse (str.Substring (2)) + yOffsetSum;
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOxOffset;
										return;
										}
									break;

								// Цвет кривой
								case "c=":
									try
										{
										values = str.Substring (2).Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										oxColor = Color.FromArgb (int.Parse (values[0]), int.Parse (values[1]), int.Parse (values[2]));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOxColor;
										return;
										}
									break;

								// Толщина кривой
								case "w=":
									try
										{
										oxWidth = uint.Parse (str.Substring (2));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOxWidth;
										return;
										}
									break;

								// Засечки на оси
								default:
									try
										{
										values = str.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										oxNotchesOffsets.Add (float.Parse (values[0], cie.NumberFormat) + xOffsetSum);
										if (minX > oxNotchesOffsets[oxNotchesOffsets.Count - 1])
											{
											minX = oxNotchesOffsets[oxNotchesOffsets.Count - 1];
											}
										if (maxX < oxNotchesOffsets[oxNotchesOffsets.Count - 1])
											{
											maxX = oxNotchesOffsets[oxNotchesOffsets.Count - 1];
											}
										oxNotchesSizes.Add (double.Parse (values[1], cie.NumberFormat));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOxNotch;
										return;
										}
									break;
								}
							}

						sourceScript.Add ("");
						break;
					#endregion

					#region Блок описания оси Oy
					case "[Oy]":
						sourceScript.Add (str);

						// Вычисление смещений
						xOffsetSum = ArraySum (xOffset);
						yOffsetSum = ArraySum (yOffset);

						// Проверка на наличие предыдущих описаний
						if (oyInited)
							{
							break;
							}
						oyInited = true;

						// Заполнение
						while (((str = SR.ReadLine ()) != null) && (str.Trim () != ""))
							{
							sourceScript.Add (str);
							currentLine++;

							switch ((str + splitters[0].ToString () + splitters[0].ToString ()).Substring (0, 2))
								{
								// Смещение оси
								case "o=":
									try
										{
										oyOffset = uint.Parse (str.Substring (2)) + xOffsetSum;
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOyOffset;
										return;
										}
									break;

								// Цвет кривой
								case "c=":
									try
										{
										values = str.Substring (2).Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										oyColor = Color.FromArgb (int.Parse (values[0]), int.Parse (values[1]), int.Parse (values[2]));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOyColor;
										return;
										}
									break;

								// Толщина кривой
								case "w=":
									try
										{
										oyWidth = uint.Parse (str.Substring (2));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOyWidth;
										return;
										}
									break;

								// Засечки на оси
								default:
									try
										{
										values = str.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);
										oyNotchesOffsets.Add (double.Parse (values[0], cie.NumberFormat) + yOffsetSum);
										if (minY > oyNotchesOffsets[oyNotchesOffsets.Count - 1])
											{
											minY = (float)oyNotchesOffsets[oyNotchesOffsets.Count - 1];
											}
										if (maxY < oyNotchesOffsets[oyNotchesOffsets.Count - 1])
											{
											maxY = (float)oyNotchesOffsets[oyNotchesOffsets.Count - 1];
											}
										oyNotchesSizes.Add (float.Parse (values[1], cie.NumberFormat));
										}
									catch
										{
										SR.Close ();
										FSI.Close ();

										initResult = InitResults.BrokenOyNotch;
										return;
										}
									break;
								}
							}

						sourceScript.Add ("");
						break;
					#endregion

					#region Блок описания подписей
					case "[Text]":
						sourceScript.Add (str);

						// Вычисление смещений
						xOffsetSum = ArraySum (xOffset);
						yOffsetSum = ArraySum (yOffset);

						// Проверка на наличие предыдущих описаний (удалена в связи с возможным повторением
						// блока во включаемых файлах)
						/*if (textInited)
							{
							break;
							}
						textInited = true;*/

						// Заполнение
						while (((str = SR.ReadLine ()) != null) && (str.Trim () != ""))
							{
							sourceScript.Add (str);
							currentLine++;

							try
								{
								values = str.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);

								textX.Add (float.Parse (values[0], cie.NumberFormat) + xOffsetSum);
								if (minX > textX[textX.Count - 1])
									{
									minX = textX[textX.Count - 1];
									}
								if (maxX < textX[textX.Count - 1])
									{
									maxX = textX[textX.Count - 1];
									}

								textY.Add (double.Parse (values[1], cie.NumberFormat) + yOffsetSum);
								if (minY > textY[textY.Count - 1])
									{
									minY = textY[textY.Count - 1];
									}
								if (maxY < textY[textY.Count - 1])
									{
									maxY = textY[textY.Count - 1];
									}

								textColors.Add (Color.FromArgb (int.Parse (values[2]), int.Parse (values[3]), int.Parse (values[4])));
								textFonts.Add (new Font ("Arial", float.Parse (values[5]), FontStyle.Regular));

								// Текст
								texts.Add (str = SR.ReadLine ());
								sourceScript.Add (str);
								currentLine++;
								}
							catch
								{
								SR.Close ();
								FSI.Close ();

								initResult = InitResults.BrokenText;
								return;
								}
							}

						sourceScript.Add ("");
						break;
						#endregion
					}
				}

			// Считываение завершено
			SR.Close ();
			FSI.Close ();

			#region Корректировка изображения
			// Выравнивание значений на ноль по минимумам
			for (int l = 0; l < linesX.Count; l++)
				{
				for (int p = 0; p < linesX[l].Count; p++)
					{
					linesX[l][p] -= minX;
					linesY[l][p] -= minY;
					}
				}

			for (int n = 0; n < oxNotchesOffsets.Count; n++)
				{
				oxNotchesOffsets[n] -= minX;
				}
			for (int n = 0; n < oyNotchesOffsets.Count; n++)
				{
				oyNotchesOffsets[n] -= minY;
				}
			oxOffset -= minY;
			oyOffset -= minX;

			for (int t = 0; t < textX.Count; t++)
				{
				textX[t] -= minX;
				textY[t] -= minY;
				}

			// Пересчёт ограничений
			maxX -= minX;
			minX = 0;
			maxY -= minY;
			minY = 0;

			#endregion

			// Завершено
			try
				{
				File.Delete (fileNameI);
				}
			catch
				{
				}
			initResult = InitResults.Ok;
			}

		// Метод суммирует элементы массива
		private float ArraySum (List<float> Array)
			{
			float sum = 0.0f;
			for (int i = 0; i < Array.Count; i++)
				{
				sum += Array[i];
				}
			return sum;
			}

		private double ArraySum (List<double> Array)
			{
			double sum = 0.0;
			for (int i = 0; i < Array.Count; i++)
				{
				sum += Array[i];
				}
			return sum;
			}
		}
	}
