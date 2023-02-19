using System;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму настроек программы
	/// </summary>
	public partial class ProgramSettings: Form
		{
		// Переменные
		private ConfigAccessor ca = new ConfigAccessor ();

		/// <summary>
		/// Конструктор. Запускает настройку программы
		/// </summary>
		public ProgramSettings ()
			{
			// Инициализация и локализация формы
			InitializeComponent ();

			Localization.SetControlsText (this);
			SaveButton.Text = Localization.GetText ("SaveButton");
			AbortButton.Text = Localization.GetText ("AbortButton");
			this.Text = Localization.GetControlText (this.Name, "T");

			// Настройка контролов
			ConfirmExit.Checked = ca.ForceExitConfirmation;
			ForceUsingBackupFile.Checked = ca.ForceUsingBackupDataFile;
			ForceShowDiagram.Checked = ca.ForceShowDiagram;
			ForceSavingColumnNames.Checked = ca.ForceSavingColumnNames;

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
			ca.ForceExitConfirmation = ConfirmExit.Checked;
			ca.ForceUsingBackupDataFile = ForceUsingBackupFile.Checked;
			ca.ForceShowDiagram = ForceShowDiagram.Checked;
			ca.ForceSavingColumnNames = ForceSavingColumnNames.Checked;

			this.Close ();
			}
		}
	}
