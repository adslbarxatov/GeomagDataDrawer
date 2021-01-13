using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает загрузку стандартных и дополнительных маркеров для построения диаграмм
	/// </summary>
	public class MarkersLoader: IDisposable
		{
		// Переменные и константы
		private List<Bitmap> markers = new List<Bitmap> (); // Массив маркеров
		private const int standartMarkersSize = 7;          // Размеры стандартных маркеров
		private const string markersDirectory = "Markers";  // Директория с маркерами

		/// <summary>
		/// Максимальное количество доступных маркеров
		/// </summary>
		public const uint MaxMarkers = 100;

		/// <summary>
		/// Возвращает изображение маркера, окрашенное указанным цветом
		/// </summary>
		/// <param name="MarkerColor">Требуемый цвет маркера</param>
		/// <param name="MarkerNumber">Номер требуемого маркера, начиная с нуля</param>
		/// <returns>Изображение маркера или изображение первого стандартного маркера, если номер маркера указан некорректно</returns>
		public Bitmap GetMarker (uint MarkerNumber, Color MarkerColor)
			{
			// Контроль
			int markerNumber = (int)MarkerNumber;
			if (MarkerNumber >= markers.Count)
				{
				markerNumber = 0;
				}

			// Создание копии маркера
			Bitmap b = (Bitmap)markers[markerNumber].Clone ();
			for (int h = 0; h < b.Height; h++)
				{
				for (int w = 0; w < b.Width; w++)
					{
					if ((uint)(b.GetPixel (w, h).ToArgb ()) == 0xFF000000)
						{
						b.SetPixel (w, h, MarkerColor);
						}
					}
				}

			// Возврат
			return b;
			}

		/// <summary>
		/// Возвращает количество доступных маркеров
		/// </summary>
		public uint MarkersCount
			{
			get
				{
				return (uint)markers.Count;
				}
			}

		/// <summary>
		/// Конструктор. Выполняет загрузку изображений маркеров
		/// </summary>
		public MarkersLoader ()
			{
			#region Добавление стандартных маркеров
			Bitmap b = new Bitmap (standartMarkersSize, standartMarkersSize);
			Brush backBrush = new SolidBrush (Color.FromArgb (0, 255, 255, 255)),
				foreBrush = new SolidBrush (Color.FromArgb (0, 0, 0));

			// Квадратик
			Graphics g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			g.FillRectangle (foreBrush, 1, 1, b.Width - 2, b.Height - 2);
			markers.Add ((Bitmap)b.Clone ());   // Нужно отвязать картинку от объекта b, иначе правка сохранится в ней
			b.Dispose ();
			g.Dispose ();

			// Кружочек
			b = new Bitmap (standartMarkersSize, standartMarkersSize);
			g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			g.FillEllipse (foreBrush, 0, 0, b.Width, b.Height);
			markers.Add ((Bitmap)b.Clone ());
			b.Dispose ();
			g.Dispose ();

			// Треугольник
			b = new Bitmap (standartMarkersSize, standartMarkersSize);
			g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			Point[] pts = new Point[] {new Point (0, b.Height ),
										new Point (b.Width /2 , 0),
										new Point (b.Width ,b.Height )};
			g.FillPolygon (foreBrush, pts);
			markers.Add ((Bitmap)b.Clone ());
			b.Dispose ();
			g.Dispose ();

			// Прямоугольник
			b = new Bitmap (standartMarkersSize, standartMarkersSize);
			g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			g.DrawRectangle (new Pen (foreBrush), 1, 1, b.Width - 2, b.Height - 2);
			markers.Add ((Bitmap)b.Clone ());
			b.Dispose ();
			g.Dispose ();

			// Колечко
			b = new Bitmap (standartMarkersSize, standartMarkersSize);
			g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			g.DrawEllipse (new Pen (foreBrush), 1, 1, b.Width - 3, b.Height - 3);
			markers.Add ((Bitmap)b.Clone ());
			b.Dispose ();
			g.Dispose ();

			// Крестик
			b = new Bitmap (standartMarkersSize, standartMarkersSize);
			g = Graphics.FromImage (b);
			g.FillRectangle (backBrush, 0, 0, b.Width, b.Height);
			g.DrawLine (new Pen (foreBrush), 1, 1, b.Width - 2, b.Height - 2);
			g.DrawLine (new Pen (foreBrush), 1, b.Height - 2, b.Width - 2, 1);
			markers.Add ((Bitmap)b.Clone ());
			b.Dispose ();
			g.Dispose ();

			// Завершение
			foreBrush.Dispose ();
			backBrush.Dispose ();
			#endregion

			#region Загрузка дополнительных маркеров из файлов
			// Проверка наличия папки
			if (!Directory.Exists (AboutForm.AppStartupPath + markersDirectory))
				{
				try
					{
					Directory.CreateDirectory (AboutForm.AppStartupPath + markersDirectory);
					}
				catch
					{
					}
				return;
				}

			// Получение списка файлов
			string[] markersImages;
			try
				{
				markersImages = Directory.GetFiles (AboutForm.AppStartupPath + markersDirectory, "*.png");
				}
			catch
				{
				return;
				}

			// Загрузка изображений
			for (int i = 0; (i < markersImages.Length) && (i < MaxMarkers); i++)
				{
				// Попытка открытия
				try
					{
					b = (Bitmap)Image.FromFile (markersImages[i]);
					if ((b.Width < 3) || (b.Height < 3) || (b.Width > 17) || (b.Height > 17) || (b.Width != b.Height))
						throw new Exception ();
					}
				catch
					{
					continue;
					}

				// Замещение цветов
				for (int y = 0; y < b.Height; y++)
					{
					for (int x = 0; x < b.Width; x++)
						{
						if ((b.GetPixel (x, y).ToArgb () & 0xFFFFFF) == 0xFFFFFF)
							{
							b.SetPixel (x, y, Color.FromArgb (255, 255, 255));
							}
						else
							{
							b.SetPixel (x, y, Color.FromArgb (0, 0, 0));
							}
						}
					}

				// Установка белого как прозрачного
				b.MakeTransparent (Color.FromArgb (255, 255, 255));

				// Добавление
				markers.Add ((Bitmap)b.Clone ());
				b.Dispose ();
				}

			// Завершение
			#endregion
			}

		/// <summary>
		/// Метод освобождает все используемые ресурсы
		/// </summary>
		public void Dispose ()
			{
			for (int i = 0; i < markers.Count; i++)
				{
				markers[i].Dispose ();
				}

			markers.Clear ();
			}
		}
	}
