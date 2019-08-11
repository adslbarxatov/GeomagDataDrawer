using System;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму настройки кривых на диаграмме
	/// </summary>
	public partial class LineStyle:Form
		{
		/// <summary>
		/// Изменённый стиль диаграммы
		/// </summary>
		public DiagramStyle NewDiagramStyle
			{
			get
				{
				return dStyle;
				}
			}
		private DiagramStyle dStyle;

		// Загрузчик маркеров
		private MarkersLoader ml;

		/// <summary>
		/// Конструктор. Запускает форму настройки кривых на диаграмме
		/// </summary>
		/// <param name="OldStyle">Исходный стиль диаграммы</param>
		public LineStyle (DiagramStyle OldStyle)
			{
			InitializeComponent ();

			// Сохранение значений
			dStyle = new DiagramStyle (OldStyle);

			// Настройка контролов
			StyleCombo.Items.Add ("(—)");
			StyleCombo.Items.Add ("(—) + (■)");
			StyleCombo.Items.Add ("(■)");

			LineWidth.Minimum = DiagramStyle.MinLineWidth;
			LineWidth.Maximum = DiagramStyle.MaxLineWidth;

			// Загрузка значений
			LineWidth.Value = dStyle.LineWidth;
			LineColor.BackColor = dStyle.LineColor;
			StyleCombo.SelectedIndex = (int)dStyle.LineDrawingFormat;
			ml = new MarkersLoader (dStyle.LineColor);
			try
				{
				MarkerNumber.Value = dStyle.LineMarkerNumber;
				}
			catch
				{
				MarkerNumber.Value = 1;
				}

			// Загрузка дополнительных маркеров
			MarkerNumber.Minimum = 1;
			MarkerNumber.Maximum = ml.Markers.Count;
			MarkerImage.BackgroundImage = ml.Markers[(int)MarkerNumber.Value - 1];

			// Запуск
			this.ShowDialog ();
			}

		// Выбор цвета
		private void LineColor_Click (object sender, System.EventArgs e)
			{
			ColorSelectDialog.Color = LineColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			LineColor.BackColor = ColorSelectDialog.Color;

			ml = new MarkersLoader (LineColor.BackColor);
			MarkerNumber.Maximum = ml.Markers.Count;
			MarkerImage.BackgroundImage = ml.Markers[(int)MarkerNumber.Value - 1];
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			dStyle.LineWidth = (uint)LineWidth.Value;
			dStyle.LineColor = LineColor.BackColor;
			dStyle.LineDrawingFormat = (DiagramStyle.DrawingLinesFormats)StyleCombo.SelectedIndex;
			dStyle.LineMarkerNumber = (uint)MarkerNumber.Value;

			this.Close ();
			}

		// Изменение стиля
		private void StyleCombo_SelectedIndexChanged (object sender, System.EventArgs e)
			{
			MarkerLabel.Enabled = MarkerNumber.Enabled = (StyleCombo.SelectedIndex != 0);
			}

		// Изменение номера маркера
		private void MarkerNumber_ValueChanged (object sender, EventArgs e)
			{
			MarkerImage.BackgroundImage = ml.Markers[(int)MarkerNumber.Value - 1];
			}
		}
	}
