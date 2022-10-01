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
		/// <param name="Language">Язык локализации</param>
		public ColumnsNamesSelector (uint SkippedRowsCount, SupportedLanguages Language)
			{
			// Инициализация и локализация формы
			InitializeComponent ();
			Localization.SetControlsText (this, Language);
			ApplyButton.Text = Localization.GetText ("ApplyButton", Language);
			AbortButton.Text = Localization.GetText ("AbortButton", Language);
			this.Text = Localization.GetControlText (this.Name, "T", Language);

			// Настройка контролов
			ColumnsCount.Maximum = ConfigAccessor.MaxSkippedLinesCount;
			try
				{
				ColumnsCount.Value = skippedRowsCount = SkippedRowsCount;
				}
			catch { }

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
