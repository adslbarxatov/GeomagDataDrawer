namespace GeomagDataDrawer
	{
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
	}
