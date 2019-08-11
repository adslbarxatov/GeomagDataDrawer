using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму редактирования данных диаграммы
	/// </summary>
	public partial class DiagramDataEditor:Form
		{
		// Переменные
		private SupportedLanguages language;	// Язык локализации

		/// <summary>
		/// Возвращает флаг, указывающий, были ли применены изменения
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
		/// Возвращает скорректированную таблицу данных или null
		/// в случае прерывания редактирования
		/// </summary>
		public DataTable ResultTable
			{
			get
				{
				return resultTable;
				}
			}
		private DataTable resultTable = null;

		/// <summary>
		/// Конструктор. Запускает форму редактирования данных
		/// </summary>
		/// <param name="Language">Язык локализации</param>
		/// <param name="SourceData">Данные диаграммы</param>
		public DiagramDataEditor (DiagramData SourceData, SupportedLanguages Language)
			{
			// Инициализация и локализация формы
			InitializeComponent ();

			LanguageProvider.SetControlsText (this, Language);	// Кнопки
			LanguageProvider.SetControlsText (ColumnNameInput, MainToolTip, Language);	// Панель имени столбца
			LanguageProvider.SetControlsText (this, MainToolTip, Language);	// Подсказки

			SaveButton.Text = LanguageProvider.GetText ("SaveButton", Language);
			AbortButton.Text = LanguageProvider.GetText ("AbortButton", Language);
			this.Text = LanguageProvider.GetControlText (this.Name, "T", Language);

			// Сохранение параметров
			language = Language;

			// Загрузка данных
			MainDataGrid.DataSource = SourceData.GetDataTable ();
			for (int i = 0; i < MainDataGrid.Columns.Count; i++)
				{
				MainDataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
				}

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			cancelled = true;

			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			cancelled = false;

			this.Close ();
			}

		// Обработка ошибки ввода данных
		private void MainDataGrid_DataError (object sender, DataGridViewDataErrorEventArgs e)
			{
			MessageBox.Show (LanguageProvider.GetText ("IncorrectValueError", language),
				ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

		// Добавление строки
		private void AddRowBefore_Click (object sender, System.EventArgs e)
			{
			DataTable table = (DataTable)MainDataGrid.DataSource;

			if (MainDataGrid.SelectedCells.Count != 0)
				{
				table.Rows.InsertAt (table.NewRow (), MainDataGrid.SelectedCells[0].RowIndex);
				}
			else
				{
				table.Rows.InsertAt (table.NewRow (), 0);
				}
			}

		private void AddRowAfter_Click (object sender, System.EventArgs e)
			{
			DataTable table = (DataTable)MainDataGrid.DataSource;

			if (MainDataGrid.SelectedCells.Count != 0)
				{
				table.Rows.InsertAt (table.NewRow (), MainDataGrid.SelectedCells[0].RowIndex + 1);
				}
			else
				{
				table.Rows.InsertAt (table.NewRow (), 0);
				}
			}

		// Удаление строк
		private void DeleteRow_Click (object sender, System.EventArgs e)
			{
			// Контроль количества строк
			if (MainDataGrid.Rows.Count <= 2)
				{
				MessageBox.Show (LanguageProvider.GetText ("NotEnoughRowsError", language), ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			DataTable table = (DataTable)MainDataGrid.DataSource;
			List<int> indices = new List<int> ();

			// Индексирование удаляемого диапазона
			for (int i = 0; i < MainDataGrid.SelectedCells.Count; i++)
				{
				if (!indices.Contains (MainDataGrid.SelectedCells[i].RowIndex))
					{
					indices.Add (MainDataGrid.SelectedCells[i].RowIndex);
					}
				}
			indices.Sort ();

			// Удаление
			for (int i = 0; i < indices.Count; i++)
				{
				table.Rows.RemoveAt (indices[i] - i);
				}

			// Переход на заведомо оставшуюся строку 
			if ((indices.Count != 0) && (MainDataGrid.Rows.Count != 0))
				{
				MainDataGrid.ClearSelection ();
				if (indices[0] - 1 < 0)
					{
					MainDataGrid.Rows[0].Cells[0].Selected = true;
					}
				else
					{
					MainDataGrid.Rows[indices[0] - 1].Cells[0].Selected = true;
					}
				}
			}

		// Подъём строки
		private void MoveRowUp_Click (object sender, System.EventArgs e)
			{
			// Контроль
			if ((MainDataGrid.Rows.Count < 2) || (MainDataGrid.SelectedCells[0].RowIndex == 0))
				{
				return;
				}

			DataTable table = (DataTable)MainDataGrid.DataSource;

			// Фиксация индекса
			int insertedIndex = MainDataGrid.SelectedCells[0].RowIndex - 1;

			// Добавление и заполнение строки
			table.Rows.InsertAt (table.NewRow (), insertedIndex++);
			table.Rows[insertedIndex - 1].ItemArray =
				(object[])table.Rows[insertedIndex + 1].ItemArray.Clone ();

			// Удаление старой строки
			table.Rows.RemoveAt (insertedIndex + 1);

			// Переход на сдвинутую строку 
			MainDataGrid.ClearSelection ();
			MainDataGrid.Rows[insertedIndex - 1].Cells[0].Selected = true;
			}

		// Спуск строки
		private void MoveRowDown_Click (object sender, System.EventArgs e)
			{
			// Контроль
			if ((MainDataGrid.Rows.Count < 2) || (MainDataGrid.SelectedCells[0].RowIndex == MainDataGrid.Rows.Count - 1))
				{
				return;
				}

			DataTable table = (DataTable)MainDataGrid.DataSource;

			// Фиксация индекса
			int insertedIndex = MainDataGrid.SelectedCells[0].RowIndex + 2;

			// Добавление и заполнение строки
			table.Rows.InsertAt (table.NewRow (), insertedIndex);
			table.Rows[insertedIndex].ItemArray =
				(object[])table.Rows[insertedIndex - 2].ItemArray.Clone ();

			// Удаление старой строки
			table.Rows.RemoveAt (insertedIndex - 2);

			// Переход на сдвинутую строку 
			MainDataGrid.ClearSelection ();
			MainDataGrid.Rows[insertedIndex - 1].Cells[0].Selected = true;
			}

		// Изменение размера окна
		private void DiagramDataEditor_Resize (object sender, System.EventArgs e)
			{
			MainDataGrid.Width = this.Width - 35;
			ColumnNameInput.Width = this.Width - 39;
			MainDataGrid.Height = this.Height - 135;

			MoveRowUp.Top = MoveRowDown.Top = AddRowBefore.Top = AddRowAfter.Top = DeleteRow.Top = this.Height - 115;

			SaveButton.Top = AbortButton.Top = this.Height - 65;
			SaveButton.Left = this.Width / 2 - 3 - SaveButton.Width;
			AbortButton.Left = this.Width / 2 + 3;
			}

		// Закрытие формы
		private void DiagramDataEditor_FormClosing (object sender, FormClosingEventArgs e)
			{
			// Прерывание выхода в случае ввода имени столбца
			if (!SaveButton.Enabled)
				{
				e.Cancel = true;
				return;
				}

			// Подтверждение
			if (cancelled)
				{
				// Проверка на отмену изменений
				if (MessageBox.Show (LanguageProvider.GetText ("AbortChanges", language), ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
					{
					e.Cancel = true;
					return;
					}
				}
			else
				{
				// Проверка на применение изменений
				if (MessageBox.Show (LanguageProvider.GetText ("ApplyChanges", language), ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
					e.Cancel = true;
					return;
					}

				// Тестовый прогон данных
				DiagramData ddt = new DiagramData ((DataTable)MainDataGrid.DataSource);

				if (ddt.InitResult != DiagramDataInitResults.Ok)
					{
					MessageBox.Show (string.Format (LanguageProvider.GetText ("DataProcessingError", language),
						DiagramDataInitResultsMessage.ErrorMessage (DiagramDataInitResults.BrokenTable, language)),
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					e.Cancel = true;
					return;
					}

				// Успешно
				resultTable = (DataTable)MainDataGrid.DataSource;
				}
			}

		// Изменение названия столбца
		private int columnWithNewName = 0;
		private void MainDataGrid_ColumnHeaderMouseDoubleClick (object sender, DataGridViewCellMouseEventArgs e)
			{
			ColumnNameInput.Visible = NewColumnName.Enabled = ApplyName.Enabled = AbortName.Enabled = true;
			MainDataGrid.Enabled = MoveRowDown.Enabled = MoveRowUp.Enabled =
				AddRowAfter.Enabled = AddRowBefore.Enabled = DeleteRow.Enabled =
				SaveButton.Enabled = AbortButton.Enabled = false;

			NewColumnName.Text = ((DataTable)MainDataGrid.DataSource).Columns[columnWithNewName = e.ColumnIndex].Caption;
			}

		// Применение названия
		private void ApplyName_Click (object sender, System.EventArgs e)
			{
			((DataTable)MainDataGrid.DataSource).Columns[columnWithNewName].Caption = NewColumnName.Text;
			MainDataGrid.Columns[columnWithNewName].HeaderText = NewColumnName.Text;

			AbortName_Click (null, null);
			}

		// Сохранение исходного названия
		private void AbortName_Click (object sender, System.EventArgs e)
			{
			ColumnNameInput.Visible = NewColumnName.Enabled = ApplyName.Enabled = AbortName.Enabled = false;
			MainDataGrid.Enabled = MoveRowDown.Enabled = MoveRowUp.Enabled =
				AddRowAfter.Enabled = AddRowBefore.Enabled = DeleteRow.Enabled =
				SaveButton.Enabled = AbortButton.Enabled = true;
			}
		}
	}
