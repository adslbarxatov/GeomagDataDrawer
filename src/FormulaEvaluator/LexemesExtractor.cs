using System;
using System.Collections.Generic;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает отдельную лексему
	/// </summary>
	public class LexemesExtractor
		{
		// Переменные
		private char[] splitters = new char[] { '\x20', '\x9', '\xA', '\xD' };

		/// <summary>
		/// Возвращает последнюю обработанную лексему. Является местом ошибки в случае присутствия таковой
		/// </summary>
		public string LastLexeme
			{
			get
				{
				return lastLexeme;
				}
			}
		private string lastLexeme = "";

		/// <summary>
		/// Конструктор. Извлекает лексемы из строкового представления выражения
		/// </summary>
		/// <param name="Expression">Строковое представление выражения</param>
		public LexemesExtractor (string Expression)
			{
			// Переменные
			int pStatus = 0;

			// Установка пробелов вокруг некоторых знаков. Необходимо для корректного парсинга
			string expression = Expression.Replace ("(", " ( ");
			expression = expression.Replace (")", " ) ");
			expression = expression.Replace ("+", " + ");
			expression = expression.Replace ("*", " * ");
			expression = expression.Replace ("/", " / ");
			expression = expression.Replace ("^", " ^ ");
			expression = expression.Replace ("-", " -");    // Для минуса правый пробел запрещён, т.к. его наличие меняет смысл выражения


			// Извлечение строковых представлений лексем. Требует разделения отдельных
			// лексем пробелами или другими разделителями из массива splitters
			string[] lexemes = expression.Split (splitters, StringSplitOptions.RemoveEmptyEntries);

			// Заполнение массива лексем
			for (int i = 0; i < lexemes.Length; i++)
				{
				extractedLexemes.Add (new Lexeme (lexemes[i]));

				// Проверка статуса скобок
				if (expressionStatus == ExpressionStatuses.Ok)
					{
					if (extractedLexemes[extractedLexemes.Count - 1].LexemeType == Lexeme.LexemeTypes.LeftParenthesis)
						{
						pStatus++;
						}
					if (extractedLexemes[extractedLexemes.Count - 1].LexemeType == Lexeme.LexemeTypes.RightParenthesis)
						{
						pStatus--;
						}

					// Эта проверка должна отлавливать случаи расположения правых скобок перед левыми
					// при их одинаковом количестве
					if (pStatus < 0)
						{
						expressionStatus = ExpressionStatuses.MisplacedRightParenthesis;
						}
					}

				// Проверка корректности следования лексем в выражении
				if (followingStatus == LexemesFollowingMatrix.LexemesFollowingStatuses.Ok)
					{
					if (i > 0)
						{
						followingStatus = LexemesFollowingMatrix.IfLexeme2FollowLexeme1 (extractedLexemes[i - 1], extractedLexemes[i]);
						}

					if (expressionStatus == ExpressionStatuses.Ok)
						{
						lastLexeme = lexemes[i];
						}
					}

				// Проверка начала и конца выражения
				if (i == 0)
					{
					switch (extractedLexemes[extractedLexemes.Count - 1].LexemeType)
						{
						case Lexeme.LexemeTypes.Division:
						case Lexeme.LexemeTypes.Exponentiation:
						case Lexeme.LexemeTypes.Minus:
						case Lexeme.LexemeTypes.Multiplication:
						case Lexeme.LexemeTypes.Plus:
						case Lexeme.LexemeTypes.RightParenthesis:
						case Lexeme.LexemeTypes.Unknown:
							expressionStatus = ExpressionStatuses.IncorrectExpressionStart;
							break;
						}
					}

				if (i == lexemes.Length - 1)
					{
					if ((extractedLexemes[extractedLexemes.Count - 1].LexemeType != Lexeme.LexemeTypes.E) &&
						(extractedLexemes[extractedLexemes.Count - 1].LexemeType != Lexeme.LexemeTypes.Number) &&
						(extractedLexemes[extractedLexemes.Count - 1].LexemeType != Lexeme.LexemeTypes.Pi) &&
						(extractedLexemes[extractedLexemes.Count - 1].LexemeType != Lexeme.LexemeTypes.RightParenthesis) &&
						(extractedLexemes[extractedLexemes.Count - 1].LexemeType != Lexeme.LexemeTypes.Variable))
						{
						expressionStatus = ExpressionStatuses.IncorrectExpressionEnd;
						}
					}
				}

			// Конечная проверка скобок
			if (pStatus > 0)
				{
				expressionStatus = ExpressionStatuses.LeftParenthesisWasNotClosed;
				}

			// Конечная проверка выражения
			if (extractedLexemes.Count == 0)
				{
				expressionStatus = ExpressionStatuses.ExpressionIsEmpty;
				}
			}

		/// <summary>
		/// Возвращает массив извлечённых лексем
		/// </summary>
		public List<Lexeme> ExtractedLexemes
			{
			get
				{
				return extractedLexemes;
				}
			}
		private List<Lexeme> extractedLexemes = new List<Lexeme> ();

		/// <summary>
		/// Перечисление статусов общего анализа выражения
		/// </summary>
		public enum ExpressionStatuses
			{
			/// <summary>
			/// Все скобки имеют корректно установленные пары
			/// </summary>
			Ok,

			/// <summary>
			/// Одна или несколько правых скобок - лишние или расположены перед левыми
			/// </summary>
			MisplacedRightParenthesis,

			/// <summary>
			/// Одна или несколько левых скобок не имеют закрывающих пар
			/// </summary>
			LeftParenthesisWasNotClosed,

			/// <summary>
			/// Некорректное начало выражения
			/// </summary>
			IncorrectExpressionStart,

			/// <summary>
			/// Некорректное окончание выражения
			/// </summary>
			IncorrectExpressionEnd,

			/// <summary>
			/// Выражение пусто
			/// </summary>
			ExpressionIsEmpty
			}

		/// <summary>
		/// Возвращает состояние скобок в загруженном выражении
		/// </summary>
		public ExpressionStatuses ExpressionStatus
			{
			get
				{
				return expressionStatus;
				}
			}
		private ExpressionStatuses expressionStatus = ExpressionStatuses.Ok;

		/// <summary>
		/// Возвращает определение корректности следования лексем в загруженном выражении
		/// </summary>
		public LexemesFollowingMatrix.LexemesFollowingStatuses FollowingStatus
			{
			get
				{
				return followingStatus;
				}
			}
		private LexemesFollowingMatrix.LexemesFollowingStatuses followingStatus =
			LexemesFollowingMatrix.LexemesFollowingStatuses.Ok;
		}
	}
