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
			RDGenerics.LoadWindowDimensions (this);

			RDLocale.SetControlsText (this);
			SaveButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Save);
			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			/*this.Text = RDLocale.GetControlText (this.Name, "T");*/
			this.Text = RDLocale.GetText (this.Name + "_T");

			// Настройка контролов
			ConfirmExit.Checked = ca.ForceExitConfirmation;

			if (RDGenerics.AppHasAccessRights (false, false))
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

		// Закрытие окна
		private void ProgramSettings_FormClosing (object sender, FormClosingEventArgs e)
			{
			RDGenerics.SaveWindowDimensions (this);
			}
		}
	}
