using System;
using System.Collections.Generic;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает строку исходной таблицы в исходном состоянии
	/// </summary>
	public class SourceTableRow:IComparable<SourceTableRow>, IEquatable<SourceTableRow>
		{
		/// <summary>
		/// Абсцисса строки
		/// </summary>
		public double X
			{
			get
				{
				return x;
				}
			}
		private double x = 0;

		/// <summary>
		/// Номер таблицы, из которой получена строка
		/// </summary>
		public uint TableNumber
			{
			get
				{
				return tableNumber;
				}
			}
		private uint tableNumber = 0;

		/// <summary>
		/// Набор ординат строки
		/// </summary>
		public List<double> Y
			{
			get
				{
				return y;
				}
			}
		private List<double> y = new List<double> ();

		/// <summary>
		/// Метод сравнивает данный экземпляр с указанным
		/// </summary>
		/// <param name="Sample">Экземпляр, с которым выполняется сравнение</param>
		/// <returns>Возвращает -1, если указанный экземпляр располагается после данного
		///                     1, если указанный экземпляр располагается перед данным
		///                     0, если экземпляры эквивалентны</returns>
		public int CompareTo (SourceTableRow Sample)
			{
			try
				{
				if (x < Sample.x)
					{
					return -1;
					}
				else if (x > Sample.x)
					{
					return 1;
					}
				else
					{
					if (tableNumber < Sample.tableNumber)
						{
						return -1;
						}
					else if (tableNumber > Sample.tableNumber)
						{
						return 1;
						}
					else
						return 0;
					}
				}
			catch
				{
				return 0;
				}
			}

		/// <summary>
		/// Конструктор. Создаёт строку данных
		/// </summary>
		/// <param name="NumberOfTable">Номер таблицы, используемый для идентификации строки</param>
		/// <param name="Abscissa">Абсцисса строки, используемая для идетнтификации</param>
		public SourceTableRow (uint NumberOfTable, double Abscissa)
			{
			tableNumber = NumberOfTable;
			x = Abscissa;
			}

		/// <summary>
		/// Метод добавляет ординату в строку данных
		/// </summary>
		/// <param name="Ordinate">Значение ординаты</param>
		public void AddOrdinate (double Ordinate)
			{
			y.Add (Ordinate);
			}

		/// <summary>
		/// Метод определяет равенство данного экземпляра с указанным
		/// </summary>
		/// <param name="Sample">Сравниваемый экземпляр</param>
		/// <returns>Возвращает true в случае соответствия экземпляров</returns>
		public bool Equals (SourceTableRow Sample)
			{
			return ((x == Sample.x) && (tableNumber == Sample.tableNumber));
			}
		}
	}
