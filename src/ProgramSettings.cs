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

			Localization.SetControlsText (this, ca.InterfaceLanguage);
			SaveButton.Text = Localization.GetText ("SaveButton", ca.InterfaceLanguage);
			AbortButton.Text = Localization.GetText ("AbortButton", ca.InterfaceLanguage);
			this.Text = Localization.GetControlText (this.Name, "T", ca.InterfaceLanguage);

			// Настройка контролов
			ConfirmExit.Checked = ca.ForceExitConfirmation;
			ForceUsingBackupFile.Checked = ca.ForceUsingBackupDataFile;
			ForceShowDiagram.Checked = ca.ForceShowDiagram;
			ForceSavingColumnNames.Checked = ca.ForceSavingColumnNames;
			/*DisableMousePlacing.Checked = ca.DisableMousePlacing;*/

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
			/*ca.DisableMousePlacing = DisableMousePlacing.Checked;*/

			this.Close ();
			}
		}
	}
