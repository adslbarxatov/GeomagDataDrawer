﻿using Microsoft.Win32;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс обеспечивает доступ к конфигурации программы
	/// </summary>
	public class ConfigAccessor
		{
		// Переменные
		private int screenWidth = 1280, screenHeight = 720;

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
				if (value > screenHeight)
					top = screenHeight;
				else if (value < 0)
					top = 0;
				else
					top = value;

				SetSetting ("Top", top.ToString ());
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
				if (value > screenWidth)
					left = screenWidth;
				else if (value < 0)
					left = 0;
				else
					left = value;

				SetSetting ("Left", left.ToString ());
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
				if (value > screenWidth)
					width = (uint)screenWidth;
				else if (value < MinWidth)
					width = MinWidth;
				else
					width = value;

				SetSetting ("Width", width.ToString ());
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
				if (value > screenHeight)
					height = (uint)screenHeight;
				else if (value < MinHeight)
					height = MinHeight;
				else
					height = value;

				SetSetting ("Height", height.ToString ());
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
				return forceExitConfirmation;
				}
			set
				{
				forceExitConfirmation = value;
				SetSetting ("ForceExitConfirmation", forceExitConfirmation ? "FEC" : "0");
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
				SetSetting ("ForceUsingBackupDataFile", forceUsingBackupDataFile ? "FUBDF" : "0");
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
				SetSetting ("ForceShowDiagram", forceShowDiagram ? "FSD" : "0");
				}
			}
		private bool forceShowDiagram;

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
				SetSetting ("ForceSavingColumnNames", forceSavingColumnNames ? "FSCN" : "0");
				}
			}
		private bool forceSavingColumnNames;

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
				SetSetting ("DisableMousePlacing", disableMousePlacing ? "DMP" : "0");
				}
			}
		private bool disableMousePlacing;

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
					skippedLinesCount = MaxSkippedLinesCount;
				else
					skippedLinesCount = value;

				SetSetting ("SkippedLinesCount", skippedLinesCount.ToString ());
				}
			}
		private uint skippedLinesCount;

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
				return expectedColumnsCount;
				}
			set
				{
				if (value > MaxExpectedColumnsCount)
					expectedColumnsCount = MaxExpectedColumnsCount;
				else if (value < MinExpectedColumnsCount)
					expectedColumnsCount = MinExpectedColumnsCount;
				else
					expectedColumnsCount = value;

				SetSetting ("ExpectedColumnsCount", expectedColumnsCount.ToString ());
				}
			}
		private uint expectedColumnsCount;

		/// <summary>
		/// Максимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MaxExpectedColumnsCount = 100;

		/// <summary>
		/// Минимальное количество столбцов для опции извлечения данных
		/// </summary>
		public const uint MinExpectedColumnsCount = 2;

		/// <summary>
		/// Возвращает или задаёт язык интерфейса
		/// </summary>
		public SupportedLanguages InterfaceLanguage
			{
			get
				{
				return interfaceLanguage;
				}
			set
				{
				Localization.CurrentLanguage = interfaceLanguage = value;
				}
			}
		private SupportedLanguages interfaceLanguage;

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

			// Язык интерфейса
			interfaceLanguage = Localization.CurrentLanguage;

			// Размеры и смещение окна
			try
				{
				top = int.Parse (GetSetting ("Top"));
				}
			catch
				{
				top = (screenHeight - (int)MinHeight) / 2;
				SetSetting ("Top", top.ToString ());
				}

			try
				{
				left = int.Parse (GetSetting ("Left"));
				}
			catch
				{
				left = (screenWidth - (int)MinWidth) / 2;
				SetSetting ("Left", left.ToString ());
				}

			try
				{
				width = uint.Parse (GetSetting ("Width"));
				}
			catch
				{
				width = MinWidth;
				SetSetting ("Width", width.ToString ());
				}

			try
				{
				height = uint.Parse (GetSetting ("Height"));
				}
			catch
				{
				height = MinHeight;
				SetSetting ("Height", height.ToString ());
				}

			// Флаги
			forceExitConfirmation = (GetSetting ("ForceExitConfirmation") == "FEC");

			string s = GetSetting ("ForceUsingBackupDataFile");
			forceUsingBackupDataFile = (s == "FUBDF");

			if (s == "")    // Требуется дополнительная обработка, т.к. значение по умолчанию - true
				{
				forceUsingBackupDataFile = true;
				SetSetting ("ForceUsingBackupDataFile", "FUBDF");
				}

			s = GetSetting ("ForceShowDiagram");
			forceShowDiagram = (s == "FSD");

			if (s == "")
				{
				forceShowDiagram = true;
				SetSetting ("ForceShowDiagram", "FSD");
				}

			s = GetSetting ("ForceSavingColumnNames");
			forceSavingColumnNames = (s == "FSCN");

			if (s == "")
				{
				forceSavingColumnNames = true;
				SetSetting ("ForceSavingColumnNames", "FSCN");
				}

			disableMousePlacing = (GetSetting ("DisableMousePlacing") == "DMP");

			// Строки поиска заголовков
			try
				{
				skippedLinesCount = uint.Parse (GetSetting ("SkippedLinesCount"));
				}
			catch
				{
				skippedLinesCount = 0;
				SetSetting ("SkippedLinesCount", skippedLinesCount.ToString ());
				}

			// Ожидаемое число столбцов
			try
				{
				expectedColumnsCount = uint.Parse (GetSetting ("ExpectedColumnsCount"));
				}
			catch
				{
				expectedColumnsCount = MinExpectedColumnsCount;
				SetSetting ("ExpectedColumnsCount", expectedColumnsCount.ToString ());
				}

			// Завершено
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
