using System;
using System.Collections.Generic;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс предоставляет механизм сборки цепочки вычисления значения выражения
	/// </summary>
	public class EvaluationChainAssembler
		{
		/// <summary>
		/// Возвращает сформированную цепочку шагов вычисления выражения
		/// </summary>
		public List<EvaluationChainElement> EvaluationChain
			{
			get
				{
				return chain;
				}
			}
		private List<EvaluationChainElement> chain = new List<EvaluationChainElement> ();

		/// <summary>
		/// Конструктор. Выполняет сборку цепочки вычислений
		/// </summary>
		public EvaluationChainAssembler (LexemesExtractor ExpressionLexemesExtractor)
			{
			// Контроль сборки
			if ((ExpressionLexemesExtractor.FollowingStatus != LexemesFollowingMatrix.LexemesFollowingStatuses.Ok) ||
				(ExpressionLexemesExtractor.ExpressionStatus != LexemesExtractor.ExpressionStatuses.Ok))
				{
				return;
				}

			// Формирование массива приоритетов
			List<int> links = new List<int> ();
			int multiplier = 10000;
			for (int i = 0; i < ExpressionLexemesExtractor.ExtractedLexemes.Count; i++)
				{
				switch (ExpressionLexemesExtractor.ExtractedLexemes[i].LexemeType)
					{
					// Установка уровня вложенности
					case Lexeme.LexemeTypes.LeftParenthesis:
						multiplier += 10000;
						break;

					case Lexeme.LexemeTypes.RightParenthesis:
						multiplier -= 10000;
						break;

					// Установка приоритетов для функций и операций
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
						links.Add (multiplier + 5000 + i);
						break;

					case Lexeme.LexemeTypes.Exponentiation:
						links.Add (multiplier + 4000 + i);
						break;

					case Lexeme.LexemeTypes.Division:
						links.Add (multiplier + 3000 + i);
						break;

					case Lexeme.LexemeTypes.Multiplication:
						links.Add (multiplier + 2000 + i);
						break;

					case Lexeme.LexemeTypes.Minus:
						links.Add (multiplier + 1000 + i);
						break;

					case Lexeme.LexemeTypes.Plus:
						links.Add (multiplier + 0 + i);
						break;

					// Установка прямых ссылок на операнды
					case Lexeme.LexemeTypes.E:
					case Lexeme.LexemeTypes.Pi:
					case Lexeme.LexemeTypes.Number:
					case Lexeme.LexemeTypes.Variable:
						links.Add (i);
						break;

					// А такого быть не должно: класс LexemesExtractor должен это исключить
					case Lexeme.LexemeTypes.Unknown:
						throw new Exception ("Достигнуто предвиденное исключение. Для коррекции необходимо обратиться к автору программы");
					}
				}

			// Сборка цепочки
			int max, maxIndex;
			while (true)
				{
				// Поиск максимального приоритета
				max = maxIndex = 0;

				for (int i = 0; i < links.Count; i++)
					{
					if (max < links[i])
						{
						max = links[i];
						maxIndex = i;
						}
					}

				// Условие прерывания
				if (max < 1000)
					{
					break;
					}

				// Обработка операций (любой сбой на этом шаге означает кривизну предыдущих обработок)
				switch (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000].LexemeType)
					{
					// Обработка функций
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
						if (links[maxIndex + 1] >= 0)
							{
							// По факту
							chain.Add (new EvaluationChainElement (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
								ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex + 1] % 1000]));
							}
						else
							{
							// По ссылке (ссылки хранятся в отрицательных числах со смещением
							chain.Add (new EvaluationChainElement (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
								(uint)(-links[maxIndex + 1] - 1)));
							}

						// То же самое
						if (!chain[chain.Count - 1].IsInited)
							{
							throw new Exception ("Достигнуто предвиденное исключение. Для коррекции необходимо обратиться к автору программы");
							}

						// Замещение операции и операнда ссылкой на результат операции
						links.RemoveAt (maxIndex);
						links.RemoveAt (maxIndex);
						links.Insert (maxIndex, -chain.Count);
						break;

					// Обработка бинарных операций
					case Lexeme.LexemeTypes.Plus:
					case Lexeme.LexemeTypes.Minus:
					case Lexeme.LexemeTypes.Multiplication:
					case Lexeme.LexemeTypes.Division:
					case Lexeme.LexemeTypes.Exponentiation:
						if (links[maxIndex - 1] >= 0)
							{
							// По факту
							if (links[maxIndex + 1] >= 0)
								{
								chain.Add (new EvaluationChainElement (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex - 1] % 1000],
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex + 1] % 1000]));
								}
							// Со вторым операндом-ссылкой
							else
								{
								chain.Add (new EvaluationChainElement (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex - 1] % 1000],
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
									(uint)(-links[maxIndex + 1] - 1)));
								}
							}
						else
							{
							// С первым операндом-ссылкой
							if (links[maxIndex + 1] >= 0)
								{
								chain.Add (new EvaluationChainElement ((uint)(-links[maxIndex - 1] - 1),
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex + 1] % 1000]));
								}
							// С обоими операндами-ссылками
							else
								{
								chain.Add (new EvaluationChainElement ((uint)(-links[maxIndex - 1] - 1),
									ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000],
									(uint)(-links[maxIndex + 1] - 1)));
								}
							}

						// То же самое
						if (!chain[chain.Count - 1].IsInited)
							{
							throw new Exception ("Достигнуто предвиденное исключение. Для коррекции необходимо обратиться к автору программы");
							}

						// Замещение операции и операнда ссылкой на результат операции
						links.RemoveAt (maxIndex - 1);
						links.RemoveAt (maxIndex - 1);
						links.RemoveAt (maxIndex - 1);
						links.Insert (maxIndex - 1, -chain.Count);
						break;

					// Остальных лексем здесь быть не должно
					default:
						throw new Exception ("Достигнуто предвиденное исключение. Для коррекции необходимо обратиться к автору программы");
					}
				}

			// Если массив шагов пуст, то операций в выражении не было
			if (chain.Count == 0)
				{
				// Необходимо для фильтрации скобок
				chain.Add (new EvaluationChainElement (ExpressionLexemesExtractor.ExtractedLexemes[links[maxIndex] % 1000]));

				if (!chain[chain.Count - 1].IsInited)
					{
					throw new Exception ("Достигнуто предвиденное исключение. Для коррекции необходимо обратиться к автору программы");
					}
				}

			// Завершено
			isInited = true;
			}

		/// <summary>
		/// Возвращает статус инициализации цепочки вычисления
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
