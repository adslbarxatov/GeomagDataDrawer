using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму настройки сетки диаграммы
	/// </summary>
	public partial class GridStyle:Form
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
		/// Конструктор. Запускает форму настройки сетки диаграммы
		/// </summary>
		/// <param name="OldStyle">Исходный стиль диаграммы</param>
		public GridStyle (DiagramStyle OldStyle)
			{
			InitializeComponent ();

			// Сохранение значений
			dStyle = new DiagramStyle (OldStyle);

			// Настройка контролов
			GridWidth.Minimum = DiagramStyle.MinLineWidth;
			GridWidth.Maximum = DiagramStyle.MaxLineWidth;
			GridWidth.Value = dStyle.GridLinesWidth;

			PrimaryColor.BackColor = dStyle.PrimaryGridColor;
			SecondaryColor.BackColor = dStyle.SecondaryGridColor;

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			dStyle.GridLinesWidth = (uint)GridWidth.Value;
			dStyle.PrimaryGridColor = PrimaryColor.BackColor;
			dStyle.SecondaryGridColor = SecondaryColor.BackColor;

			this.Close ();
			}

		// Выбор цвета
		private void PrimaryColor_Click (object sender, System.EventArgs e)
			{
			ColorSelectDialog.Color = PrimaryColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			PrimaryColor.BackColor = ColorSelectDialog.Color;
			}

		private void SecondaryColor_Click (object sender, System.EventArgs e)
			{
			ColorSelectDialog.Color = SecondaryColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			SecondaryColor.BackColor = ColorSelectDialog.Color;
			}

		// Отключение осей
		private void TurnOff_Click (object sender, System.EventArgs e)
			{
			PrimaryColor.BackColor = DiagramStyle.ImageBackColor;
			SecondaryColor.BackColor = DiagramStyle.ImageBackColor;
			}
		}
	}
