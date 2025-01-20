using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает форму генерации диаграммы на основе введённой формулы
	/// </summary>
	public partial class FormulaEvaluator: Form
		{
		/// <summary>
		/// Возвращает true, если ввод был отменён
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
		/// Возвращает массив абсцисс сгенерированной кривой
		/// </summary>
		public List<double> X
			{
			get
				{
				return x;
				}
			}
		private List<double> x = new List<double> ();

		/// <summary>
		/// Возвращает массив ординат сгенерированной кривой
		/// </summary>
		public List<List<double>> Y
			{
			get
				{
				return y;
				}
			}
		private List<List<double>> y = new List<List<double>> ();

		/// <summary>
		/// Возвращает список названий столбцов данных
		/// </summary>
		public List<string> ColumnsNames
			{
			get
				{
				return columnsNames;
				}
			}
		private List<string> columnsNames = new List<string> ();

		/// <summary>
		/// Конструктор. Запускает форму ввода формулы
		/// </summary>
		public FormulaEvaluator ()
			{
			// Инициализация и локазизация формы
			InitializeComponent ();

			/*this.Text = RDLocale.GetControlText ("FormulaEvaluator", "T");*/
			this.Text = RDLocale.GetText (this.Name + "_T");
			ApplyButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_OK);
			AbortButton.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Cancel);
			RDLocale.SetControlsText (this);

			StartValue.Maximum = EndValue.Maximum = (decimal)ExpressionEvaluator.EvaluationLimit;
			StartValue.Minimum = EndValue.Minimum = (decimal)-ExpressionEvaluator.EvaluationLimit;
			StepValue.Maximum = (decimal)ExpressionEvaluator.EvaluationLimit;

			// Начальная обработка
			AddButton_Click (null, null);
			StartValue_ValueChanged (null, null);

			// Запуск
			this.ShowDialog ();
			}

		// Отмена
		private void AbortButton_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, EventArgs e)
			{
			// Контроль диапазона
			if ((StepValue.Value == 0) ||
				((uint)(Math.Abs (EndValue.Value - StartValue.Value) / StepValue.Value) + 1 >
				DiagramData.MaxDataRows) || (EndValue.Value == StartValue.Value))
				{
				RDInterface.LocalizedMessageBox (RDMessageTypes.Warning_Center, "IncorrectRangeError");
				return;
				}

			// Блокировка
			this.Enabled = false;

			// Заполнение
			for (int c = 0; c < CurvesList.Items.Count; c++)
				{
				// Получение цепочки вычисления
				LexemesExtractor le = new LexemesExtractor (CurvesList.Items[c].ToString ());
				EvaluationChainAssembler eca = new EvaluationChainAssembler (le);
				ExpressionEvaluator ee;

				// Подготовка
				y.Add (new List<double> ());

				// Генерация
				for (double i = (double)Math.Min (StartValue.Value, EndValue.Value); i <
					(double)Math.Max (StartValue.Value, EndValue.Value); i += (double)StepValue.Value)
					{
					if (c == 0)
						x.Add (i);

					ee = new ExpressionEvaluator (eca, i);
					y[y.Count - 1].Add (ee.Result);
					}

				// Последнее значение
				if (c == 0)
					x.Add ((double)Math.Max (StartValue.Value, EndValue.Value));

				ee = new ExpressionEvaluator (eca, (double)Math.Max (StartValue.Value, EndValue.Value));
				y[y.Count - 1].Add (ee.Result);

				// Название столбца
				columnsNames.Add (CurvesList.Items[c].ToString ());

				// Прогон на точки разрыва
				for (int i = 2; i < y[y.Count - 1].Count; i++)
					{
					// Если два соседних значения отличаются по знаку и отстоят друг от друга
					// (по модулю) более чем на 10⁶, скорее всего, это – точка разрыва
					if (y[y.Count - 1][i - 2] * y[y.Count - 1][i] < -ExpressionEvaluator.EvaluationLimit)
						y[y.Count - 1][i - 1] = double.NaN;
					}
				}

			// Завершение
			cancelled = false;
			this.Close ();
			}

		// Обновление статуса проверки формулы и добавление кривой
		private void AddButton_Click (object sender, EventArgs e)
			{
			// Блокировка контролов
			FormulaBox.ReadOnly = true;
			AddButton.Enabled = DeleteButton.Enabled = false;
			ApplyButton.Enabled = AbortButton.Enabled = false;

			// Контроль порядка лексем в выражении
			LexemesExtractor le = new LexemesExtractor (FormulaBox.Text);
			switch (le.FollowingStatus)
				{
				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedConstant:
					InfoLabel.Text = string.Format (RDLocale.GetText ("MisplacedConstantError"), le.LastLexeme);
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedDivisionOperator:
					InfoLabel.Text = RDLocale.GetText ("MisplacedDivisionOperatorError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedExponentiationOperator:
					InfoLabel.Text = RDLocale.GetText ("MisplacedExponentiationOperatorError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedFunctionCall:
					InfoLabel.Text = string.Format (RDLocale.GetText ("MisplacedFunctionCallError"),
						le.LastLexeme);
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedLeftParenthesis:
					InfoLabel.Text = RDLocale.GetText ("MisplacedLeftParenthesisError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedMinusOperator:
					InfoLabel.Text = RDLocale.GetText ("MisplacedMinusOperatorError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedMultiplicationOperator:
					InfoLabel.Text = RDLocale.GetText ("MisplacedMultiplicationOperatorError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedNumber:
					InfoLabel.Text = string.Format (RDLocale.GetText ("MisplacedNumberError"),
						le.LastLexeme);
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedPlusOperator:
					InfoLabel.Text = RDLocale.GetText ("MisplacedPlusOperatorError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedRightParenthesis:
					InfoLabel.Text = RDLocale.GetText ("MisplacedRightParenthesisError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.MisplacedVariable:
					InfoLabel.Text = RDLocale.GetText ("MisplacedVariableError");
					break;

				case LexemesFollowingMatrix.LexemesFollowingStatuses.UnknownLexeme:
					InfoLabel.Text = string.Format (RDLocale.GetText ("UnknownLexemeError"),
						le.LastLexeme);
					break;
				}

			if (le.FollowingStatus != LexemesFollowingMatrix.LexemesFollowingStatuses.Ok)
				{
				InfoLabel.ForeColor = Color.FromArgb (128, 32, 0);

				FormulaBox.ReadOnly = false;
				AddButton.Enabled = (CurvesList.Items.Count < DiagramData.MaxLines);
				DeleteButton.Enabled = ApplyButton.Enabled = (CurvesList.Items.Count > 0);
				AbortButton.Enabled = true;

				return;
				}

			// Первичная обработка выражения
			switch (le.ExpressionStatus)
				{
				case LexemesExtractor.ExpressionStatuses.ExpressionIsEmpty:
					InfoLabel.Text = RDLocale.GetText ("ExpressionIsEmptyError");
					break;

				case LexemesExtractor.ExpressionStatuses.IncorrectExpressionEnd:
					InfoLabel.Text = string.Format (RDLocale.GetText ("IncorrectExpressionEndError"),
						le.LastLexeme);
					break;

				case LexemesExtractor.ExpressionStatuses.IncorrectExpressionStart:
					InfoLabel.Text = string.Format (RDLocale.GetText ("IncorrectExpressionStartError"),
						le.LastLexeme);
					break;

				case LexemesExtractor.ExpressionStatuses.LeftParenthesisWasNotClosed:
					InfoLabel.Text = RDLocale.GetText ("LeftParenthesisWasNotClosedError");
					break;

				case LexemesExtractor.ExpressionStatuses.MisplacedRightParenthesis:
					InfoLabel.Text = RDLocale.GetText ("UnexpectedRightParenthesisError");
					break;
				}

			if (le.ExpressionStatus != LexemesExtractor.ExpressionStatuses.Ok)
				{
				InfoLabel.ForeColor = Color.FromArgb (128, 0, 0);

				FormulaBox.ReadOnly = false;
				AddButton.Enabled = (CurvesList.Items.Count < DiagramData.MaxLines);
				DeleteButton.Enabled = ApplyButton.Enabled = (CurvesList.Items.Count > 0);
				AbortButton.Enabled = true;

				return;
				}

			// Проверка успешно завершена
			InfoLabel.ForeColor = Color.FromArgb (0, 128, 0);
			InfoLabel.Text = RDLocale.GetText ("NoErrors") + RDLocale.RN + "f(x) = ";
			string formula = "";
			for (int i = 0; i < le.ExtractedLexemes.Count; i++)
				{
				formula += le.ExtractedLexemes[i].LexemeValue;
				InfoLabel.Text += le.ExtractedLexemes[i].LexemeValue;
				}

			// Добавление кривой в список
			CurvesList.Items.Add (formula);

			// Разблокировка контролов
			FormulaBox.ReadOnly = false;
			AddButton.Enabled = (CurvesList.Items.Count < DiagramData.MaxLines);
			DeleteButton.Enabled = ApplyButton.Enabled = true;
			AbortButton.Enabled = true;
			}

		private void FormulaBox_KeyDown (object sender, KeyEventArgs e)
			{
			if (e.KeyCode == Keys.Return)
				AddButton_Click (sender, null);
			}

		// Удаление выбранной кривой из списка
		private void DeleteButton_Click (object sender, EventArgs e)
			{
			if (CurvesList.SelectedIndex >= 0)
				{
				CurvesList.Items.RemoveAt (CurvesList.SelectedIndex);
				DeleteButton.Enabled = ApplyButton.Enabled = (CurvesList.Items.Count > 0);
				}
			}

		// Определение количества точек
		private void StartValue_ValueChanged (object sender, EventArgs e)
			{
			/*Label05.Text = RDLocale.GetControlText ("FormulaEvaluator", "Label05W");*/
			Label05.Text = RDLocale.GetText (this.Name + "_Label05W");
			if (StepValue.Value == 0)
				Label05.Text += " ∞";
			else
				Label05.Text += (" " + ((uint)(Math.Abs (EndValue.Value - StartValue.Value) /
					StepValue.Value) + 1).ToString ());

			Label05.Text += (" / " + DiagramData.MaxDataRows.ToString ());
			}
		}
	}
