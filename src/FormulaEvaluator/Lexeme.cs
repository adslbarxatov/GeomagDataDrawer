using System.Globalization;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает отдельную лексему
	/// </summary>
	public class Lexeme
		{
		// Переменные
		private string lexemeValue;
		private LexemeTypes lexemeType;
		//private uint lexemeIndex;

		// Константы настройки
		private CultureInfo ce = new CultureInfo ("en-us");

		/// <summary>
		/// Конструктор. Формирует лексему по её строковому представлению
		/// </summary>
		/// <param name="LexemePresentation">Строка, представляющая лексему. Не должна содержать посторонних символов, в т.ч. пробелы</param>
		public Lexeme (string LexemePresentation)
			{
			// Начальная установка
			lexemeValue = LexemePresentation;
			lexemeType = LexemeTypes.Unknown;
			//lexemeIndex = 0;

			// Знаки и имена функций
			switch (LexemePresentation.ToLower ())
				{
				case "+":
					lexemeType = LexemeTypes.Plus;
					lexemeValue = " + ";
					break;

				case "-":
				case "–":
					lexemeType = LexemeTypes.Minus;
					lexemeValue = " – ";
					break;

				case "*":
				case "•":
					lexemeType = LexemeTypes.Multiplication;
					lexemeValue = " • ";
					break;

				case "/":
				case ":":
					lexemeType = LexemeTypes.Division;
					lexemeValue = " / ";
					break;

				case "^":
					lexemeType = LexemeTypes.Exponentiation;
					lexemeValue = " ^ ";
					break;

				case "(":
					lexemeType = LexemeTypes.LeftParenthesis;
					lexemeValue = "(";
					break;

				case ")":
					lexemeType = LexemeTypes.RightParenthesis;
					lexemeValue = ")";
					break;

				case "sin":
					lexemeType = LexemeTypes.Sinus;
					lexemeValue = "sin ";
					break;

				case "cos":
					lexemeType = LexemeTypes.Cosinus;
					lexemeValue = "cos ";
					break;

				case "tg":
					lexemeType = LexemeTypes.Tangens;
					lexemeValue = "tg ";
					break;

				case "ctg":
					lexemeType = LexemeTypes.Cotangens;
					lexemeValue = "ctg ";
					break;

				case "arcsin":
					lexemeType = LexemeTypes.Arcsinus;
					lexemeValue = "arcsin ";
					break;

				case "arccos":
					lexemeType = LexemeTypes.Arccosinus;
					lexemeValue = "arccos ";
					break;

				case "arctg":
					lexemeType = LexemeTypes.Arctangens;
					lexemeValue = "arctg ";
					break;

				case "arcctg":
					lexemeType = LexemeTypes.Arccotangens;
					lexemeValue = "arcctg ";
					break;

				case "ln":
					lexemeType = LexemeTypes.NaturalLogarithm;
					lexemeValue = "ln ";
					break;

				case "abs":
					lexemeType = LexemeTypes.Abs;
					lexemeValue = "abs ";
					break;

				// Отлавливает переменную без номера
				case "x":
					lexemeType = LexemeTypes.Variable;
					lexemeValue = "x";
					break;

				case "п":
				case "pi":
					lexemeType = LexemeTypes.Pi;
					lexemeValue = "П";
					break;

				case "e":
					lexemeType = LexemeTypes.E;
					lexemeValue = "e";
					break;
				}

			// Числа с запятыми (вместе со следующим вариантом вынесены в конец функции, чтобы не цеплять Exception
			// при каждой нечисловой лексеме, т.е. практически постоянно и с соответствующими тормозами)
			lexemeValue = lexemeValue.Replace (',', '.');

			// Числа с точками
			try
				{
				double a = double.Parse (lexemeValue, ce.NumberFormat);

				// Точно число
				lexemeValue = a.ToString ();
				lexemeType = LexemeTypes.Number;
				return;
				}
			catch
				{
				}

			// Имена переменных с индексами (попробуем без регулярных выражений)
			/*if (LexemePresentation.Length == 2)
				{
				if (((LexemePresentation[0] == 'x') || (LexemePresentation[0] == 'X')) &&
					(LexemePresentation[1] >= '0') && (LexemePresentation[1] <= '9'))
					{
					lexemeType = LexemeTypes.Variable;
					lexemeValue = "x" + LexemePresentation.Substring (1,1);
					lexemeIndex = uint.Parse (LexemePresentation.Substring (1, 1));
					}
				}

			if (LexemePresentation.Length == 3)
				{
				if (((LexemePresentation[0] == 'x') || (LexemePresentation[0] == 'X')) &&
					(LexemePresentation[1] >= '0') && (LexemePresentation[1] <= '9') &&
					(LexemePresentation[2] >= '0') && (LexemePresentation[2] <= '9'))
					{
					lexemeType = LexemeTypes.Variable;
					lexemeValue = "x" + LexemePresentation.Substring (1, 2);
					lexemeIndex = uint.Parse (LexemePresentation.Substring (1, 2));
					}
				}*/
			}

		/// <summary>
		/// Возвращает строковое представление лексемы
		/// </summary>
		public string LexemeValue
			{
			get
				{
				return lexemeValue;
				}
			}

		/// <summary>
		/// Перечисление допустимых типов лексем в выражениях
		/// </summary>
		public enum LexemeTypes
			{
			/// <summary>
			/// Число
			/// </summary>
			Number = 0,

			/// <summary>
			/// Знак "плюс"
			/// </summary>
			Plus = 1,

			/// <summary>
			/// Знак "минус"
			/// </summary>
			Minus = 2,

			/// <summary>
			/// Знак "умножение"
			/// </summary>
			Multiplication = 3,

			/// <summary>
			/// Знак "деление"
			/// </summary>
			Division = 4,

			/// <summary>
			/// Знак "возведение в степень"
			/// </summary>
			Exponentiation = 5,

			/// <summary>
			/// Левая скобка
			/// </summary>
			LeftParenthesis = 6,

			/// <summary>
			/// Правая скобка
			/// </summary>
			RightParenthesis = 7,

			/// <summary>
			/// Функция "синус"
			/// </summary>
			Sinus = 8,

			/// <summary>
			/// Функция "косинус"
			/// </summary>
			Cosinus = 9,

			/// <summary>
			/// Функция "тангенс"
			/// </summary>
			Tangens = 10,

			/// <summary>
			/// Функция "котангенс"
			/// </summary>
			Cotangens = 11,

			/// <summary>
			/// Функция "арксинус"
			/// </summary>
			Arcsinus = 12,

			/// <summary>
			/// Функция "арккосинус"
			/// </summary>
			Arccosinus = 13,

			/// <summary>
			/// Функция "арктангенс"
			/// </summary>
			Arctangens = 14,

			/// <summary>
			/// Функция "арккотангенс"
			/// </summary>
			Arccotangens = 15,

			/// <summary>
			/// Функция "натуральный логарифм"
			/// </summary>
			NaturalLogarithm = 16,

			/// <summary>
			/// Функция "модуль"
			/// </summary>
			Abs = 17,

			/// <summary>
			/// Переменная
			/// </summary>
			Variable = 18,

			/// <summary>
			/// Число П
			/// </summary>
			Pi = 19,

			/// <summary>
			/// Число e
			/// </summary>
			E = 20,

			/// <summary>
			/// Неизвестная лексема
			/// </summary>
			Unknown = 21
			}

		/// <summary>
		/// Возвращает тип лексемы
		/// </summary>
		public LexemeTypes LexemeType
			{
			get
				{
				return lexemeType;
				}
			}

		/*/// <summary>
		/// Возвращает индекс лексемы (для переменных)
		/// </summary>
		public uint LexemeIndex
			{
			get
				{
				return lexemeIndex;
				}
			}*/
		}
	}
