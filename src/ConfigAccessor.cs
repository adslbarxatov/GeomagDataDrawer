using System;
using System.IO;
using System.Windows.Forms;

namespace GeomagDataDrawer
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
				return top;
				}
			set
				{
				if (value > Screen.PrimaryScreen.Bounds.Height)
					{
					top = Screen.PrimaryScreen.Bounds.Height;
					}
				else if (value < 0)
					{
					top = 0;
					}
				else
					{
					top = value;
					}
				}
			}
		private int top;

		/// <summary>
		/// Возвращает или задаёт смещение окна относительно левого края экрана
		/// </summary>
		public int Left
			{
			get
				{
				return left;
				}
			set
				{
				if (value > Screen.PrimaryScreen.Bounds.Width)
					{
					left = Screen.PrimaryScreen.Bounds.Width;
					}
				else if (value < 0)
					{
					left = 0;
					}
				else
					{
					left = value;
					}
				}
			}
		private int left;

		/// <summary>
		/// Возвращает или задаёт ширину окна
		/// </summary>
		public uint Width
			{
			get
				{
				return width;
				}
			set
				{
				if (value > Screen.PrimaryScreen.Bounds.Width)
					{
					width = (uint)Screen.PrimaryScreen.Bounds.Width;
					}
				else if (value < MinWidth)
					{
					width = MinWidth;
					}
				else
					{
					width = value;
					}
				}
			}
		private uint width;

		/// <summary>
		/// Возвращает или задаёт высоту окна
		/// </summary>
		public uint Height
			{
			get
				{
				return height;
				}
			set
				{
				if (value > Screen.PrimaryScreen.Bounds.Height)
					{
					height = (uint)Screen.PrimaryScreen.Bounds.Height;
					}
				else if (value < MinHeight)
					{
					height = MinHeight;
					}
				else
					{
					height = value;
					}
				}
			}
		private uint height;

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
		public const string LineParametersFileName = "GeomagDataDrawer.txt";

		// Константа с именем файла настроек приложения
		private const string ConfigFileName = "GeomagDataDrawer.cfg";

		/// <summary>
		/// Возвращает или задаёт необходимость подтверждения выхода из программы
		/// </summary>
		public bool ForceExitConfirmation
			{
			get
				{
				return forceExitConfirmation;
				}
			set
				{
				forceExitConfirmation = value;
				}
			}
		private bool forceExitConfirmation;

		/// <summary>
		/// Возвращает или задаёт необходимость использования автоматически сохраняемого файла данных
		/// </summary>
		public bool ForceUsingBackupDataFile
			{
			get
				{
				return forceUsingBackupDataFile;
				}
			set
				{
				forceUsingBackupDataFile = value;
				}
			}
		private bool forceUsingBackupDataFile;

		/// <summary>
		/// Возвращает или задаёт необходимость автоматического добавления первых столбцов на диаграмму
		/// </summary>
		public bool ForceShowDiagram
			{
			get
				{
				return forceShowDiagram;
				}
			set
				{
				forceShowDiagram = value;
				}
			}
		private bool forceShowDiagram;

		/// <summary>
		/// Возвращает или задаёт количество первых строк файла, используемых для поиска подписей
		/// </summary>
		public uint SkippedLinesCount
			{
			get
				{
				return skippedLinesCount;
				}
			set
				{
				if (value > MaxSkippedLinesCount)
					{
					skippedLinesCount = MaxSkippedLinesCount;
					}
				else
					{
					skippedLinesCount = value;
					}
				}
			}
		private uint skippedLinesCount;

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
				return forceSavingColumnNames;
				}
			set
				{
				forceSavingColumnNames = value;
				}
			}
		private bool forceSavingColumnNames;

		/// <summary>
		/// Возвращает или задаёт предполагаемое количество столбцов для опции извлечения данных
		/// </summary>
		public uint ExpectedColumnsCount
			{
			get
				{
				return expectedColumnsCount;
				}
			set
				{
				if (value > MaxExpectedColumnsCount)
					{
					expectedColumnsCount = MaxExpectedColumnsCount;
					}
				else if (value < 2)
					{
					expectedColumnsCount = 2;
					}
				else
					{
					expectedColumnsCount = value;
					}
				}
			}
		private uint expectedColumnsCount;

		/// <summary>
		/// Максимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MaxExpectedColumnsCount = 100;

		/// <summary>
		/// Возвращает или задаёт необходимость отключения дополнительных функций мыши
		/// </summary>
		public bool DisableMousePlacing
			{
			get
				{
				return disableMousePlacing;
				}
			set
				{
				disableMousePlacing = value;
				}
			}
		private bool disableMousePlacing;

		/// <summary>
		/// Возвращает или задаёт язык интерфейса программы
		/// </summary>
		public SupportedLanguages InterfaceLanguage
			{
			get
				{
				return interfaceLanguage;
				}
			set
				{
				interfaceLanguage = value;
				}
			}
		private SupportedLanguages interfaceLanguage;

		/// <summary>
		/// Конструктор. Загружает ранее сохранённые параметры работы программы
		/// </summary>
		public ConfigAccessor ()
			{
			// Загрузка стандартных значений
			width = MinWidth;
			height = MinHeight;
			left = (int)(Screen.PrimaryScreen.Bounds.Width - width) / 2;
			top = (int)(Screen.PrimaryScreen.Bounds.Height - height) / 2;
			forceExitConfirmation = true;
			forceUsingBackupDataFile = true;
			forceShowDiagram = true;
			skippedLinesCount = 0;
			forceSavingColumnNames = true;
			expectedColumnsCount = 2;
			disableMousePlacing = false;
			interfaceLanguage = SupportedLanguages.ru_ru;

			// Попытка открытия стандартного файла настроек
			FileStream FS = null;
			try
				{
				FS = new FileStream (Application.StartupPath + "\\" + ConfigFileName, FileMode.Open);
				}
			catch
				{
				return;
				}
			BinaryReader BR = new BinaryReader (FS);

			// Получение значений (присваивается через свойства для выполнения ограничений)
			try
				{
				Left = BR.ReadUInt16 ();
				Top = BR.ReadUInt16 ();
				Width = BR.ReadUInt16 ();
				Height = BR.ReadUInt16 ();
				ForceExitConfirmation = BR.ReadBoolean ();
				ForceUsingBackupDataFile = BR.ReadBoolean ();
				ForceShowDiagram = BR.ReadBoolean ();
				SkippedLinesCount = BR.ReadUInt16 ();
				ForceSavingColumnNames = BR.ReadBoolean ();
				ExpectedColumnsCount = BR.ReadUInt16 ();
				DisableMousePlacing = BR.ReadBoolean ();
				InterfaceLanguage = (SupportedLanguages)BR.ReadUInt16 ();
				}
			catch
				{
				}

			// Завершение
			BR.Close ();
			FS.Close ();
			}

		/// <summary>
		/// Метод сохраняет параметры работы программы
		/// </summary>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool SaveConfiguration ()
			{
			// Запись в файл
			FileStream FS = null;
			try
				{
				FS = new FileStream (Application.StartupPath + "\\" + ConfigFileName, FileMode.Create);
				}
			catch
				{
				return false;
				}
			BinaryWriter BW = new BinaryWriter (FS);

			BW.Write ((UInt16)left);
			BW.Write ((UInt16)top);
			BW.Write ((UInt16)width);
			BW.Write ((UInt16)height);
			BW.Write (forceExitConfirmation);
			BW.Write (forceUsingBackupDataFile);
			BW.Write (forceShowDiagram);
			BW.Write ((UInt16)skippedLinesCount);
			BW.Write (forceSavingColumnNames);
			BW.Write ((UInt16)expectedColumnsCount);
			BW.Write (disableMousePlacing);
			BW.Write ((UInt16)interfaceLanguage);

			BW.Close ();
			FS.Close ();
			return true;
			}
		}
	}
