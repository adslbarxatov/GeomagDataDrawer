using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму настройки осей диаграммы
	/// </summary>
	public partial class AxesStyle:Form
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

		/// <summary>
		/// Возвращает состояние опции "Определить автоматически" [число основных делений]
		/// </summary>
		public bool AutomaticDivisions
			{
			get
				{
				return AutoDivisions.Checked;
				}
			}

		/// <summary>
		/// Конструктор. Запускает форму настройки осей диаграммы
		/// </summary>
		/// <param name="OldStyle">Исходный стиль диаграммы</param>
		public AxesStyle (DiagramStyle OldStyle)
			{
			InitializeComponent ();

			// Сохранение значений
			dStyle = new DiagramStyle (OldStyle);

			// Настройка контролов
			OxPrimaryDiv.Minimum = DiagramStyle.MinPrimaryDivisions;
			OxPrimaryDiv.Maximum = DiagramStyle.MaxPrimaryDivisions;
			OxPrimaryDiv.Value = dStyle.XPrimaryDivisions;
			OyPrimaryDiv.Minimum = DiagramStyle.MinPrimaryDivisions;
			OyPrimaryDiv.Maximum = DiagramStyle.MaxPrimaryDivisions;
			OyPrimaryDiv.Value = dStyle.YPrimaryDivisions;
			OxSecondaryDiv.Minimum = DiagramStyle.MinSecondaryDivisions;
			OxSecondaryDiv.Maximum = DiagramStyle.MaxSecondaryDivisions;
			OxSecondaryDiv.Value = dStyle.XSecondaryDivisions;
			OySecondaryDiv.Minimum = DiagramStyle.MinSecondaryDivisions;
			OySecondaryDiv.Maximum = DiagramStyle.MaxSecondaryDivisions;
			OySecondaryDiv.Value = dStyle.YSecondaryDivisions;

			AxesWidth.Minimum = DiagramStyle.MinLineWidth;
			AxesWidth.Maximum = DiagramStyle.MaxLineWidth;
			AxesWidth.Value = dStyle.AxesLinesWidth;
			AxesColor.BackColor = dStyle.AxesColor;

			OxPlacementCombo.Items.Add ("Автоматически");
			OxPlacementCombo.Items.Add ("Сверху");
			OxPlacementCombo.Items.Add ("Снизу");
			OxPlacementCombo.Items.Add ("По центру");
			OxPlacementCombo.SelectedIndex = (int)dStyle.OxPlacement;

			OyPlacementCombo.Items.Add ("Автоматически");
			OyPlacementCombo.Items.Add ("Слева");
			OyPlacementCombo.Items.Add ("Справа");
			OyPlacementCombo.Items.Add ("Посередине");
			OyPlacementCombo.SelectedIndex = (int)dStyle.OyPlacement;

			OxFormatCombo.Items.Add ("Нормальный");
			OxFormatCombo.Items.Add ("Экспоненциальный");
			OxFormatCombo.SelectedIndex = (int)dStyle.OxFormat;

			OyFormatCombo.Items.Add ("Нормальный");
			OyFormatCombo.Items.Add ("Экспоненциальный");
			OyFormatCombo.SelectedIndex = (int)dStyle.OyFormat;

			// Запуск
			this.ShowDialog ();
			}

		// Выбор цвета
		private void AxesColor_Click (object sender, System.EventArgs e)
			{
			ColorSelectDialog.Color = AxesColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			AxesColor.BackColor = ColorSelectDialog.Color;
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			dStyle.XPrimaryDivisions = (uint)OxPrimaryDiv.Value;
			dStyle.YPrimaryDivisions = (uint)OyPrimaryDiv.Value;
			dStyle.XSecondaryDivisions = (uint)OxSecondaryDiv.Value;
			dStyle.YSecondaryDivisions = (uint)OySecondaryDiv.Value;
			dStyle.AxesLinesWidth = (uint)AxesWidth.Value;
			dStyle.AxesColor = AxesColor.BackColor;
			dStyle.OxPlacement = (DiagramStyle.AxesPlacements)OxPlacementCombo.SelectedIndex;
			dStyle.OyPlacement = (DiagramStyle.AxesPlacements)OyPlacementCombo.SelectedIndex;
			dStyle.OxFormat = (DiagramStyle.NumbersFormat)OxFormatCombo.SelectedIndex;
			dStyle.OyFormat = (DiagramStyle.NumbersFormat)OyFormatCombo.SelectedIndex;

			this.Close ();
			}

		// Отключение осей
		private void TurnOff_Click (object sender, System.EventArgs e)
			{
			AxesColor.BackColor = DiagramStyle.ImageBackColor;
			}

		// Блокировка контролов при установке опции автоопределения числа основных делений
		private void AutoDivisions_CheckedChanged (object sender, System.EventArgs e)
			{
			OxPrimaryDiv.Enabled = OyPrimaryDiv.Enabled = !AutoDivisions.Checked;
			}
		}
	}
