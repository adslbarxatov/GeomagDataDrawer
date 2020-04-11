using System;
using System.Drawing;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс-описатель стиля диаграммы
	/// </summary>
	public class DiagramStyle
		{
		/// <summary>
		/// Конструктор. Формирует стандартный стиль
		/// </summary>
		/// <param name="LineNumber">Номер кривой (используется для задания начального цвета)</param>
		/// <param name="PLineName">Имя создаваемой кривой</param>
		/// <param name="SourceXColumnNumber">Номер столбца абсцисс, использованного при создании кривой</param>
		/// <param name="SourceYColumnNumber">Номер столбца ординат, использованного при создании кривой</param>
		public DiagramStyle (uint LineNumber, string PLineName, uint SourceXColumnNumber, uint SourceYColumnNumber)
			{
			// Номера столбцов данных
			xColumnNumber = SourceXColumnNumber;
			yColumnNumber = SourceYColumnNumber;

			// Диаграмма отрисовывается
			allowDrawing = true;

			// Число основных засечек определяется пользователем
			autoPrimaryDivisions = true;

			// 5 основных делений (6 засечек) и по 2 дополнительных (по 1 промежуточной засечке)
			xPrimaryDivisions = yPrimaryDivisions = 5;
			xSecondaryDivisions = ySecondaryDivisions = 2;

			// Шрифт Arial, 10 пт, обычный
			textFont = new Font ("Arial", 9.0f, FontStyle.Regular);

			// Шрифт Arial, 8 пт, обычный
			axesFont = new Font ("Arial", 8.0f, FontStyle.Regular);

			// Чёрные оси, засечки и подписи
			axesColor = textFontColor = axesFontColor = Color.FromArgb (0, 0, 0);

			// Различные цвета для последовательно добавляемых кривых
			switch (LineNumber % 13)
				{
				case 0:
					lineColor = Color.FromArgb (255, 0, 0);
					break;

				case 1:
					lineColor = Color.FromArgb (0, 224, 0);
					break;

				case 2:
					lineColor = Color.FromArgb (0, 0, 255);
					break;

				case 3:
					lineColor = Color.FromArgb (255, 128, 0);
					break;

				case 4:
					lineColor = Color.FromArgb (192, 192, 0);
					break;

				case 5:
					lineColor = Color.FromArgb (0, 255, 128);
					break;

				case 6:
					lineColor = Color.FromArgb (0, 224, 224);
					break;

				case 7:
					lineColor = Color.FromArgb (0, 128, 255);
					break;

				case 8:
					lineColor = Color.FromArgb (255, 0, 255);
					break;

				case 9:
					lineColor = Color.FromArgb (128, 0, 0);
					break;

				case 10:
					lineColor = Color.FromArgb (0, 128, 0);
					break;

				case 11:
					lineColor = Color.FromArgb (0, 0, 128);
					break;

				case 12:
					lineColor = Color.FromArgb (100, 100, 100);
					break;
				}

			// Толщина кривых, осей и линий сетки - 1 px
			axesLinesWidth = lineWidth = gridLinesWidth = 1;

			// Стандартный размер изображения - квадрат со стороной 500 px
			diagramImageWidth = diagramImageHeight = 500;

			// Без смещения относительно верхнего левого угла
			diagramImageLeftOffset = diagramImageTopOffset = 0;

			// Границы диапазона построения по умолчанию сброшены
			minX = maxX = 0.0f;
			minY = maxY = 0.0;

			// Цвет сетки по умолчанию совпадает с цветом фона (белый)
			primaryGridColor = secondaryGridColor = imageBackColor = Color.FromArgb (0, 255, 255, 255);

			// Номера маркеров для всех линий - 1 (квадратик)
			lineMarkerNumber = 1;

			// Диаграмма не транспонирована
			switchXY = false;

			// Отрисовываются кривые без маркеров
			lineDrawingFormat = DrawingLinesFormats.OnlyLine;

			// Автоматическое расположение осей
			oxPlacement = oyPlacement = AxesPlacements.Auto;

			// Расположение подписей к осям
			lineNameLeftOffset = (uint)(diagramImageWidth * 0.4);
			lineNameTopOffset = (uint)(diagramImageWidth * MaxLineNameTopMargin);
			lineName = PLineName;
			autoTextOffset = true;

			// Представление подписей на осях
			oxFormat = oyFormat = NumbersFormat.Normal;
			}

		/// <summary>
		/// Конструктор. Копирует указанный стиль и создаёт новый экземпляр
		/// </summary>
		/// <param name="OldStyle">Исходный стиль</param>
		public DiagramStyle (DiagramStyle OldStyle)
			{
			this.xColumnNumber = OldStyle.xColumnNumber;
			this.yColumnNumber = OldStyle.yColumnNumber;

			this.allowDrawing = OldStyle.allowDrawing;
			this.autoTextOffset = OldStyle.autoTextOffset;
			this.axesColor = Color.FromArgb (OldStyle.axesColor.ToArgb ());
			this.axesLinesWidth = OldStyle.axesLinesWidth;
			this.diagramImageHeight = OldStyle.diagramImageHeight;
			this.diagramImageWidth = OldStyle.diagramImageWidth;
			this.diagramImageLeftOffset = OldStyle.diagramImageLeftOffset;
			this.diagramImageTopOffset = OldStyle.diagramImageTopOffset;
			this.gridLinesWidth = OldStyle.gridLinesWidth;
			this.lineColor = Color.FromArgb (OldStyle.lineColor.ToArgb ());
			this.lineDrawingFormat = OldStyle.lineDrawingFormat;
			this.lineMarkerNumber = OldStyle.lineMarkerNumber;
			this.lineName = OldStyle.lineName;
			this.lineNameLeftOffset = OldStyle.lineNameLeftOffset;
			this.lineNameTopOffset = OldStyle.lineNameTopOffset;
			this.lineWidth = OldStyle.lineWidth;
			this.maxX = OldStyle.maxX;
			this.maxY = OldStyle.maxY;
			this.minX = OldStyle.minX;
			this.minY = OldStyle.minY;
			this.oxPlacement = OldStyle.oxPlacement;
			this.oyPlacement = OldStyle.oyPlacement;
			this.oxTextOffset = OldStyle.oxTextOffset;
			this.oyTextOffset = OldStyle.oyTextOffset;
			this.primaryGridColor = Color.FromArgb (OldStyle.primaryGridColor.ToArgb ());
			this.secondaryGridColor = Color.FromArgb (OldStyle.secondaryGridColor.ToArgb ());
			this.switchXY = OldStyle.switchXY;
			this.textFont = new Font (OldStyle.textFont.FontFamily.Name, OldStyle.textFont.Size, OldStyle.textFont.Style);
			this.textFontColor = Color.FromArgb (OldStyle.textFontColor.ToArgb ());
			this.axesFont = new Font (OldStyle.axesFont.FontFamily.Name, OldStyle.axesFont.Size, OldStyle.axesFont.Style);
			this.axesFontColor = Color.FromArgb (OldStyle.axesFontColor.ToArgb ());
			this.autoPrimaryDivisions = OldStyle.autoPrimaryDivisions;
			this.xPrimaryDivisions = OldStyle.xPrimaryDivisions;
			this.xSecondaryDivisions = OldStyle.xSecondaryDivisions;
			this.yPrimaryDivisions = OldStyle.yPrimaryDivisions;
			this.ySecondaryDivisions = OldStyle.ySecondaryDivisions;
			this.oxFormat = OldStyle.oxFormat;
			this.oyFormat = OldStyle.oyFormat;
			}

		#region Общие параметры диаграммы

		/// <summary>
		/// Возвращает или задаёт номер столбца абсцисс, использованного при создании кривой
		/// </summary>
		public uint XColumnNumber
			{
			get
				{
				return xColumnNumber;
				}
			set
				{
				xColumnNumber = value;
				}
			}
		private uint xColumnNumber;

		/// <summary>
		/// Возвращает или задаёт номер столбца ординат, использованного при создании кривой
		/// </summary>
		public uint YColumnNumber
			{
			get
				{
				return yColumnNumber;
				}
			set
				{
				yColumnNumber = value;
				}
			}
		private uint yColumnNumber;

		/// <summary>
		/// Возвращает фоновый цвет диаграммы
		/// </summary>
		public static Color ImageBackColor
			{
			get
				{
				return imageBackColor;
				}
			}
		private static Color imageBackColor;

		/// <summary>
		/// Указывает, следует ли отрисовывать диаграмму
		/// </summary>
		public bool AllowDrawing
			{
			get
				{
				return allowDrawing;
				}
			set
				{
				allowDrawing = value;
				}
			}
		private bool allowDrawing;

		/// <summary>
		/// Указывает, следует ли транспонировать диаграмму
		/// </summary>
		public bool SwitchXY
			{
			get
				{
				return switchXY;
				}
			set
				{
				switchXY = value;
				}
			}
		private bool switchXY;

		/// <summary>
		/// Формат отрисовки кривой
		/// </summary>
		public DrawingLinesFormats LineDrawingFormat
			{
			get
				{
				return lineDrawingFormat;
				}
			set
				{
				lineDrawingFormat = value;
				}
			}
		private DrawingLinesFormats lineDrawingFormat;

		/// <summary>
		/// Маркер для кривой
		/// </summary>
		public uint LineMarkerNumber
			{
			get
				{
				return lineMarkerNumber;
				}
			set
				{
				lineMarkerNumber = value;
				}
			}
		private uint lineMarkerNumber;

		#endregion

		#region Размеры изображения и смещение диаграммы
		/// <summary>
		/// Минимально допустимая ширина изображения диаграммы
		/// </summary>
		public const uint MinImageWidth = 10;

		/// <summary>
		/// Минимально допустимая высота изображения диаграммы
		/// </summary>
		public const uint MinImageHeight = 10;

		/// <summary>
		/// Максимально допустимая ширина изображения диаграммы
		/// </summary>
		public const uint MaxImageWidth = 10000;

		/// <summary>
		/// Максимально допустимая высота изображения диаграммы
		/// </summary>
		public const uint MaxImageHeight = 10000;

		/// <summary>
		/// Задаёт или возвращает ширину изображения диаграммы
		/// </summary>
		public uint DiagramImageWidth
			{
			get
				{
				return diagramImageWidth;
				}
			set
				{
				diagramImageWidth = value;

				if (diagramImageWidth < MinImageWidth)
					{
					diagramImageWidth = MinImageWidth;
					}
				if (diagramImageWidth > MaxImageWidth)
					{
					diagramImageWidth = MaxImageWidth;
					}

				// Принудительное обновление зависимых параметров
				this.OyTextOffset = oyTextOffset;
				this.LineNameLeftOffset = lineNameLeftOffset;
				}
			}
		private uint diagramImageWidth;

		/// <summary>
		/// Задаёт или возвращает высоту изображения диаграммы
		/// </summary>
		public uint DiagramImageHeight
			{
			get
				{
				return diagramImageHeight;
				}
			set
				{
				diagramImageHeight = value;

				if (diagramImageHeight < MinImageHeight)
					{
					diagramImageHeight = MinImageHeight;
					}
				if (diagramImageHeight > MaxImageHeight)
					{
					diagramImageHeight = MaxImageHeight;
					}

				// Принудительное обновление зависимых параметров
				this.OxTextOffset = oxTextOffset;
				this.LineNameTopOffset = lineNameTopOffset;
				}
			}
		private uint diagramImageHeight;

		/// <summary>
		/// Задаёт или возвращает смещение изображения от левого края конечного изображения
		/// </summary>
		public uint DiagramImageLeftOffset
			{
			get
				{
				if (diagramImageLeftOffset > MaxImageWidth - diagramImageWidth)
					{
					diagramImageLeftOffset = MaxImageWidth - diagramImageWidth;
					}

				return diagramImageLeftOffset;
				}
			set
				{
				// Благодаря типу данных не может быть меньше нуля
				if (value > MaxImageWidth - diagramImageWidth)
					{
					diagramImageLeftOffset = MaxImageWidth - diagramImageWidth;
					}
				else
					{
					diagramImageLeftOffset = value;
					}
				}
			}
		private uint diagramImageLeftOffset;

		/// <summary>
		/// Задаёт или возвращает смещение изображения от верхнего края конечного изображения
		/// </summary>
		public uint DiagramImageTopOffset
			{
			get
				{
				if (diagramImageTopOffset > MaxImageHeight - diagramImageHeight)
					{
					diagramImageTopOffset = MaxImageHeight - diagramImageHeight;
					}

				return diagramImageTopOffset;
				}
			set
				{
				// Благодаря типу данных не может быть меньше нуля
				if (value > MaxImageHeight - diagramImageHeight)
					{
					diagramImageTopOffset = MaxImageHeight - diagramImageHeight;
					}
				else
					{
					diagramImageTopOffset = value;
					}
				}
			}
		private uint diagramImageTopOffset;
		#endregion

		#region Диапазон построения диаграммы
		/// <summary>
		/// Задаёт или возвращает максимальную абсциссу диапазона построения
		/// </summary>
		public double MaxX
			{
			get
				{
				return maxX;
				}
			set
				{
				maxX = value;
				}
			}
		private double maxX;

		/// <summary>
		/// Задаёт или возвращает минимальную абсциссу диапазона построения
		/// </summary>
		public double MinX
			{
			get
				{
				return minX;
				}
			set
				{
				minX = value;
				}
			}
		private double minX;

		/// <summary>
		/// Задаёт или возвращает максимальную ординату диапазона построения
		/// </summary>
		public double MaxY
			{
			get
				{
				return maxY;
				}
			set
				{
				maxY = value;
				}
			}
		private double maxY;

		/// <summary>
		/// Задаёт или возвращает минимальную ординату диапазона построения
		/// </summary>
		public double MinY
			{
			get
				{
				return minY;
				}
			set
				{
				minY = value;
				}
			}
		private double minY;
		#endregion

		#region Толщина линий диаграммы
		/// <summary>
		/// Минимально допустимая толщина линий диаграммы
		/// </summary>
		public const uint MinLineWidth = 1;

		/// <summary>
		/// Максимально допустимая толщина линий диаграммы
		/// </summary>
		public const uint MaxLineWidth = 10;

		/// <summary>
		/// Задаёт или возвращает толщину осей диаграммы
		/// </summary>
		public uint AxesLinesWidth
			{
			get
				{
				return axesLinesWidth;
				}
			set
				{
				axesLinesWidth = value;

				if (axesLinesWidth < MinLineWidth)
					{
					axesLinesWidth = MinLineWidth;
					}
				if (axesLinesWidth > MaxLineWidth)
					{
					axesLinesWidth = MaxLineWidth;
					}
				}
			}
		private uint axesLinesWidth;

		/// <summary>
		/// Задаёт или возвращает толщину линий сетки
		/// </summary>
		public uint GridLinesWidth
			{
			get
				{
				return gridLinesWidth;
				}
			set
				{
				gridLinesWidth = value;

				if (gridLinesWidth < MinLineWidth)
					{
					gridLinesWidth = MinLineWidth;
					}
				if (gridLinesWidth > MaxLineWidth)
					{
					gridLinesWidth = MaxLineWidth;
					}
				}
			}
		private uint gridLinesWidth;

		/// <summary>
		/// Задаёт или возвращает толщину линии кривой
		/// </summary>
		public uint LineWidth
			{
			get
				{
				return lineWidth;
				}
			set
				{
				lineWidth = value;

				if (lineWidth < MinLineWidth)
					{
					lineWidth = MinLineWidth;
					}
				if (lineWidth > MaxLineWidth)
					{
					lineWidth = MaxLineWidth;
					}
				}
			}
		private uint lineWidth;
		#endregion

		#region Цвета диаграммы
		/// <summary>
		/// Задаёт или возвращает цвет осей диаграммы
		/// </summary>
		public Color AxesColor
			{
			get
				{
				return axesColor;
				}
			set
				{
				axesColor = value;

				if (axesColor.ToArgb () == Color.FromArgb (255, 255, 255).ToArgb ())
					{
					axesColor = imageBackColor;
					}
				}
			}
		private Color axesColor;

		/// <summary>
		/// Задаёт или возвращает цвет основных линий сетки
		/// </summary>
		public Color PrimaryGridColor
			{
			get
				{
				return primaryGridColor;
				}
			set
				{
				primaryGridColor = value;

				if (primaryGridColor.ToArgb () == Color.FromArgb (255, 255, 255).ToArgb ())
					{
					primaryGridColor = imageBackColor;
					}
				}
			}
		private Color primaryGridColor;

		/// <summary>
		/// Задаёт или возвращает цвет дополнительных линий сетки
		/// </summary>
		public Color SecondaryGridColor
			{
			get
				{
				return secondaryGridColor;
				}
			set
				{
				secondaryGridColor = value;

				if (secondaryGridColor.ToArgb () == Color.FromArgb (255, 255, 255).ToArgb ())
					{
					secondaryGridColor = imageBackColor;
					}
				}
			}
		private Color secondaryGridColor;

		/// <summary>
		/// Задаёт или возвращает цвет кривой
		/// </summary>
		public Color LineColor
			{
			get
				{
				return lineColor;
				}
			set
				{
				lineColor = value;
				}
			}
		private Color lineColor;
		#endregion

		#region Шрифт и цвет подписей
		/// <summary>
		/// Максимально допустимый размер шрифта для подписей диаграммы
		/// </summary>
		public const uint MaxFontSize = 36;

		/// <summary>
		/// Минимально допустимый размер шрифта для подписей диаграммы
		/// </summary>
		public const uint MinFontSize = 4;

		/// <summary>
		/// Задаёт или возвращает шрифт для подписей диаграммы
		/// </summary>
		public Font TextFont
			{
			get
				{
				return textFont;
				}
			set
				{
				textFont = value;
				}
			}
		private Font textFont;

		/// <summary>
		/// Задаёт или возвращает шрифт для подписей осей диаграммы
		/// </summary>
		public Font AxesFont
			{
			get
				{
				return axesFont;
				}
			set
				{
				axesFont = value;
				}
			}
		private Font axesFont;

		/// <summary>
		/// Задаёт или возвращает цвет для подписей диаграммы
		/// </summary>
		public Color TextFontColor
			{
			get
				{
				return textFontColor;
				}
			set
				{
				textFontColor = value;
				}
			}
		private Color textFontColor;

		/// <summary>
		/// Задаёт или возвращает цвет для подписей осей диаграммы
		/// </summary>
		public Color AxesFontColor
			{
			get
				{
				return axesFontColor;
				}
			set
				{
				axesFontColor = value;
				}
			}
		private Color axesFontColor;
		#endregion

		#region Основные и дополнительные засечки на осях
		/// <summary>
		/// Минимально допустимое число основных делений
		/// </summary>
		public const uint MinPrimaryDivisions = 1;

		/// <summary>
		/// Минимально допустимое число подразделений основных делений
		/// </summary>
		public const uint MinSecondaryDivisions = 1;

		/// <summary>
		/// Максиимально допустимое число основных делений
		/// </summary>
		public const uint MaxPrimaryDivisions = 100;

		/// <summary>
		/// Максимально допустимое число подразделений основных делений
		/// </summary>
		public const uint MaxSecondaryDivisions = 10;

		// Метод рассчитывает оптимальное число засечек на оси (результат - от 4 до 10)
		private uint GetPrimaryDivisions (double MinValue, double MaxValue)
			{
			// Вычисление числа единичных делений на оси
			double roundPosition = 10.0;
			for (; roundPosition >= -10.0; roundPosition -= 1.0)
				{
				if (((int)((int)(MinValue / Math.Pow (10.0, roundPosition)) * Math.Pow (10.0, roundPosition)) == (int)MinValue) &&
					((int)((int)(MaxValue / Math.Pow (10.0, roundPosition)) * Math.Pow (10.0, roundPosition)) == (int)MaxValue))
					{
					//roundPosition += 1.0;
					break;
					}
				}
			uint length = (uint)(Math.Abs (MinValue - MaxValue) * Math.Pow (10.0, -roundPosition));

			// Считаем 10 оптимальным для случая, когда length не позволяет использовать другие варианты
			uint result = 10;

			// Ищем другие подходящие варианты (начиная с большего)
			for (uint i = result; i > 3; i--)
				{
				if ((uint)(length / i) * i == length)
					{
					result = i;
					break;
					}
				}

			return result;
			}

		/// <summary>
		/// Указывает, следует ли автоматически рассчитывать число основных засечек на осях
		/// или необходимо использовать пользовательские настройки
		/// </summary>
		public bool AutoPrimaryDivisions
			{
			get
				{
				return autoPrimaryDivisions;
				}
			set
				{
				autoPrimaryDivisions = value;
				}
			}
		private bool autoPrimaryDivisions;

		/// <summary>
		/// Задаёт или возвращает число основных делений на оси Ox (число засечек - 1)
		/// </summary>
		public uint XPrimaryDivisions
			{
			get
				{
				if (autoPrimaryDivisions)
					{
					return GetPrimaryDivisions (minX, maxX);
					}
				return xPrimaryDivisions;
				}
			set
				{
				xPrimaryDivisions = value;

				if (xPrimaryDivisions < MinPrimaryDivisions)
					{
					xPrimaryDivisions = MinPrimaryDivisions;
					}
				if (xPrimaryDivisions > MaxPrimaryDivisions)
					{
					xPrimaryDivisions = MaxPrimaryDivisions;
					}
				}
			}
		private uint xPrimaryDivisions;

		/// <summary>
		/// Задаёт или возвращает число подразделений основных делений на оси Ox (число засечек + 1)
		/// </summary>
		public uint XSecondaryDivisions
			{
			get
				{
				return xSecondaryDivisions;
				}
			set
				{
				xSecondaryDivisions = value;

				if (xSecondaryDivisions < MinSecondaryDivisions)
					{
					xSecondaryDivisions = MinSecondaryDivisions;
					}
				if (xSecondaryDivisions > MaxSecondaryDivisions)
					{
					xSecondaryDivisions = MaxSecondaryDivisions;
					}
				}
			}
		private uint xSecondaryDivisions;

		/// <summary>
		/// Задаёт или возвращает число основных делений на оси Oy (число засечек - 1)
		/// </summary>
		public uint YPrimaryDivisions
			{
			get
				{
				if (autoPrimaryDivisions)
					{
					return GetPrimaryDivisions (minY, maxY);
					}
				return yPrimaryDivisions;
				}
			set
				{
				yPrimaryDivisions = value;

				if (yPrimaryDivisions < MinPrimaryDivisions)
					{
					yPrimaryDivisions = MinPrimaryDivisions;
					}
				if (yPrimaryDivisions > MaxPrimaryDivisions)
					{
					yPrimaryDivisions = MaxPrimaryDivisions;
					}
				}
			}
		private uint yPrimaryDivisions;

		/// <summary>
		/// Задаёт или возвращает число подразделений основных делений на оси Oy (число засечек + 1)
		/// </summary>
		public uint YSecondaryDivisions
			{
			get
				{
				return ySecondaryDivisions;
				}
			set
				{
				ySecondaryDivisions = value;

				if (ySecondaryDivisions < MinSecondaryDivisions)
					{
					ySecondaryDivisions = MinSecondaryDivisions;
					}
				if (ySecondaryDivisions > MaxSecondaryDivisions)
					{
					ySecondaryDivisions = MaxSecondaryDivisions;
					}
				}
			}
		private uint ySecondaryDivisions;
		#endregion

		#region Масштабирование
		/// <summary>
		/// Минимальная величина масштаба изображения
		/// </summary>
		public const float MinScale = 1.0f;

		/// <summary>
		/// Максимальная величина масштаба изображения
		/// </summary>
		public const float MaxScale = 10.0f;

		/// <summary>
		/// Метод масштабирует зависимые от размера конечного изображения параметры кривой линии
		/// Допускает только увеличение, но не более, чем в 10 раз
		/// </summary>
		/// <param name="Scale">Величина масштаба</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool ApplyLineScale (float Scale)
			{
			return ApplyScale (Scale, true);
			}

		/// <summary>
		/// Метод масштабирует зависимые от размера конечного изображения параметры дополнительного объекта
		/// Допускает только увеличение, но не более, чем в 10 раз
		/// </summary>
		/// <param name="Scale">Величина масштаба</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool ApplyObjectScale (float Scale)
			{
			return ApplyScale (Scale, false);
			}

		private bool ApplyScale (float Scale, bool ApplyForLine)
			{
			// Ограничение
			if ((Scale < MinScale) || (Scale > MaxScale))
				return false;

			// Масштабирование (выполняется в обход ограничений, описаных в свойствах)
			this.diagramImageHeight = (uint)((float)this.diagramImageHeight * Scale);
			this.diagramImageLeftOffset = (uint)((float)this.diagramImageLeftOffset * Scale);
			this.diagramImageTopOffset = (uint)((float)this.diagramImageTopOffset * Scale);
			this.diagramImageWidth = (uint)((float)this.diagramImageWidth * Scale);
			this.lineWidth = (uint)((float)this.lineWidth * Scale);
			this.textFont = new Font (textFont.FontFamily, (float)this.textFont.Size * Scale, textFont.Style);
			this.lineNameLeftOffset = (uint)((float)this.lineNameLeftOffset * Scale);
			this.lineNameTopOffset = (uint)((float)this.lineNameTopOffset * Scale);
			this.oxTextOffset = (uint)((float)this.oxTextOffset * Scale);
			this.oyTextOffset = (uint)((float)this.oyTextOffset * Scale);

			if (ApplyForLine)
				{
				this.axesLinesWidth = (uint)((float)this.axesLinesWidth * Scale);
				this.gridLinesWidth = (uint)((float)this.gridLinesWidth * Scale);
				this.axesFont = new Font (axesFont.FontFamily, (float)this.axesFont.Size * Scale, axesFont.Style);
				}
			else
				{
				this.minX *= Scale;
				this.minY *= Scale;
				this.maxX *= Scale;
				this.maxY *= Scale;
				}

			return true;
			}

		#endregion

		#region Расположения осей

		/// <summary>
		/// Расположение оси Ox
		/// </summary>
		public AxesPlacements OxPlacement
			{
			get
				{
				return oxPlacement;
				}
			set
				{
				oxPlacement = value;
				}
			}
		private AxesPlacements oxPlacement;

		/// <summary>
		/// Расположение оси Oy
		/// </summary>
		public AxesPlacements OyPlacement
			{
			get
				{
				return oyPlacement;
				}
			set
				{
				oyPlacement = value;
				}
			}
		private AxesPlacements oyPlacement;
		#endregion

		#region Расположения подписей на осях
		/// <summary>
		/// Максимальное пользовательское смещение имени кривой относительно левого края изображения
		/// </summary>
		public const float MaxLineNameLeftMargin = 0.95f;

		/// <summary>
		/// Максимальное пользовательское смещение имени кривой относительно верхнего края изображения
		/// </summary>
		public const float MaxLineNameTopMargin = 0.94f;

		/// <summary>
		/// Максимальное пользовательское смещение подписей оси Oy
		/// </summary>
		public const float MaxOyTextMargin = 0.95f;

		/// <summary>
		/// Максимальное пользовательское смещение подписей оси Ox
		/// </summary>
		public const float MaxOxTextMargin = 0.87f;

		/// <summary>
		/// Пользовательское смещение подписей оси Ox
		/// </summary>
		public uint OxTextOffset
			{
			get
				{
				return oxTextOffset;
				}
			set
				{
				if (value > (uint)(diagramImageHeight * MaxOxTextMargin))
					{
					oxTextOffset = (uint)(diagramImageHeight * MaxOxTextMargin);
					}
				else
					{
					oxTextOffset = value;
					}
				}
			}
		private uint oxTextOffset;

		/// <summary>
		/// Пользовательское смещение подписей оси Oy
		/// </summary>
		public uint OyTextOffset
			{
			get
				{
				return oyTextOffset;
				}
			set
				{
				if (value > (uint)(diagramImageWidth * MaxOyTextMargin))
					{
					oyTextOffset = (uint)(diagramImageWidth * MaxOyTextMargin);
					}
				else
					{
					oyTextOffset = value;
					}
				}
			}
		private uint oyTextOffset;

		/// <summary>
		/// Пользовательское имя кривой
		/// </summary>
		public string LineName
			{
			get
				{
				return lineName;
				}
			set
				{
				lineName = value;
				}
			}
		private string lineName;

		/// <summary>
		/// Максимальная длина пользовательского имени кривой
		/// </summary>
		public const uint MaxLineNameLength = 200;

		/// <summary>
		/// Указывает, следует ли автоматически рассчитывать положения подписей к осям
		/// или необходимо использовать пользовательские настройки
		/// </summary>
		public bool AutoTextOffset
			{
			get
				{
				return autoTextOffset;
				}
			set
				{
				autoTextOffset = value;
				}
			}
		private bool autoTextOffset;

		/// <summary>
		/// Пользовательское смещение имени кривой относительно левого края изображения
		/// </summary>
		public uint LineNameLeftOffset
			{
			get
				{
				return lineNameLeftOffset;
				}
			set
				{
				if (value > (uint)(diagramImageWidth * MaxLineNameLeftMargin))
					{
					lineNameLeftOffset = (uint)(diagramImageWidth * MaxLineNameLeftMargin);
					}
				else
					{
					lineNameLeftOffset = value;
					}
				}
			}
		private uint lineNameLeftOffset;

		/// <summary>
		/// Пользовательское смещение имени кривой относительно верхнего края изображения
		/// </summary>
		public uint LineNameTopOffset
			{
			get
				{
				return lineNameTopOffset;
				}
			set
				{
				if (value > (uint)(diagramImageHeight * MaxLineNameTopMargin))
					{
					lineNameTopOffset = (uint)(diagramImageHeight * MaxLineNameTopMargin);
					}
				else
					{
					lineNameTopOffset = value;
					}
				}
			}
		private uint lineNameTopOffset;
		#endregion

		#region Представление чисел на диаграмме

		/// <summary>
		/// Представление подписей оси Ox
		/// </summary>
		public NumbersFormat OxFormat
			{
			get
				{
				return oxFormat;
				}
			set
				{
				oxFormat = value;
				}
			}
		private NumbersFormat oxFormat;

		/// <summary>
		/// Представление подписей оси Oy
		/// </summary>
		public NumbersFormat OyFormat
			{
			get
				{
				return oyFormat;
				}
			set
				{
				oyFormat = value;
				}
			}
		private NumbersFormat oyFormat;
		#endregion
		}
	}
