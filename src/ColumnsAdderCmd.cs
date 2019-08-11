using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму ввода параметрической строки для построения новой кривой
	/// </summary>
	public partial class ColumnsAdderCmd:Form
		{
		// Переменные
		private uint dataColumnsCount = 0;	// Число столбцов в исходном массиве данных
		private SupportedLanguages language;

		/// <summary>
		/// Возвращает номер столбца данных, интерпретируемого как столбец абсцисс
		/// </summary>
		public List<uint> XColumnNumber
			{
			get
				{
				return xColumnNumber;
				}
			}
		private List<uint> xColumnNumber = new List<uint> ();

		/// <summary>
		/// Возвращает номер столбца данных, интерпретируемого как столбец ординат
		/// </summary>
		public List<uint> YColumnNumber
			{
			get
				{
				return yColumnNumber;
				}
			}
		private List<uint> yColumnNumber = new List<uint> ();

		/// <summary>
		/// Возвращает ширину изображения новой диаграммы
		/// </summary>
		public List<uint> ImageWidth
			{
			get
				{
				return imageWidth;
				}
			}
		private List<uint> imageWidth = new List<uint> ();

		/// <summary>
		/// Возвращает высоту изображения новой диаграммы
		/// </summary>
		public List<uint> ImageHeight
			{
			get
				{
				return imageHeight;
				}
			}
		private List<uint> imageHeight = new List<uint> ();

		/// <summary>
		/// Возвращает смещение изображения новой диаграммы относительно левой границы экрана
		/// </summary>
		public List<uint> ImageLeft
			{
			get
				{
				return imageLeft;
				}
			}
		private List<uint> imageLeft = new List<uint> ();

		/// <summary>
		/// Возвращает смещение изображения новой диаграммы относительно верхней границы экрана
		/// </summary>
		public List<uint> ImageTop
			{
			get
				{
				return imageTop;
				}
			}
		private List<uint> imageTop = new List<uint> ();

		/// <summary>
		/// Возвращает подпись диаграммы
		/// </summary>
		public List<string> LineName
			{
			get
				{
				return lineName;
				}
			}
		private List<string> lineName = new List<string> ();

		/// <summary>
		/// Возвращает смещение подписи диаграммы относительно левой границы экрана
		/// </summary>
		public List<uint> LineNameLeftOffset
			{
			get
				{
				return lineNameLeftOffset;
				}
			}
		private List<uint> lineNameLeftOffset = new List<uint> ();

		/// <summary>
		/// Возвращает смещение подписи диаграммы относительно верхней границы экрана
		/// </summary>
		public List<uint> LineNameTopOffset
			{
			get
				{
				return lineNameTopOffset;
				}
			}
		private List<uint> lineNameTopOffset = new List<uint> ();

		/// <summary>
		/// Возвращает флаг, указывающий, следует ли определять автоматически положение подписи диаграммы
		/// </summary>
		public List<bool> AutoNameOffset
			{
			get
				{
				return autoNameOffset;
				}
			}
		private List<bool> autoNameOffset = new List<bool> ();

		/// <summary>
		/// Возвращает true, если выбор данных был отменён
		/// </summary>
		public bool Cancelled
			{
			get
				{
				return cancelled;
				}
			}
		private bool cancelled = true;

		/// <summary>
		/// Конструктор. Инициализирует интерфейс ввода параметров
		/// </summary>
		/// <param name="DataColumnsCount">Число доступных столбцов исходного массива данных</param>
		/// <param name="Silent">Инициализация в тихом режиме</param>
		/// <param name="Language">Язык локализации</param>
		public ColumnsAdderCmd (uint DataColumnsCount, bool Silent, SupportedLanguages Language)
			{
			// Инициализация и локализация формы
			InitializeComponent ();
			LanguageProvider.SetControlsText (this, Language);
			ApplyButton.Text = LanguageProvider.GetText ("ApplyButton", Language);
			AbortButton.Text = LanguageProvider.GetText ("AbortButton", Language);
			this.Text = LanguageProvider.GetControlText (this.Name, "T", Language);

			// Настройка контролов
			OFDialog.Filter = SFDialog.Filter = LanguageProvider.GetControlText (this.Name, "OFDialog_F", Language);
			OFDialog.Title = LanguageProvider.GetControlText (this.Name, "OFDialog", Language);
			SFDialog.Title = LanguageProvider.GetControlText (this.Name, "SFDialog", Language);

			// Загрузка параметров
			dataColumnsCount = DataColumnsCount;
			language = Language;

			// Запуск
			if (!Silent)
				{
				this.ShowDialog ();
				}
			}

		// Отмена
		private void AbortButton_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// ОК
		private void ApplyButton_Click (object sender, System.EventArgs e)
			{
			cancelled = false;

			this.Close ();
			}

		// Обработка строки параметров
		private void CommandLine_TextChanged (object sender, System.EventArgs e)
			{
			// Блокировка кнопки ОК
			ApplyButton.Enabled = false;
			ProcessingMsg.ForeColor = Color.FromArgb (192, 0, 0);

			// Сброс состояния
			autoNameOffset.Clear ();
			xColumnNumber.Clear ();
			yColumnNumber.Clear ();
			imageWidth.Clear ();
			imageHeight.Clear ();
			imageLeft.Clear ();
			imageTop.Clear ();
			lineName.Clear ();
			lineNameLeftOffset.Clear ();
			lineNameTopOffset.Clear ();

			// Обработка
			ProcessingMsg.Text = ProcessCommandLine (CommandLine.Text);
			if (ProcessingMsg.Text == LanguageProvider.GetControlText (this.Name, "Correct", language))
				{
				// Успешно
				ApplyButton.Enabled = true;
				ProcessingMsg.ForeColor = Color.FromArgb (0, 192, 0);
				}
			}

		// Метод обрабатывает строку и в случае успеха добавляет её в список готовых строк
		private string ProcessCommandLine (string Line)
			{
			// Разделение параметров
			char[] splitters = new char[] { ';' };	// Массив сплиттеров
			string[] values = Line.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);

			// Предварительные значения
			bool autoNameOffsetT = true;
			uint xColumnNumberT = 0;
			uint yColumnNumberT = 1;
			uint imageWidthT = 0;
			uint imageHeightT = 0;
			uint imageLeftT = 0;
			uint imageTopT = 0;
			string lineNameT = "";
			uint lineNameLeftOffsetT = 0;
			uint lineNameTopOffsetT = 0;

			if ((values.Length == 6) || (values.Length == 9))
				{
				// Короткая строка
				try
					{
					xColumnNumberT = uint.Parse (values[0]);
					yColumnNumberT = uint.Parse (values[1]);
					xColumnNumberT--;
					yColumnNumberT--;
					}
				catch
					{
					return LanguageProvider.GetControlText (this.Name, "ICNError", language);
					}

				try
					{
					imageWidthT = uint.Parse (values[2]);
					imageHeightT = uint.Parse (values[3]);
					}
				catch
					{
					return LanguageProvider.GetControlText (this.Name, "IDSError", language);
					}

				try
					{
					imageLeftT = uint.Parse (values[4]);
					imageTopT = uint.Parse (values[5]);
					}
				catch
					{
					return LanguageProvider.GetControlText (this.Name, "IDOError", language);
					}

				// Длинная строка
				if (values.Length == 9)
					{
					autoNameOffsetT = false;

					lineNameT = values[6];

					try
						{
						lineNameLeftOffsetT = uint.Parse (values[7]);
						lineNameTopOffsetT = uint.Parse (values[8]);
						}
					catch
						{
						return LanguageProvider.GetControlText (this.Name, "ITOError", language);
						}
					}
				}
			else
				{
				return LanguageProvider.GetControlText (this.Name, "IPCError", language);
				}

			// Контроль диапазонов
			if ((xColumnNumberT >= dataColumnsCount) || (yColumnNumberT >= dataColumnsCount))
				{
				return string.Format (LanguageProvider.GetControlText (this.Name, "OCNError", language), dataColumnsCount);
				}

			if ((imageWidthT > DiagramStyle.MaxImageWidth) || (imageWidthT < DiagramStyle.MinImageWidth) ||
				(imageHeightT > DiagramStyle.MaxImageHeight) || (imageHeightT < DiagramStyle.MinImageHeight))
				{
				return string.Format (LanguageProvider.GetControlText (this.Name, "ODSError", language),
					DiagramStyle.MinImageWidth, DiagramStyle.MaxImageWidth, DiagramStyle.MinImageHeight, DiagramStyle.MaxImageHeight);
				}

			if ((imageLeftT > DiagramStyle.MaxImageWidth) || (imageTopT > DiagramStyle.MaxImageHeight))
				{
				return string.Format (LanguageProvider.GetControlText (this.Name, "ODOError", language),
					DiagramStyle.MaxImageWidth, DiagramStyle.MaxImageHeight);
				}

			if ((lineNameLeftOffsetT > imageWidthT) || (lineNameTopOffsetT > imageHeightT))
				{
				return string.Format (LanguageProvider.GetControlText (this.Name, "OTOError", language),
					imageWidthT, imageHeightT);
				}

			// Успешно
			autoNameOffset.Add (autoNameOffsetT);
			xColumnNumber.Add (xColumnNumberT);
			yColumnNumber.Add (yColumnNumberT);
			imageWidth.Add (imageWidthT);
			imageHeight.Add (imageHeightT);
			imageLeft.Add (imageLeftT);
			imageTop.Add (imageTopT);
			lineName.Add (lineNameT);
			lineNameLeftOffset.Add (lineNameLeftOffsetT);
			lineNameTopOffset.Add (lineNameTopOffsetT);

			return LanguageProvider.GetControlText (this.Name, "Correct", language);
			}

		// Выбор варианта загрузки
		private void MultiCmd_CheckedChanged (object sender, System.EventArgs e)
			{
			if (MultiCmd.Checked)
				{
				CommandLine.Enabled = false;
				SelectFile.Enabled = true;

				ApplyButton.Enabled = false;
				}
			}

		private void SingleCmd_CheckedChanged (object sender, System.EventArgs e)
			{
			if (SingleCmd.Checked)
				{
				CommandLine.Enabled = true;
				SelectFile.Enabled = false;

				ApplyButton.Enabled = false;

				// Принудительная повторная обработка строки
				CommandLine_TextChanged (null, null);
				}
			}

		// Выбор файла
		private void SelectFile_Click (object sender, System.EventArgs e)
			{
			OFDialog.ShowDialog ();
			}

		private void OFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Блокировка кнопки ОК
			ApplyButton.Enabled = false;
			ProcessingResults.ForeColor = Color.FromArgb (192, 0, 0);

			// Загрузка параметров
			if (!LoadParametersFile (OFDialog.FileName))
				{
				return;
				}

			// Конечная обработка
			if (autoNameOffset.Count != 0)
				{
				ApplyButton.Enabled = true;
				}

			if (ProcessingResults.Text == "")
				{
				ProcessingResults.Text = LanguageProvider.GetControlText (this.Name, "Correct", language);
				ProcessingResults.ForeColor = Color.FromArgb (0, 192, 0);
				}
			}

		/// <summary>
		/// Метод извлекает параметры добавляемых кривых из указанного файла
		/// </summary>
		/// <param name="FileName">Имя загружаемого файла</param>
		/// <returns>Возвращает true, если загрузка выполнена успешно</returns>
		public bool LoadParametersFile (string FileName)
			{
			// Сброс состояния
			autoNameOffset.Clear ();
			xColumnNumber.Clear ();
			yColumnNumber.Clear ();
			imageWidth.Clear ();
			imageHeight.Clear ();
			imageLeft.Clear ();
			imageTop.Clear ();
			lineName.Clear ();
			lineNameLeftOffset.Clear ();
			lineNameTopOffset.Clear ();
			ProcessingResults.Text = "";

			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (FileName, FileMode.Open);
				}
			catch
				{
				ProcessingResults.Text = LanguageProvider.GetText ("TemplateLoadError", language);
				return false;
				}

			// Файл открыт
			StreamReader SR = new StreamReader (FS, Encoding.GetEncoding (1251));

			// Чтение и обработка
			uint line = 0;
			while (!SR.EndOfStream)
				{
				string res = ProcessCommandLine (SR.ReadLine ());
				line++;

				if (res != LanguageProvider.GetControlText (this.Name, "Correct", language))
					{
					ProcessingResults.Text += ("#" + line.ToString () + ": " + res + "\r\n");
					}

				if (line >= DiagramData.MaxLines)
					{
					ProcessingResults.Text += (string.Format (LanguageProvider.GetText ("LinesOverloadError", language),
						DiagramData.MaxLines) + "\r\n");
					break;
					}
				}

			// Завершено
			SR.Close ();
			FS.Close ();
			return true;
			}

		/// <summary>
		/// Метод создаёт образец файла параметров добавляемых кривых
		/// </summary>
		/// <param name="FileName">Имя создаваемого файла</param>
		/// <returns>Возвращает true в случае успеха</returns>
		public bool CreateParametersFile (string FileName)
			{
			// Попытка открытия файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (FileName, FileMode.Create);
				}
			catch
				{
				ProcessingResults.Text = LanguageProvider.GetText ("TemplateSaveError", language);
				return false;
				}

			// Запись и завершение
			FS.Write (GeomagDataDrawer.Properties.Resources.LineParameters, 0, GeomagDataDrawer.Properties.Resources.LineParameters.Length);
			FS.Close ();
			return true;
			}

		/// <summary>
		/// Метод сохраняет параметры стиля указанного объекта DiagramData в файл, запрашивая его имя
		/// </summary>
		/// <param name="Data">Объект DiagramData, стиль которого следует использовать для создания файла параметров</param>
		/// <returns>Возвращает 0 в случае успеха, -1, если входные параметры некорректны, -2, если не удаётся создать файл,
		/// 1, если файл не был выбран</returns>
		public int SaveParametersFile (DiagramData Data)
			{
			// Контроль параметров
			if ((Data == null) || (Data.InitResult != DiagramDataInitResults.Ok))
				{
				return -1;
				}

			// Запрос имени файла
			if (SFDialog.ShowDialog () != System.Windows.Forms.DialogResult.OK)
				{
				return 1;
				}

			// Запись
			if (!WriteParametersFile (Data, SFDialog.FileName))
				{
				return -2;
				}

			// Успешно
			return 0;
			}

		// Метод сохраняет параметры стилей в файл шаблона
		private bool WriteParametersFile (DiagramData Data, string FileName)
			{
			// Попытка создания файла файла
			FileStream FS = null;
			try
				{
				FS = new FileStream (FileName, FileMode.Create);
				}
			catch
				{
				return false;
				}
			StreamWriter SW = new StreamWriter (FS, Encoding.GetEncoding (1251));

			// Запись
			for (int i = 0; i < Data.LinesCount; i++)
				{
				SW.Write ((Data.GetStyle (i).XColumnNumber + 1).ToString () + ";");
				SW.Write ((Data.GetStyle (i).YColumnNumber + 1).ToString () + ";");
				SW.Write (Data.GetStyle (i).DiagramImageWidth.ToString () + ";");
				SW.Write (Data.GetStyle (i).DiagramImageHeight.ToString () + ";");
				SW.Write (Data.GetStyle (i).DiagramImageLeftOffset.ToString () + ";");
				SW.Write (Data.GetStyle (i).DiagramImageTopOffset.ToString () + ";");
				SW.Write (Data.GetStyle (i).LineName + ";");
				SW.Write (Data.GetStyle (i).LineNameLeftOffset.ToString () + ";");
				SW.WriteLine (Data.GetStyle (i).LineNameTopOffset.ToString ());
				}

			// Завершение
			SW.Close ();
			FS.Close ();
			return true;
			}
		}
	}
