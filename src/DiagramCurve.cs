namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает доступ к служебной информации, формируемой при создании новой кривой
	/// </summary>
	public class DiagramCurve
		{
		// Местные минимумы и максимумы (по сравнению с хранящимися в DiagramStyle)
		// являются настоящими и не должны изменяться нигде, кроме как в DiagramData

		/// <summary>
		/// Возвращает минимальное значение абсциссы кривой
		/// </summary>
		public double MinimumX
			{
			get
				{
				return minimumX;
				}
			}
		private double minimumX = double.NegativeInfinity;

		/// <summary>
		/// Возвращает максимальное значение абсциссы кривой
		/// </summary>
		public double MaximumX
			{
			get
				{
				return maximumX;
				}
			}
		private double maximumX = double.PositiveInfinity;

		/// <summary>
		/// Возвращает минимальное значение ординаты кривой
		/// </summary>
		public double MinimumY
			{
			get
				{
				return minimumY;
				}
			}
		private double minimumY = double.NegativeInfinity;

		/// <summary>
		/// Возвращает максимальное значение ординаты кривой
		/// </summary>
		public double MaximumY
			{
			get
				{
				return maximumY;
				}
			}
		private double maximumY = double.PositiveInfinity;

		/// <summary>
		/// Возвращает позицию округления для абсцисс кривой
		/// </summary>
		public int XRoundPosition
			{
			get
				{
				return xRoundPosition;
				}
			}
		private int xRoundPosition = 0;

		/// <summary>
		/// Возвращает позицию округления для ординат кривой
		/// </summary>
		public int YRoundPosition
			{
			get
				{
				return yRoundPosition;
				}
			}
		private int yRoundPosition = 0;

		/// <summary>
		/// Конструктор. Инициализирует новую кривую
		/// </summary>
		/// <param name="MinX">Минимальная абсцисса кривой</param>
		/// <param name="MaxX">Максимальная абсцисса кривой</param>
		/// <param name="MinY">Минимальная ордината кривой</param>
		/// <param name="MaxY">Максимальная ордината кривой</param>
		/// <param name="XRoundPos">Позиция округления абсцисс кривой</param>
		/// <param name="YRoundPos">Позиция округления ординат кривой</param>
		public DiagramCurve (double MinX, double MaxX, double MinY, double MaxY, int XRoundPos, int YRoundPos)
			{
			SetValues (MinX, MaxX, MinY, MaxY, XRoundPos, YRoundPos);
			}

		/// <summary>
		/// Метод заменяет имеющиеся значения параметров кривой вновь переданными
		/// </summary>
		/// <param name="MinX">Минимальная абсцисса кривой</param>
		/// <param name="MaxX">Максимальная абсцисса кривой</param>
		/// <param name="MinY">Минимальная ордината кривой</param>
		/// <param name="MaxY">Максимальная ордината кривой</param>
		/// <param name="XRoundPos">Позиция округления абсцисс кривой</param>
		/// <param name="YRoundPos">Позиция округления ординат кривой</param>
		public void SetValues (double MinX, double MaxX, double MinY, double MaxY, int XRoundPos, int YRoundPos)
			{
			minimumX = MinX;
			maximumX = MaxX;
			minimumY = MinY;
			maximumY = MaxY;
			xRoundPosition = XRoundPos;
			yRoundPosition = YRoundPos;
			}
		}
	}
