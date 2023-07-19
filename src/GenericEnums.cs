namespace RD_AAOW
	{
	/// <summary>
	/// Виды входных файлов данных
	/// </summary>
	public enum DataInputTypes
		{
		/// <summary>
		/// Собственный файл данных программы
		/// </summary>
		GDD = 1,

		/// <summary>
		/// Файлы Microsoft Office Excel '97 и '07
		/// </summary>
		XLS = 2,

		/// <summary>
		/// Файлы табличных данных Windows CSV
		/// </summary>
		CSV = 3,

		/// <summary>
		/// Файл неизвестной структуры (извлечение доступных данных)
		/// </summary>
		Unknown = 4,

		/// <summary>
		/// Файл не указан (используется для поддержки запуска из командной строки)
		/// </summary>
		Unspecified = -1
		}

	/// <summary>
	/// Виды выходных файлов данных
	/// </summary>
	public enum DataOutputTypes
		{
		/// <summary>
		/// Собственный файл данных программы
		/// </summary>
		GDD = 1,

		/// <summary>
		/// Файлы с произвольным набором данных
		/// </summary>
		ANY = 2,

		/// <summary>
		/// Файлы табличных данных Windows CSV
		/// </summary>
		CSV = 3
		}

	/// <summary>
	/// Виды выходных файлов изображений
	/// </summary>
	public enum ImageOutputTypes
		{
		/// <summary>
		/// PNG
		/// </summary>
		PNG = 4,

		/// <summary>
		/// SVG
		/// </summary>
		SVG = 5,

		/// <summary>
		/// EMF
		/// </summary>
		EMF = 6
		}

	/// <summary>
	/// Перечисление описывает возможные типы дополнительных объектов диаграммы
	/// </summary>
	public enum DiagramAdditionalObjects
		{
		/// <summary>
		/// Линия (старый вариант)
		/// </summary>
		OldLine = 0,

		/// <summary>
		/// Диагональная линия «северо-запад – юго-восток»
		/// </summary>
		LineNWtoSE = 6,

		/// <summary>
		/// Диагональная линия «юго-запад – северо-восток»
		/// </summary>
		LineSWtoNE = 7,

		/// <summary>
		/// Горизонтальная линия
		/// </summary>
		LineH = 8,

		/// <summary>
		/// Вертикальная линия
		/// </summary>
		LineV = 9,

		/// <summary>
		/// Прямоугольник
		/// </summary>
		Rectangle = 1,

		/// <summary>
		/// Заполненный прямоугольник
		/// </summary>
		FilledRectangle = 2,

		/// <summary>
		/// Эллипс
		/// </summary>
		Ellipse = 3,

		/// <summary>
		/// Заполненный эллипс
		/// </summary>
		FilledEllipse = 4,

		/// <summary>
		/// Текстовая строка
		/// </summary>
		Text = 5
		}

	/// <summary>
	/// Перечисление результатов инициализации класса-описателя диаграммы
	/// </summary>
	public enum DiagramDataInitResults
		{
		/// <summary>
		/// Файл данных успешно загружен
		/// </summary>
		Ok = 0,

		/// <summary>
		/// Файл данных не найден или недоступен
		/// </summary>
		FileNotAvailable = -1,

		/// <summary>
		/// Файл данных имеет некорректную структуру
		/// </summary>
		BrokenFile = -2,

		/// <summary>
		/// Файл данных содержит недостаточное число строк и/или столбцов
		/// </summary>
		NotEnoughData = -3,

		/// <summary>
		/// Некоторые значения в таблице некорректны
		/// </summary>
		BrokenTable = -11,

		/// <summary>
		/// Использование библиотеки поддержки файлов Excel не разрешено
		/// </summary>
		ExcelNotAvailable = -21,

		/// <summary>
		/// Класс-описатель диаграммы не инициализирован
		/// </summary>
		NotInited = 1
		}

	/*
	/// <summary>
	/// Класс предоставляет метод, формирующий сообщения об ошибках
	/// </summary>
	public static class DiagramDataInitResultsMessage
		{
		/// <summary>
		/// Метод формирует сообщение об ошибке
		/// </summary>
		/// <param name="Result">Результат инициализации класса</param>
		/// <returns>Сообщение об ошибке</returns>
		public static string ErrorMessage (DiagramDataInitResults Result)
			{
			switch (Result)
				{
				case DiagramDataInitResults.NotInited:
					return Localization.GetText ("NotInitedError");

				case DiagramDataInitResults.FileNotAvailable:
					return Localization.GetText ("FileNotAvailableError");

				case DiagramDataInitResults.BrokenFile:
					return Localization.GetText ("BrokenFileError");

				case DiagramDataInitResults.NotEnoughData:
					return Localization.GetText ("NotEnoughDataError");

				case DiagramDataInitResults.BrokenTable:
					return Localization.GetText ("BrokenTableError");

				case DiagramDataInitResults.ExcelNotAvailable:
					return Localization.GetText ("ExcelNotAvailableError");

				default:    // В том числе - OK
					return "";
				}
			}
		}
	*/

	/// <summary>
	/// Возможные форматы отрисовки кривых
	/// </summary>
	public enum DrawingLinesFormats
		{
		/// <summary>
		/// Линия без маркеров
		/// </summary>
		OnlyLine = 0,

		/// <summary>
		/// Линия с маркерами
		/// </summary>
		LineWithMarkers = 1,

		/// <summary>
		/// Только маркеры
		/// </summary>
		OnlyMarkers = 2
		}

	/// <summary>
	/// Возможные виды представления чисел на диаграмме
	/// </summary>
	public enum NumbersFormat
		{
		/// <summary>
		/// Нормальный
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Экспоненциальный
		/// </summary>
		Exponential = 1,

		/// <summary>
		/// Дата
		/// </summary>
		Date = 2
		}

	/// <summary>
	/// Возможные расположения осей
	/// </summary>
	public enum AxesPlacements
		{
		/// <summary>
		/// Автоматическое (в точке 0 или со стороны нуля другой оси)
		/// </summary>
		Auto = 0,

		/// <summary>
		/// Слева или сверху
		/// </summary>
		LeftTop = 1,

		/// <summary>
		/// Справа или снизу
		/// </summary>
		RightBottom = 2,

		/// <summary>
		/// По центру или посередине
		/// </summary>
		MiddleCenter = 3
		}

	/// <summary>
	/// Возможные результаты инициализации класса
	/// </summary>
	public enum VectorAdapterInitResults
		{
		/// <summary>
		/// Класс не инициализирован
		/// </summary>
		NotInited = 1,

		/// <summary>
		/// Не удалось создать файл
		/// </summary>
		CannotCreateFile = -1,

		/// <summary>
		/// Некорректный размер изображения
		/// </summary>
		IncorrectImageSize = -2,

		/// <summary>
		/// Файл успешно открыт. Редактирование доступно
		/// </summary>
		Opened = 0,

		/// <summary>
		/// Запись файла завершена. Редактирование недоступно
		/// </summary>
		Closed = 2
		}
	}
