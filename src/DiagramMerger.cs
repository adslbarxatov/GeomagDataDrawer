using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму настроек программы
	/// </summary>
	public partial class DiagramMerger:Form
		{
		/// <summary>
		/// Оси совмещения
		/// </summary>
		public enum MergeAxes
			{
			/// <summary>
			/// Ox
			/// </summary>
			Ox,

			/// <summary>
			/// Oy
			/// </summary>
			Oy,

			/// <summary>
			/// Обе
			/// </summary>
			Both
			}

		/// <summary>
		/// Выбранная ось совмещения
		/// </summary>
		public MergeAxes MergeAxe
			{
			get
				{
				return mergeAxe;
				}
			}
		private MergeAxes mergeAxe = MergeAxes.Ox;

		/// <summary>
		/// Варианты совмещения
		/// </summary>
		public enum MergeVariants
			{
			/// <summary>
			/// На месте первой кривой
			/// </summary>
			FirstLine,

			/// <summary>
			/// На месте второй кривой
			/// </summary>
			SecondLine
			}

		/// <summary>
		/// Выбранный вариант совмещения
		/// </summary>
		public MergeVariants MergeVariant
			{
			get
				{
				return mergeVariant;
				}
			}
		private MergeVariants mergeVariant = MergeVariants.FirstLine;

		/// <summary>
		/// Возвращает флаг, указывающий, было ли отменено совмещение
		/// </summary>
		public bool Cancelled
			{
			get
				{
				return cancelled;
				}
			}
		private bool cancelled = true;

		/// <summary>
		/// Возвращает флаг, указывающий, следует ли раскрашивать подписи осей
		/// </summary>
		public bool PaintAxes
			{
			get
				{
				return paintAxes;
				}
			}
		private bool paintAxes = false;

		/// <summary>
		/// Конструктор. Запускает настройку программы
		/// </summary>
		/// <param name="Line1Name">Имя первой совмещаемой кривой</param>
		/// <param name="Line2Name">Имя второй совмещаемой кривой</param>
		/// <param name="Language">Язык локализации</param>
		public DiagramMerger (string Line1Name, string Line2Name, SupportedLanguages Language)
			{
			// Инициализация и локализация формы
			InitializeComponent ();

			LanguageProvider.SetControlsText (this, Language);			// Кнопки
			LanguageProvider.SetControlsText (MergingAxes, Language);	// Панели
			LanguageProvider.SetControlsText (MergingVariant, Language);

			ApplyButton.Text = LanguageProvider.GetText ("ApplyButton", Language);
			AbortButton.Text = LanguageProvider.GetText ("AbortButton", Language);
			this.Text = LanguageProvider.GetControlText (this.Name, "T", Language);

			// Сохранение параметров
			FirstLine.Text = LanguageProvider.GetControlText (MergingVariant.Name, "Line", Language) + " " + Line1Name;
			SecondLine.Text = LanguageProvider.GetControlText (MergingVariant.Name, "Line", Language) + " " + Line2Name;

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void SaveAbort_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Сохранение
		private void SaveSettings_Click (object sender, System.EventArgs e)
			{
			// Возврат параметров
			if (Ox.Checked)
				{
				mergeAxe = MergeAxes.Ox;
				}
			else if (Oy.Checked)
				{
				mergeAxe = MergeAxes.Oy;
				}
			else
				{
				mergeAxe = MergeAxes.Both;
				}

			if (FirstLine.Checked)
				{
				mergeVariant = MergeVariants.FirstLine;
				}
			else
				{
				mergeVariant = MergeVariants.SecondLine;
				}

			paintAxes = PaintingAxes.Checked;

			// Выход
			cancelled = false;
			this.Close ();
			}
		}
	}
