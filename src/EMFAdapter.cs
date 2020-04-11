using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает поддержку формата EMF
	/// </summary>
	public class EMFAdapter:IVectorAdapter
		{
		// Переменные
		private uint width, height;
		private string fileName;
		private Metafile mf = null;
		private Graphics g = null;

		/// <summary>
		/// Конструктор. Создаёт новый файл EMF с указанными параметрами и начинает его редактирование
		/// </summary>
		/// <param name="FileName">Имя файла EMF</param>
		/// <param name="MaxHeight">Ограничение изображения по высоте</param>
		/// <param name="MaxWidth">Ограничение изображения по ширине</param>
		public EMFAdapter (string FileName, uint MaxWidth, uint MaxHeight)
			{
			// Контроль параметров
			if (MaxWidth * MaxHeight == 0)
				{
				initResult = VectorAdapterInitResults.IncorrectImageSize;
				return;
				}

			// Сохранение параметров
			width = MaxWidth;
			height = MaxHeight;
			fileName = FileName;

			// Попытка создания файла
			try
				{
				Graphics g0 = Application.OpenForms[0].CreateGraphics ();	// Похоже, больше неоткуда
				mf = new Metafile (FileName, g0.GetHdc ());					// и иначе никак
				g = Graphics.FromImage (mf);
				g0.Dispose ();
				}
			catch
				{
				initResult = VectorAdapterInitResults.CannotCreateFile;
				return;
				}

			// Успешно завершено
			initResult = VectorAdapterInitResults.Opened;

			// Создание области доступной отрисовки
			ResetClipBox ();
			}

		/// <summary>
		/// Метод закрывает файл
		/// </summary>
		public bool CloseFile ()
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Закрытие
			g.Dispose ();
			mf.Dispose ();

			initResult = VectorAdapterInitResults.Closed;
			return true;
			}

		/// <summary>
		/// Метод закрывает группу объектов (в данном формате не используется)
		/// </summary>
		/// <returns>Возвращает true</returns>
		public bool CloseGroup ()
			{
			return true;
			}

		/// <summary>
		/// Метод выполняет отрисовку линии в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса начала линии</param>
		/// <param name="Y1">Ордината начала линии</param>
		/// <param name="X2">Абсцисса конца линии</param>
		/// <param name="Y2">Ордината конца линии</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <param name="LineColor">Цвет линии</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров (выход за область отрисовки)</returns>
		public bool DrawLine (double X1, double Y1, double X2, double Y2, uint LineWidth, Color LineColor)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) ||
				(LineWidth == 0))
				{
				return false;
				}

			// Пересчёт координат
			double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			g.DrawLine (new Pen (LineColor, LineWidth), (float)x1, (float)y1, (float)x2, (float)y2);
			return true;
			}

		// Границы области отрисовки
		private double clipBoxX1, clipBoxX2;
		private double clipBoxY1, clipBoxY2;

		/// <summary>
		/// Метод выполняет отрисовку прямоугольника в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла прямоугольника</param>
		/// <param name="Y1">Ордината верхнего левого угла прямоугольника</param>
		/// <param name="X2">Абсцисса нижнего правого угла прямоугольника</param>
		/// <param name="Y2">Ордината нижнего правого угла прямоугольника</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <param name="RectangleColor">Цвет прямоугольника</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров (выход за область отрисовки)</returns>
		public bool DrawRectangle (double X1, double Y1, double X2, double Y2, uint LineWidth, Color RectangleColor)
			{
			return CreateRectangle (X1, Y1, X2, Y2, LineWidth, RectangleColor, false);
			}

		/// <summary>
		/// Метод выполняет отрисовку заполненного прямоугольника в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла прямоугольника</param>
		/// <param name="Y1">Ордината верхнего левого угла прямоугольника</param>
		/// <param name="X2">Абсцисса нижнего правого угла прямоугольника</param>
		/// <param name="Y2">Ордината нижнего правого угла прямоугольника</param>
		/// <param name="RectangleColor">Цвет прямоугольника</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров (выход за область отрисовки)</returns>
		public bool FillRectangle (double X1, double Y1, double X2, double Y2, Color RectangleColor)
			{
			return CreateRectangle (X1, Y1, X2, Y2, 1, RectangleColor, true);
			}

		private bool CreateRectangle (double X1, double Y1, double X2, double Y2, uint LineWidth, Color RectangleColor, bool Fill)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) ||
				(LineWidth == 0))
				{
				return false;
				}

			// Пересчёт координат
			double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			if (Fill)
				{
				g.FillRectangle (new SolidBrush (RectangleColor), (float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
				}
			else
				{
				g.DrawRectangle (new Pen (RectangleColor, LineWidth), (float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
				}
			return true;
			}

		/// <summary>
		/// Метод выполняет отрисовку эллипса в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла эллипса</param>
		/// <param name="Y1">Ордината верхнего левого угла эллипса</param>
		/// <param name="X2">Абсцисса нижнего правого угла эллипса</param>
		/// <param name="Y2">Ордината нижнего правого угла эллипса</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <param name="EllipseColor">Цвет эллипса</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров (выход за область отрисовки)</returns>
		public bool DrawEllipse (double X1, double Y1, double X2, double Y2, uint LineWidth, Color EllipseColor)
			{
			return CreateEllipse (X1, Y1, X2, Y2, LineWidth, EllipseColor, false);
			}

		/// <summary>
		/// Метод выполняет отрисовку заполненного эллипса в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла эллипса</param>
		/// <param name="Y1">Ордината верхнего левого угла эллипса</param>
		/// <param name="X2">Абсцисса нижнего правого угла эллипса</param>
		/// <param name="Y2">Ордината нижнего правого угла эллипса</param>
		/// <param name="EllipseColor">Цвет эллипса</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров (выход за область отрисовки)</returns>
		public bool FillEllipse (double X1, double Y1, double X2, double Y2, Color EllipseColor)
			{
			return CreateEllipse (X1, Y1, X2, Y2, 1, EllipseColor, true);
			}

		private bool CreateEllipse (double X1, double Y1, double X2, double Y2, uint LineWidth, Color RectangleColor, bool Fill)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) ||
				(LineWidth == 0))
				{
				return false;
				}

			// Пересчёт координат
			double x1 = 0.0f, x2 = 0.0f, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			if (Fill)
				{
				g.FillEllipse (new SolidBrush (RectangleColor), (float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
				}
			else
				{
				g.DrawEllipse (new Pen (RectangleColor, LineWidth), (float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
				}
			return true;
			}

		/// <summary>
		/// Метод выполняет вставку маркера в файл и его сохранение в виде изображения
		/// </summary>
		/// <param name="CenterX">Абсцисса центра изображения маркера</param>
		/// <param name="CenterY">Ордината центра изображения маркера</param>
		/// <param name="LineNumber">Номер кривой. Используется для задания имени маркера и его привязки к кривой</param>
		/// <param name="MarkerImage">Изображение маркера</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания параметров</returns>
		public bool DrawMarker (Bitmap MarkerImage, double CenterX, double CenterY, uint LineNumber)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль параметров
			if ((MarkerImage == null) ||
				(CenterX < clipBoxX1) || (CenterX > clipBoxX2) ||
				(CenterY < clipBoxY1) || (CenterY > clipBoxY2))
				{
				return false;
				}

			// Отрисовка
			g.DrawImage (MarkerImage, (float)CenterX - (float)MarkerImage.Width / 2.0f, (float)CenterY - (float)MarkerImage.Height / 2.0f);
			return true;
			}

		/// <summary>
		/// Метод выполняет отрисовку текста в открытый файл
		/// </summary>
		/// <param name="X">Абсцисса верхнего левого угла области текста</param>
		/// <param name="Y">Ордината верхнего левого угла области текста</param>
		/// <param name="TextToDraw">Строка текста</param>
		/// <param name="TextFont">Шрифт текста</param>
		/// <param name="TextColor">Цвет текста</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания параметров</returns>
		public bool DrawText (double X, double Y, string TextToDraw, Font TextFont, Color TextColor)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль значений
			// Отключено ограничение по полю отрисовки
			if ((TextToDraw == null) || (TextToDraw == "") || (TextFont == null))
				{
				return false;
				}

			// Стабилизация положения
			SizeF sz = g.MeasureString (TextToDraw, TextFont);

			// Отрисовка (перечёркивание не выполняется)
			g.DrawString (TextToDraw, TextFont, new SolidBrush (TextColor), (float)X, (float)Y - sz.Height);
			return true;
			}

		/// <summary>
		/// Метод добавляет комментарий в тело метафайла (в данном формате не используется)
		/// </summary>
		/// <param name="CommentText">Массив строк комментария</param>
		/// <returns>Возвращает true</returns>
		public bool AddComment (List<string> CommentText)
			{
			return true;
			}

		/// <summary>
		/// Возвращает результат инициализации класса
		/// </summary>
		public VectorAdapterInitResults InitResult
			{
			get
				{
				return initResult;
				}
			}
		private VectorAdapterInitResults initResult = VectorAdapterInitResults.NotInited;

		/// <summary>
		/// Метод открывает группу объектов (в данном формате не используется)
		/// </summary>
		/// <returns>Возвращает true</returns>
		public bool OpenGroup ()
			{
			return true;
			}

		/// <summary>
		/// Метод выполняет сброс области отрисовки до размера изображения
		/// </summary>
		public void ResetClipBox ()
			{
			SetClipBox (0.0f, 0.0, width, height);
			}

		/// <summary>
		/// Метод выполняет установку размера области отрисовки
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла области</param>
		/// <param name="Y1">Ордината верхнего левого угла области</param>
		/// <param name="X2">Абсцисса нижнего правого угла области</param>
		/// <param name="Y2">Ордината нижнего правого угла области</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания диапазонов</returns>
		public bool SetClipBox (double X1, double Y1, double X2, double Y2)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				{
				return false;
				}

			// Контроль значений
			if (((int)X1 < 0) || ((uint)X1 > width) ||
				((int)Y1 < 0) || ((uint)Y1 > height) ||
				((int)X2 < 0) || ((uint)X2 > width) ||
				((int)Y2 < 0) || ((uint)Y2 > height))
				{
				return false;
				}

			if ((X1 >= X2) || (Y1 >= Y2))
				{
				return false;
				}

			// Установка значений
			clipBoxX1 = X1;
			clipBoxX2 = X2;
			clipBoxY1 = Y1;
			clipBoxY2 = Y2;
			return true;
			}
		}
	}
