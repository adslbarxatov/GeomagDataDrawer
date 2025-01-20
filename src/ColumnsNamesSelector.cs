using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму выбора параметров извлечения имён столбцов из файлов данных
	/// </summary>
	public partial class ColumnsNamesSelector: Form
		{
		/// <summary>
		/// Конструктор. Запускает форму
		/// </summary>
		/// <param name="SkippedRowsCount">Количество строк, используемое для поиска имён столбцов данных, 
		/// полученное из конфигурации программы</param>
		public ColumnsNamesSelector (uint SkippedRowsCount)
			{
			// Инициализация и локализация формы
			InitializeComponent ();
			RDLocale.SetControlsText (this);
			ApplyButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK);
			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			/*this.Text = RDLocale.GetControlText (this.Name, "T");*/
			this.Text = RDLocale.GetText (this.Name + "_T");

			// Настройка контролов
			ColumnsCount.Maximum = ConfigAccessor.MaxSkippedLinesCount;
			try
				{
				skippedRowsCount = SkippedRowsCount;
				ColumnsCount.Value = skippedRowsCount;
				}
			catch
				{
				ColumnsCount.Value = 0;
				}

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
		/// Возвращает количество строк, используемое для поиска имён столбцов данных
		/// </summary>
		public uint SkippedRowsCount
			{
			get
				{
				return skippedRowsCount;
				}
			}
		private uint skippedRowsCount;

		// Извлечение
		private void GetData_Click (object sender, EventArgs e)
			{
			// Сохранение параметров
			skippedRowsCount = (uint)ColumnsCount.Value;

			// Завершение
			cancelled = false;
			this.Close ();
			}
		}
	}
