namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает доступ к конфигурации программы
	/// </summary>
	public class ConfigAccessor
		{
		/// <summary>
		/// Возвращает минимально возможную ширину окна
		/// </summary>
		public const uint MinWidth = 560;

		/// <summary>
		/// Возвращает минимально возможную высоту окна
		/// </summary>
		public const uint MinHeight = 635;

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
				return RDGenerics.GetSettings (forceExitConfirmationPar, true);
				}
			set
				{
				RDGenerics.SetSettings (forceExitConfirmationPar, value);
				}
			}
		private const string forceExitConfirmationPar = "ForceExitConfirmation";

		/// <summary>
		/// Возвращает или задаёт необходимость использования автоматически сохраняемого файла данных
		/// </summary>
		public bool ForceUsingBackupDataFile
			{
			get
				{
				return RDGenerics.GetSettings (forceUsingBackupDataFilePar, true);
				}
			set
				{
				RDGenerics.SetSettings (forceUsingBackupDataFilePar, value);
				}
			}
		private const string forceUsingBackupDataFilePar = "ForceUsingBackupDataFile";

		/// <summary>
		/// Возвращает или задаёт необходимость автоматического добавления первых столбцов на диаграмму
		/// </summary>
		public bool ForceShowDiagram
			{
			get
				{
				return RDGenerics.GetSettings (forceShowDiagramPar, true);
				}
			set
				{
				RDGenerics.SetSettings (forceShowDiagramPar, value);
				}
			}
		private const string forceShowDiagramPar = "ForceShowDiagram";

		/// <summary>
		/// Возвращает или задаёт необходимость сохранения имён столбцов
		/// </summary>
		public bool ForceSavingColumnNames
			{
			get
				{
				return RDGenerics.GetSettings (forceSavingColumnNamesPar, true);
				}
			set
				{
				RDGenerics.SetSettings (forceSavingColumnNamesPar, value);
				}
			}
		private const string forceSavingColumnNamesPar = "ForceSavingColumnNames";

		/// <summary>
		/// Возвращает или задаёт количество первых строк файла, используемых для поиска подписей
		/// </summary>
		public uint SkippedLinesCount
			{
			get
				{
				return RDGenerics.GetSettings (skippedLinesCountPar, 0);
				}
			set
				{
				RDGenerics.SetSettings (skippedLinesCountPar, value > MaxSkippedLinesCount ?
					MaxSkippedLinesCount : 0);
				}
			}
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
				return RDGenerics.GetSettings (expectedColumnsCountPar, MinExpectedColumnsCount);
				}
			set
				{
				uint v = value;
				if (v < MinExpectedColumnsCount)
					v = MinExpectedColumnsCount;
				if (v > MaxExpectedColumnsCount)
					v = MaxExpectedColumnsCount;
				RDGenerics.SetSettings (expectedColumnsCountPar, v);
				}
			}
		private const string expectedColumnsCountPar = "ExpectedColumnsCount";

		/// <summary>
		/// Максимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MaxExpectedColumnsCount = 100;

		/// <summary>
		/// Минимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MinExpectedColumnsCount = 2;
		}
	}
