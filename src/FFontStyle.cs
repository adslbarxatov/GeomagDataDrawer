using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму настройки шрифта подписей диаграммы
	/// </summary>
	public partial class FFontStyle:Form
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
		/// Конструктор. Принимает исходный стиль диаграммы
		/// </summary>
		public FFontStyle (DiagramStyle DStyle)
			{
			InitializeComponent ();

			// Сохранение значений
			dStyle = new DiagramStyle (DStyle);

			// Настройка контролов
			FontSize.Minimum = DiagramStyle.MinFontSize;
			FontSize.Maximum = DiagramStyle.MaxFontSize;
			FontSize.Value = dStyle.FontSize;

			FontColor.BackColor = dStyle.FontColor;

			// Запуск
			this.ShowDialog ();
			}

		// Выбор цвета
		private void FontColor_Click (object sender, System.EventArgs e)
			{
			ColorSelectDialog.Color = FontColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			FontColor.BackColor = ColorSelectDialog.Color;
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			dStyle.FontSize = (uint)FontSize.Value;
			dStyle.FontColor = FontColor.BackColor;

			this.Close ();
			}
		}
	}
