using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму настроек программы
	/// </summary>
	public partial class DiagramMerger: Form
		{
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
		public DiagramMerger (string Line1Name, string Line2Name)
			{
			// Инициализация и локализация формы
			InitializeComponent ();

			// Кнопки
			RDLocale.SetControlsText (this);

			// Панели
			RDLocale.SetControlsText (MergingAxes);
			RDLocale.SetControlsText (MergingVariant);
			/*Ox.Text = Ox.Name;
			Oy.Text = Oy.Name;*/

			ApplyButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK);
			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			/*this.Text = RDLocale.GetControlText (this.Name, "T");*/
			this.Text = RDLocale.GetText (this.Name + "_T");

			// Сохранение параметров
			string line = RDLocale.GetText (MergingVariant.Name + "_Line");
			/*FirstLine.Text = RDLocale.GetControlText (MergingVariant.Name, "Line") + " " + Line1Name;
			SecondLine.Text = RDLocale.GetControlText (MergingVariant.Name, "Line") + " " + Line2Name;*/
			FirstLine.Text = line + " " + Line1Name;
			SecondLine.Text = line + " " + Line2Name;

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void SaveAbort_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Сохранение
		private void SaveSettings_Click (object sender, EventArgs e)
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
