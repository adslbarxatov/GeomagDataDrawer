using Microsoft.Win32;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает доступ к конфигурации программы
	/// </summary>
	public class ConfigAccessor
		{
		/// <summary>
		/// Возвращает или задаёт смещение окна относительно верхнего края экрана
		/// </summary>
		public int Top
			{
			get
				{
				try
					{
					return int.Parse (GetSetting ("Top"));
					}
				catch
					{
					SetSetting ("Top", ((screenHeight - MinHeight) / 2).ToString ());
					return (screenHeight - (int)MinHeight) / 2;
					}
				}
			set
				{
				if (value > screenHeight)
					{
					SetSetting ("Top", screenHeight.ToString ());
					}
				else if (value < 0)
					{
					SetSetting ("Top", "0");
					}
				else
					{
					SetSetting ("Top", value.ToString ());
					}
				}
			}

		/// <summary>
		/// Возвращает или задаёт смещение окна относительно левого края экрана
		/// </summary>
		public int Left
			{
			get
				{
				try
					{
					return int.Parse (GetSetting ("Left"));
					}
				catch
					{
					SetSetting ("Left", ((screenWidth - MinWidth) / 2).ToString ());
					return (screenWidth - (int)MinWidth) / 2;
					}
				}
			set
				{
				if (value > screenWidth)
					{
					SetSetting ("Left", screenWidth.ToString ());
					}
				else if (value < 0)
					{
					SetSetting ("Left", "0");
					}
				else
					{
					SetSetting ("Left", value.ToString ());
					}
				}
			}

		/// <summary>
		/// Возвращает или задаёт ширину окна
		/// </summary>
		public uint Width
			{
			get
				{
				try
					{
					return uint.Parse (GetSetting ("Width"));
					}
				catch
					{
					SetSetting ("Width", MinWidth.ToString ());
					return MinWidth;
					}
				}
			set
				{
				if (value > screenWidth)
					{
					SetSetting ("Width", screenWidth.ToString ());
					}
				else if (value < MinWidth)
					{
					SetSetting ("Width", MinWidth.ToString ());
					}
				else
					{
					SetSetting ("Width", value.ToString ());
					}
				}
			}

		/// <summary>
		/// Возвращает или задаёт высоту окна
		/// </summary>
		public uint Height
			{
			get
				{
				try
					{
					return uint.Parse (GetSetting ("Height"));
					}
				catch
					{
					SetSetting ("Height", MinHeight.ToString ());
					return MinHeight;
					}
				}
			set
				{
				if (value > screenHeight)
					{
					SetSetting ("Height", screenHeight.ToString ());
					}
				else if (value < MinHeight)
					{
					SetSetting ("Height", MinHeight.ToString ());
					}
				else
					{
					SetSetting ("Height", value.ToString ());
					}
				}
			}

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
		public const string BackupDataFileName = "Backup.gdd";

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
				return (GetSetting ("ForceExitConfirmation") == "FEC");
				}
			set
				{
				SetSetting ("ForceExitConfirmation", value ? "FEC" : "0");
				}
			}

		/// <summary>
		/// Возвращает или задаёт необходимость использования автоматически сохраняемого файла данных
		/// </summary>
		public bool ForceUsingBackupDataFile
			{
			get
				{
				return (GetSetting ("ForceUsingBackupDataFile") == "FUBDF");
				}
			set
				{
				SetSetting ("ForceUsingBackupDataFile", value ? "FUBDF" : "0");
				}
			}

		/// <summary>
		/// Возвращает или задаёт необходимость автоматического добавления первых столбцов на диаграмму
		/// </summary>
		public bool ForceShowDiagram
			{
			get
				{
				string s = GetSetting ("ForceShowDiagram");

				if (s == "")
					{
					SetSetting ("ForceShowDiagram", "FSD");
					return true;
					}

				return (s == "FSD");
				}
			set
				{
				SetSetting ("ForceShowDiagram", value ? "FSD" : "0");
				}
			}

		/// <summary>
		/// Возвращает или задаёт количество первых строк файла, используемых для поиска подписей
		/// </summary>
		public uint SkippedLinesCount
			{
			get
				{
				try
					{
					return uint.Parse (GetSetting ("SkippedLinesCount"));
					}
				catch
					{
					SetSetting ("SkippedLinesCount", "0");
					return 0;
					}
				}
			set
				{
				if (value > MaxSkippedLinesCount)
					{
					SetSetting ("SkippedLinesCount", MaxSkippedLinesCount.ToString ());
					}
				else
					{
					SetSetting ("SkippedLinesCount", value.ToString ());
					}
				}
			}

		/// <summary>
		/// Максимальное количество первых строк, пропускаемых при загрузке файла
		/// </summary>
		public const uint MaxSkippedLinesCount = 100;

		/// <summary>
		/// Возвращает или задаёт необходимость сохранения имён столбцов
		/// </summary>
		public bool ForceSavingColumnNames
			{
			get
				{
				string s = GetSetting ("ForceSavingColumnNames");

				if (s == "")
					{
					SetSetting ("ForceSavingColumnNames", "FSCN");
					return true;
					}

				return (s == "FSCN");
				}
			set
				{
				SetSetting ("ForceSavingColumnNames", value ? "FSCN" : "0");
				}
			}

		/// <summary>
		/// Возвращает или задаёт предполагаемое количество столбцов для опции извлечения данных
		/// </summary>
		public uint ExpectedColumnsCount
			{
			get
				{
				try
					{
					return uint.Parse (GetSetting ("ExpectedColumnsCount"));
					}
				catch
					{
					SetSetting ("ExpectedColumnsCount", MinExpectedColumnsCount.ToString ());
					return MinExpectedColumnsCount;
					}
				}
			set
				{
				if (value > MaxExpectedColumnsCount)
					{
					SetSetting ("ExpectedColumnsCount", MaxExpectedColumnsCount.ToString ());
					}
				else if (value < MinExpectedColumnsCount)
					{
					SetSetting ("ExpectedColumnsCount", MinExpectedColumnsCount.ToString ());
					}
				else
					{
					SetSetting ("ExpectedColumnsCount", value.ToString ());
					}
				}
			}

		/// <summary>
		/// Максимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MaxExpectedColumnsCount = 100;

		/// <summary>
		/// Минимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MinExpectedColumnsCount = 2;

		/// <summary>
		/// Возвращает или задаёт необходимость отключения дополнительных функций мыши
		/// </summary>
		public bool DisableMousePlacing
			{
			get
				{
				return (GetSetting ("DisableMousePlacing") == "DMP");
				}
			set
				{
				SetSetting ("DisableMousePlacing", value ? "DMP" : "0");
				}
			}

		/// <summary>
		/// Конструктор. Загружает ранее сохранённые параметры работы программы
		/// </summary>
		public ConfigAccessor ()
			{
			// Запрос размеров текущего экрана
			try
				{
				screenWidth = Screen.PrimaryScreen.Bounds.Width;
				screenHeight = Screen.PrimaryScreen.Bounds.Height;
				}
			catch
				{
				}
			}

		private int screenWidth = 1280, screenHeight = 720;
		private SupportedLanguages language = Localization.CurrentLanguage;

		/// <summary>
		/// Возвращает или задаёт язык интерфейса
		/// </summary>
		public SupportedLanguages InterfaceLanguage
			{
			get
				{
				return language;
				}
			set
				{
				Localization.CurrentLanguage = language = value;
				}
			}

		// Метод получает значение настройки из реестра
		private string GetSetting (string ValueName)
			{
			string res = "";
			try
				{
				res = Registry.GetValue (ProgramDescription.AssemblySettingsKey, ValueName, "").ToString ();
				}
			catch
				{
				}

			return res;
			}

		// Метод задаёт значение настройки в реестре
		private void SetSetting (string ValueName, string Value)
			{
			try
				{
				Registry.SetValue (ProgramDescription.AssemblySettingsKey, ValueName, Value);
				}
			catch
				{
				}
			}
		}
	}
