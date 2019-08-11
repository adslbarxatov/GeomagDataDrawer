﻿using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму выбора данных для построения новой кривой
	/// </summary>
	public partial class ColumnsAdder:Form
		{
		/// <summary>
		/// Возвращает номер столбца данных, интерпретируемого как столбец абсцисс
		/// </summary>
		public uint XColumnNumber
			{
			get
				{
				return xColumnNumber;
				}
			}
		private uint xColumnNumber = 0;

		/// <summary>
		/// Возвращает номер столбца данных, интерпретируемого как столбец ординат
		/// </summary>
		public uint YColumnNumber
			{
			get
				{
				return yColumnNumber;
				}
			}
		private uint yColumnNumber = 1;

		/// <summary>
		/// Возвращает тип добавляемого объекта
		/// </summary>
		public DiagramAdditionalObjects AdditionalObjectType
			{
			get
				{
				return additionalObjectType;
				}
			}
		private DiagramAdditionalObjects additionalObjectType = DiagramAdditionalObjects.Text;

		/// <summary>
		/// Возвращает флаг, указывающий, является ли добавляемый объект диаграммой
		/// </summary>
		public bool IsNewObjectADiagram
			{
			get
				{
				return isNewObjectADiagram;
				}
			}
		private bool isNewObjectADiagram = true;

		/// <summary>
		/// Возвращает true, если выбор данных был отменён
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
		/// Конструктор. Запускает форму выбора данных
		/// </summary>
		/// <param name="SourceData">Исходные данные диаграммы</param>
		/// <param name="Language">Язык локализации</param>
		public ColumnsAdder (DiagramData SourceData, SupportedLanguages Language)
			{
			// Инициализация
			InitializeComponent ();
			this.Text = LanguageProvider.GetControlText ("ColumnsAdder", "TN", Language);

			// Заполнение списка типов объектов
			for (int i = 1; i <= 9; i++)
				{
				ObjectCombo.Items.Add (LanguageProvider.GetControlText ("ColumnsAdder", "ObjectType_" + i.ToString (), Language));
				}
			ObjectCombo.Text = ObjectCombo.Items[0].ToString ();

			// Основные настройки
			ColumnsAdderConstructor (SourceData, -1, -1, Language);
			}

		/// <summary>
		/// Конструктор. Запускает форму выбора данных, используя сведения о ранее выбранных столбцах
		/// </summary>
		/// <param name="SourceData">Исходные данные диаграммы</param>
		/// <param name="OldXColumnNumber">Номер ранее выбранного столбца данных, интерпретируемых как столбец абсцисс</param>
		/// <param name="OldYColumnNumber">Номер ранее выбранного столбца данных, интерпретируемых как столбец ординат</param>
		/// <param name="Language">Язык локализации</param>
		public ColumnsAdder (DiagramData SourceData, int OldXColumnNumber, int OldYColumnNumber, SupportedLanguages Language)
			{
			// Инициализация
			InitializeComponent ();
			this.Text = LanguageProvider.GetControlText ("ColumnsAdder", "TE", Language);

			// Запрет на обновление данных дополнительных объектов
			Radio02.Enabled = false;

			// Основные настройки
			ColumnsAdderConstructor (SourceData, OldXColumnNumber, OldYColumnNumber, Language);
			}

		// Конструктор
		private void ColumnsAdderConstructor (DiagramData SourceData, int OldXColumnNumber, int OldYColumnNumber, SupportedLanguages Language)
			{
			// Локазизация формы
			ApplyButton.Text = LanguageProvider.GetText ("ApplyButton", Language);
			AbortButton.Text = LanguageProvider.GetText ("AbortButton", Language);
			LanguageProvider.SetControlsText (this, Language);

			// Загрузка параметров
			for (uint col = 0; col < SourceData.DataColumnsCount; col++)
				{
				XCombo.Items.Add (SourceData.ColumnPresentation (col));
				YCombo.Items.Add (SourceData.ColumnPresentation (col));
				}

			// Определение отображаемых столбцов
			if ((OldXColumnNumber >= 0) && (OldXColumnNumber < SourceData.DataColumnsCount))
				{
				xColumnNumber = (uint)OldXColumnNumber;
				}
			if ((OldYColumnNumber >= 0) && (OldYColumnNumber < SourceData.DataColumnsCount))
				{
				yColumnNumber = (uint)OldYColumnNumber;
				}

			XCombo.Text = XCombo.Items[(int)xColumnNumber].ToString ();
			YCombo.Text = YCombo.Items[(int)yColumnNumber].ToString ();

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			xColumnNumber = (uint)XCombo.SelectedIndex;
			yColumnNumber = (uint)YCombo.SelectedIndex;
			additionalObjectType = (DiagramAdditionalObjects)(ObjectCombo.SelectedIndex + 1);
			isNewObjectADiagram = Radio01.Checked;

			cancelled = false;
			this.Close ();
			}

		// Выбор варианта добавления
		private void Radio01_CheckedChanged (object sender, System.EventArgs e)
			{
			XCombo.Enabled = YCombo.Enabled = Radio01.Checked;
			ObjectCombo.Enabled = Radio02.Checked;
			}
		}
	}
