﻿namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает доступ к конфигурации программы
	/// </summary>
	public class ConfigAccessor
		{
		/// <summary>
		/// Возвращает минимально возможную ширину окна
		/// </summary>
		public const uint MinWidth = 555;

		/// <summary>
		/// Возвращает минимально возможную высоту окна
		/// </summary>
		public const uint MinHeight = 660;

		/// <summary>
		/// Возвращает имя автоматически сохраняемого файла данных
		/// </summary>
		public const string BackupDataFileName = "Backup." + ProgramDescription.AppDataExtension;

		/// <summary>
		/// Возвращает имя стандартного файла параметров предпросмотра диаграммы
		/// </summary>
		public const string LineParametersFileName = ProgramDescription.AssemblyMainName + ".txt";

		/// <summary>
		/// Возвращает или задаёт необходимость подтверждения выхода из программы
		/// </summary>
		public bool ForceExitConfirmation
			{
			get
				{
				/*return forceExitConfirmation;
				*/
				return RDGenerics.GetSettings (forceExitConfirmationPar, true);
				}
			set
				{
				/*forceExitConfirmation = value;
				RDGenerics.SetAppSettingsValue ("ForceExitConfirmation", forceExitConfirmation ? "FEC" : "0");*/
				RDGenerics.SetSettings (forceExitConfirmationPar, value);
				}
			}
		/*private bool forceExitConfirmation;
		*/
		private const string forceExitConfirmationPar = "ForceExitConfirmation";

		/// <summary>
		/// Возвращает или задаёт необходимость использования автоматически сохраняемого файла данных
		/// </summary>
		public bool ForceUsingBackupDataFile
			{
			get
				{
				/*return forceUsingBackupDataFile;
				*/
				return RDGenerics.GetSettings (forceUsingBackupDataFilePar, true);
				}
			set
				{
				/*forceUsingBackupDataFile = value;
				RDGenerics.SetAppSettingsValue ("ForceUsingBackupDataFile", forceUsingBackupDataFile ?
					"FUBDF" : "0");*/
				RDGenerics.SetSettings (forceUsingBackupDataFilePar, value);
				}
			}
		/*private bool forceUsingBackupDataFile;
		*/
		private const string forceUsingBackupDataFilePar = "ForceUsingBackupDataFile";

		/// <summary>
		/// Возвращает или задаёт необходимость автоматического добавления первых столбцов на диаграмму
		/// </summary>
		public bool ForceShowDiagram
			{
			get
				{
				/*return forceShowDiagram;
				*/
				return RDGenerics.GetSettings (forceShowDiagramPar, true);
				}
			set
				{
				/*forceShowDiagram = value;
				RDGenerics.SetAppSettingsValue ("ForceShowDiagram", forceShowDiagram ? "FSD" : "0");*/
				RDGenerics.SetSettings (forceShowDiagramPar, value);
				}
			}
		/*private bool forceShowDiagram;
		*/
		private const string forceShowDiagramPar = "ForceShowDiagram";

		/// <summary>
		/// Возвращает или задаёт необходимость сохранения имён столбцов
		/// </summary>
		public bool ForceSavingColumnNames
			{
			get
				{
				/*return forceSavingColumnNames;
				*/
				return RDGenerics.GetSettings (forceSavingColumnNamesPar, true);
				}
			set
				{
				/*forceSavingColumnNames = value;
				RDGenerics.SetAppSettingsValue ("ForceSavingColumnNames", forceSavingColumnNames ? "FSCN" : "0");*/
				RDGenerics.SetSettings (forceSavingColumnNamesPar, value);
				}
			}
		/*private bool forceSavingColumnNames;
		*/
		private const string forceSavingColumnNamesPar = "ForceSavingColumnNames";

		/// <summary>
		/// Возвращает или задаёт количество первых строк файла, используемых для поиска подписей
		/// </summary>
		public uint SkippedLinesCount
			{
			get
				{
				/*return skippedLinesCount;
				*/
				return RDGenerics.GetSettings (skippedLinesCountPar, 0);
				}
			set
				{
				/*if (value > MaxSkippedLinesCount)
					skippedLinesCount = MaxSkippedLinesCount;
				else
					skippedLinesCount = value;

				RDGenerics.SetAppSettingsValue ("SkippedLinesCount", skippedLinesCount.ToString ());*/
				RDGenerics.SetSettings (skippedLinesCountPar, value > MaxSkippedLinesCount ?
					MaxSkippedLinesCount : 0);
				}
			}
		/*private uint skippedLinesCount;
		*/
		private const string skippedLinesCountPar = "SkippedLinesCount";

		/// <summary>
		/// Максимальное количество первых строк, пропускаемых при загрузке файла
		/// </summary>
		public const uint MaxSkippedLinesCount = 100;

		/// <summary>
		/// Возвращает или задаёт предполагаемое количество столбцов для опции извлечения данных
		/// </summary>
		public uint ExpectedColumnsCount
			{
			get
				{
				/*return expectedColumnsCount;
				*/
				return RDGenerics.GetSettings (expectedColumnsCountPar, MinExpectedColumnsCount);
				}
			set
				{
				/*if (value > MaxExpectedColumnsCount)
					expectedColumnsCount = MaxExpectedColumnsCount;
				else if (value < MinExpectedColumnsCount)
					expectedColumnsCount = MinExpectedColumnsCount;
				else
					expectedColumnsCount = value;

				RDGenerics.SetAppSettingsValue ("ExpectedColumnsCount", expectedColumnsCount.ToString ());*/
				uint v = value;
				if (v < MinExpectedColumnsCount)
					v = MinExpectedColumnsCount;
				if (v > MaxExpectedColumnsCount)
					v = MaxExpectedColumnsCount;
				RDGenerics.SetSettings (expectedColumnsCountPar, v);
				}
			}
		/*private uint expectedColumnsCount;
		*/
		private const string expectedColumnsCountPar = "ExpectedColumnsCount";

		/// <summary>
		/// Максимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MaxExpectedColumnsCount = 100;

		/// <summary>
		/// Минимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MinExpectedColumnsCount = 2;

		/*/// <summary>
		/// Конструктор. Загружает ранее сохранённые параметры работы программы
		/// </summary>
		public ConfigAccessor ()
			{
			// Флаги
			forceExitConfirmation = (RDGenerics.GetAppSettingsValue ("ForceExitConfirmation") == "FEC");

			string s = RDGenerics.GetAppSettingsValue ("ForceUsingBackupDataFile");
			forceUsingBackupDataFile = (s == "FUBDF") && RDGenerics.AppHasAccessRights (false, false);

			if (s == "")    // Требуется дополнительная обработка, т.к. значение по умолчанию - true
				{
				forceUsingBackupDataFile = true;
				RDGenerics.SetAppSettingsValue ("ForceUsingBackupDataFile", "FUBDF");
				}

			s = RDGenerics.GetAppSettingsValue ("ForceShowDiagram");
			forceShowDiagram = (s == "FSD");

			if (s == "")
				{
				forceShowDiagram = true;
				RDGenerics.SetAppSettingsValue ("ForceShowDiagram", "FSD");
				}

			s = RDGenerics.GetAppSettingsValue ("ForceSavingColumnNames");
			forceSavingColumnNames = (s == "FSCN");

			if (s == "")
				{
				forceSavingColumnNames = true;
				RDGenerics.SetAppSettingsValue ("ForceSavingColumnNames", "FSCN");
				}

			// Строки поиска заголовков
			try
				{
				skippedLinesCount = uint.Parse (RDGenerics.GetAppSettingsValue ("SkippedLinesCount"));
				}
			catch
				{
				skippedLinesCount = 0;
				RDGenerics.SetAppSettingsValue ("SkippedLinesCount", skippedLinesCount.ToString ());
				}

			// Ожидаемое число столбцов
			try
				{
				expectedColumnsCount = uint.Parse (RDGenerics.GetAppSettingsValue ("ExpectedColumnsCount"));
				}
			catch
				{
				expectedColumnsCount = MinExpectedColumnsCount;
				RDGenerics.SetAppSettingsValue ("ExpectedColumnsCount", expectedColumnsCount.ToString ());
				}

			// Завершено
			}*/
		}
	}
