using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс-описатель данных диаграммы
	/// </summary>
	public class DiagramData
		{
		// ПЕРЕМЕННЫЕ
		private char[] anyDataSplitters = new char[] { ' ', '\t', ';' };    // Массивы сплиттеров
		private char[] anyHeadersSplitters = new char[] { '\t', ';' };
		private char[] csvSplitters = new char[] { ';' };
		private char[] dateSplitters = new char[] { '.', '/', '-' };
		private CultureInfo cie = new CultureInfo ("en-us");            // Десятичная точка
		private CultureInfo cir = new CultureInfo ("ru-ru");            // Десятичная запятая

		// Исходный массив данных
		private List<List<double>> dataValues = new List<List<double>> ();      
		
		// Имена столбцов исходного массива данных
		private List<string> dataColumnNames = new List<string> ();

#if !DataProcessingOnly

		// Массив кривых диаграммы
		private List<DiagramCurve> curves = new List<DiagramCurve> ();          

		// Массив сопоставленных стилей отображения
		private List<DiagramStyle> lineStyles = new List<DiagramStyle> ();

		// Массив дополнительных объектов
		private List<DiagramAdditionalObjects> additionalObjects = new List<DiagramAdditionalObjects> ();

		// Стили дополнительных объектов
		private List<DiagramStyle> additionalObjectsStyles = new List<DiagramStyle> ();

		// Правый отступ от края изображения до диаграммы (в долях)
		private const float RightMargin = LeftMargin + DiagramFieldPart;

		// Нижний отступ от края изображения до диаграммы (в долях)
		private const float BottomMargin = TopMargin + DiagramFieldPart;    
		
#endif

		// КОНСТАНТЫ
		private const float LeftMargin = 0.05f;     // Левый отступ от края изображения до диаграммы (в долях)
		private const float TopMargin = 0.01f;      // Верхний отступ от края изображения до диаграммы (в долях)
		private const float DiagramFieldPart = 0.9f;        // Доля поля диаграммы

		/// <summary>
		/// Максимально допустимое количество кривых на диаграмме
		/// </summary>
		public const uint MaxLines = 20;

		/// <summary>
		/// Максимально допустимое количество дополнительных объектов
		/// </summary>
		public const uint MaxAdditionalObjects = 50;

		/// <summary>
		/// Максимально допустимое количество столбцов данных
		/// </summary>
		public const uint MaxDataColumns = 100;

		/// <summary>
		/// Максимально допустимое количество строк данных
		/// </summary>
		public const uint MaxDataRows = 20001;

#if !DataProcessingOnly

		/// <summary>
		/// Метод возвращает ссылку на стиль указанной кривой или заданного объекта
		/// </summary>
		/// <param name="LineOrObjectNumber">Номер кривой или объекта, стиль которой или которого требуется вернуть</param>
		/// <returns>Запрошенный стиль или NULL, если номер кривой указан некорректно или список кривых пуст</returns>
		public DiagramStyle GetStyle (int LineOrObjectNumber)
			{
			if ((LineOrObjectNumber >= lineStyles.Count + additionalObjects.Count) || (LineOrObjectNumber < 0))
				{
				return null;
				}
			else if (LineOrObjectNumber < lineStyles.Count)
				{
				return lineStyles[LineOrObjectNumber];
				}
			else
				{
				return additionalObjectsStyles[LineOrObjectNumber - lineStyles.Count];
				}
			}

#endif

		/// <summary>
		/// Метод возвращает имя столбца исходных данных
		/// </summary>
		/// <param name="ColumnNumber">Номер столбца, имя которого требуется вернуть</param>
		/// <returns>Имя указанного столбца или NULL, если номер кривой указан некорректно, 
		/// или список кривых пуст</returns>
		public string GetDataColumnName (uint ColumnNumber)
			{
			if (ColumnNumber >= dataValues.Count)
				{
				return null;
				}
			else
				{
				return dataColumnNames[(int)ColumnNumber];
				}
			}

#if !DataProcessingOnly

		/// <summary>
		/// Метод возвращает тип дополнительного объекта
		/// </summary>
		/// <param name="ObjectNumber">Номер объекта, тип которого требуется вернуть</param>
		/// <returns>Тип объекта</returns>
		public DiagramAdditionalObjects GetObjectType (uint ObjectNumber)
			{
			if (ObjectNumber > additionalObjects.Count)
				return DiagramAdditionalObjects.LineH;

			return additionalObjects[(int)ObjectNumber];
			}

#endif

		/// <summary>
		/// Конструктор. Загружает данные из указанного файла.
		/// В том числе – GDD-файла, но в этом случае SkippedLinesCount игнорируется
		/// </summary>
		/// <param name="DataFileName">Путь к файлу данных</param>
		/// <param name="DataFileType">Тип файла данных</param>
		/// <param name="SkippedLinesCount">Количество пропускаемых первых строк файла</param>
		public DiagramData (string DataFileName, DataInputTypes DataFileType, uint SkippedLinesCount)
			{
			#region Выбор варианта загрузки

			switch (DataFileType)
				{
				// Следующие обработки выполняются в данном методе
				case DataInputTypes.CSV:
					break;

#if !DataProcessingOnly

				// Следующие обработки выполняются в дополнительных методах
				case DataInputTypes.XLS:
					// Контроль доступа к компонентам
					if (!File.Exists (RDGenerics.AppStartupPath + ProgramDescription.CriticalComponents_New[0]) ||
						!File.Exists (RDGenerics.AppStartupPath + ProgramDescription.CriticalComponents_New[1]) ||
						!File.Exists (RDGenerics.AppStartupPath + ProgramDescription.CriticalComponents_New[2]))
						{
						initResult = DiagramDataInitResults.ExcelNotAvailable;
						return;
						}

					// Вызов (контроль внутри функции невозможен, т.к. сразу при её вызове происходит контроль ссылок)
					LoadExcelFile (DataFileName, SkippedLinesCount);
					return;

				case DataInputTypes.GDD:
					LoadGDDFile (DataFileName);
					return;

#endif

				// Остальные файлы не подлежат обработке
				default:
					initResult = DiagramDataInitResults.BrokenFile;
					return;
				}
			#endregion

			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (DataFileName, FileMode.Open);
				}
			catch
				{
				initResult = DiagramDataInitResults.FileNotAvailable;
				return;
				}

			// Файл открыт
			StreamReader SR = new StreamReader (FS);

			// Создание вспомогательных переменных
			string s;
			string[] values = null;

			// Пропуск строк
			for (uint i = 0; i < SkippedLinesCount; i++)
				SR.ReadLine ();

			#region Определение размерности файла
			try
				{
				s = SR.ReadLine ();
				values = s.Split (csvSplitters, StringSplitOptions.RemoveEmptyEntries);

				if (values.Length < 2)
					{
					SR.Close ();
					FS.Close ();
					initResult = DiagramDataInitResults.NotEnoughData;
					return;
					}
				}
			catch
				{
				SR.Close ();
				FS.Close ();
				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}
			int dataColumnsCount = (values.Length > MaxDataColumns) ? (int)MaxDataColumns : values.Length;
			#endregion

			// Сброс указателя чтения в файле (потребуется повторный пропуск)
			FS.Seek (0, SeekOrigin.Begin);
			SR = new StreamReader (FS, Encoding.GetEncoding (1251));

			// Разметка массива
			for (int i = 0; i < dataColumnsCount; i++)
				dataValues.Add (new List<double> ());

			#region Извлечение имён столбцов (по возможности)

			bool namesFound = false;
			for (uint i = 0; i < SkippedLinesCount; i++)
				{
				// Считывание строки
				s = SR.ReadLine ();
				if (namesFound)
					{
					continue;
					}

				values = s.Split (csvSplitters, StringSplitOptions.RemoveEmptyEntries);

				// Сохранение имён строк
				if (values.Length == dataColumnsCount)
					{
					for (int j = 0; j < dataColumnsCount; j++)
						{
						// Разведение дублирующихся имён
						string dcn = values[j];
						while (dataColumnNames.Contains (dcn))
							{
							dcn += "1";
							}
						dataColumnNames.Add (dcn);
						}
					namesFound = true;
					}
				}

			// Контроль наличия имён
			if (!namesFound)
				{
				for (int j = 0; j < dataColumnsCount; j++)
					{
					dataColumnNames.Add ("c." + (j + 1).ToString ());
					}
				}

			#endregion

			#region Чтение всех значений по шаблону

			while (!SR.EndOfStream)
				{
				// Получение строки и разделение её на элементы; пропуск в случае пустого абзаца
				s = SR.ReadLine ();
				values = s.Split (csvSplitters, StringSplitOptions.RemoveEmptyEntries);

				if (values.Length == 0)
					continue;

				// Любое исключение здесь, включая IndexOutOfRange, будет означать нарушение в структуре файла
				for (int i = 0; i < dataColumnsCount; i++)
					{
					try
						{
						// Заполнение строки
						dataValues[i].Add (double.Parse (PrepareDataValue (values[i], true), cie.NumberFormat));
						}
					catch
						{
						// Разрешение на корректировку проблемных и недостающих значений
						dataValues[i].Add (0);
						}
					}

				// Контроль переполнения
				if (dataValues[0].Count > MaxDataRows)
					break;
				}

			// Чтение завершено
			SR.Close ();
			FS.Close ();

			#endregion

			// Контроль количества строк
			if (dataValues[0].Count < 2)
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Успешно
			initResult = DiagramDataInitResults.Ok;
			}

		/// <summary>
		/// Конструктор. Извлекает данные из файла
		/// </summary>
		/// <param name="DataFileName">Путь к файлу</param>
		/// <param name="ColumnsCount">Ожидаемое количество столбцов данных</param>
		/// <param name="SkippedLinesCount">Количество пропускаемых первых строк файла</param>
		public DiagramData (string DataFileName, uint ColumnsCount, uint SkippedLinesCount)
			{
			LoadRawText (false, DataFileName, ColumnsCount, SkippedLinesCount);
			}

		/// <summary>
		/// Конструктор. Извлекает данные из буфера обмена
		/// </summary>
		/// <param name="ColumnsCount">Ожидаемое количество столбцов данных</param>
		/// <param name="SkippedLinesCount">Количество пропускаемых первых строк файла</param>
		public DiagramData (uint ColumnsCount, uint SkippedLinesCount)
			{
			LoadRawText (true, null, ColumnsCount, SkippedLinesCount);
			}

		// Метод загружает данные диаграммы из любого текста, передаваемого из файла или буфера обмена
		private void LoadRawText (bool FromClipboard, string DataFileName, uint ColumnsCount, 
			uint SkippedLinesCount)
			{
			// Контроль значений
			if ((ColumnsCount < 2) || (ColumnsCount > MaxDataColumns) || !FromClipboard && (DataFileName == null))
				throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (1)");

			// Попытка открытия файла
			FileStream FS = null;
			TextReader SR = null;

			if (!FromClipboard)
				{
				try
					{
					FS = new FileStream (DataFileName, FileMode.Open);
					}
				catch
					{
					initResult = DiagramDataInitResults.FileNotAvailable;
					return;
					}
				SR = new StreamReader (FS, Encoding.GetEncoding (1251));
				}
			else
				{
				if (Clipboard.ContainsText ())
					{
					SR = new StringReader (Clipboard.GetText (TextDataFormat.Text));
					}
				else
					{
					return;
					}
				}

			// Разметка массива
			for (uint i = 0; i < ColumnsCount; i++)
				dataValues.Add (new List<double> ());

			// Создание вспомогательных переменных
			string s;                   // Промежуточные строки
			string[] values = null;     // Извлечённые значения

			#region Извлечение имён столбцов (по возможности)
			bool namesFound = false;
			for (uint i = 0; i < SkippedLinesCount; i++)
				{
				// Считывание строки
				s = SR.ReadLine ();
				if (namesFound)
					{
					continue;
					}

				// Определение сплиттера и разделение
				// (при извлечении имён используется простая техника с известными разделителями)
				values = s.Split (anyHeadersSplitters, StringSplitOptions.RemoveEmptyEntries);

				// Сохранение имён строк
				if (values.Length == ColumnsCount)
					{
					for (int j = 0; (j < ColumnsCount) && (j < values.Length); j++)
						{
						string dcn = values[j];
						while (dataColumnNames.Contains (dcn))
							{
							dcn += "1";
							}
						dataColumnNames.Add (dcn);
						}
					namesFound = true;
					}
				}

			// Контроль количества имён
			for (int j = dataColumnNames.Count; j < ColumnsCount; j++)
				{
				dataColumnNames.Add ("c." + (j + 1).ToString ());
				}
			#endregion

			#region Чтение значений

			while (SR.Peek () > -1)
				{
				// Разделение и контроль длины
				values = PrepareDataLine (SR.ReadLine (), false).Split (anyDataSplitters, StringSplitOptions.RemoveEmptyEntries);
				if (values.Length == 0)
					continue;

				// Заполнение строки
				for (int i = 0; i < ColumnsCount; i++)
					{
					// Прерывание на случай недостаточного числа элементов в строке
					// (исключает лишние срабатывания переключателя culture)
					if (i >= values.Length)
						{
						dataValues[i].Add (0.0);
						continue;
						}

					// Попытка извлечения значения с текущим десятичным разделителем
					double parsed = 0.0;
					double.TryParse (PrepareDataValue (values[i], false), NumberStyles.Float, cie.NumberFormat, out parsed);
					dataValues[i].Add (parsed);
					}

				// Контроль переполнения
				if (dataValues[0].Count > MaxDataRows)
					break;
				}

			// Чтение завершено
			SR.Close ();
			if (!FromClipboard)
				FS.Close ();

			#endregion

			// Контроль количества строк
			if (dataValues[0].Count < 2)
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Успешно
			initResult = DiagramDataInitResults.Ok;
			}

		/// <summary>
		/// Конструктор. Загружает данные из таблицы данных.
		/// Предназначен для внутреннего использования
		/// </summary>
		/// <param name="Table">Таблица в формате базы данных</param>
		internal DiagramData (DataTable Table)
			{
			// Контроль
			if (Table == null)
				throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (3)");

			// Вызов функции загрузки
			LoadDataFromTable (Table, 0);
			}

		/// <summary>
		/// Конструктор. Загружает данные из массивов, переданных генератором кривых.
		/// Предназначен для внутреннего использования
		/// </summary>
		/// <param name="X">Массив абсцисс кривых</param>
		/// <param name="Y">Массив ординат кривых</param>
		/// <param name="ColumnsNames">Нащвания столбцов данных</param>
		internal DiagramData (List<double> X, List<List<double>> Y, List<string> ColumnsNames)
			{
			// Контроль
			if ((X == null) || (Y == null) || (X.Count == 0))
				throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (4)");

			if ((X.Count < 2) || (Y.Count != ColumnsNames.Count))
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Заполнение
			for (int c = 0; c < Y.Count; c++)
				{
				// Контроль
				if (X.Count != Y[c].Count)
					throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (5)");

				// Создание структуры
				if (c == 0)
					{
					dataValues.Add (new List<double> ());
					dataColumnNames.Add ("x");
					}
				dataValues.Add (new List<double> ());
				dataColumnNames.Add (ColumnsNames[c]);

				// Интерпретация данных
				for (int r = 0; r < X.Count; r++)
					{
					if (c == 0)
						dataValues[c].Add (X[r]);

					dataValues[c + 1].Add (Y[c][r]);
					}
				}

			// Успешно
			initResult = DiagramDataInitResults.Ok;
			}

		/// <summary>
		/// Конструктор. Загружает данные из массива, сформированного путём слияния таблиц данных.
		/// Предназначен для внутреннего использования
		/// </summary>
		/// <param name="ColumnNames">Имена столбцов данных</param>
		/// <param name="DataTable">Таблица значений</param>
		internal DiagramData (List<List<double>> DataTable, List<string> ColumnNames)
			{
			// Контроль
			if ((DataTable == null) || (ColumnNames == null))
				throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (6)");

			if ((DataTable.Count < 2) || (ColumnNames.Count < 2))
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Заполнение имён столбцов
			dataColumnNames.AddRange (ColumnNames);

			// Заполнение данных
			for (int r = 0; r < DataTable.Count; r++)
				{
				// Контроль
				if (DataTable[r].Count != ColumnNames.Count)
					throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (7)");

				for (int c = 0; c < DataTable[r].Count; c++)
					{
					if (r == 0)
						{
						dataValues.Add (new List<double> ());
						}
					dataValues[c].Add (DataTable[r][c]);
					}
				}

			// Успешно
			initResult = DiagramDataInitResults.Ok;
			}

#if !DataProcessingOnly

		// Метод загружает файл в формате GDD
		private void LoadGDDFile (string DataFileName)
			{
			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (DataFileName, FileMode.Open);
				}
			catch
				{
				initResult = DiagramDataInitResults.FileNotAvailable;
				return;
				}

			// Файл открыт
			BinaryReader BR = new BinaryReader (FS, Encoding.GetEncoding (1251));

			try
				{
				BR.ReadString ();   // Версия файла
				}
			catch
				{
				BR.Close ();
				FS.Close ();
				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}

			#region Чтение блока данных
			// Получение размерности файла
			uint rows = 0;
			uint dataColumnsCount = 0;
			try
				{
				if ((rows = BR.ReadUInt16 ()) > MaxDataRows)
					throw new Exception (); // Значит, файл повреждён (не NotEnoughData)
				if ((dataColumnsCount = BR.ReadUInt16 ()) > MaxDataColumns)
					throw new Exception ();


				// Контроль
				if ((rows < 2) || (dataColumnsCount < 2))   // Программа так делать не умеет
					throw new Exception ();

				// Разметка массива и чтение имён столбцов
				for (int i = 0; i < dataColumnsCount; i++)
					{
					dataValues.Add (new List<double> ());

					dataColumnNames.Add (BR.ReadString ());
					}
				}
			catch
				{
				BR.Close ();
				FS.Close ();
				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}

			// Чтение основного массива данных
			for (int i = 0; i < rows; i++)
				{
				// Любое исключение здесь, включая IndexOutOfRange (некорректное число элементов, пустая строка, конец файла), 
				// будет означать нарушение в структуре файла
				try
					{
					for (int j = 0; j < dataColumnsCount; j++)
						{
						dataValues[j].Add (BR.ReadDouble ());
						}
					}
				catch
					{
					BR.Close ();
					FS.Close ();
					initResult = DiagramDataInitResults.BrokenFile;
					return;
					}
				}

			// Предположительно успешная загрузка (необходима имитация для корректной работы методов добавления)
			initResult = DiagramDataInitResults.Ok;
			#endregion

			#region Чтение данных о кривых
			// Получение числа кривых
			uint linesCount = 0;
			try
				{
				if ((linesCount = BR.ReadUInt16 ()) > MaxLines)
					throw new Exception ();
				}
			catch
				{
				BR.Close ();
				FS.Close ();
				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}
			#endregion

			#region Чтение стилей
			for (int i = 0; i < linesCount; i++)
				{
				DiagramStyle style = null;
				if (!EjectStyle (BR, out style) || (AddDiagram (style.XColumnNumber, style.YColumnNumber) < 0))
					{
					BR.Close ();
					FS.Close ();
					initResult = DiagramDataInitResults.BrokenFile;
					return;
					}
				lineStyles[i] = new DiagramStyle (style);
				}
			#endregion

			#region Чтение данных о дополнительных объектах
			// Получение числа объектов
			uint additionalObjectsCount = 0;
			try
				{
				additionalObjectsCount = BR.ReadUInt16 ();
				}
			catch
				{
				// Игнорировать ошибку на случай открытия файла версии старше 4.4
				}
			if (additionalObjectsCount > MaxAdditionalObjects)
				{
				additionalObjectsCount = MaxAdditionalObjects;
				}
			#endregion

			#region Чтение стилей дополнительных объектов
			// Количество объектов останется равным нулю для старой версии файла
			for (int i = 0; i < additionalObjectsCount; i++)
				{
				// Чтение типа объекта
				try
					{
					DiagramAdditionalObjects dao = (DiagramAdditionalObjects)BR.ReadUInt16 ();
					if (dao == DiagramAdditionalObjects.OldLine)    // Старая линия заменяется на ближайшую по смыслу
						{
						dao = DiagramAdditionalObjects.LineNWtoSE;
						}
					additionalObjects.Add (dao);
					}
				catch
					{
					BR.Close ();
					FS.Close ();
					initResult = DiagramDataInitResults.BrokenFile;
					return;
					}

				// Чтение стиля объекта
				DiagramStyle style = null;
				if (!EjectStyle (BR, out style))
					{
					BR.Close ();
					FS.Close ();
					initResult = DiagramDataInitResults.BrokenFile;
					return;
					}
				additionalObjectsStyles.Add (style);
				}
			#endregion

			// Чтение успешно завершено
			BR.Close ();
			FS.Close ();
			}

#endif

		// Метод загружает файл в формате Excel
		private void LoadExcelFile (string DataFileName, uint SkippedLinesCount)
			{
			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (DataFileName, FileMode.Open);
				}
			catch
				{
				initResult = DiagramDataInitResults.FileNotAvailable;
				return;
				}

			// Чтение данных
			ExcelDataReader.IExcelDataReader excelReader = null;
			DataTable table = null;
			try
				{
				if (DataFileName.Contains ("xlsx"))
					{
					excelReader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader (FS);
					}
				else
					{
					excelReader = ExcelDataReader.ExcelReaderFactory.CreateBinaryReader (FS);
					}

				DataSet dataSet = ExcelDataReader.ExcelDataReaderExtensions.AsDataSet (excelReader, null);
				table = dataSet.Tables[0];
				}
			catch
				{
				if (excelReader != null)
					excelReader.Close ();
				FS.Close ();

				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}

			// Успешно
			excelReader.Close ();
			FS.Close ();

			// Загрузка данных
			LoadDataFromTable (table, SkippedLinesCount);
			}

		// Подметод загружает данные из объекта типа DataTable
		private void LoadDataFromTable (DataTable table, uint SkippedLinesCount)
			{
			// Контроль количества столбцов
			if (table.Columns.Count < 2)
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Интерпретация данных
			for (int c = 0; c < table.Columns.Count; c++)
				{
				dataValues.Add (new List<double> ());
				if (dataValues.Count >= MaxDataColumns)
					{
					break;
					}
				}

			try
				{
				// Поиск имён столбцов
				for (uint r = 0; r < SkippedLinesCount; r++)
					{
					// Если строка не пуста
					if (table.Rows[(int)r].ItemArray[0].ToString ().Trim () != "")
						{
						// Использовать её для именования столбцов
						for (int c = 0; c < dataValues.Count; c++)
							{
							string dcn = table.Rows[(int)r].ItemArray[c].ToString ();
							while (dataColumnNames.Contains (dcn))
								{
								dcn += "1";
								}
							dataColumnNames.Add (dcn);

							// Если ячейка пуста, заменить её значение на стандартное
							if (dataColumnNames[dataColumnNames.Count - 1].Trim () == "")
								{
								dataColumnNames[dataColumnNames.Count - 1] = "c." + dataColumnNames.Count.ToString ();
								}
							}

						// Имена найдены
						break;
						}
					}

				// Если имена не найдены
				if (dataColumnNames.Count == 0)
					{
					for (int c = 0; c < dataValues.Count; c++)
						{
						dataColumnNames.Add (table.Columns[c].Caption);
						}
					}
				}
			catch
				{
				initResult = DiagramDataInitResults.BrokenFile;
				return;
				}

			// Чтение данных
			for (uint r = SkippedLinesCount; (r < table.Rows.Count) && (r < MaxDataRows); r++)
				{
				for (int c = 0; c < dataValues.Count; c++)
					{
					try
						{
						dataValues[c].Add (double.Parse (PrepareDataValue (table.Rows[(int)r].ItemArray[c].ToString (), true),
							cie.NumberFormat));
						}
					catch
						{
						dataValues[c].Add (0);
						}
					}
				}

			// Контроль количества строк
			if (dataValues[0].Count < 2)
				{
				initResult = DiagramDataInitResults.NotEnoughData;
				return;
				}

			// Успешно
			initResult = DiagramDataInitResults.Ok;
			}

		/// <summary>
		/// Возвращает количество столбцов в загруженном массиве данных
		/// </summary>
		public uint DataColumnsCount
			{
			get
				{
				return (uint)dataValues.Count;
				}
			}

#if !DataProcessingOnly

		/// <summary>
		/// Метод добавляет новую кривую в список сформированных кривых, используя загруженные данные
		/// </summary>
		/// <param name="XColumnNumber">Номер столбца в списке исходных данных, интерпретируемый 
		/// как столбец абсцисс</param>
		/// <param name="YColumnNumber">Номер столбца в списке исходных данных, интерпретируемый 
		/// как столбец ординат</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны;
		/// -3, если на диаграмме присутствует максимально допустимое количество кривых</returns>
		public int AddDiagram (uint XColumnNumber, uint YColumnNumber)
			{
			return AddLineData (XColumnNumber, YColumnNumber, -1);
			}

		/// <summary>
		/// Метод добавляет новый дополнительный объект на диаграмму
		/// </summary>
		/// <param name="ObjectType">Тип дополнительного объекта</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован;
		/// -3, если на диаграмме присутствует максимально допустимое количество кривых</returns>
		public int AddObject (DiagramAdditionalObjects ObjectType)
			{
			// Ограничение на количество добавляемых объектов
			if (additionalObjects.Count >= MaxAdditionalObjects)
				{
				return -3;
				}

			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			// Добавление в массив
			string type = "";
			switch (ObjectType)
				{
				case DiagramAdditionalObjects.Ellipse:
				case DiagramAdditionalObjects.FilledEllipse:
					type = "Ell. ";
					break;

				case DiagramAdditionalObjects.FilledRectangle:
				case DiagramAdditionalObjects.Rectangle:
					type = "R. ";
					break;

				case DiagramAdditionalObjects.LineH:
				case DiagramAdditionalObjects.LineV:
				case DiagramAdditionalObjects.LineNWtoSE:
				case DiagramAdditionalObjects.LineSWtoNE:
				case DiagramAdditionalObjects.OldLine:      // Изоляция исключений
					type = "L. ";
					break;

				case DiagramAdditionalObjects.Text:
				default:
					type = "T. ";
					break;
				}

			if (ObjectType == DiagramAdditionalObjects.OldLine)
				{
				additionalObjects.Add (DiagramAdditionalObjects.LineNWtoSE);    // Защита от возможных дефектов интерфейса
				}
			else
				{
				additionalObjects.Add (ObjectType);
				}
			additionalObjectsStyles.Add (new DiagramStyle ((uint)additionalObjects.Count - 1,
				type + additionalObjects.Count.ToString (), 0, 0));

			// Завершено
			return 0;
			}

		/// <summary>
		/// Метод заменяет данные имеющейся кривой, сохраняя настройки стиля
		/// </summary>
		/// <param name="LineToReplace">Кривая, данные которой следует заменить</param>
		/// <param name="XColumnNumber">Номер столбца в списке исходных данных, интерпретируемый
		/// как столбец абсцисс</param>
		/// <param name="YColumnNumber">Номер столбца в списке исходных данных, интерпретируемый 
		/// как столбец ординат</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int ReplaceDiagram (uint LineToReplace, uint XColumnNumber, uint YColumnNumber)
			{
			return AddLineData (XColumnNumber, YColumnNumber, (int)LineToReplace);
			}

		// Метод добавляет данные кривой в список кривых или заменяет данные имеющейся кривой
		private int AddLineData (uint XColumnNumber, uint YColumnNumber, int LineToReplace)
			{
			// Определение номера добавляемого/замещаемого столбца
			int currentLine = curves.Count;
			bool replace = false;
			if ((LineToReplace >= 0) && (LineToReplace < curves.Count))
			// Замена
				{
				currentLine = LineToReplace;
				replace = true;
				}
			else
			// Добавление
				{
				// Ограничение на количество добавляемых кривых
				if (curves.Count >= MaxLines)
					{
					return -3;
					}
				}

			// Промежуточные границы построения
			double minX = double.NegativeInfinity;
			double maxX = double.PositiveInfinity;
			double minY = double.NegativeInfinity;
			double maxY = double.PositiveInfinity;

			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if ((XColumnNumber >= dataValues.Count) || (YColumnNumber >= dataValues.Count))
				{
				return -2;
				}

			// Обработка столбцов
			for (int row = 0; row < dataValues[0].Count; row++)
				{
				// Поиск максимумов и минимумов по оси Oy
				if (!double.IsNaN (dataValues[(int)YColumnNumber][row]))    // Создаёт потенциальную опасность выхода на max|min = Infinity
					{                                                       // Но пока что хрен с ним
					if (minY == double.NegativeInfinity)
						{
						minY = maxY = dataValues[(int)YColumnNumber][row];
						}
					else if (minY > dataValues[(int)YColumnNumber][row])
						{
						minY = dataValues[(int)YColumnNumber][row];
						}
					else if (maxY < dataValues[(int)YColumnNumber][row])
						{
						maxY = dataValues[(int)YColumnNumber][row];
						}
					}

				// Поиск максимумов и минимумов по оси Ox
				if (!double.IsNaN (dataValues[(int)XColumnNumber][row]))
					{
					if (minX == double.NegativeInfinity)
						{
						minX = maxX = dataValues[(int)XColumnNumber][row];
						}
					else if (minX > dataValues[(int)XColumnNumber][row])
						{
						minX = dataValues[(int)XColumnNumber][row];
						}
					else if (maxX < dataValues[(int)XColumnNumber][row])
						{
						maxX = dataValues[(int)XColumnNumber][row];
						}
					}
				}

			// Пересчёт максимума и минимума по осям и расчёт округлений
			int xRoundPos = 0, yRoundPos = 0;
			RecalculateMaxMin (minX, maxX, minY, maxY, out minX, out maxX, out minY, out maxY,
				out xRoundPos, out yRoundPos);

			// Добавление в массив служебных параметров
			if (replace)
				{
				curves[currentLine].SetValues (minX, maxX, minY, maxY, xRoundPos, yRoundPos);
				}
			else
				{
				curves.Add (new DiagramCurve (minX, maxX, minY, maxY, xRoundPos, yRoundPos));
				}

			// Добавление стиля или замена параметров стиля
			if (replace)
				{
				lineStyles[currentLine].XColumnNumber = XColumnNumber;
				lineStyles[currentLine].YColumnNumber = YColumnNumber;
				}
			else
				{
				lineStyles.Add (new DiagramStyle ((uint)curves.Count - 1, dataColumnNames[(int)YColumnNumber] + " (" +
					dataColumnNames[(int)XColumnNumber] + ")", XColumnNumber, YColumnNumber));
				}

			// Установка диапазонов построения
			lineStyles[currentLine].MinX = minX;
			lineStyles[currentLine].MaxX = maxX;
			lineStyles[currentLine].MinY = minY;
			lineStyles[currentLine].MaxY = maxY;

			// Завершено
			return 0;
			}

		/// <summary>
		/// Метод удаляет указанную кривую из диаграммы
		/// </summary>
		/// <param name="LineNumber">Номер кривой в списке сформированных кривых</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int DeleteDiagram (uint LineNumber)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if (LineNumber >= curves.Count)
				{
				return -2;
				}

			// Удаление
			curves.RemoveAt ((int)LineNumber);
			lineStyles.RemoveAt ((int)LineNumber);

			// Завершено
			return 0;
			}

		/// <summary>
		/// Метод удаляет указанный объект из диаграммы
		/// </summary>
		/// <param name="ObjectNumber">Номер объекта</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int DeleteObject (uint ObjectNumber)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if (ObjectNumber >= additionalObjects.Count)
				{
				return -2;
				}

			// Удаление
			additionalObjects.RemoveAt ((int)ObjectNumber);
			additionalObjectsStyles.RemoveAt ((int)ObjectNumber);

			// Завершено
			return 0;
			}

		// Метод пересчитывает максимумы и минимумы диапазонов с округлением до первой ненулевой цифры слева
		private void RecalculateMaxMin (double OldMinX, double OldMaxX, double OldMinY, double OldMaxY,
			out double NewMinX, out double NewMaxX, out double NewMinY, out double NewMaxY,
			out int XRoundPosition, out int YRoundPosition)
			{
			// Пересчёт максимума и минимума по оси Oy
			for (YRoundPosition = -10; YRoundPosition < 10; YRoundPosition++)
				{
				if (((int)(OldMaxY / Math.Pow (10.0, (double)YRoundPosition)) == 0) &&
					((int)(OldMinY / Math.Pow (10.0, (double)YRoundPosition)) == 0))
					{
					YRoundPosition--;
					break;
					}
				}

			NewMaxY = Math.Ceiling (OldMaxY / Math.Pow (10.0, (double)YRoundPosition)) * Math.Pow (10.0, (double)YRoundPosition);
			NewMinY = Math.Floor (OldMinY / Math.Pow (10.0, (double)YRoundPosition)) * Math.Pow (10.0, (double)YRoundPosition);

			// Пересчёт максимума и минимума по оси Ox
			for (XRoundPosition = -10; XRoundPosition < 10; XRoundPosition++)
				{
				if (((int)(OldMaxX / Math.Pow (10.0, (double)XRoundPosition)) == 0) &&
					((int)(OldMinX / Math.Pow (10.0, (double)XRoundPosition)) == 0))
					{
					XRoundPosition--;
					break;
					}
				}

			NewMaxX = Math.Ceiling (OldMaxX / Math.Pow (10.0, (double)XRoundPosition)) * Math.Pow (10.0, (double)XRoundPosition);
			NewMinX = Math.Floor (OldMinX / Math.Pow (10.0, (double)XRoundPosition)) * Math.Pow (10.0, (double)XRoundPosition);
			}

		/// <summary>
		/// Возвращает количество сформированных кривых
		/// </summary>
		public uint LinesCount
			{
			get
				{
				return (uint)curves.Count;
				}
			}

		/// <summary>
		/// Возвращает количество дополнительных объектов
		/// </summary>
		public uint AdditionalObjectsCount
			{
			get
				{
				return (uint)additionalObjects.Count;
				}
			}

		/// <summary>
		/// Метод возвращает строковое представление указанного столбца данных
		/// </summary>
		/// <param name="ColumnNumber">Номер столбца в массиве данных</param>
		/// <returns>Строковое представление столбца (или пустая строка при некорректно заданных 
		/// параметрах)</returns>
		public string ColumnPresentation (uint ColumnNumber)
			{
			if (ColumnNumber < dataValues.Count)
				{
				return (dataColumnNames[(int)ColumnNumber] + ": " + dataValues[(int)ColumnNumber][0].ToString (cir.NumberFormat) +
					((dataValues[0].Count > 1) ? ("; " + dataValues[(int)ColumnNumber][1].ToString (cir.NumberFormat)) : "") +
					((dataValues[0].Count > 2) ? ("; " + dataValues[(int)ColumnNumber][2].ToString (cir.NumberFormat)) : "") +
					((dataValues[0].Count > 3) ? ("; " + dataValues[(int)ColumnNumber][3].ToString (cir.NumberFormat)) : "") +
					"; ...");
				}
			else
				{
				return "";
				}
			}

		/// <summary>
		/// Возвращает позицию цифры в значениях максимума и минимума по оси Ox указанной кривой,
		/// до которой следует выполнить округление
		/// </summary>
		/// <param name="LineNumber">Номер кривой в списке</param>
		/// <returns>Позиция округления (или 0 при некорректно заданных параметрах)</returns>
		public int XRoundPos (uint LineNumber)
			{
			if (LineNumber < curves.Count)
				{
				return curves[(int)LineNumber].XRoundPosition;
				}
			else
				{
				return 3;   // Чтобы интерфейс не добавлял знаки после запятой, нужные диаграммам
				}
			}

		/// <summary>
		/// Возвращает позицию цифры в значениях максимума и минимума по оси Oy указанной кривой,
		/// до которой следует выполнить округление
		/// </summary>
		/// <param name="LineNumber">Номер кривой в списке</param>
		/// <returns>Позиция округления (или 0 при некорректно заданных параметрах)</returns>
		public int YRoundPos (uint LineNumber)
			{
			if (LineNumber < curves.Count)
				{
				return curves[(int)LineNumber].YRoundPosition;
				}
			else
				{
				return 3;   // Чтобы интерфейс не добавлял знаки после запятой, нужные диаграммам
				}
			}

		/// <summary>
		/// Возвращает ширину полного изображения диаграммы
		/// </summary>
		public uint DiagramWidth
			{
			get
				{
				uint res = DiagramStyle.MinImageWidth;

				// Поиск границы (непревышение максимально допустимой границы обеспечивается структурой
				// свойств .DiagramImage_ЧтоНибудь_Offset)
				for (int i = 0; i < lineStyles.Count; i++)
					{
					if (res < lineStyles[i].DiagramImageLeftOffset + lineStyles[i].DiagramImageWidth)
						{
						res = lineStyles[i].DiagramImageLeftOffset + lineStyles[i].DiagramImageWidth;
						}
					}

				Bitmap b = new Bitmap (10, 10);
				Graphics g = Graphics.FromImage (b);
				for (int i = 0; i < additionalObjectsStyles.Count; i++)
					{
					// В случае текстовой строки нуждается в пересчёте
					if (additionalObjects[i] == DiagramAdditionalObjects.Text)
						{
						SizeF sz = g.MeasureString (additionalObjectsStyles[i].LineName, additionalObjectsStyles[i].TextFont);
						additionalObjectsStyles[i].DiagramImageWidth = (uint)sz.Width;
						}

					// Стандарт
					if (res < additionalObjectsStyles[i].DiagramImageLeftOffset + additionalObjectsStyles[i].DiagramImageWidth)
						{
						res = additionalObjectsStyles[i].DiagramImageLeftOffset + additionalObjectsStyles[i].DiagramImageWidth;
						}
					}
				g.Dispose ();
				b.Dispose ();

				return res;
				}
			}

		/// <summary>
		/// Возвращает высоту полного изображения диаграммы
		/// </summary>
		public uint DiagramHeight
			{
			get
				{
				uint res = DiagramStyle.MinImageHeight;

				// Поиск границы
				for (int i = 0; i < lineStyles.Count; i++)
					{
					if (res < lineStyles[i].DiagramImageTopOffset + lineStyles[i].DiagramImageHeight)
						{
						res = lineStyles[i].DiagramImageTopOffset + lineStyles[i].DiagramImageHeight;
						}
					}

				Bitmap b = new Bitmap (10, 10);
				Graphics g = Graphics.FromImage (b);
				for (int i = 0; i < additionalObjectsStyles.Count; i++)
					{
					// В случае текстовой строки нуждается в пересчёте
					if (additionalObjects[i] == DiagramAdditionalObjects.Text)
						{
						SizeF sz = g.MeasureString (additionalObjectsStyles[i].LineName, additionalObjectsStyles[i].TextFont);
						additionalObjectsStyles[i].DiagramImageHeight = (uint)sz.Height;
						}

					// Стандарт
					if (res < additionalObjectsStyles[i].DiagramImageTopOffset + additionalObjectsStyles[i].DiagramImageHeight)
						{
						res = additionalObjectsStyles[i].DiagramImageTopOffset + additionalObjectsStyles[i].DiagramImageHeight;
						}
					}
				g.Dispose ();
				b.Dispose ();

				return res;
				}
			}

#endif

		/// <summary>
		/// Возвращает результат инициализации класса
		/// </summary>
		public DiagramDataInitResults InitResult
			{
			get
				{
				return initResult;
				}
			}
		private DiagramDataInitResults initResult = DiagramDataInitResults.NotInited;

#if !DataProcessingOnly

		// Метод формирует изображение одной кривой
		private void DrawDiagram (int LineNumber, DiagramStyle LineStyle, Graphics DrawField, 
			bool IsSelected)
			{
			// Создание пустого изображения заданного размера с белым фоном
			Bitmap lineImage = new Bitmap ((int)LineStyle.DiagramImageWidth, (int)LineStyle.DiagramImageHeight);    // Будущее изображение
			Graphics g = Graphics.FromImage (lineImage);    // Графический дескриптор
			g.Clear (DiagramStyle.ImageBackColor);

			// Установка отрисовки подписей без сглаживания (устраняет чёрную ауру при отрисовке подписей в Win8)
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

			// Получение графического дескриптора фрагмента изображения для отрисовки кривых (автообрезка)
			Graphics g2 = Graphics.FromImage (lineImage);
			g2.Clip = new Region (new Rectangle ((int)((float)LineStyle.DiagramImageWidth * LeftMargin),
				(int)((float)LineStyle.DiagramImageHeight * TopMargin), (int)((float)(LineStyle.DiagramImageWidth + 2) * DiagramFieldPart),
				(int)((float)(LineStyle.DiagramImageHeight + 2) * DiagramFieldPart)));

			// Создание переменных
			float x1, x2;   // Вспомогательные переменные для отрисовки линий
			float y1, y2;
			double ptx1 = 0.0, ptx2 = 0.0;
			double pty1 = 0.0, pty2 = 0.0;
			float ox = 0.0f, oy = 0.0f; // Точки фиксации осей
			Brush br = null;        // Кисть для заливок
			Pen p = null;           // Карандаш для линий
			SizeF sz;       // Переменная для расчёта центральных позиций для подписей
			float NotchSize;    // Вычисляемый размер засечек
			double styleMinX, styleMaxX;            // Вспомогательные переменные для транспонирования диаграмм
			double styleMinY, styleMaxY;
			uint styleXPrimaryDiv, styleYPrimaryDiv,
				styleXSecondaryDiv, styleYSecondaryDiv;
			string txt;

			// Получение текущего маркера
			MarkersLoader ml = new MarkersLoader ();
			Bitmap currentMarker = ml.GetMarker (LineStyle.LineMarkerNumber - 1, LineStyle.LineColor);
			ml.Dispose ();

			// Отрисовка рамки, если кривая выделена на диаграмме
			if (IsSelected)
				{
				br = new SolidBrush (Color.FromArgb (16, 0, 0, 0));
				g.FillRectangle (br, 0, 0, LineStyle.DiagramImageWidth, LineStyle.DiagramImageHeight);
				br.Dispose ();
				}

			#region Транспонирование диапазонов при необходимости
			if (!LineStyle.SwitchXY)
				{
				styleMaxX = LineStyle.MaxX;
				styleMaxY = LineStyle.MaxY;
				styleMinX = LineStyle.MinX;
				styleMinY = LineStyle.MinY;
				styleXPrimaryDiv = LineStyle.XPrimaryDivisions;
				styleYPrimaryDiv = LineStyle.YPrimaryDivisions;
				styleXSecondaryDiv = LineStyle.XSecondaryDivisions;
				styleYSecondaryDiv = LineStyle.YSecondaryDivisions;
				}
			else
				{
				styleMaxX = LineStyle.MaxY;
				styleMaxY = LineStyle.MaxX;
				styleMinX = LineStyle.MinY;
				styleMinY = LineStyle.MinX;
				styleXPrimaryDiv = LineStyle.YPrimaryDivisions;
				styleYPrimaryDiv = LineStyle.XPrimaryDivisions;
				styleXSecondaryDiv = LineStyle.YSecondaryDivisions;
				styleYSecondaryDiv = LineStyle.XSecondaryDivisions;
				}
			#endregion

			#region Определение положения оси Ox
			switch (LineStyle.OxPlacement)
				{
				case AxesPlacements.Auto:
					if ((styleMaxY <= 0.0) && (styleMinY <= 0.0) && (styleMaxY >= styleMinY) ||
						(styleMaxY >= 0.0) && (styleMinY >= 0.0) && (styleMaxY < styleMinY))
						{
						oy = (float)LineStyle.DiagramImageHeight * TopMargin;
						}
					else if ((styleMaxY >= 0.0) && (styleMinY >= 0.0) && (styleMaxY >= styleMinY) ||
						(styleMaxY <= 0.0) && (styleMinY <= 0.0) && (styleMaxY < styleMinY))
						{
						oy = (float)LineStyle.DiagramImageHeight * BottomMargin;
						}
					else
						{
						oy = (float)LineStyle.DiagramImageHeight * (float)(TopMargin + DiagramFieldPart * styleMaxY / (styleMaxY - styleMinY));
						}
					break;

				case AxesPlacements.LeftTop:
					oy = (float)LineStyle.DiagramImageHeight * TopMargin;
					break;

				case AxesPlacements.RightBottom:
					oy = (float)LineStyle.DiagramImageHeight * BottomMargin;
					break;

				case AxesPlacements.MiddleCenter:
					oy = (float)LineStyle.DiagramImageHeight * (DiagramFieldPart / 2.0f + TopMargin);
					break;
				}
			#endregion

			#region Определение положения оси Oy
			switch (LineStyle.OyPlacement)
				{
				case AxesPlacements.Auto:
					if ((styleMaxX >= 0.0) && (styleMinX >= 0.0) && (styleMaxX >= styleMinX) ||
						(styleMaxX <= 0.0) && (styleMinX <= 0.0) && (styleMaxX < styleMinX))
						{
						ox = (float)LineStyle.DiagramImageWidth * LeftMargin;
						}
					else if ((styleMaxX <= 0.0) && (styleMinX <= 0.0) && (styleMaxX >= styleMinX) ||
						(styleMaxX >= 0.0) && (styleMinX >= 0.0) && (styleMaxX < styleMinX))
						{
						ox = (float)LineStyle.DiagramImageWidth * RightMargin;
						}
					else
						{
						ox = (float)LineStyle.DiagramImageWidth * (float)(LeftMargin + DiagramFieldPart *
							(-styleMinX) / (styleMaxX - styleMinX));
						}
					break;

				case AxesPlacements.LeftTop:
					ox = (float)LineStyle.DiagramImageWidth * LeftMargin;
					break;

				case AxesPlacements.RightBottom:
					ox = (float)LineStyle.DiagramImageWidth * RightMargin;
					break;

				case AxesPlacements.MiddleCenter:
					ox = (float)LineStyle.DiagramImageWidth * (DiagramFieldPart / 2.0f + LeftMargin);
					break;
				}
			#endregion

			#region Засечки на оси Ox
			// Определение размера засечек
			NotchSize = 2.0f * LineStyle.AxesLinesWidth;

			// Подготовка к формированию подписей
			if (br != null)
				{
				br.Dispose ();
				}
			br = new SolidBrush (LineStyle.AxesFontColor);

			// Большие засечки
			y1 = oy - NotchSize;
			y2 = oy + NotchSize;

			for (uint i = 0; i <= styleXPrimaryDiv; i++)
				{
				x1 = (float)LineStyle.DiagramImageWidth * (float)(LeftMargin + DiagramFieldPart * ((double)i / (double)styleXPrimaryDiv));

				// Сетка
				if (LineStyle.PrimaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
					{
					p = new Pen (LineStyle.PrimaryGridColor, LineStyle.GridLinesWidth);
					g.DrawLine (p, x1, (float)LineStyle.DiagramImageHeight * TopMargin, x1, (float)LineStyle.DiagramImageHeight *
						BottomMargin);
					}

				// Засечки
				if (p != null)
					{
					p.Dispose ();
					}
				p = new Pen (LineStyle.AxesColor, LineStyle.AxesLinesWidth);
				g.DrawLine (p, x1, y1, x1, y2);

				// Подписи
				switch (LineStyle.OxFormat)
					{
					case NumbersFormat.Normal:
					default:
						txt = (styleMinX + (styleMaxX - styleMinX) * (double)i /
							(double)styleXPrimaryDiv).ToString (cie.NumberFormat);
						break;

					case NumbersFormat.Exponential:
						txt = (styleMinX + (styleMaxX - styleMinX) * (double)i /
							(double)styleXPrimaryDiv).ToString ("0.0#########E+0");
						break;

					case NumbersFormat.Date:
						/*try
							{
							double d1 = styleMinX + (styleMaxX - styleMinX) * (double)i /
								(double)styleXPrimaryDiv;
							DateTime d2 = new DateTime ((int)d1, 1, 1);
							d2 = d2.AddDays ((d1 - (int)d1) * (d2.Year % 4 == 0 ? 366.0 : 365.0));
							txt = d2.ToString ("dd.MM.yyyy");
							}
						catch
							{
							txt = "00.00.0000";
							}*/
						txt = DecompressDateValue (styleMinX + (styleMaxX - styleMinX) * (double)i /
							(double)styleXPrimaryDiv);
						break;
					}
				sz = g.MeasureString (txt, LineStyle.AxesFont);

				if (oy < (float)LineStyle.DiagramImageHeight / 2.0f)
					{
					g.DrawString (txt, LineStyle.AxesFont, br, x1 - sz.Width / 2.0f,
						(LineStyle.AutoTextOffset) ? (y2 + sz.Height / 3.0f) : (LineStyle.OxTextOffset));

					// Возврат расположения подписей (записывается в стиль)
					if (LineStyle.AutoTextOffset)
						{
						LineStyle.OxTextOffset = (uint)(y2 + sz.Height / 3.0f);
						}
					}
				else
					{
					g.DrawString (txt, LineStyle.AxesFont, br, x1 - sz.Width / 2.0f,
						(LineStyle.AutoTextOffset) ? (y2 - sz.Height - 2.0f * NotchSize) : (LineStyle.OxTextOffset));

					// Возврат расположения подписей (записывается в стиль)
					if (LineStyle.AutoTextOffset)
						{
						LineStyle.OxTextOffset = (uint)(y2 - sz.Height - 2.0f * NotchSize);
						}
					}
				}

			// Малые засечки
			y1 += NotchSize / 2.0f;
			y2 -= NotchSize / 2.0f;
			for (uint i = 0; i < styleXPrimaryDiv; i++)
				{
				for (uint j = 1; j < styleXSecondaryDiv; j++)
					{
					x1 = (float)LineStyle.DiagramImageWidth * (float)(LeftMargin + DiagramFieldPart * ((double)i /
						(double)styleXPrimaryDiv + (double)j / (double)(styleXSecondaryDiv * styleXPrimaryDiv)));

					// Сетка
					if (LineStyle.SecondaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
						{
						if (p != null)
							{
							p.Dispose ();
							}
						p = new Pen (LineStyle.SecondaryGridColor, LineStyle.GridLinesWidth);
						g.DrawLine (p, x1, (float)LineStyle.DiagramImageHeight * TopMargin, x1,
							(float)LineStyle.DiagramImageHeight * BottomMargin);
						}

					// Засечки
					if (p != null)
						{
						p.Dispose ();
						}
					p = new Pen (LineStyle.AxesColor, LineStyle.AxesLinesWidth);
					g.DrawLine (p, x1, y1, x1, y2);
					}
				}
			#endregion

			#region Засечки на оси Oy
			// Большие засечки
			x1 = ox - NotchSize;
			x2 = ox + NotchSize;

			for (uint i = 0; i <= styleYPrimaryDiv; i++)
				{
				y1 = (float)LineStyle.DiagramImageHeight * (float)(TopMargin + DiagramFieldPart * ((double)i / (double)styleYPrimaryDiv));

				// Отрисовка сетки, если необходимо
				if (LineStyle.PrimaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
					{
					if (p != null)
						{
						p.Dispose ();
						}
					p = new Pen (LineStyle.PrimaryGridColor, LineStyle.GridLinesWidth);
					g.DrawLine (p, (float)LineStyle.DiagramImageWidth * LeftMargin, y1, (float)LineStyle.DiagramImageWidth *
						RightMargin, y1);
					}

				// Засечки
				if (p != null)
					{
					p.Dispose ();
					}
				p = new Pen (LineStyle.AxesColor, LineStyle.AxesLinesWidth);
				g.DrawLine (p, x1, y1, x2, y1);

				// Подписи
				switch (LineStyle.OyFormat)
					{
					case NumbersFormat.Normal:
					default:
						txt = (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleYPrimaryDiv).ToString (cie.NumberFormat);
						break;

					case NumbersFormat.Exponential:
						txt = (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleYPrimaryDiv).ToString ("0.0#########E+0");
						break;

					case NumbersFormat.Date:
						/*try
							{
							double d1 = styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
								(double)styleYPrimaryDiv;
							DateTime d2 = new DateTime ((int)d1, 1, 1);
							d2 = d2.AddDays ((d1 - (int)d1) * (d2.Year % 4 == 0 ? 366 : 365));
							txt = d2.ToString ("dd.MM.yyyy");
							}
						catch
							{
							txt = "00.00.0000";
							}*/
						txt = DecompressDateValue (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleYPrimaryDiv);
						break;
					}
				sz = g.MeasureString (txt, LineStyle.AxesFont);

				if (ox < (float)LineStyle.DiagramImageWidth / 2.0f)
					{
					g.DrawString (txt, LineStyle.AxesFont, br,
						(LineStyle.AutoTextOffset) ? (x2 + 2.0f * NotchSize) : (LineStyle.OyTextOffset), y1 - sz.Height / 4.0f);

					// Возврат расположения подписей (записывается в стиль)
					if (LineStyle.AutoTextOffset)
						{
						LineStyle.OyTextOffset = (uint)(x2 + 2.0f * NotchSize);
						}
					}
				else
					{
					g.DrawString (txt, LineStyle.AxesFont, br,
						(LineStyle.AutoTextOffset) ? (x2 - sz.Width - 2.0f * NotchSize) : (LineStyle.OyTextOffset),
						y1 - sz.Height / 4.0f);

					// Возврат расположения подписей (записывается в стиль)
					if (LineStyle.AutoTextOffset)
						{
						LineStyle.OyTextOffset = (uint)(x2 - sz.Width - 2.0f * NotchSize);
						}
					}
				}

			// Малые засечки
			x1 += NotchSize / 2.0f;
			x2 -= NotchSize / 2.0f;
			for (uint i = 0; i < styleYPrimaryDiv; i++)
				{
				for (uint j = 1; j < styleYSecondaryDiv; j++)
					{
					y1 = (float)LineStyle.DiagramImageHeight * (float)(TopMargin + DiagramFieldPart * ((double)i / (double)styleYPrimaryDiv +
						(double)j / (double)(styleYSecondaryDiv * styleYPrimaryDiv)));

					// Сетка
					if (LineStyle.SecondaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
						{
						if (p != null)
							{
							p.Dispose ();
							}
						p = new Pen (LineStyle.SecondaryGridColor, LineStyle.GridLinesWidth);
						g.DrawLine (p, (float)LineStyle.DiagramImageWidth * LeftMargin, y1, (float)LineStyle.DiagramImageWidth *
							RightMargin, y1);
						}

					// Засечки
					if (p != null)
						{
						p.Dispose ();
						}
					p = new Pen (LineStyle.AxesColor, LineStyle.AxesLinesWidth);
					g.DrawLine (p, x1, y1, x2, y1);
					}
				}
			#endregion

			// Кривые отрисовываются над сеткой, но под осями
			// Без этого ограничения можно натолкнуться на деление на ноль
			if ((LineStyle.MinX != LineStyle.MaxX) && (LineStyle.MinY != LineStyle.MaxY))
				{
				#region Отрисовка кривых
				p = new Pen (LineStyle.LineColor, LineStyle.LineWidth);
				br = new SolidBrush (LineStyle.LineColor);

				for (int i = 1; i < dataValues[0].Count; i++)
					{
					// Определение кривой для построения
					ptx1 = dataValues[(int)lineStyles[LineNumber].XColumnNumber][i - 1];
					ptx2 = dataValues[(int)lineStyles[LineNumber].XColumnNumber][i];
					pty1 = dataValues[(int)lineStyles[LineNumber].YColumnNumber][i - 1];
					pty2 = dataValues[(int)lineStyles[LineNumber].YColumnNumber][i];

					// Отрисовка с учётом обмена осей
					if (!LineStyle.SwitchXY)
						{
						x1 = (float)LineStyle.DiagramImageWidth * (LeftMargin + DiagramFieldPart * (float)(ptx1 - LineStyle.MinX) /
							(float)(LineStyle.MaxX - LineStyle.MinX));
						y1 = (float)LineStyle.DiagramImageHeight * (TopMargin + DiagramFieldPart * (float)(LineStyle.MaxY - pty1) /
							(float)(LineStyle.MaxY - LineStyle.MinY));
						x2 = (float)LineStyle.DiagramImageWidth * (LeftMargin + DiagramFieldPart * (float)(ptx2 - LineStyle.MinX) /
							(float)(LineStyle.MaxX - LineStyle.MinX));
						y2 = (float)LineStyle.DiagramImageHeight * (TopMargin + DiagramFieldPart * (float)(LineStyle.MaxY - pty2) /
							(float)(LineStyle.MaxY - LineStyle.MinY));
						}
					else
						{
						x1 = (float)LineStyle.DiagramImageWidth * (LeftMargin + DiagramFieldPart * (float)(pty1 - LineStyle.MinY) /
							(float)(LineStyle.MaxY - LineStyle.MinY));
						y1 = (float)LineStyle.DiagramImageHeight * (TopMargin + DiagramFieldPart * (float)(LineStyle.MaxX - ptx1) /
							(float)(LineStyle.MaxX - LineStyle.MinX));
						x2 = (float)LineStyle.DiagramImageWidth * (LeftMargin + DiagramFieldPart * (float)(pty2 - LineStyle.MinY) /
							(float)(LineStyle.MaxY - LineStyle.MinY));
						y2 = (float)LineStyle.DiagramImageHeight * (TopMargin + DiagramFieldPart * (float)(LineStyle.MaxX - ptx2) /
							(float)(LineStyle.MaxX - LineStyle.MinX));
						}

					if (!float.IsNaN (x1) && !float.IsNaN (x2) && !float.IsNaN (y1) && !float.IsNaN (y2))
						{
						// Линия
						if (LineStyle.LineDrawingFormat != DrawingLinesFormats.OnlyMarkers)
							{
							g2.DrawLine (p, x1, y1, x2, y2);
							}

						// Маркеры
						if (LineStyle.LineDrawingFormat != DrawingLinesFormats.OnlyLine)
							{
							if (i == 1)
								{
								g2.DrawImage (currentMarker, (int)x1 - currentMarker.Width / 2, (int)y1 - currentMarker.Height / 2);
								}
							g2.DrawImage (currentMarker, (int)x2 - currentMarker.Width / 2, (int)y2 - currentMarker.Height / 2);
							}
						}
					}
				#endregion
				}

			#region Отрисовка осей
			if (p != null)
				{
				p.Dispose ();
				}
			p = new Pen (LineStyle.AxesColor, LineStyle.AxesLinesWidth);
			g.DrawLine (p, ox, (float)LineStyle.DiagramImageHeight * TopMargin, ox, (float)LineStyle.DiagramImageHeight * BottomMargin);
			g.DrawLine (p, (float)LineStyle.DiagramImageWidth * LeftMargin, oy, (float)LineStyle.DiagramImageWidth * RightMargin, oy);
			#endregion

			#region Легенда
			// Расчёт положения
			if (br != null)
				{
				br.Dispose ();
				}
			br = new SolidBrush (LineStyle.TextFontColor);

			// Отрисовка подписи с учётом масштабирования и размера шрифта
			if (LineStyle.AutoTextOffset)
				{
				string str = dataColumnNames[(int)LineStyle.YColumnNumber] + " @ " +
					dataColumnNames[(int)LineStyle.XColumnNumber];
				sz = g.MeasureString (str, LineStyle.TextFont);

				g.DrawString (str, LineStyle.TextFont, br,
					(uint)((float)LineStyle.DiagramImageWidth * LeftMargin) + (uint)(2 * LineNumber * sz.Width) %
					(uint)(Math.Abs ((float)LineStyle.DiagramImageWidth * DiagramFieldPart - sz.Width) + 1.0f),
					(float)LineStyle.DiagramImageHeight * (BottomMargin + 3.0f * TopMargin));
				}
			else
				{
				string str = LineStyle.LineName;
				sz = g.MeasureString (str, LineStyle.TextFont);

				g.DrawString (str, LineStyle.TextFont, br,
					LineStyle.LineNameLeftOffset, LineStyle.LineNameTopOffset);
				}
			#endregion

			// Финальная отрисовка
			DrawField.DrawImage (lineImage, LineStyle.DiagramImageLeftOffset, LineStyle.DiagramImageTopOffset);

			// Готово
			lineImage.Dispose ();
			g.Dispose ();
			g2.Dispose ();
			if (br != null)
				br.Dispose ();
			if (p != null)
				p.Dispose ();
			currentMarker.Dispose ();
			}

		// Метод формирует изображение одного дополнительного объекта
		private void DrawObject (int ObjectNumber, DiagramStyle ObjectStyle, Graphics DrawField, 
			bool IsSelected)
			{
			// Создание пустого изображения заданного размера с белым фоном
			Bitmap objectImage = new Bitmap ((int)ObjectStyle.DiagramImageWidth, (int)ObjectStyle.DiagramImageHeight);  // Будущее изображение
			Graphics g = Graphics.FromImage (objectImage);  // Графический дескриптор
			g.Clear (DiagramStyle.ImageBackColor);

			// Установка отрисовки подписей без сглаживания (устраняет чёрную ауру при отрисовке подписей в Win8)
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

			// Переменные
			Pen p;
			Brush br;
			Brush brt;

			// Отрисовка рамки, если кривая выделена на диаграмме
			if (IsSelected)
				{
				br = new SolidBrush (Color.FromArgb (16, 0, 0, 0));
				g.FillRectangle (br, 0, 0, ObjectStyle.DiagramImageWidth, ObjectStyle.DiagramImageHeight);
				br.Dispose ();
				}

			// Расчёт корректировочного коэффициента (для коррекции размера под толщину линий)
			float correction = (float)(ObjectStyle.LineWidth >> 1); // Устанавливает новое смещение только на чётных значениях

			// Отрисовка объекта
			p = new Pen (ObjectStyle.LineColor, ObjectStyle.LineWidth);
			br = new SolidBrush (ObjectStyle.LineColor);
			brt = new SolidBrush (ObjectStyle.TextFontColor);

			switch (additionalObjects[ObjectNumber])
				{
				case DiagramAdditionalObjects.Ellipse:
					g.DrawEllipse (p, 0 + correction, 0 + correction,
						ObjectStyle.DiagramImageWidth - ObjectStyle.LineWidth, ObjectStyle.DiagramImageHeight - ObjectStyle.LineWidth);
					break;

				case DiagramAdditionalObjects.FilledEllipse:
					g.FillEllipse (br, 0, 0, ObjectStyle.DiagramImageWidth - 1, ObjectStyle.DiagramImageHeight - 1);
					break;

				case DiagramAdditionalObjects.Rectangle:
					g.DrawRectangle (p, 0 + correction, 0 + correction,
						ObjectStyle.DiagramImageWidth - ObjectStyle.LineWidth, ObjectStyle.DiagramImageHeight - ObjectStyle.LineWidth);
					break;

				case DiagramAdditionalObjects.FilledRectangle:
					g.FillRectangle (br, 0, 0, ObjectStyle.DiagramImageWidth - 1, ObjectStyle.DiagramImageHeight - 1);
					break;

				case DiagramAdditionalObjects.OldLine:      // Изоляция исключений
				case DiagramAdditionalObjects.LineNWtoSE:
					g.DrawLine (p, 0 + correction, 0 + correction,
						ObjectStyle.DiagramImageWidth - ObjectStyle.LineWidth, ObjectStyle.DiagramImageHeight - ObjectStyle.LineWidth);
					break;

				case DiagramAdditionalObjects.LineSWtoNE:
					g.DrawLine (p, ObjectStyle.DiagramImageWidth - ObjectStyle.LineWidth, 0 + correction,
						0 + correction, ObjectStyle.DiagramImageHeight - ObjectStyle.LineWidth);
					break;

				case DiagramAdditionalObjects.LineH:
					g.DrawLine (p, 0 + correction, ObjectStyle.DiagramImageHeight / 2,
						ObjectStyle.DiagramImageWidth - ObjectStyle.LineWidth, ObjectStyle.DiagramImageHeight / 2);
					break;

				case DiagramAdditionalObjects.LineV:
					g.DrawLine (p, ObjectStyle.DiagramImageWidth / 2, 0 + correction,
						ObjectStyle.DiagramImageWidth / 2, ObjectStyle.DiagramImageHeight - ObjectStyle.LineWidth);
					break;

				case DiagramAdditionalObjects.Text:
					g.DrawString (ObjectStyle.LineName, ObjectStyle.TextFont, brt, 0, 0);
					break;
				}

			p.Dispose ();
			br.Dispose ();
			brt.Dispose ();

			// Финальная отрисовка
			DrawField.DrawImage (objectImage, ObjectStyle.DiagramImageLeftOffset, ObjectStyle.DiagramImageTopOffset);

			// Готово
			objectImage.Dispose ();
			g.Dispose ();
			}

		/// <summary>
		/// Метод формирует полное изображение диаграммы, используя стандартные стили и собственный 
		/// размер диаграммы
		/// </summary>
		/// <param name="DrawField">Поле отрисовки диаграммы</param>
		/// <param name="CurrentLines">Список кривых, выделяемых на диаграмме; может быть null</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int DrawAllDiagrams (Graphics DrawField, ListBox.SelectedIndexCollection CurrentLines)
			{
			return DrawAllDiagrams (DiagramWidth, DiagramHeight, lineStyles, additionalObjectsStyles, DrawField, CurrentLines);
			}

		/// <summary>
		/// Метод формирует полное изображение диаграммы, применяя к нему нестандартный набор стилей 
		/// и новые размеры
		/// </summary>
		/// <param name="WidthValue">Ширина конечного изображения</param>
		/// <param name="HeightValue">Высота конечного изображения</param>
		/// <param name="DrawField">Поле для отрисовки диаграммы</param>
		/// <param name="NewLinesStyles">Набор стилей кривых, применяемый к диаграмме</param>
		/// <param name="NewObjectsStyles">Набор стилей объектов, применяемый к диаграмме</param>
		/// <param name="CurrentLines">Список кривых, выделяемых на диаграмме; может быть null</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int DrawAllDiagrams (uint WidthValue, uint HeightValue, List<DiagramStyle> NewLinesStyles,
			List<DiagramStyle> NewObjectsStyles, Graphics DrawField, ListBox.SelectedIndexCollection 
			CurrentLines)
			{
			// Контроль значений
			if ((WidthValue < DiagramStyle.MinImageWidth) || (HeightValue < DiagramStyle.MinImageHeight) ||
				(NewLinesStyles == null) || (NewLinesStyles.Count != curves.Count) ||
				(NewObjectsStyles == null) || (NewObjectsStyles.Count != additionalObjects.Count))
				{
				return -2;
				}

			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			// Создание поля для отрисовки
			DrawField.Clear (DiagramStyle.ImageBackColor);

			// Отрисовка изображения
			for (int i = 0; i < curves.Count; i++)
				{
				if (NewLinesStyles[i].AllowDrawing)
					{
					bool isSelected = (CurrentLines != null) && (CurrentLines.Contains (i));
					DrawDiagram (i, NewLinesStyles[i], DrawField, isSelected);
					}
				}
			for (int i = 0; i < additionalObjects.Count; i++)
				{
				if (NewObjectsStyles[i].AllowDrawing)
					{
					bool isSelected = (CurrentLines != null) && (CurrentLines.Contains (i + curves.Count));
					DrawObject (i, NewObjectsStyles[i], DrawField, isSelected);
					}
				}

			// Готово
			return 0;
			}

		// Метод формирует векторное изображение одной кривой
		private void DrawDiagram (uint X, uint Y, int LineNumber, IVectorAdapter VectorAdapter)
			{
			// Создание верхней группы
			VectorAdapter.OpenGroup ();

			// Полная область отрисовки
			VectorAdapter.SetClipBox ((double)X, (double)Y, (double)X + (double)lineStyles[LineNumber].DiagramImageWidth,
				(double)Y + (double)lineStyles[LineNumber].DiagramImageHeight);

			// Создание переменных
			Bitmap b = new Bitmap (50, 50);     // Вспомогательные переменные для расчёта позиций подписей
			Graphics g = Graphics.FromImage (b);
			SizeF sz;
			double x1, x2;  // Вспомогательные переменные для отрисовки линий
			double y1, y2;
			double ptx1 = 0.0;
			double pty1 = 0.0;
			float ox = 0.0f, oy = 0.0f; // Точки фиксации осей
			float NotchSize;    // Вычисляемый размер засечек
			double styleMinX, styleMaxX;            // Вспомогательные переменные для транспонирования диаграмм
			double styleMinY, styleMaxY;
			uint styleXPrimaryDiv, styleYPrimaryDiv,
				styleXSecondaryDiv, styleYSecondaryDiv;
			string txt;

			// Получение текущего маркера
			MarkersLoader ml = new MarkersLoader ();
			Bitmap currentMarker = ml.GetMarker (lineStyles[LineNumber].LineMarkerNumber - 1, lineStyles[LineNumber].LineColor);
			ml.Dispose ();

			#region Транспонирование диапазонов при необходимости
			if (!lineStyles[LineNumber].SwitchXY)
				{
				styleMaxX = lineStyles[LineNumber].MaxX;
				styleMaxY = lineStyles[LineNumber].MaxY;
				styleMinX = lineStyles[LineNumber].MinX;
				styleMinY = lineStyles[LineNumber].MinY;
				styleXPrimaryDiv = lineStyles[LineNumber].XPrimaryDivisions;
				styleYPrimaryDiv = lineStyles[LineNumber].YPrimaryDivisions;
				styleXSecondaryDiv = lineStyles[LineNumber].XSecondaryDivisions;
				styleYSecondaryDiv = lineStyles[LineNumber].YSecondaryDivisions;
				}
			else
				{
				styleMaxX = lineStyles[LineNumber].MaxY;
				styleMaxY = lineStyles[LineNumber].MaxX;
				styleMinX = lineStyles[LineNumber].MinY;
				styleMinY = lineStyles[LineNumber].MinX;
				styleXPrimaryDiv = lineStyles[LineNumber].YPrimaryDivisions;
				styleYPrimaryDiv = lineStyles[LineNumber].XPrimaryDivisions;
				styleXSecondaryDiv = lineStyles[LineNumber].YSecondaryDivisions;
				styleYSecondaryDiv = lineStyles[LineNumber].XSecondaryDivisions;
				}
			#endregion

			#region Определение положения оси Ox
			switch (lineStyles[LineNumber].OxPlacement)
				{
				case AxesPlacements.Auto:
					if ((styleMaxY <= 0.0) && (styleMinY <= 0.0) && (styleMaxY >= styleMinY) ||
						(styleMaxY >= 0.0) && (styleMinY >= 0.0) && (styleMaxY < styleMinY))
						{
						oy = (float)lineStyles[LineNumber].DiagramImageHeight * TopMargin;
						}
					else if ((styleMaxY >= 0.0) && (styleMinY >= 0.0) && (styleMaxY >= styleMinY) ||
						(styleMaxY <= 0.0) && (styleMinY <= 0.0) && (styleMaxY < styleMinY))
						{
						oy = (float)lineStyles[LineNumber].DiagramImageHeight * BottomMargin;
						}
					else
						{
						oy = (float)lineStyles[LineNumber].DiagramImageHeight * (float)(TopMargin + DiagramFieldPart * styleMaxY / (styleMaxY - styleMinY));
						}
					break;

				case AxesPlacements.LeftTop:
					oy = (float)lineStyles[LineNumber].DiagramImageHeight * TopMargin;
					break;

				case AxesPlacements.RightBottom:
					oy = (float)lineStyles[LineNumber].DiagramImageHeight * BottomMargin;
					break;

				case AxesPlacements.MiddleCenter:
					oy = (float)lineStyles[LineNumber].DiagramImageHeight * (DiagramFieldPart / 2.0f + TopMargin);
					break;
				}
			#endregion

			#region Определение положения оси Oy
			switch (lineStyles[LineNumber].OyPlacement)
				{
				case AxesPlacements.Auto:
					if ((styleMaxX >= 0.0f) && (styleMinX >= 0.0f) && (styleMaxX >= styleMinX) ||
						(styleMaxX <= 0.0f) && (styleMinX <= 0.0f) && (styleMaxX < styleMinX))
						{
						ox = (float)lineStyles[LineNumber].DiagramImageWidth * LeftMargin;
						}
					else if ((styleMaxX <= 0.0f) && (styleMinX <= 0.0f) && (styleMaxX >= styleMinX) ||
						(styleMaxX >= 0.0f) && (styleMinX >= 0.0f) && (styleMaxX < styleMinX))
						{
						ox = (float)lineStyles[LineNumber].DiagramImageWidth * RightMargin;
						}
					else
						{
						ox = (float)lineStyles[LineNumber].DiagramImageWidth * (float)(LeftMargin + DiagramFieldPart * (-styleMinX) /
							(styleMaxX - styleMinX));
						}
					break;

				case AxesPlacements.LeftTop:
					ox = (float)lineStyles[LineNumber].DiagramImageWidth * LeftMargin;
					break;

				case AxesPlacements.RightBottom:
					ox = (float)lineStyles[LineNumber].DiagramImageWidth * RightMargin;
					break;

				case AxesPlacements.MiddleCenter:
					ox = (float)lineStyles[LineNumber].DiagramImageWidth * (DiagramFieldPart / 2.0f + LeftMargin);
					break;
				}
			#endregion

			#region Ось Ox
			// Определение размера засечек
			NotchSize = 2.0f * lineStyles[LineNumber].AxesLinesWidth;

			// Большие засечки
			y1 = (double)Y + oy - NotchSize;
			y2 = (double)Y + oy + NotchSize;

			// Сетка
			VectorAdapter.OpenGroup ();

			for (uint i = 0; i <= styleXPrimaryDiv; i++)
				{
				// Сетка по большим засечкам
				if (lineStyles[LineNumber].PrimaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
					{
					x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)(LeftMargin + DiagramFieldPart *
						((double)i / (double)styleXPrimaryDiv));

					VectorAdapter.DrawLine (x1, (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * TopMargin, x1,
						(double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * BottomMargin,
						lineStyles[LineNumber].GridLinesWidth, lineStyles[LineNumber].PrimaryGridColor);
					}

				if (i < styleXPrimaryDiv)   // Больших засечек больше на одну, чем фаз с малыми засечками между ними
					{
					for (uint j = 1; j < styleXSecondaryDiv; j++)
						{
						// Сетка по малым засечкам
						if (lineStyles[LineNumber].SecondaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
							{
							x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)(LeftMargin + DiagramFieldPart *
								((double)i / (double)styleXPrimaryDiv + (double)j / (double)(styleXSecondaryDiv * styleXPrimaryDiv)));

							VectorAdapter.DrawLine (x1, (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * TopMargin, x1,
								(double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * BottomMargin,
								lineStyles[LineNumber].GridLinesWidth, lineStyles[LineNumber].SecondaryGridColor);
							}
						}
					}
				}

			VectorAdapter.CloseGroup ();

			// Ось
			VectorAdapter.OpenGroup ();

			VectorAdapter.DrawLine ((double)X + (double)lineStyles[LineNumber].DiagramImageWidth * LeftMargin, (double)Y + oy,
				(double)X + (double)lineStyles[LineNumber].DiagramImageWidth * RightMargin, (double)Y + oy,
				lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);

			// Засечки
			for (uint i = 0; i <= styleXPrimaryDiv; i++)
				{
				x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)(LeftMargin + DiagramFieldPart *
					((double)i / (double)styleXPrimaryDiv));

				// Засечки (большие)
				VectorAdapter.DrawLine (x1, y1, x1, y2, lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);

				// Засечки (малые)
				if (i < styleXPrimaryDiv)
					{
					for (uint j = 1; j < styleXSecondaryDiv; j++)
						{
						x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)(LeftMargin + DiagramFieldPart *
							((double)i / (double)styleXPrimaryDiv + (double)j / (double)(styleXSecondaryDiv * styleXPrimaryDiv)));

						VectorAdapter.DrawLine (x1, y1 + NotchSize / 2.0f, x1, y2 - NotchSize / 2.0f, lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);
						}
					}
				}

			VectorAdapter.CloseGroup ();

			// Подписи
			VectorAdapter.OpenGroup ();

			for (uint i = 0; i <= styleXPrimaryDiv; i++)
				{
				x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)(LeftMargin + DiagramFieldPart *
					((double)i / (double)styleXPrimaryDiv));

				switch (lineStyles[LineNumber].OxFormat)
					{
					case NumbersFormat.Normal:
					default:
						txt = (styleMinX + (styleMaxX - styleMinX) * (double)i / (double)styleXPrimaryDiv).ToString (cie.NumberFormat);
						break;

					case NumbersFormat.Exponential:
						txt = (styleMinX + (styleMaxX - styleMinX) * (double)i /
							(double)styleXPrimaryDiv).ToString ("0.0#########E+0");
						break;

					case NumbersFormat.Date:
						/*try
							{
							double d1 = styleMinX + (styleMaxX - styleMinX) * (double)i /
								(double)styleXPrimaryDiv;
							DateTime d2 = new DateTime ((int)d1, 1, 1);
							d2 = d2.AddDays ((d1 - (int)d1) * (d2.Year % 4 == 0 ? 366 : 365));
							txt = d2.ToString ("dd.MM.yyyy");
							}
						catch
							{
							txt = "00.00.0000";
							}*/
						txt = DecompressDateValue (styleMinX + (styleMaxX - styleMinX) * (double)i /
							(double)styleXPrimaryDiv);
						break;
					}
				sz = g.MeasureString (txt, lineStyles[LineNumber].AxesFont);

				if (oy < (float)lineStyles[LineNumber].DiagramImageHeight / 2.0f)
					{
					VectorAdapter.DrawText (x1 - sz.Width / 2.0f, (lineStyles[LineNumber].AutoTextOffset) ? (y2 + sz.Height) : (Y + lineStyles[LineNumber].OxTextOffset + sz.Height),
						txt, lineStyles[LineNumber].AxesFont, lineStyles[LineNumber].AxesFontColor);
					}
				else
					{
					VectorAdapter.DrawText (x1 - sz.Width / 2.0f, (lineStyles[LineNumber].AutoTextOffset) ? (y2 - sz.Height / 2) : (Y + lineStyles[LineNumber].OxTextOffset + sz.Height),
						txt, lineStyles[LineNumber].AxesFont, lineStyles[LineNumber].AxesFontColor);
					}
				}

			VectorAdapter.CloseGroup ();

			#endregion

			#region Ось Oy

			x1 = (float)X + ox - NotchSize;
			x2 = (float)X + ox + NotchSize;

			// Сетка
			VectorAdapter.OpenGroup ();

			for (uint i = 0; i <= styleYPrimaryDiv; i++)
				{
				// Сетка по большим засечкам
				if (lineStyles[LineNumber].PrimaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
					{
					y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)(TopMargin + DiagramFieldPart * ((double)i /
					(double)styleYPrimaryDiv));

					VectorAdapter.DrawLine ((double)X + (double)lineStyles[LineNumber].DiagramImageWidth * LeftMargin, y1,
						(double)X + (double)lineStyles[LineNumber].DiagramImageWidth * RightMargin, y1,
						lineStyles[LineNumber].GridLinesWidth, lineStyles[LineNumber].PrimaryGridColor);
					}

				// Сетка по малым засечкам
				if (i < styleYPrimaryDiv)
					{
					for (uint j = 1; j < styleYSecondaryDiv; j++)
						{
						if (lineStyles[LineNumber].SecondaryGridColor.ToArgb () != DiagramStyle.ImageBackColor.ToArgb ())
							{
							y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)(TopMargin + DiagramFieldPart *
								((double)i / (double)styleYPrimaryDiv + (double)j / (double)(styleYSecondaryDiv * styleYPrimaryDiv)));

							VectorAdapter.DrawLine ((double)X + (double)lineStyles[LineNumber].DiagramImageWidth * LeftMargin, y1,
								(double)X + (double)lineStyles[LineNumber].DiagramImageWidth * RightMargin, y1,
								lineStyles[LineNumber].GridLinesWidth, lineStyles[LineNumber].SecondaryGridColor);
							}
						}
					}
				}

			VectorAdapter.CloseGroup ();

			// Ось
			VectorAdapter.OpenGroup ();

			VectorAdapter.DrawLine ((double)X + ox, (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * TopMargin,
				(double)X + ox, (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * BottomMargin,
				lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);

			// Засечки (большие)
			for (uint i = 0; i <= styleYPrimaryDiv; i++)
				{
				y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)(TopMargin + DiagramFieldPart * ((double)i /
					(double)styleYPrimaryDiv));
				VectorAdapter.DrawLine (x1, y1, x2, y1, lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);

				// Засечки (малые)
				if (i < styleYPrimaryDiv)
					{
					for (uint j = 1; j < styleYSecondaryDiv; j++)
						{
						y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)(TopMargin + DiagramFieldPart *
							((double)i / (double)styleYPrimaryDiv + (double)j / (double)(styleYSecondaryDiv * styleYPrimaryDiv)));

						VectorAdapter.DrawLine (x1 + NotchSize / 2.0f, y1, x2 - NotchSize / 2.0f, y1, lineStyles[LineNumber].AxesLinesWidth, lineStyles[LineNumber].AxesColor);
						}
					}
				}

			VectorAdapter.CloseGroup ();

			// Подписи
			VectorAdapter.OpenGroup ();

			for (uint i = 0; i <= styleYPrimaryDiv; i++)
				{
				y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)(TopMargin + DiagramFieldPart * ((double)i /
					(double)styleYPrimaryDiv));

				switch (lineStyles[LineNumber].OyFormat)
					{
					case NumbersFormat.Normal:
					default:
						txt = (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleYPrimaryDiv).ToString (cie.NumberFormat);
						break;

					case NumbersFormat.Exponential:
						txt = (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleYPrimaryDiv).ToString ("0.0#########E+0");
						break;

					case NumbersFormat.Date:
						/*try
							{
							double d1 = styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
								(double)styleXPrimaryDiv;
							DateTime d2 = new DateTime ((int)d1, 1, 1);
							d2 = d2.AddDays ((d1 - (int)d1) * (d2.Year % 4 == 0 ? 366 : 365));
							txt = d2.ToString ("dd.MM.yyyy");
							}
						catch
							{
							txt = "00.00.0000";
							}*/
						txt = DecompressDateValue (styleMinY + (styleMaxY - styleMinY) * (double)(styleYPrimaryDiv - i) /
							(double)styleXPrimaryDiv);
						break;
					}
				sz = g.MeasureString (txt, lineStyles[LineNumber].AxesFont);

				if (ox < (float)lineStyles[LineNumber].DiagramImageWidth / 2.0f)
					{
					VectorAdapter.DrawText ((lineStyles[LineNumber].AutoTextOffset) ? (x2 + 2.0f * NotchSize) : (X + lineStyles[LineNumber].OyTextOffset), y1 + sz.Height / 2.0f,
						txt, lineStyles[LineNumber].AxesFont, lineStyles[LineNumber].AxesFontColor);
					}
				else
					{
					VectorAdapter.DrawText ((lineStyles[LineNumber].AutoTextOffset) ? (x2 - sz.Width * 0.8f - 2.0f * NotchSize) : (X + lineStyles[LineNumber].OyTextOffset), y1 + sz.Height / 2.0f,
						txt, lineStyles[LineNumber].AxesFont, lineStyles[LineNumber].AxesFontColor);
					}
				}

			VectorAdapter.CloseGroup ();
			#endregion

			#region Легенда
			// Отрисовка подписи с учётом масштабирования и размера шрифта
			if (lineStyles[LineNumber].AutoTextOffset)
				{
				string str = dataColumnNames[(int)lineStyles[LineNumber].YColumnNumber] + " @ " +
					dataColumnNames[(int)lineStyles[LineNumber].XColumnNumber];
				sz = g.MeasureString (str, lineStyles[LineNumber].TextFont);

				VectorAdapter.DrawText (X + (uint)((float)lineStyles[LineNumber].DiagramImageWidth * LeftMargin) + (uint)(2 * LineNumber * sz.Width) %
					(uint)(Math.Abs (lineStyles[LineNumber].DiagramImageWidth * DiagramFieldPart - sz.Width) + 1.0f),
					Y + (float)lineStyles[LineNumber].DiagramImageHeight * (BottomMargin + 3.0f * TopMargin) + sz.Height, str,
					lineStyles[LineNumber].TextFont, lineStyles[LineNumber].TextFontColor);
				}
			else
				{
				string str = lineStyles[LineNumber].LineName;
				sz = g.MeasureString (str, lineStyles[LineNumber].TextFont);

				VectorAdapter.DrawText (X + lineStyles[LineNumber].LineNameLeftOffset, Y + lineStyles[LineNumber].LineNameTopOffset + sz.Height, str, lineStyles[LineNumber].TextFont, lineStyles[LineNumber].TextFontColor);
				}
			#endregion

			// Область отрисовки линии диаграммы
			VectorAdapter.SetClipBox ((double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (double)LeftMargin,
				(double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (double)TopMargin,
				(double)X + (double)(lineStyles[LineNumber].DiagramImageWidth + 2) * (double)RightMargin,
				(double)Y + (double)(lineStyles[LineNumber].DiagramImageHeight + 2) * (double)BottomMargin);

			// Без этого ограничения можно натолкнуться на деление на ноль
			if ((lineStyles[LineNumber].MinX != lineStyles[LineNumber].MaxX) && (lineStyles[LineNumber].MinY != lineStyles[LineNumber].MaxY))
				{
				#region Отрисовка кривых
				// Открытие группы для кривой
				VectorAdapter.OpenGroup ();

				// Отрисовка маркеров и сборка ломаной линии
				List<double> xv = new List<double> ();
				List<double> yv = new List<double> ();

				for (int i = 0; i < dataValues[0].Count; i++)
					{
					// Определение кривой для построения
					ptx1 = dataValues[(int)lineStyles[LineNumber].XColumnNumber][i];
					pty1 = dataValues[(int)lineStyles[LineNumber].YColumnNumber][i];

					// Отрисовка с учётом обмена осей
					if (!lineStyles[LineNumber].SwitchXY)
						{
						x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (LeftMargin + DiagramFieldPart *
							(ptx1 - lineStyles[LineNumber].MinX) /
							(double)(lineStyles[LineNumber].MaxX - lineStyles[LineNumber].MinX));
						y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (TopMargin + DiagramFieldPart *
							(lineStyles[LineNumber].MaxY - pty1) /
							(double)(lineStyles[LineNumber].MaxY - lineStyles[LineNumber].MinY));
						}
					else
						{
						x1 = (double)X + (double)lineStyles[LineNumber].DiagramImageWidth * (LeftMargin + DiagramFieldPart *
							(pty1 - lineStyles[LineNumber].MinY) /
							(double)(lineStyles[LineNumber].MaxY - lineStyles[LineNumber].MinY));
						y1 = (double)Y + (double)lineStyles[LineNumber].DiagramImageHeight * (TopMargin + DiagramFieldPart *
							(lineStyles[LineNumber].MaxX - ptx1) /
							(double)(lineStyles[LineNumber].MaxX - lineStyles[LineNumber].MinX));
						}

					// Линия
					xv.Add (x1);
					yv.Add (y1);

					// Маркеры
					if (lineStyles[LineNumber].LineDrawingFormat != DrawingLinesFormats.OnlyLine)
						{
						VectorAdapter.DrawMarker (currentMarker, x1, y1, (uint)LineNumber);
						}
					}

				// Отрисовка ломаной линии
				if (lineStyles[LineNumber].LineDrawingFormat != DrawingLinesFormats.OnlyMarkers)
					{
					for (int i = 0; i < xv.Count - 1; i++)
						{
						VectorAdapter.DrawLine (xv[i], yv[i], xv[i + 1], yv[i + 1], lineStyles[LineNumber].LineWidth,
							lineStyles[LineNumber].LineColor);
						}
					}
				xv.Clear ();
				yv.Clear ();

				// Закрытие группы для кривой
				VectorAdapter.CloseGroup ();
				#endregion
				}

			// Готово
			g.Dispose ();
			b.Dispose ();
			ml.Dispose ();
			VectorAdapter.CloseGroup ();
			}

		// Метод формирует векторное изображение одной кривой
		private void DrawObject (uint X, uint Y, int ObjectNumber, IVectorAdapter VectorAdapter)
			{
			VectorAdapter.SetClipBox ((double)X, (double)Y, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth,
				(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight);

			switch (additionalObjects[ObjectNumber])
				{
				case DiagramAdditionalObjects.Ellipse:
					VectorAdapter.DrawEllipse ((double)X, (double)Y, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
						(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
						additionalObjectsStyles[ObjectNumber].LineWidth, additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.FilledEllipse:
					VectorAdapter.FillEllipse ((double)X, (double)Y, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
						(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
						additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.Rectangle:
					VectorAdapter.DrawRectangle ((double)X, (double)Y, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
						(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
						additionalObjectsStyles[ObjectNumber].LineWidth, additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.FilledRectangle:
					VectorAdapter.FillRectangle ((double)X, (double)Y, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
						(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
						additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.OldLine:  // Изоляция исключений
				case DiagramAdditionalObjects.LineNWtoSE:
					VectorAdapter.DrawLine ((double)X + 0, (double)Y + 0, (double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
							(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
							additionalObjectsStyles[ObjectNumber].LineWidth,
							additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.LineSWtoNE:
					VectorAdapter.DrawLine ((double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1, (double)Y + 0,
							(double)X + 0, (double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
							additionalObjectsStyles[ObjectNumber].LineWidth,
							additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.LineH:
					VectorAdapter.DrawLine ((double)X + 0, (double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight / 2.0,
							(double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth - 1,
							(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight / 2.0,
							additionalObjectsStyles[ObjectNumber].LineWidth,
							additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.LineV:
					VectorAdapter.DrawLine ((double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth / 2.0, (double)Y + 0,
							(double)X + (double)additionalObjectsStyles[ObjectNumber].DiagramImageWidth / 2.0,
							(double)Y + (double)additionalObjectsStyles[ObjectNumber].DiagramImageHeight - 1,
							additionalObjectsStyles[ObjectNumber].LineWidth,
							additionalObjectsStyles[ObjectNumber].LineColor);
					break;

				case DiagramAdditionalObjects.Text:
					VectorAdapter.DrawText ((double)X, (double)Y, additionalObjectsStyles[ObjectNumber].LineName,
						additionalObjectsStyles[ObjectNumber].TextFont,
						additionalObjectsStyles[ObjectNumber].TextFontColor);
					break;
				}
			}

		/// <summary>
		/// Метод формирует файл с изображением диаграммы в векторном формате
		/// </summary>
		/// <param name="VectorAdapter">Адаптер отрисовки векторного изображения</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован;
		/// -3 в случае сбоя инициализации адаптера векторного изображения</returns>
		public int DrawAllDiagrams (IVectorAdapter VectorAdapter)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			// Создание нового файла
			if ((VectorAdapter == null) || (VectorAdapter.InitResult != VectorAdapterInitResults.Opened))
				{
				return -3;
				}

			// Отрисовка изображений
			for (int i = 0; i < curves.Count; i++)
				{
				if (lineStyles[i].AllowDrawing)
					{
					DrawDiagram (lineStyles[i].DiagramImageLeftOffset, lineStyles[i].DiagramImageTopOffset, i, VectorAdapter);
					}
				}

			// Отрисовка дополнительных объектов
			for (int i = 0; i < additionalObjects.Count; i++)
				{
				if (additionalObjectsStyles[i].AllowDrawing)
					{
					DrawObject (additionalObjectsStyles[i].DiagramImageLeftOffset, additionalObjectsStyles[i].DiagramImageTopOffset,
						i, VectorAdapter);
					}
				}

			// Готово
			if (!VectorAdapter.CloseFile ())
				throw new Exception (Localization.GetText ("ExceptionMessage", SupportedLanguages.en_us) + " (8)");
			return 0;
			}

		// Метод сохраняет загруженные экземпляром класса данные и стили в GDD-файл
		private int SaveGDDFile (string DataFileName)
			{
			// Создание файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (DataFileName, FileMode.Create);
				}
			catch
				{
				return -2;
				}

			// Начало записи
			BinaryWriter BW = new BinaryWriter (FS, Encoding.GetEncoding (1251));
			BW.Write ("Geomag data drawer file format. File version: " + ProgramDescription.AssemblyVersion +
				". Creation date: " + DateTime.Now.ToString ("dd.MM.yyyy, HH:mm:ss"));  // Запись версии и даты

			// ЗАПИСЬ БЛОКА ДАННЫХ
			// Запись размерности
			BW.Write ((UInt16)dataValues[0].Count);
			BW.Write ((UInt16)dataValues.Count);

			// Запись имён столбцов
			for (int row = 0; row < dataColumnNames.Count; row++)
				{
				BW.Write (dataColumnNames[row]);
				}

			// Запись исходных данных
			for (int row = 0; row < dataValues[0].Count; row++)
				{
				for (int col = 0; col < dataValues.Count; col++)
					{
					BW.Write (dataValues[col][row]);
					}
				}

			// ЗАПИСЬ ПАРАМЕТРОВ КРИВЫХ
			// Запись количества кривых и номеров столбцов для построения кривых
			BW.Write ((UInt16)curves.Count);

			// ЗАПИСЬ СТИЛЕЙ КРИВЫХ
			for (int i = 0; i < curves.Count; i++)
				{
				if (!FlushStyle (BW, lineStyles[i]))
					{
					BW.Close ();
					FS.Close ();
					return -3;
					}
				}

			// ЗАПИСЬ ПАРАМЕТРОВ ДОПОЛНИТЕЛЬНЫХ ОБЪЕКТОВ
			BW.Write ((UInt16)additionalObjects.Count);

			// ЗАПИСЬ СТИЛЕЙ ДОПОЛНИТЕЛЬНЫХ ОБЪЕКТОВ
			for (int i = 0; i < additionalObjects.Count; i++)
				{
				BW.Write ((UInt16)additionalObjects[i]);

				if (!FlushStyle (BW, additionalObjectsStyles[i]))
					{
					BW.Close ();
					FS.Close ();
					return -3;
					}
				}

			// Завершено
			BW.Close ();
			FS.Close ();
			return 0;
			}

#endif

		/// <summary>
		/// Метод сохраняет загруженные экземпляром класса данные в файл
		/// </summary>
		/// <param name="DataFileName">Путь для сохраняемого файла</param>
		/// <param name="DataFileType">Тип файла данных</param>
		/// <param name="SaveColumnNames">Флаг, требующий сохранения имён столбцов данных</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если не удалось записать файл</returns>
		public int SaveDataFile (string DataFileName, DataOutputTypes DataFileType, bool SaveColumnNames)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			// Выбор варианта сохранения
			switch (DataFileType)
				{
				// Следующие обработки выполняются в данном методе
				case DataOutputTypes.ANY:
					break;

				case DataOutputTypes.CSV:
					break;

				// Следующие обработки выполняются в дополнительных методах
#if !DataProcessingOnly
				case DataOutputTypes.GDD:
					return SaveGDDFile (DataFileName);
#endif

				// Остальные файлы не подлежат обработке
				default:
					return -2;
				}

			// Создание файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (DataFileName, FileMode.Create);
				}
			catch
				{
				return -2;
				}
			StreamWriter SW = new StreamWriter (FS, Encoding.GetEncoding (1251));

			// Запись имён столбцов
			if (SaveColumnNames)
				{
				for (int i = 0; i < dataColumnNames.Count; i++)
					{
					switch (DataFileType)
						{
						case DataOutputTypes.ANY:
							SW.Write (dataColumnNames[i] + anyDataSplitters[1].ToString ());
							break;

						case DataOutputTypes.CSV:
							SW.Write (dataColumnNames[i] + csvSplitters[0].ToString ());
							break;
						}
					}
				SW.Write ("\r\n");
				}

			// ЗАПИСЬ БЛОКА ДАННЫХ
			for (int row = 0; row < dataValues[0].Count; row++)
				{
				for (int col = 0; col < dataValues.Count; col++)
					{
					switch (DataFileType)
						{
						case DataOutputTypes.ANY:
							SW.Write (dataValues[col][row].ToString (cie.NumberFormat) + anyDataSplitters[1].ToString ());
							break;

						case DataOutputTypes.CSV:
							SW.Write (dataValues[col][row].ToString (cir.NumberFormat) + csvSplitters[0].ToString ());
							break;
						}
					}

				SW.Write ("\r\n");
				}

			// Завершено
			SW.Close ();
			FS.Close ();
			return 0;
			}

#if !DataProcessingOnly

		/// <summary>
		/// Метод загружает стили кривых из файла и добавляет указанные в них кривые на диаграмму
		/// </summary>
		/// <param name="StyleFileName">Путь к файлу стиля</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; -2, если входные параметры некорректны;
		/// -3, если файл стиля недоступен; -4, если файл стиля повреждён</returns>
		public int LoadStyle (string StyleFileName)
			{
			return StyleLoadingExecutor (StyleFileName, false, null);
			}

		/// <summary>
		/// Метод загружает стили кривых из файла и применяет их к указанным кривым
		/// </summary>
		/// <param name="LineNumbers">Номера кривых для применения стилей</param>
		/// <param name="StyleFileName">Путь к файлу стиля</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны;
		/// -3, если файл стиля недоступен; -4, если файл стиля повреждён</returns>
		public int LoadStyle (string StyleFileName, ListBox.SelectedIndexCollection LineNumbers)
			{
			return StyleLoadingExecutor (StyleFileName, true, LineNumbers);
			}

		/// <summary>
		/// Метод присваивает заданный стиль указанной кривой
		/// Предназначен для внутреннего использования
		/// </summary>
		/// <param name="SourceStyle">Присваиваемый стиль</param>
		/// <param name="LineOrObjectNumber">Номер кривой, стиль которой требуется заменить</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		internal int LoadStyle (int LineOrObjectNumber, DiagramStyle SourceStyle)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if ((LineOrObjectNumber < 0) || (LineOrObjectNumber > lineStyles.Count + additionalObjects.Count) || (SourceStyle == null))
				{
				return -2;
				}

			// Присвоение
			if (LineOrObjectNumber < lineStyles.Count)
				{
				lineStyles[LineOrObjectNumber] = new DiagramStyle (SourceStyle);

				// Установка настроек (стилевые параметры игнорируются, т.к. конкретная кривая может иметь другие границы)
				lineStyles[LineOrObjectNumber].MinX = curves[LineOrObjectNumber].MinimumX;
				lineStyles[LineOrObjectNumber].MaxX = curves[LineOrObjectNumber].MaximumX;
				lineStyles[LineOrObjectNumber].MinY = curves[LineOrObjectNumber].MinimumY;
				lineStyles[LineOrObjectNumber].MaxY = curves[LineOrObjectNumber].MaximumY;
				}
			else
				{
				additionalObjectsStyles[LineOrObjectNumber - lineStyles.Count] = new DiagramStyle (SourceStyle);
				}

			// Успешно
			return 0;
			}

		// Метод загружает файл стиля и применяет его к имеющимся кривым (Update = true) или добавляет новые кривые (Update = false)
		private int StyleLoadingExecutor (string StyleFileName, bool Update, ListBox.SelectedIndexCollection LineNumbers)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if (Update && ((LineNumbers == null) || (LineNumbers.Count == 0)))
				{
				return -2;
				}

			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (StyleFileName, FileMode.Open);
				}
			catch
				{
				return -3;
				}

			// Файл открыт
			BinaryReader BR = new BinaryReader (FS, Encoding.GetEncoding (1251));

			// Получение количества стилей в файле
			uint stylesCount = 0;
			try
				{
				BR.ReadString ();       // Версия файла
				stylesCount = BR.ReadUInt16 ();
				uint i = 1 / stylesCount;   // В случае нулевого значения вызовет исключение
				}
			catch
				{
				BR.Close ();
				FS.Close ();
				return -4;
				}

			// Обновление стилей (насколько это возможно)
			if (Update)
				{
				for (int i = 0; (i < stylesCount) && (i < LineNumbers.Count); i++)
					{
					// Извлечение стиля
					DiagramStyle style = null;
					if (!EjectStyle (BR, out style))
						{
						BR.Close ();
						FS.Close ();
						return -4;
						}

					// Применение стиля
					if (LineNumbers[i] < lineStyles.Count)
						{
						uint xColumnNumber = lineStyles[LineNumbers[i]].XColumnNumber;
						uint yColumnNumber = lineStyles[LineNumbers[i]].YColumnNumber;

						lineStyles[LineNumbers[i]] = new DiagramStyle (style);

						// Установка настроек (стилевые параметры игнорируются, т.к. конкретная кривая может иметь другие границы и номера столбцов)
						lineStyles[LineNumbers[i]].MinX = curves[LineNumbers[i]].MinimumX;
						lineStyles[LineNumbers[i]].MaxX = curves[LineNumbers[i]].MaximumX;
						lineStyles[LineNumbers[i]].MinY = curves[LineNumbers[i]].MinimumY;
						lineStyles[LineNumbers[i]].MaxY = curves[LineNumbers[i]].MaximumY;
						lineStyles[LineNumbers[i]].XColumnNumber = xColumnNumber;
						lineStyles[LineNumbers[i]].YColumnNumber = yColumnNumber;
						}
					else
						{
						additionalObjectsStyles[LineNumbers[i] - lineStyles.Count] = new DiagramStyle (style);
						}
					}
				}

			// Добавление кривых
			else
				{
				for (int i = 0; i < stylesCount; i++)
					{
					// Извлечение стиля
					DiagramStyle style = null;
					if (!EjectStyle (BR, out style))
						{
						BR.Close ();
						FS.Close ();
						return -4;
						}

					// Если указанные столбцы позволяют добавить кривую
					if (AddDiagram (style.XColumnNumber, style.YColumnNumber) == 0)
						{
						lineStyles[lineStyles.Count - 1] = new DiagramStyle (style);

						// Установка настроек (стилевые параметры игнорируются, т.к. конкретная кривая может иметь другие границы)
						lineStyles[lineStyles.Count - 1].MinX = curves[lineStyles.Count - 1].MinimumX;
						lineStyles[lineStyles.Count - 1].MaxX = curves[lineStyles.Count - 1].MaximumX;
						lineStyles[lineStyles.Count - 1].MinY = curves[lineStyles.Count - 1].MinimumY;
						lineStyles[lineStyles.Count - 1].MaxY = curves[lineStyles.Count - 1].MaximumY;
						}
					}
				}

			BR.Close ();
			FS.Close ();

			// Успешно завершено
			return 0;
			}

		// Метод считывает один стиль из файла, дескриптор чтения которого передаётся как параметр
		private bool EjectStyle (BinaryReader BR, out DiagramStyle LoadedStyle)
			{
			// Инициализация стиля
			LoadedStyle = null;

			// Чтение
			try
				{
				LoadedStyle = new DiagramStyle (0, "0", BR.ReadUInt16 (), BR.ReadUInt16 ());

				LoadedStyle.AxesColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.AxesLinesWidth = BR.ReadUInt16 ();
				LoadedStyle.GridLinesWidth = BR.ReadUInt16 ();
				LoadedStyle.OxPlacement = (AxesPlacements)BR.ReadUInt16 ();
				LoadedStyle.OyPlacement = (AxesPlacements)BR.ReadUInt16 ();
				LoadedStyle.PrimaryGridColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.SecondaryGridColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.SwitchXY = BR.ReadBoolean ();
				LoadedStyle.TextFont = new Font (BR.ReadString (), BR.ReadSingle (), (FontStyle)BR.ReadUInt16 ());
				LoadedStyle.TextFontColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.AxesFont = new Font (BR.ReadString (), BR.ReadSingle (), (FontStyle)BR.ReadUInt16 ());
				LoadedStyle.AxesFontColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.AutoPrimaryDivisions = BR.ReadBoolean ();
				LoadedStyle.XPrimaryDivisions = BR.ReadUInt16 ();
				LoadedStyle.XSecondaryDivisions = BR.ReadUInt16 ();
				LoadedStyle.YPrimaryDivisions = BR.ReadUInt16 ();
				LoadedStyle.YSecondaryDivisions = BR.ReadUInt16 ();
				LoadedStyle.AutoTextOffset = BR.ReadBoolean ();
				LoadedStyle.AllowDrawing = BR.ReadBoolean ();
				LoadedStyle.DiagramImageHeight = BR.ReadUInt16 ();
				LoadedStyle.DiagramImageWidth = BR.ReadUInt16 ();
				LoadedStyle.DiagramImageLeftOffset = BR.ReadUInt16 ();
				LoadedStyle.DiagramImageTopOffset = BR.ReadUInt16 ();
				LoadedStyle.LineColor = Color.FromArgb (BR.ReadInt32 ());
				LoadedStyle.LineDrawingFormat = (DrawingLinesFormats)BR.ReadUInt16 ();
				LoadedStyle.LineMarkerNumber = BR.ReadUInt16 ();
				LoadedStyle.LineName = BR.ReadString ();
				LoadedStyle.LineNameLeftOffset = BR.ReadUInt16 ();
				LoadedStyle.LineNameTopOffset = BR.ReadUInt16 ();
				LoadedStyle.LineWidth = BR.ReadUInt16 ();
				LoadedStyle.MaxX = BR.ReadDouble ();
				LoadedStyle.MaxY = BR.ReadDouble ();
				LoadedStyle.MinX = BR.ReadDouble ();
				LoadedStyle.MinY = BR.ReadDouble ();
				LoadedStyle.OxTextOffset = BR.ReadUInt16 ();
				LoadedStyle.OyTextOffset = BR.ReadUInt16 ();
				LoadedStyle.OxFormat = (NumbersFormat)BR.ReadUInt16 ();
				LoadedStyle.OyFormat = (NumbersFormat)BR.ReadUInt16 ();
				}
			catch
				{
				return false;
				}

			// Успешно
			return true;
			}

		/// <summary>
		/// Метод сохраняет стиль указанной кривой в файл
		/// </summary>
		/// <param name="LineNumbers">Номера кривых, стили которых требуется сохранить</param>
		/// <param name="StyleFileName">Путь для сохраняемого файла стиля</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны;
		/// -3, если не удалось записать файл стиля</returns>
		public int SaveStyle (string StyleFileName, ListBox.SelectedIndexCollection LineNumbers)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				return -1;

			if ((LineNumbers == null) || (LineNumbers.Count == 0))
				return -2;

			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (StyleFileName, FileMode.Create);
				}
			catch
				{
				return -3;
				}

			// Файл открыт
			BinaryWriter BW = new BinaryWriter (FS, Encoding.GetEncoding (1251));
			BW.Write ("Geomag data drawer style file. File version: " + ProgramDescription.AssemblyVersion +
				". Creation date: " + DateTime.Now.ToString ("dd.MM.yyyy, HH:mm:ss"));  // Запись версии и даты

			// Запись числа стилей
			BW.Write ((UInt16)LineNumbers.Count);

			// Сохранение стилей
			for (int i = 0; i < LineNumbers.Count; i++)
				{
				if (LineNumbers[i] < lineStyles.Count)
					{
					if (!FlushStyle (BW, lineStyles[LineNumbers[i]]))
						{
						BW.Close ();    // Функция FlushStyle всегда возвращает true. Этот фрагмент оставлен до лучших времён
						FS.Close ();
						return -4;
						}
					}
				else
					{
					if (!FlushStyle (BW, additionalObjectsStyles[LineNumbers[i] - lineStyles.Count]))
						{
						BW.Close ();
						FS.Close ();
						return -4;
						}
					}
				}

			BW.Close ();
			FS.Close ();

			// Завершено
			return 0;
			}

		// Метод записывает один стиль в файл, дескриптор записи которого передаётся как параметр
		private bool FlushStyle (BinaryWriter BW, DiagramStyle StyleToSave)
			{
			// Запись
			BW.Write ((UInt16)StyleToSave.XColumnNumber);
			BW.Write ((UInt16)StyleToSave.YColumnNumber);

			BW.Write ((Int32)StyleToSave.AxesColor.ToArgb ());
			BW.Write ((UInt16)StyleToSave.AxesLinesWidth);
			BW.Write ((UInt16)StyleToSave.GridLinesWidth);
			BW.Write ((UInt16)StyleToSave.OxPlacement);
			BW.Write ((UInt16)StyleToSave.OyPlacement);
			BW.Write ((Int32)StyleToSave.PrimaryGridColor.ToArgb ());
			BW.Write ((Int32)StyleToSave.SecondaryGridColor.ToArgb ());
			BW.Write (StyleToSave.SwitchXY);
			BW.Write (StyleToSave.TextFont.FontFamily.Name);
			BW.Write (StyleToSave.TextFont.Size);
			BW.Write ((UInt16)StyleToSave.TextFont.Style);
			BW.Write ((Int32)StyleToSave.TextFontColor.ToArgb ());
			BW.Write (StyleToSave.AxesFont.FontFamily.Name);
			BW.Write (StyleToSave.AxesFont.Size);
			BW.Write ((UInt16)StyleToSave.AxesFont.Style);
			BW.Write ((Int32)StyleToSave.AxesFontColor.ToArgb ());
			BW.Write (StyleToSave.AutoPrimaryDivisions);
			BW.Write ((UInt16)StyleToSave.XPrimaryDivisions);
			BW.Write ((UInt16)StyleToSave.XSecondaryDivisions);
			BW.Write ((UInt16)StyleToSave.YPrimaryDivisions);
			BW.Write ((UInt16)StyleToSave.YSecondaryDivisions);
			BW.Write (StyleToSave.AutoTextOffset);
			BW.Write (StyleToSave.AllowDrawing);
			BW.Write ((UInt16)StyleToSave.DiagramImageHeight);
			BW.Write ((UInt16)StyleToSave.DiagramImageWidth);
			BW.Write ((UInt16)StyleToSave.DiagramImageLeftOffset);
			BW.Write ((UInt16)StyleToSave.DiagramImageTopOffset);
			BW.Write ((Int32)StyleToSave.LineColor.ToArgb ());
			BW.Write ((UInt16)StyleToSave.LineDrawingFormat);
			BW.Write ((UInt16)StyleToSave.LineMarkerNumber);
			BW.Write (StyleToSave.LineName);
			BW.Write ((UInt16)StyleToSave.LineNameLeftOffset);
			BW.Write ((UInt16)StyleToSave.LineNameTopOffset);
			BW.Write ((UInt16)StyleToSave.LineWidth);
			BW.Write (StyleToSave.MaxX);
			BW.Write (StyleToSave.MaxY);
			BW.Write (StyleToSave.MinX);
			BW.Write (StyleToSave.MinY);
			BW.Write ((UInt16)StyleToSave.OxTextOffset);
			BW.Write ((UInt16)StyleToSave.OyTextOffset);
			BW.Write ((UInt16)StyleToSave.OxFormat);
			BW.Write ((UInt16)StyleToSave.OyFormat);

			// Вообще стоило бы try добавить. Но я надеюсь, что на переполненном диске
			// эту программу использовать никто не будет
			return true;
			}

		/// <summary>
		/// Метод сбрасывает стиль указанной кривой или указанного объекта к начальным установкам
		/// </summary>
		/// <param name="LineOrObjectNumber">Номер кривой и ил объекта, стиль которых требуется сбросить</param>
		/// <returns>Возвращает 0 в случае успеха; -1, если класс не был успешно инициализирован; 
		/// -2, если входные параметры некорректны</returns>
		public int ResetStyle (uint LineOrObjectNumber)
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return -1;
				}

			if (LineOrObjectNumber >= curves.Count + additionalObjects.Count)
				{
				return -2;
				}

			// Сброс
			if (LineOrObjectNumber < lineStyles.Count)
				{
				lineStyles[(int)LineOrObjectNumber] = new DiagramStyle (LineOrObjectNumber, lineStyles[(int)LineOrObjectNumber].LineName,
					lineStyles[(int)LineOrObjectNumber].XColumnNumber, lineStyles[(int)LineOrObjectNumber].YColumnNumber);

				// Установка настроек
				lineStyles[(int)LineOrObjectNumber].MinX = curves[(int)LineOrObjectNumber].MinimumX;
				lineStyles[(int)LineOrObjectNumber].MaxX = curves[(int)LineOrObjectNumber].MaximumX;
				lineStyles[(int)LineOrObjectNumber].MinY = curves[(int)LineOrObjectNumber].MinimumY;
				lineStyles[(int)LineOrObjectNumber].MaxY = curves[(int)LineOrObjectNumber].MaximumY;
				}
			else
				{
				additionalObjectsStyles[(int)LineOrObjectNumber - lineStyles.Count] = new DiagramStyle (LineOrObjectNumber,
					additionalObjectsStyles[(int)LineOrObjectNumber - lineStyles.Count].LineName, 0, 0);
				}

			// Завершено
			return 0;
			}

		/// <summary>
		/// Метод возвращает исходный массив данных в виде, доступном для DataGridView
		/// </summary>
		/// <returns>Возвращает массив данных или null, если класс не был корректно инициализирован</returns>
		public DataTable GetDataTable ()
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				return null;

			DataTable table = new DataTable ();
			for (int col = 0; col < dataValues.Count; col++)
				{
				try
					{
					table.Columns.Add (new DataColumn (dataColumnNames[col], typeof (double)));
					}
				catch
					{
					table.Columns.Add (new DataColumn ("C" + col.ToString ("D02"), typeof (double)));
					}
				}

			// Исходный массив данных
			if (this.dataValues != null)
				{
				for (int row = 0; row < dataValues[0].Count; row++)
					{
					List<object> newRow = new List<object> ();
					for (int col = 0; col < dataValues.Count; col++)
						{
						newRow.Add (dataValues[col][row]);
						}
					table.Rows.Add ();
					table.Rows[table.Rows.Count - 1].ItemArray = newRow.ToArray ();
					}
				}

			// Завершение
			return table;
			}


#endif

		/// <summary>
		/// Метод возвращает исходный массив данных
		/// </summary>
		/// <returns>Возвращает массив данных или null, если класс не был корректно инициализирован</returns>
		public List<List<double>> GetData ()
			{
			// Контроль состояния
			if (initResult != DiagramDataInitResults.Ok)
				{
				return null;
				}

			return new List<List<double>> (dataValues);
			}

		// Метод подготавливает строку данных к обработке (замещает все недопустимые символы
		// штатными разделителями)
		private string PrepareDataLine (string SourceValue, bool RemoveSplitters)
			{
			string result = "";

			// Формирование строки с допустимыми символами
			for (int i = 0; i < SourceValue.Length; i++)
				{
				if (Char.IsDigit (SourceValue[i]) || (SourceValue[i] == 'E') || (SourceValue[i] == 'N') ||
					(SourceValue[i] == 'a') || (SourceValue[i] == '.') || (SourceValue[i] == '-') ||
					(SourceValue[i] == '/'))
					result += SourceValue[i];

				else if (SourceValue[i] == ',')
					result += ".";

				else if (SourceValue[i] == 'e')
					result += "E";

				else if (SourceValue[i] == 'n')
					result += "N";

				else if (SourceValue[i] == 'A')
					result += "a";

				else
					result += (RemoveSplitters ? "" : anyDataSplitters[0].ToString ());
				}

			return result;
			}

		// Метод подготавливает строковое значение к преобразованию в число
		private string PrepareDataValue (string SourceValue, bool UseFilter)
			{
			// Формирование строки только с подходящими символами с удалением разделителей
			string result = (UseFilter ? PrepareDataLine (SourceValue, true) : SourceValue);

			// Обработка случая с датой (отсечка неподходящих по длине или составу строк)
			if ((result.Length > 10) || (result.Length < 7))    // Длина поля даты
				return result;

			if (result.Contains ("E") || result.Contains ("N") || result.Contains ("a") || result.Contains ("-"))
				return result;

			// Контроль количества элементов
			string[] values = result.Split (dateSplitters, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length != 3)
				return result;

			// Сборка даты в представление в виде года с дробной частью
			DateTime dt;
			if (result.Contains (dateSplitters[0].ToString ())) // Дата в формате ДД.ММ.ГГ[ГГ]
				try
					{
					dt = DateTime.Parse (result, cir.DateTimeFormat);
					}
				catch
					{
					return result;
					}

			else // if (result.Contains (dateSplitters[1].ToString ())) // Дата в формате ММ/ДД/ГГ[ГГ]
				try
					{
					dt = DateTime.Parse (result, cie.DateTimeFormat);
					}
				catch
					{
					return result;
					}

			return ((double)dt.Year + (dt.DayOfYear - 1) / (dt.Year % 4 == 0 ? 366.0 : 365.0)).ToString (cie.NumberFormat);
			}

		// Метод разворачивает значение даты из рационального числа
		private string DecompressDateValue (double DoubleValue)
			{
			try
				{
				DateTime d2 = new DateTime ((int)DoubleValue, 1, 1);
				d2 = d2.AddDays ((DoubleValue - (int)DoubleValue) * (d2.Year % 4 == 0 ? 366 : 365));
				return d2.ToString ("dd.MM.yyyy");
				}
			catch
				{
				return "00.00.0000";
				}
			}

		/*#if !DataProcessingOnly

				// Методы выполняют преобразование значения в дату
				private long MakeDateFromEdgeValue (double Value)
					{
					int d = (int)((long)Value % 100);
					int m = (int)(((long)Value / 100) % 100);
					int y = (int)(((long)Value / 10000) % 10000);

					try
						{
						return new DateTime (y, m, d).ToFileTime ();
						}
					catch
						{
						return 0;
						}
					}

		#endif*/
		}
	}
