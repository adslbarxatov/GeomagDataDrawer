﻿namespace RD_AAOW
	{
	/// <summary>
	/// Класс предоставляет метод проверки корректности последовательности лексем в выражении
	/// </summary>
	public static class LexemesFollowingMatrix
		{
		// Матрица допусков
		private static int[][] matrix = new int[][] { 
			//			num		+	-	*	/	^	(	)	sin	cos	tg	ctg	arcsin	arccos	arctg	arcctg	ln	abs	x	П	e	?
			// ^ -> Number
			new int[]	{-1,	0,	0,	0,	0,	0,	0,	-1,	-1,	-1,	-1,	-1,	-1,		-1,		-1,		-1,		-1,	-1,	-1,	-1,	-1,	-2},
			// ^ -> +
			new int[]	{0,		-3,	-3,	-3,	-3,	-3,	-3,	0,	-3,	-3,	-3,	-3,	-3,		-3,		-3,		-3,		-3,	-3,	0,	0,	0,	-2},
			// ^ -> -
			new int[]	{0,		-4,	-4,	-4,	-4,	-4,	-4,	0,	-4,	-4,	-4,	-4,	-4,		-4,		-4,		-4,		-4,	-4,	0,	0,	0,	-2},
			// ^ -> *
			new int[]	{0,		-5,	-5,	-5,	-5,	-5,	-5,	0,	-5,	-5,	-5,	-5,	-5,		-5,		-5,		-5,		-5,	-5,	0,	0,	0,	-2},
			// ^ -> /
			new int[]	{0,		-6,	-6,	-6,	-6,	-6,	-6,	0,	-6,	-6,	-6,	-6,	-6,		-6,		-6,		-6,		-6,	-6,	0,	0,	0,	-2},
			// ^ -> ^
			new int[]	{0,		-7,	-7,	-7,	-7,	-7,	-7,	0,	-7,	-7,	-7,	-7,	-7,		-7,		-7,		-7,		-7,	-7,	0,	0,	0,	-2},
			// ^ -> (
			new int[]	{-8,	0,	0,	0,	0,	0,	0,	-8,	0,	0,	0,	0,	0,		0,		0,		0,		0,	0,	-8,	-8,	-8,	-2},
			// ^ -> )
			new int[]	{0,		-9,	-9,	-9,	-9,	-9,	-9,	0,	-9,	-9,	-9,	-9,	-9,		-9,		-9,		-9,		-9,	-9,	0,	0,	0,	-2},

			//			num		+	-	*	/	^	(	)		sin		cos		tg		ctg		arcsin	arccos	arctg	arcctg	ln		abs		x		П		e		?
			// ^ -> sin, cos, ... (8)
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			
			// ^ -> ln, abs
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},
			new int[]	{-10,	0,	0,	0,	0,	0,	0,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-10,	-2},

			// ^ -> x
			new int[]	{-11,	0,	0,	0,	0,	0,	0,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-11,	-2},

			// ^ -> П, e
			new int[]	{-12,	0,	0,	0,	0,	0,	0,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-2},
			new int[]	{-12,	0,	0,	0,	0,	0,	0,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-12,	-2},

			// ^ -> ?
			new int[]	{-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2,		-2}
			};

		/// <summary>
		/// Перечисление описывает возможные статусы следования лексем
		/// </summary>
		public enum LexemesFollowingStatuses
			{
			/// <summary>
			/// Следование допустимо
			/// </summary>
			Ok = 0,

			/// <summary>
			/// Число не может следовать за указанной лексемой
			/// </summary>
			MisplacedNumber = -1,

			/// <summary>
			/// Неопознанная лексема в выражении
			/// </summary>
			UnknownLexeme = -2,

			/// <summary>
			/// Оператор сложения не может следовать за указанной лексемой
			/// </summary>
			MisplacedPlusOperator = -3,

			/// <summary>
			/// Оператор вычитания не может следовать за указанной лексемой
			/// </summary>
			MisplacedMinusOperator = -4,

			/// <summary>
			/// Оператор умножения не может следовать за указанной лексемой
			/// </summary>
			MisplacedMultiplicationOperator = -5,

			/// <summary>
			/// Оператор деления не может следовать за указанной лексемой
			/// </summary>
			MisplacedDivisionOperator = -6,

			/// <summary>
			/// Оператор возведения в степень не может следовать за указанной лексемой
			/// </summary>
			MisplacedExponentiationOperator = -7,

			/// <summary>
			/// Левая скобка не может следовать за указанной лексемой
			/// </summary>
			MisplacedLeftParenthesis = -8,

			/// <summary>
			/// Правая скобка не может следовать за указанной лексемой
			/// </summary>
			MisplacedRightParenthesis = -9,

			/// <summary>
			/// Имя функции не может следовать за указанной лексемой
			/// </summary>
			MisplacedFunctionCall = -10,

			/// <summary>
			/// Переменная не может следовать за указанной лексемой
			/// </summary>
			MisplacedVariable = -11,

			/// <summary>
			/// Константа не может следовать за указанной лексемой
			/// </summary>
			MisplacedConstant = -12
			}

		/// <summary>
		/// Метод определяет допустимость следования лексем в указанном порядке
		/// </summary>
		/// <param name="Lexeme1">Первая лексема</param>
		/// <param name="Lexeme2">Вторая лексема</param>
		/// <returns>Возвращает определение для возможности следования второй лексемы за первой</returns>
		public static LexemesFollowingStatuses IfLexeme2FollowLexeme1 (Lexeme Lexeme1, Lexeme Lexeme2)
			{
			return (LexemesFollowingStatuses)matrix[(int)Lexeme2.LexemeType][(int)Lexeme1.LexemeType];
			}
		}
	}