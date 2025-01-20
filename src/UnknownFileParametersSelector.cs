using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму выбора параметров извлечения данных из текстовых файлов
	/// </summary>
	public partial class UnknownFileParametersSelector: Form
		{
		/// <summary>
		/// Конструктор. Запускает форму
		/// </summary>
		/// <param name="ExpectedColumnsCount">Ожидаемое количество столбцов, полученное из конфигурации 
		/// программы</param>
		/// <param name="SelectAbscissas">Флаг, указывающий, следует ли запрашивать номер столбца абсцисс</param>
		public UnknownFileParametersSelector (uint ExpectedColumnsCount, bool SelectAbscissas)
			{
			// Инициализация и локализация формы
			InitializeComponent ();
			RDLocale.SetControlsText (this);
			ApplyButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK);
			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			/*this.Text = RDLocale.GetControlText (this.Name, "T");*/
			this.Text = RDLocale.GetText (this.Name + "_T");

			// Настройка контролов
			ColumnsCount.Maximum = ConfigAccessor.MaxExpectedColumnsCount;
			try
				{
				ColumnsCount.Value = dataColumnsCount = ExpectedColumnsCount;
				Abscissas.Maximum = ColumnsCount.Value + 1;
				Abscissas.Value = abscissasColumn = 1;
				}
			catch { }

			Label03.Visible = Abscissas.Visible = SelectAbscissas;

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void BAbort_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		/// <summary>
		/// Возвращает флаг, сообщающий об отмене операции
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
		/// Возвращает ожидаемое количество столбцов данных
		/// </summary>
		public uint DataColumnsCount
			{
			get
				{
				return dataColumnsCount;
				}
			}
		private uint dataColumnsCount;

		/// <summary>
		/// Возвращает номер столбца (начиная с 0), значения которого следует использовать как абсциссы
		/// </summary>
		public uint AbscissasColumn
			{
			get
				{
				return abscissasColumn - 1;
				}
			}
		private uint abscissasColumn;

		// Извлечение
		private void GetData_Click (object sender, EventArgs e)
			{
			// Сохранение параметров
			dataColumnsCount = (uint)ColumnsCount.Value;
			abscissasColumn = (uint)Abscissas.Value;

			// Завершение
			cancelled = false;
			this.Close ();
			}

		// Изменение количества столбцов
		private void ColumnsCount_ValueChanged (object sender, EventArgs e)
			{
			Abscissas.Maximum = ColumnsCount.Value;
			}
		}
	}
