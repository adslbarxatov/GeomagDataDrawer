using System.Collections.Generic;
using System.Drawing;

namespace RD_AAOW
	{
	/// <summary>
	/// Интерфейс описывает требования к адаптеру отрисовки векторного изображения
	/// </summary>
	public interface IVectorAdapter
		{
		/// <summary>
		/// Метод добавляет комментарий в тело метафайла (если возможно)
		/// </summary>
		/// <param name="CommentText">Массив строк комментария</param>
		/// <returns>Возвращает true в случае успеха</returns>
		bool AddComment (List<string> CommentText);

		/// <summary>
		/// Метод закрывает файл
		/// </summary>
		bool CloseFile ();

		/// <summary>
		/// Метод закрывает группу объектов
		/// </summary>
		/// <returns>Возвращает true в случае успеха или false, если все группы закрыты</returns>
		bool CloseGroup ();

		/// <summary>
		/// Метод выполняет отрисовку линии в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса начала линии</param>
		/// <param name="Y1">Ордината начала линии</param>
		/// <param name="X2">Абсцисса конца линии</param>
		/// <param name="Y2">Ордината конца линии</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <param name="LineColor">Цвет линии</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		bool DrawLine (double X1, double Y1, double X2, double Y2, uint LineWidth, Color LineColor);

		/// <summary>
		/// Метод выполняет отрисовку прямоугольника в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла прямоугольника</param>
		/// <param name="Y1">Ордината верхнего левого угла прямоугольника</param>
		/// <param name="X2">Абсцисса нижнего правого угла прямоугольника</param>
		/// <param name="Y2">Ордината нижнего правого угла прямоугольника</param>
		/// <param name="RectangleColor">Цвет прямоугольника</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		bool DrawRectangle (double X1, double Y1, double X2, double Y2, uint LineWidth, Color RectangleColor);

		/// <summary>
		/// Метод выполняет отрисовку заполненного прямоугольника в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла прямоугольника</param>
		/// <param name="Y1">Ордината верхнего левого угла прямоугольника</param>
		/// <param name="X2">Абсцисса нижнего правого угла прямоугольника</param>
		/// <param name="Y2">Ордината нижнего правого угла прямоугольника</param>
		/// <param name="RectangleColor">Цвет прямоугольника</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		bool FillRectangle (double X1, double Y1, double X2, double Y2, Color RectangleColor);

		/// <summary>
		/// Метод выполняет отрисовку эллипса в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла эллипса</param>
		/// <param name="Y1">Ордината верхнего левого угла эллипса</param>
		/// <param name="X2">Абсцисса нижнего правого угла эллипса</param>
		/// <param name="Y2">Ордината нижнего правого угла эллипса</param>
		/// <param name="EllipseColor">Цвет эллипса</param>
		/// <param name="LineWidth">Толщина линии</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		bool DrawEllipse (double X1, double Y1, double X2, double Y2, uint LineWidth, Color EllipseColor);

		/// <summary>
		/// Метод выполняет отрисовку заполненного эллипса в открытый файл
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла эллипса</param>
		/// <param name="Y1">Ордината верхнего левого угла эллипса</param>
		/// <param name="X2">Абсцисса нижнего правого угла эллипса</param>
		/// <param name="Y2">Ордината нижнего правого угла эллипса</param>
		/// <param name="EllipseColor">Цвет эллипса</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		bool FillEllipse (double X1, double Y1, double X2, double Y2, Color EllipseColor);

		/// <summary>
		/// Метод выполняет вставку маркера в файл и его сохранение в виде изображения
		/// </summary>
		/// <param name="CenterX">Абсцисса центра изображения маркера</param>
		/// <param name="CenterY">Ордината центра изображения маркера</param>
		/// <param name="LineNumber">Номер кривой. Используется для задания имени маркера 
		/// и его привязки к кривой</param>
		/// <param name="MarkerImage">Изображение маркера</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания параметров</returns>
		bool DrawMarker (Bitmap MarkerImage, double CenterX, double CenterY, uint LineNumber);

		/// <summary>
		/// Метод выполняет отрисовку текста в открытый файл
		/// </summary>
		/// <param name="X">Абсцисса верхнего левого угла области текста</param>
		/// <param name="Y">Ордината верхнего левого угла области текста</param>
		/// <param name="TextToDraw">Строка текста</param>
		/// <param name="TextFont">Шрифт текста</param>
		/// <param name="TextColor">Цвет текста</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания параметров</returns>
		bool DrawText (double X, double Y, string TextToDraw, Font TextFont, Color TextColor);

		/// <summary>
		/// Возвращает результат инициализации класса
		/// </summary>
		VectorAdapterInitResults InitResult
			{
			get;
			}

		/// <summary>
		/// Метод открывает группу объектов
		/// </summary>
		/// <returns>Возвращает true в случае успеха или false, если превышено число допустимых 
		/// уровней вложения групп</returns>
		bool OpenGroup ();

		/// <summary>
		/// Метод выполняет сброс области отрисовки до размера изображения
		/// </summary>
		void ResetClipBox ();

		/// <summary>
		/// Метод выполняет установку размера области отрисовки
		/// </summary>
		/// <param name="X1">Абсцисса верхнего левого угла области</param>
		/// <param name="Y1">Ордината верхнего левого угла области</param>
		/// <param name="X2">Абсцисса нижнего правого угла области</param>
		/// <param name="Y2">Ордината нижнего правого угла области</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания диапазонов</returns>
		bool SetClipBox (double X1, double Y1, double X2, double Y2);
		}

	/// <summary>
	/// Класс предоставляет вспомогательные функции для представителей интерфейса IVectorAdapter
	/// </summary>
	public static class VectorAdapterSupport
		{
		/// <summary>
		/// Метод пересчитывает координаты линии, умещая её в границы ClipBox 
		/// </summary>
		/// <param name="ClipBoxX1">Граница ClipBox</param>
		/// <param name="ClipBoxX2">Граница ClipBox</param>
		/// <param name="ClipBoxY1">Граница ClipBox</param>
		/// <param name="ClipBoxY2">Граница ClipBox</param>
		/// <param name="OldX1">Исходная координата</param>
		/// <param name="OldX2">Исходная координата</param>
		/// <param name="OldY1">Исходная координата</param>
		/// <param name="OldY2">Исходная координата</param>
		/// <param name="NewX1">Полученная координата</param>
		/// <param name="NewX2">Полученная координата</param>
		/// <param name="NewY1">Полученная координата</param>
		/// <param name="NewY2">Полученная координата</param>
		/// <returns>Возвращает бинарный статус, указывающий, какие границы были пересчитаны (0 - никакие, 
		/// 1 или 2 - первая или вторая, 3 - обе)</returns>
		public static uint InboundCoords (double ClipBoxX1, double ClipBoxX2, double ClipBoxY1, double ClipBoxY2,
			double OldX1, double OldX2, double OldY1, double OldY2,
			out double NewX1, out double NewX2, out double NewY1, out double NewY2)
			{
			// Переменные
			uint inboundedPoint = 0;

			// Пересчёт по абсциссам
			if (OldX1 < ClipBoxX1)
				{
				NewX1 = ClipBoxX1;
				inboundedPoint |= 1;
				}
			else if (OldX1 > ClipBoxX2)
				{
				NewX1 = ClipBoxX2;
				inboundedPoint |= 1;
				}
			else
				{
				NewX1 = OldX1;
				}

			if (OldX2 < ClipBoxX1)
				{
				NewX2 = ClipBoxX1;
				inboundedPoint |= 2;
				}
			else if (OldX2 > ClipBoxX2)
				{
				NewX2 = ClipBoxX2;
				inboundedPoint |= 2;
				}
			else
				{
				NewX2 = OldX2;
				}

			NewY1 = (OldX1 != OldX2) ? ((NewX1 - OldX1) / (OldX2 - OldX1)) * (OldY2 - OldY1) + OldY1 : OldY1;
			NewY2 = (OldX1 != OldX2) ? ((NewX2 - OldX1) / (OldX2 - OldX1)) * (OldY2 - OldY1) + OldY1 : OldY2;

			// Пересчёт по ординатам
			if (OldY1 < ClipBoxY1)
				{
				NewY1 = ClipBoxY1;
				inboundedPoint |= 1;
				}
			else if (OldY1 > ClipBoxY2)
				{
				NewY1 = ClipBoxY2;
				inboundedPoint |= 1;
				}

			if (OldY2 < ClipBoxY1)
				{
				NewY2 = ClipBoxY1;
				inboundedPoint |= 2;
				}
			else if (OldY2 > ClipBoxY2)
				{
				NewY2 = ClipBoxY2;
				inboundedPoint |= 2;
				}

			NewX1 = (OldY1 != OldY2) ? ((NewY1 - OldY1) / (OldY2 - OldY1)) * (OldX2 - OldX1) + OldX1 : NewX1;
			NewX2 = (OldY1 != OldY2) ? ((NewY2 - OldY1) / (OldY2 - OldY1)) * (OldX2 - OldX1) + OldX1 : NewX2;

			return inboundedPoint;
			}
		}
	}
