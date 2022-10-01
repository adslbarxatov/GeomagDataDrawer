using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает поддержку формата SVG
	/// </summary>
	public class SVGAdapter: RD_AAOW.IVectorAdapter
		{
		// Общие переменные
		private FileStream FS = null;
		private StreamWriter SW = null;
		private uint width, height;
		private string fileName;
		private CultureInfo cie = new CultureInfo ("en-us");

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
		/// Конструктор. Создаёт новый файл SVG с указанными параметрами и начинает его редактирование
		/// </summary>
		/// <param name="FileName">Имя файла SVG</param>
		/// <param name="MaxHeight">Ограничение изображения по высоте</param>
		/// <param name="MaxWidth">Ограничение изображения по ширине</param>
		public SVGAdapter (string FileName, uint MaxWidth, uint MaxHeight)
			{
			// Контроль параметров
			if (MaxWidth * MaxHeight == 0)
				{
				initResult = VectorAdapterInitResults.IncorrectImageSize;
				return;
				}

			// Попытка создания файла
			try
				{
				FS = new FileStream (FileName, FileMode.Create);
				}
			catch
				{
				initResult = VectorAdapterInitResults.CannotCreateFile;
				return;
				}
			SW = new StreamWriter (FS, Encoding.GetEncoding ("utf-8"));

			// Запись заголовка
			SW.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			SW.WriteLine ("<!DOCTYPE svg []>");
			SW.WriteLine ("<svg version=\"1.1\" width=\"100%\" height=\"100%\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");

			// Сохранение значений
			width = MaxWidth;
			height = MaxHeight;
			fileName = FileName;

			// Успешно завершено
			initResult = VectorAdapterInitResults.Opened;

			// Создание области доступной отрисовки
			ResetClipBox ();
			}

		// Границы области отрисовки
		private double clipBoxX1, clipBoxX2;
		private double clipBoxY1, clipBoxY2;

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
		/// <returns>Возвращает true в случае успеха или false в случае некорректного 
		/// задания диапазонов</returns>
		public bool SetClipBox (double X1, double Y1, double X2, double Y2)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			if (((int)X1 < 0) || ((uint)X1 > width) ||
				((int)Y1 < 0) || ((uint)Y1 > height) ||
				((int)X2 < 0) || ((uint)X2 > width) || ((int)Y2 < 0) || ((uint)Y2 > height))
				return false;

			if ((X1 >= X2) || (Y1 >= Y2))
				return false;

			// Установка значений
			clipBoxX1 = X1;
			clipBoxX2 = X2;
			clipBoxY1 = Y1;
			clipBoxY2 = Y2;
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
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		public bool DrawLine (double X1, double Y1, double X2, double Y2, uint LineWidth, Color LineColor)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) || (LineWidth == 0))
				return false;

			// Пересчёт координат
			double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			SW.WriteLine ("<line stroke=\"#" + (LineColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
				"\" stroke-width=\"" + ((double)LineWidth / 2.0).ToString (cie.NumberFormat) +
				"\" x1=\"" + x1.ToString (cie.NumberFormat) +
				"\" y1=\"" + y1.ToString (cie.NumberFormat) +
				"\" x2=\"" + x2.ToString (cie.NumberFormat) +
				"\" y2=\"" + y2.ToString (cie.NumberFormat) + "\"/>");

			return true;
			}

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
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров
		/// (выход за область отрисовки)</returns>
		public bool FillRectangle (double X1, double Y1, double X2, double Y2, Color RectangleColor)
			{
			return CreateRectangle (X1, Y1, X2, Y2, 1, RectangleColor, true);
			}

		private bool CreateRectangle (double X1, double Y1, double X2, double Y2, uint LineWidth,
			Color RectangleColor, bool Fill)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) || (LineWidth == 0))
				return false;

			// Пересчёт координат
			double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			if (Fill)
				{
				SW.WriteLine ("<rect fill=\"#" + (RectangleColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
					"\" x=\"" + x1.ToString (cie.NumberFormat) +
					"\" y=\"" + y1.ToString (cie.NumberFormat) +
					"\" width=\"" + (x2 - x1).ToString (cie.NumberFormat) +
					"\" height=\"" + (y2 - y1).ToString (cie.NumberFormat) + "\"/>");
				}
			else
				{
				SW.WriteLine ("<rect stroke=\"#" + (RectangleColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
					"\" stroke-width=\"" + ((double)LineWidth / 2.0).ToString (cie.NumberFormat) +
					"\" fill=\"none" +
					"\" x=\"" + x1.ToString (cie.NumberFormat) +
					"\" y=\"" + y1.ToString (cie.NumberFormat) +
					"\" width=\"" + (x2 - x1).ToString (cie.NumberFormat) +
					"\" height=\"" + (y2 - y1).ToString (cie.NumberFormat) + "\"/>");
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
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
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
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания размеров 
		/// (выход за область отрисовки)</returns>
		public bool FillEllipse (double X1, double Y1, double X2, double Y2, Color EllipseColor)
			{
			return CreateEllipse (X1, Y1, X2, Y2, 1, EllipseColor, true);
			}

		private bool CreateEllipse (double X1, double Y1, double X2, double Y2, uint LineWidth,
			Color RectangleColor, bool Fill)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			// Если вся линия выходит за границу отрисовки
			if ((X1 < clipBoxX1) && (X2 < clipBoxX1) ||
				(X1 > clipBoxX2) && (X2 > clipBoxX2) ||
				(Y1 < clipBoxY1) && (Y2 < clipBoxY1) ||
				(Y1 > clipBoxY2) && (Y2 > clipBoxY2) || (LineWidth == 0))
				return false;

			// Пересчёт координат
			double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0;
			VectorAdapterSupport.InboundCoords (clipBoxX1, clipBoxX2, clipBoxY1, clipBoxY2,
				X1, X2, Y1, Y2, out x1, out x2, out y1, out y2);

			// Отрисовка
			if (Fill)
				{
				SW.WriteLine ("<ellipse fill=\"#" + (RectangleColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
					"\" cx=\"" + (x1 + (x2 - x1) / 2).ToString (cie.NumberFormat) +
					"\" cy=\"" + (y1 + (y2 - y1) / 2).ToString (cie.NumberFormat) +
					"\" rx=\"" + ((x2 - x1) / 2).ToString (cie.NumberFormat) +
					"\" ry=\"" + ((y2 - y1) / 2).ToString (cie.NumberFormat) + "\"/>");
				}
			else
				{
				SW.WriteLine ("<ellipse stroke=\"#" + (RectangleColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
					"\" stroke-width=\"" + ((double)LineWidth / 2.0).ToString (cie.NumberFormat) +
					"\" fill=\"none" +
					"\" cx=\"" + (x1 + (x2 - x1) / 2).ToString (cie.NumberFormat) +
					"\" cy=\"" + (y1 + (y2 - y1) / 2).ToString (cie.NumberFormat) +
					"\" rx=\"" + ((x2 - x1) / 2).ToString (cie.NumberFormat) +
					"\" ry=\"" + ((y2 - y1) / 2).ToString (cie.NumberFormat) + "\"/>");
				}

			return true;
			}

		/// <summary>
		/// Метод закрывает файл
		/// </summary>
		public bool CloseFile ()
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			if (groupLevel != 0)
				return false;

			// Закрытие
			SW.WriteLine ("</svg>");
			SW.Close ();
			FS.Close ();
			initResult = VectorAdapterInitResults.Closed;

			return true;
			}

		// Переменная включает таблицу преобразования Base64
		private string[] base64EncodingTable = new string[] {
			"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
			"Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f",
			"g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
			"w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "/"
			};

		// Метод кодирует файл согласно стандарту Base64
		private string Base64FileEncode (string FileName)
			{
			// Переменные
			string codeString = "";

			// Попытка открытия файла
			FileStream IFS = null;
			try
				{
				IFS = new FileStream (FileName, FileMode.Open);
				}
			catch
				{
				return codeString;
				}

			// Чтение
			while (IFS.Position < IFS.Length)
				{
				// Получение байтов
				long b1 = IFS.ReadByte (),
					b2 = IFS.ReadByte (),
					b3 = IFS.ReadByte ();

				// Склеивание в цепочку
				long ch;
				if (b2 < 0)
					{
					ch = (b1 << 16);
					}
				else if (b3 < 0)
					{
					ch = (b1 << 16) | (b2 << 8);
					}
				else
					{
					ch = (b1 << 16) | (b2 << 8) | b3;
					}

				// Разделение на шестёрки
				codeString += base64EncodingTable[(ch & 0xFC0000) >> 18];

				if (b2 >= 0)
					{
					codeString += base64EncodingTable[(ch & 0x3F000) >> 12];
					}
				else
					{
					codeString += "=";
					}

				if (b3 >= 0)
					{
					codeString += base64EncodingTable[(ch & 0xFC0) >> 6];
					codeString += base64EncodingTable[ch & 0x3F];
					}
				else
					{
					codeString += "=";
					}
				}

			// Завершено
			IFS.Close ();
			return codeString;
			}

		/// <summary>
		/// Метод выполняет отрисовку изображения в открытый файл SVG
		/// </summary>
		/// <param name="X">Абсцисса верхнего левого угла изображения</param>
		/// <param name="Y">Ордината верхнего левого угла изображения</param>
		/// <param name="ImageFileName">Имя файла изображения</param>
		/// <param name="ImageHeight">Конечная высота изображения</param>
		/// <param name="ImageWidth">Конечная ширина изображения</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного задания параметров</returns>
		public bool DrawImage (double X, double Y, string ImageFileName, uint ImageWidth, uint ImageHeight)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			if ((X < clipBoxX1) || (X > clipBoxX2) ||
				(Y < clipBoxY1) || (Y > clipBoxY2) ||
				(ImageWidth * ImageHeight == 0))
				return false;

			// Кодирование изображения
			string codeString = Base64FileEncode (ImageFileName);
			if (codeString.Length == 0)
				return false;

			// Отрисовка
			SW.WriteLine ("<image overflow=\"visible\" x=\"" + X.ToString (cie.NumberFormat) +
				"\" y=\"" + Y.ToString (cie.NumberFormat) + "\" width=\"" + ImageWidth.ToString () +
				"\" height=\"" + ImageHeight + "\" xlink:href=\"data:image/png;base64," + codeString + "\"></image>");

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
				return false;

			// Контроль значений
			// Отключено ограничение по полю отрисовки
			if ((TextToDraw == null) || (TextToDraw == "") || (TextFont == null))
				return false;

			// Отрисовка (перечёркивание не выполняется)
			SW.WriteLine ("<text x=\"" + X.ToString (cie.NumberFormat) + "\" y=\"" +
				Y.ToString (cie.NumberFormat) + "\" fill=\"#" + (TextColor.ToArgb () & 0xFFFFFF).ToString ("X06") +
				"\" font-family=\"'" + TextFont.Name + "'\" font-size=\"" + TextFont.Size + "\">" +
				(TextFont.Bold ? "<tspan style=\"font-weight: bold;\">" : "") +
				(TextFont.Italic ? "<tspan style=\"font-style: italic;\">" : "") +
				(TextFont.Underline ? "<tspan style=\"text-decoration: underline;\">" : "") +
				//(TextFont.Strikeout ? "<tspan style=\"text-decoration: ;\">" : "") +
				TextToDraw +
				//(TextFont.Strikeout ? "</tspan>" : "") +
				(TextFont.Underline ? "</tspan>" : "") +
				(TextFont.Italic ? "</tspan>" : "") +
				(TextFont.Bold ? "</tspan>" : "") +
				"</text>");

			return true;
			}

		/// <summary>
		/// Метод добавляет комментарий в тело метафайла (если возможно)
		/// </summary>
		/// <param name="CommentText">Массив строк комментария</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool AddComment (List<string> CommentText)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль значений
			if (CommentText == null)
				return false;

			// Запись
			SW.WriteLine ("<!--");
			for (int i = 0; i < CommentText.Count; i++)
				SW.WriteLine (CommentText[i]);

			SW.WriteLine ("-->");
			return true;
			}

		// Переменная состояния группы
		private uint groupLevel = 0;
		private const uint MaxGroupLevel = 3;

		/// <summary>
		/// Метод открывает группу объектов
		/// </summary>
		/// <returns>Возвращает true в случае успеха или false, если превышено число допустимых уровней 
		/// вложения групп</returns>
		public bool OpenGroup ()
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			if (groupLevel == MaxGroupLevel)    // Ограничение - до трёх уровней
				return false;

			// Открытие группы
			SW.WriteLine ("<g>");
			groupLevel++;
			return true;
			}

		/// <summary>
		/// Метод закрывает группу объектов
		/// </summary>
		/// <returns>Возвращает true в случае успеха или false, если все группы закрыты</returns>
		public bool CloseGroup ()
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			if (groupLevel == 0)
				return false;

			// Закрытие группы
			SW.WriteLine ("</g>");
			groupLevel--;
			return true;
			}

		/// <summary>
		/// Метод выполняет вставку маркера в файл и его сохранение в виде изображения
		/// </summary>
		/// <param name="CenterX">Абсцисса центра изображения маркера</param>
		/// <param name="CenterY">Ордината центра изображения маркера</param>
		/// <param name="LineNumber">Номер кривой. Используется для задания имени маркера 
		/// и его привязки к кривой</param>
		/// <param name="MarkerImage">Изображение маркера</param>
		/// <returns>Возвращает true в случае успеха или false в случае некорректного 
		/// задания параметров</returns>
		public bool DrawMarker (Bitmap MarkerImage, double CenterX, double CenterY, uint LineNumber)
			{
			// Контроль состояния
			if (initResult != VectorAdapterInitResults.Opened)
				return false;

			// Контроль параметров
			if ((MarkerImage == null) ||
				(CenterX < clipBoxX1) || (CenterX > clipBoxX2) ||
				(CenterY < clipBoxY1) || (CenterY > clipBoxY2))
				return false;

			// Контроль наличия директории с маркерами
			if (!Directory.Exists (fileName + ".img"))
				{
				try
					{
					Directory.CreateDirectory (fileName + ".img");
					}
				catch
					{
					return false;
					}
				}

			// Контроль наличия нужного маркера
			if (!File.Exists (fileName + ".img\\" + LineNumber.ToString () + ".png"))
				{
				try
					{
					MarkerImage.Save (fileName + ".img\\" + LineNumber.ToString () + ".png", ImageFormat.Png);
					}
				catch
					{
					return false;
					}
				}

			// Запись маркера в файл

			SW.WriteLine ("<image overflow=\"visible\" x=\"" + (CenterX - (double)MarkerImage.Width /
				2.0).ToString (cie.NumberFormat) +
				"\" y=\"" + (CenterY - (double)MarkerImage.Height / 2.0).ToString (cie.NumberFormat) + "\" width=\"" +
				MarkerImage.Width.ToString () + "\" height=\"" + MarkerImage.Height.ToString () + "\" xlink:href=\"" +
				Path.GetFileName (fileName) + ".img/" + LineNumber.ToString () + ".png\"></image>");

			return true;
			}
		}
	}
