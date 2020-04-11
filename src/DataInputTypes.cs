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
	}
