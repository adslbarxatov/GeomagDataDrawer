using System;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает отдельный шаг вычисления выражения
	/// </summary>
	public class EvaluationChainElement
		{
		/// <summary>
		/// Виды допустимых операндов
		/// </summary>
		public enum OperandTypes
			{
			/// <summary>
			/// Нет операнда. Переменные operand*Value не используются
			/// </summary>
			Nothing,

			/// <summary>
			/// Ссылка на другой шаг вычисления. Переменные operand*Value содержат номера шагов
			/// </summary>
			Link,

			/// <summary>
			/// Число. Переменные operand*Value содержат величины
			/// </summary>
			Number,

			/// <summary>
			/// Переменная. Переменные operand*Value не используются
			/// </summary>
			Variable
			}

		/// <summary>
		/// Виды допустимых операций
		/// </summary>
		public enum OperationTypes
			{
			/// <summary>
			/// Без выполнения операции
			/// </summary>
			Nothing = 0,

			/// <summary>
			/// Сложение
			/// </summary>
			Addition = 1,		// Аналогично LexemeTypes

			/// <summary>
			/// Вычитание
			/// </summary>
			Subtraction = 2,

			/// <summary>
			/// Умножение
			/// </summary>
			Multiplication = 3,

			/// <summary>
			/// Деление
			/// </summary>
			Division = 4,

			/// <summary>
			/// Возведение в степень
			/// </summary>
			Exponentiation = 5,

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
			Abs = 17
			}

		/// <summary>
		/// Тип первого операнда шага вычисления
		/// </summary>
		public OperandTypes Operand1Type
			{
			get
				{
				return operand1Type;
				}
			}
		private OperandTypes operand1Type = OperandTypes.Nothing;

		/// <summary>
		/// Тип второго операнда шага вычисления
		/// </summary>
		public OperandTypes Operand2Type
			{
			get
				{
				return operand2Type;
				}
			}
		private OperandTypes operand2Type = OperandTypes.Nothing;

		/// <summary>
		/// Значение первого операнда шага вычисления
		/// </summary>
		public double Operand1Value
			{
			get
				{
				return operand1Value;
				}
			}
		private double operand1Value = 0.0;

		/// <summary>
		/// Значение второго операнда шага вычисления
		/// </summary>
		public double Operand2Value
			{
			get
				{
				return operand2Value;
				}
			}
		private double operand2Value = 0.0;

		/*/// <summary>
		/// Значение индекса первого операнда шага вычисления (для переменных)
		/// </summary>
		public uint Operand1Index
			{
			get
				{
				return operand1Index;
				}
			}
		private uint operand1Index = 0;

		/// <summary>
		/// Значение индекса второго операнда шага вычисления (для переменных)
		/// </summary>
		public uint Operand2Index
			{
			get
				{
				return operand2Index;
				}
			}
		private uint operand2Index = 0;*/

		/// <summary>
		/// Тип операции шага вычисления
		/// </summary>
		public OperationTypes OperationType
			{
			get
				{
				return operationType;
				}
			}
		private OperationTypes operationType = OperationTypes.Nothing;

		// Функция обработки нессылочных операндов
		private bool LoadRealOperand (Lexeme Operand, out OperandTypes OperandType, out double OperandValue/*, out uint OperandIndex*/)
			{
			// Контроль операнда 1
			switch (Operand.LexemeType)
				{
				case Lexeme.LexemeTypes.E:
					OperandType = OperandTypes.Number;
					OperandValue = Math.E;
					//OperandIndex = 0;
					return true;

				case Lexeme.LexemeTypes.Number:
					OperandType = OperandTypes.Number;
					OperandValue = double.Parse (Operand.LexemeValue);
					//OperandIndex = 0;
					return true;

				case Lexeme.LexemeTypes.Pi:
					OperandType = OperandTypes.Number;
					OperandValue = Math.PI;
					//OperandIndex = 0;
					return true;

				case Lexeme.LexemeTypes.Variable:
					OperandType = OperandTypes.Variable;
					OperandValue = 0.0;
					//OperandIndex = Operand.LexemeIndex;
					return true;

				// В остальных случаях инициализация считается неуспешной
				default:
					OperandType = OperandTypes.Nothing;
					OperandValue = 0.0;
					//OperandIndex = 0;
					return false;
				}
			}

		// Функция обработки бинарных операций
		private bool LoadBinaryOperation (Lexeme Operation)
			{
			switch (Operation.LexemeType)
				{
				case Lexeme.LexemeTypes.Plus:
				case Lexeme.LexemeTypes.Minus:
				case Lexeme.LexemeTypes.Multiplication:
				case Lexeme.LexemeTypes.Division:
				case Lexeme.LexemeTypes.Exponentiation:
					operationType = (OperationTypes)Operation.LexemeType;
					return true;

				// В остальных случаях загрузка считается неуспешной
				default:
					return false;
				}
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "бинарная операция" без ссылок
		/// </summary>
		/// <param name="Operand1">Первый операнд (число, константа или переменная)</param>
		/// <param name="Operand2">Второй операнд (число, константа или переменная)</param>
		/// <param name="Operation">Выполняемая операция</param>
		public EvaluationChainElement (Lexeme Operand1, Lexeme Operation, Lexeme Operand2)
			{
			// Контроль операнда 1
			if (!LoadRealOperand (Operand1, out operand1Type, out operand1Value/*, out operand1Index*/))
				{
				return;
				}

			// Контроль операнда 2
			if (!LoadRealOperand (Operand2, out operand2Type, out operand2Value/*, out operand2Index*/))
				{
				return;
				}

			// Контроль операции
			if (!LoadBinaryOperation (Operation))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		// Функция обработки ссылочных операндов
		private bool LoadLinkedOperand (uint Operand, out OperandTypes OperandType, out double LinkNumber)
			{
			OperandType = OperandTypes.Link;
			LinkNumber = Operand;
			return true;
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "бинарная операция" с первым операндом-ссылкой
		/// </summary>
		/// <param name="Operand1">Ссылка на шаг вычисления, содержащий первый операнд</param>
		/// <param name="Operand2">Второй операнд (число, константа или переменная)</param>
		/// <param name="Operation">Выполняемая операция</param>
		public EvaluationChainElement (uint Operand1, Lexeme Operation, Lexeme Operand2)
			{
			// Контроль операнда 1
			if (!LoadLinkedOperand (Operand1, out operand1Type, out operand1Value))
				{
				return;
				}

			// Контроль операнда 2
			if (!LoadRealOperand (Operand2, out operand2Type, out operand2Value/*, out operand2Index*/))
				{
				return;
				}

			// Контроль операции
			if (!LoadBinaryOperation (Operation))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "бинарная операция" со вторым операндом-ссылкой
		/// </summary>
		/// <param name="Operand1">Первый операнд (число, константа или переменная)</param>
		/// <param name="Operand2">Ссылка на шаг вычисления, содержащий второй операнд</param>
		/// <param name="Operation">Выполняемая операция</param>
		public EvaluationChainElement (Lexeme Operand1, Lexeme Operation, uint Operand2)
			{
			// Контроль операнда 1
			if (!LoadRealOperand (Operand1, out operand1Type, out operand1Value/*, out operand1Index*/))
				{
				return;
				}

			// Контроль операнда 2
			if (!LoadLinkedOperand (Operand2, out operand2Type, out operand2Value))
				{
				return;
				}

			// Контроль операции
			if (!LoadBinaryOperation (Operation))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "бинарная операция" с обоими операндами-ссылками
		/// </summary>
		/// <param name="Operand1">Ссылка на шаг вычисления, содержащий первый операнд</param>
		/// <param name="Operand2">Ссылка на шаг вычисления, содержащий второй операнд</param>
		/// <param name="Operation">Выполняемая операция</param>
		public EvaluationChainElement (uint Operand1, Lexeme Operation, uint Operand2)
			{
			// Контроль операнда 1
			if (!LoadLinkedOperand (Operand1, out operand1Type, out operand1Value))
				{
				return;
				}

			// Контроль операнда 2
			if (!LoadLinkedOperand (Operand2, out operand2Type, out operand2Value))
				{
				return;
				}

			// Контроль операции
			if (!LoadBinaryOperation (Operation))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		// Функция обработки вызовов функций
		private bool LoadFunction (Lexeme Function)
			{
			switch (Function.LexemeType)
				{
				case Lexeme.LexemeTypes.Sinus:
				case Lexeme.LexemeTypes.Cosinus:
				case Lexeme.LexemeTypes.Tangens:
				case Lexeme.LexemeTypes.Cotangens:
				case Lexeme.LexemeTypes.Arcsinus:
				case Lexeme.LexemeTypes.Arccosinus:
				case Lexeme.LexemeTypes.Arctangens:
				case Lexeme.LexemeTypes.Arccotangens:
				case Lexeme.LexemeTypes.NaturalLogarithm:
				case Lexeme.LexemeTypes.Abs:
					operationType = (OperationTypes)Function.LexemeType;
					return true;

				// В остальных случаях загрузка считается неуспешной
				default:
					return false;
				}
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "вызов функции"
		/// </summary>
		/// <param name="Operand">Операнд (число, константа или переменная)</param>
		/// <param name="Function">Вызываемая функция</param>
		public EvaluationChainElement (Lexeme Function, Lexeme Operand)
			{
			// Контроль операнда
			if (!LoadRealOperand (Operand, out operand2Type, out operand2Value/*, out operand2Index*/))
				{
				return;
				}

			// Контроль операции
			if (!LoadFunction (Function))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "вызов функции" с операндом-ссылкой
		/// </summary>
		/// <param name="Operand">Ссылка на шаг вычисления, содержащий операнд</param>
		/// <param name="Function">Вызываемая функция</param>
		public EvaluationChainElement (Lexeme Function, uint Operand)
			{
			// Контроль операнда
			if (!LoadLinkedOperand (Operand, out operand2Type, out operand2Value))
				{
				return;
				}

			// Контроль операции
			if (!LoadFunction (Function))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		/// <summary>
		/// Конструктор. Создаёт шаг вычисления по типу "отдельный операнд"
		/// </summary>
		/// <param name="Operand">Операнд (число, константа или переменная)</param>
		public EvaluationChainElement (Lexeme Operand)
			{
			// Контроль операнда
			if (!LoadRealOperand (Operand, out operand2Type, out operand2Value/*, out operand2Index*/))
				{
				return;
				}

			// Успешно
			isInited = true;
			}

		/// <summary>
		/// Возвращает статус инициализации шага вычисления
		/// </summary>
		public bool IsInited
			{
			get
				{
				return isInited;
				}
			}
		private bool isInited = false;
		}
	}
