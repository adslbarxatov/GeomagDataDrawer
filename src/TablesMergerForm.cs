using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает главную форму програмы
	/// </summary>
	public partial class TableMergerForm:Form
		{
		// Переменные
		private SupportedLanguages formLanguage = SupportedLanguages.ru_ru;

		private List<List<List<double>>> dataTables = new List<List<List<double>>> ();
		private List<List<string>> columnNames = new List<List<string>> ();
		private List<uint> abscissasColumnsNumbers = new List<uint> ();

		private List<SourceTableRow> dataRows = new List<SourceTableRow> ();

		private List<List<double>> mergedTable = new List<List<double>> ();
		private List<string> mergedColumnNames = new List<string> ();

		private bool allowClose = false;
		private int mergeType;
		private bool success = false;

		/// <summary>
		/// Конструктор. Запускает интерфейс мерджера
		/// </summary>
		public TableMergerForm ()
			{
			// Инициализация окна
			InitializeComponent ();

			// Настройка контролов
			OFDialog.Filter = "Все файлы данных|*.*";
			OFDialog.Multiselect = true;
			OFDialog.Title = LanguageProvider.GetControlText ("MainForm", "OFDialog", formLanguage);
			SFDialog.Filter = string.Format (LanguageProvider.GetControlText ("MainForm", "SFDialog_F", formLanguage),
				"Geomag data drawer");
			SFDialog.FilterIndex = 3;
			SFDialog.Title = LanguageProvider.GetControlText ("MainForm", "SFDialog", formLanguage);

			ProcessingResults.Items.Add (ProgramDescription.AssemblyTitle + " запущен в " + DateTime.Now.ToString ("dd.MM.yyyy HH:mm"));
			ProcessingResults.SelectedIndex = ProcessingResults.Items.Count - 1;

			MergeType.Items.Add ("Использовать абсциссы, имеющиеся во всех файлах");
			MergeType.Items.Add ("Добавлять недостающие абсциссы, заполнять ординаты нулями");
			//MergeType.Items.Add ("Добавлять недостающие абсциссы, интерполировать ординаты");
			MergeType.SelectedIndex = 0;

			this.Text = ProgramDescription.AssemblyTitle;
			}

		// Добавление файлов в обработку
		private void AddFiles_Click (object sender, System.EventArgs e)
			{
			OFDialog.ShowDialog ();
			}

		private void OFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Запрос параметров обработки файлов
			UnknownFileParSelector ufps = new UnknownFileParSelector (2, formLanguage, true);
			if (ufps.Cancelled)
				return;

			ColumnsNamesSelector cns = new ColumnsNamesSelector (0, formLanguage);
			if (cns.Cancelled)
				return;

			// Для каждого файла
			for (int i = 0; i < OFDialog.FileNames.Length; i++)
				{
				// Формирование таблицы данных
				DiagramData dd = new DiagramData (OFDialog.FileNames[i], ufps.DataColumnsCount, cns.SkippedRowsCount);
				if (dd.InitResult != DiagramDataInitResults.Ok)
					{
					ProcessingResults.Items.Add ("Файл «" + Path.GetFileName (OFDialog.FileNames[i]) + "» не был добавлен: " +
						DiagramDataInitResultsMessage.ErrorMessage (dd.InitResult, formLanguage));
					continue;
					}

				// Добавление в списки
				dataTables.Add (dd.GetData ());
				columnNames.Add (new List<string> ());
				for (uint c = 0; c < dd.DataColumnsCount; c++)
					{
					columnNames[columnNames.Count - 1].Add (dd.GetDataColumnName (c));
					}
				abscissasColumnsNumbers.Add (ufps.AbscissasColumn);
				FileNamesList.Items.Add (OFDialog.FileNames[i]);
				ProcessingResults.Items.Add ("Добавлен файл «" + Path.GetFileName (OFDialog.FileNames[i]) + "»: " +
					dataTables[dataTables.Count - 1].Count + " столбцов, " + dataTables[dataTables.Count - 1][0].Count + " строк");
				}

			// Завершено
			ProcessingResults.SelectedIndex = ProcessingResults.Items.Count - 1;
			}

		// Сброс списка файлов
		private void ClearFiles_Click (object sender, EventArgs e)
			{
			FileNamesList.Items.Clear ();
			dataTables.Clear ();
			columnNames.Clear ();
			abscissasColumnsNumbers.Clear ();
			ProcessingResults.Items.Add ("Список файлов сброшен");
			ProcessingResults.SelectedIndex = ProcessingResults.Items.Count - 1;
			}

		// Запуск обработки
		private void BeginProcessing_Click (object sender, EventArgs e)
			{
			// Контроль
			if (dataTables.Count < 2)
				{
				MessageBox.Show ("Недостаточно файлов для объединения", ProgramDescription.AssemblyTitle,
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Подготовка
			AddFiles.Enabled = ClearFiles.Enabled = MergeType.Enabled = BeginProcessing.Enabled =
				SaveResult.Enabled = BExit.Enabled = success = false;
			dataRows.Clear ();
			mergedTable.Clear ();
			mergedColumnNames.Clear ();
			mergeType = MergeType.SelectedIndex;

			// Запуск 
			HardWorkExecutor hwe = new HardWorkExecutor (ExecuteMerge);

			// Завершено
			AddFiles.Enabled = ClearFiles.Enabled = MergeType.Enabled = BeginProcessing.Enabled =
				BExit.Enabled = true;
			if (success)
				{
				ProcessingResults.Items.Add ("Сформирована таблица: " + mergedTable.Count.ToString () + " строк, " +
					mergedTable[0].Count.ToString () + " столбцов");
				ProcessingResults.SelectedIndex = ProcessingResults.Items.Count - 1;
				SaveResult.Enabled = true;
				}
			}

		// Процесс, выполняющий объединение
		private void ExecuteMerge (object sender, DoWorkEventArgs e)
			{
			// Перегонка данных
			mergedColumnNames.Add ("x");

			for (int f = 0; f < dataTables.Count; f++)
				{
				// Сборка строк
				for (int r = 0; r < dataTables[f][(int)abscissasColumnsNumbers[f]].Count; r++)
					{
					// Возврат прогресса
					((BackgroundWorker)sender).ReportProgress ((int)(100.0 * (double)r /
						(double)dataTables[f][(int)abscissasColumnsNumbers[f]].Count),
						"Сборка строк: файл №" + (f + 1).ToString () + ", строка " + r.ToString ());

					// Создание строки
					dataRows.Add (new SourceTableRow ((uint)f, dataTables[f][(int)abscissasColumnsNumbers[f]][r]));

					for (int c = 0; c < dataTables[f].Count; c++)
						{
						// Пропуск столбца абсцисс
						if (c == abscissasColumnsNumbers[f])
							continue;

						// Добавление ординат
						dataRows[dataRows.Count - 1].AddOrdinate (dataTables[f][c][r]);

						// Завершение работы, если получено требование от диалога
						if (((BackgroundWorker)sender).CancellationPending)
							{
							e.Cancel = true;
							return;
							}
						}
					}

				// Сборка имён столбцов
				for (int c = 0; c < columnNames[f].Count; c++)
					{
					if (c != (int)abscissasColumnsNumbers[f])
						mergedColumnNames.Add (columnNames[f][c]);
					}
				}

			// Сортировка
			((BackgroundWorker)sender).ReportProgress (-1, "Сортировка строк...");
			dataRows.Sort ();

			// Сборка итоговой таблицы
			double currentAbscissa = double.NaN;
			for (int r = 0; r < dataRows.Count; r++)
				{
				// Возврат прогресса
				((BackgroundWorker)sender).ReportProgress ((int)(100.0 * (double)r / (double)dataRows.Count),
					"Объединение строк в таблицу: строка " + r.ToString ());

				// Добавление строк в таблицу
				if (currentAbscissa != dataRows[r].X)
					{
					currentAbscissa = dataRows[r].X;
					mergedTable.Add (new List<double> ());
					mergedTable[mergedTable.Count - 1].Add (dataRows[r].X);
					}

				// Сборка строк
				for (uint t = 0; t < dataTables.Count; t++)
					{
					// i может отличаться от r в случае пропусков строк
					int i = dataRows.IndexOf (new SourceTableRow (t, currentAbscissa));

					// Требуется дозаполнение
					if (i < 0)
						{
						switch (mergeType)
							{
							// Удаление неполных строк
							case 0:
								// Следующая схема вызовет выход из цикла обработки строк с данной ординатой
								mergedTable.RemoveAt (mergedTable.Count - 1);
								t = (uint)dataTables.Count - 1;
								break;

							// Заполнение нулями
							case 1:
								for (int c = 1; c < dataTables[(int)t].Count; c++)
									{
									mergedTable[mergedTable.Count - 1].Add (0);
									}
								break;
							}
						}

					// Объект найден
					else
						{
						mergedTable[mergedTable.Count - 1].AddRange (dataRows[i].Y);
						}
					}

				// Пропуск возможных дублей строк
				r = dataRows.FindLastIndex (x => x.X == currentAbscissa);
				}

			// Финальный контроль
			if (mergedTable.Count < 2)
				{
				MessageBox.Show ("В объединённой таблице недостаточно строк, соответствующих условиям отбора",
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Завершено
			e.Result = null;
			success = true;
			}

		// Выход из программы
		private void BExit_Click (object sender, EventArgs e)
			{
			if (MessageBox.Show ("Все несохранённые результаты обработки будут утеряны.\nВыйти из программы?",
				ProgramDescription.AssemblyTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
				allowClose = true;
				this.Close ();
				}
			}

		private void TableMergerForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			e.Cancel = !allowClose;
			}

		// Сохранение таблицы
		private void SaveResult_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			DiagramData dd = new DiagramData (mergedTable, mergedColumnNames);
			if (dd.SaveDataFile (SFDialog.FileName, (DataOutputTypes)SFDialog.FilterIndex, true) != 0)
				{
				MessageBox.Show ("Не удалось сохранить файл данных", ProgramDescription.AssemblyTitle,
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			else
				{
				ProcessingResults.Items.Add ("Файл с конечной таблицей сохранён");
				ProcessingResults.SelectedIndex = ProcessingResults.Items.Count - 1;
				}
			}
		}
	}
