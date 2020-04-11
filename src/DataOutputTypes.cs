namespace RD_AAOW
	{
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
	}
