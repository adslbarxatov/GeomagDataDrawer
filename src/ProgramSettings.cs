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
			SaveButton.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Save);
			AbortButton.Text = Localization.GetDefaultText (LzDefaultTextValues.Button_Cancel);
			this.Text = Localization.GetControlText (this.Name, "T");

			// Настройка контролов
			ConfirmExit.Checked = ca.ForceExitConfirmation;

			if (RDGenerics.IsStartupPathAccessible)
				ForceUsingBackupFile.Checked = ca.ForceUsingBackupDataFile;
			else
				ForceUsingBackupFile.Checked = ForceUsingBackupFile.Enabled = false;

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
