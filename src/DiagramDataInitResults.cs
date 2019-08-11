namespace GeomagDataDrawer
	{
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

	/// <summary>
	/// Класс предоставляет метод, формирующий сообщения об ошибках
	/// </summary>
	public static class DiagramDataInitResultsMessage
		{
		/// <summary>
		/// Метод формирует сообщение об ошибке
		/// </summary>
		/// <param name="Result">Результат инициализации класса</param>
		/// <param name="Language">Язык локализации</param>
		/// <returns>Сообщение об ошибке</returns>
		public static string ErrorMessage (DiagramDataInitResults Result, SupportedLanguages Language)
			{
			switch (Result)
				{
				case DiagramDataInitResults.NotInited:
					return LanguageProvider.GetText ("NotInitedError", Language);

				case DiagramDataInitResults.FileNotAvailable:
					return LanguageProvider.GetText ("FileNotAvailableError", Language);

				case DiagramDataInitResults.BrokenFile:
					return LanguageProvider.GetText ("BrokenFileError", Language);

				case DiagramDataInitResults.NotEnoughData:
					return LanguageProvider.GetText ("NotEnoughDataError", Language);

				case DiagramDataInitResults.BrokenTable:
					return LanguageProvider.GetText ("BrokenTableError", Language);

				case DiagramDataInitResults.ExcelNotAvailable:
					return LanguageProvider.GetText ("ExcelNotAvailableError", Language);

				default:	// В том числе - OK
					return "";
				}
			}
		}
	}
