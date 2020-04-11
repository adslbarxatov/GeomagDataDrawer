using System;
using System.IO;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class SVGGeneratorForm:Form
		{
		// Переменные
		bool showSuccessMessage = true;

		/// <summary>
		/// Конструктор. Запускает главную форму программы
		/// </summary>
		public SVGGeneratorForm ()
			{
			InitializeComponent ();

			// Настройка контролов
			PrepareMainForm ();

			// Запуск
			this.ShowDialog ();
			}

		// Настройка контролов главного окна
		private void PrepareMainForm ()
			{
			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			OFDialog.Title = OFLabel.Text = "Выберите сценарий для формирования изображения";
			OFDialog.Filter = SSDialog.Filter = "Файлы сценариев (*.sc)|*.sc|Все файлы|*.*";

			SFDialog.Title = SFLabel.Text = "Укажите место для сохранения изображения";
			SFDialog.Filter = "Файлы векторных изображений SVG|*.svg|Файлы векторных изображений EMF|*.emf";

			SSDialog.Title = "Укажите место для сохранения образца сценария";
			}

		/// <summary>
		/// Конструктор. Выполняет генерацию изображения в тихом режиме
		/// </summary>
		/// <param name="SourcePath">Имя файла сценария</param>
		/// <param name="DestinationPath">Имя файла изображения</param>
		/// <param name="Format">Формат фалйа изображения</param>
		public SVGGeneratorForm (string SourcePath, string DestinationPath, string Format)
			{
			InitializeComponent ();

			// Настройка окна под тихое выполнение
			OFName.Text = SourcePath;
			SFName.Text = DestinationPath;
			PrepareMainForm ();

			switch (Format.ToLower ())
				{
				case "emf":
					SFDialog.FilterIndex = 2;
					break;

				default:
					SFDialog.FilterIndex = 1;
					break;
				}

			showSuccessMessage = false;
			GenerateImage.Enabled = BExit.Enabled = OFSelect.Enabled = SFSelect.Enabled = SaveSample.Enabled = false;

			// Выполнение
			this.Show ();
			this.Visible = false;
			GenerateImage_Click (null, null);
			this.Close ();
			}

		// Выбор входного файла
		private void OFSelect_Click (object sender, System.EventArgs e)
			{
			OFDialog.ShowDialog ();
			}

		private void OFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			OFName.Text = OFDialog.FileName;
			}

		// Выбор выходного файла
		private void SFSelect_Click (object sender, System.EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			SFName.Text = SFDialog.FileName;
			}

		// Выход
		private void BExit_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Генерация
		private void GenerateImage_Click (object sender, System.EventArgs e)
			{
			// Контроль параметров
			if (OFName.Text == "")
				{
				MessageBox.Show ("Не задан файл сценария для формирования изображения", ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			if (SFName.Text == "")
				{
				MessageBox.Show ("Не задано расположение создаваемого изображения", ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Чтение файла сценария
			SVGScriptReader svgsr = new SVGScriptReader (OFName.Text);
			if (svgsr.InitResult != SVGScriptReader.InitResults.Ok)
				{
				string msg = "Ошибка чтения файла сценария: ";

				switch (svgsr.InitResult)
					{
					case SVGScriptReader.InitResults.BrokenLineColor:
						msg += ("ошибка описания цвета кривой в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenLinePoint:
						msg += ("ошибка описания точки кривой в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenLineWidth:
						msg += ("ошибка описания толщины кривой в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOxColor:
						msg += ("ошибка описания цвета оси Ox в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOxNotch:
						msg += ("ошибка описания засечки на оси Ox в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOxOffset:
						msg += ("ошибка описания смещения оси Ox в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOxWidth:
						msg += ("ошибка описания толщины оси Ox в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOyColor:
						msg += ("ошибка описания цвета оси Oy в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOyNotch:
						msg += ("ошибка описания засечки на оси Oy в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOyOffset:
						msg += ("ошибка описания смещения оси Oy в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenOyWidth:
						msg += ("ошибка описания толщины оси Oy в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.BrokenText:
						msg += ("ошибка описания текстовой подписи в строке " + svgsr.CurrentLine.ToString ());
						break;

					case SVGScriptReader.InitResults.CannotCreateTMP:
						msg += "не удаётся создать вспомогательный файл. Возможно, расположение файла сценария недоступно для записи";
						break;

					case SVGScriptReader.InitResults.CannotIncludeFile:
						msg += "подключаемый файл «" + svgsr.FaliedIncludeFile + "» не найден, недоступен, или параметры его подключения некорректны";
						break;

					case SVGScriptReader.InitResults.FileNotAvailable:
						msg += "файл сценария не найден или недоступен";
						break;

					case SVGScriptReader.InitResults.IncludeDeepOverflow:
						msg += "превышено максимально допустимое количество подключений. Возможно, присутствует циклическая ссылка";
						break;

					default:	// f.e., NotInited
						throw new Exception (ProgramDescription.AssemblyExceptionMessage);
					}

				MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Запись файла SVG
			IVectorAdapter vectorAdapter;
			switch (SFDialog.FilterIndex)
				{
				// EMF
				case 2:
					vectorAdapter = new EMFAdapter (SFName.Text, (uint)(svgsr.MaxX - svgsr.MinX), (uint)(svgsr.MaxY - svgsr.MinY));
					break;

				// SVG
				default:
					vectorAdapter = new SVGAdapter (SFName.Text, (uint)(svgsr.MaxX - svgsr.MinX), (uint)(svgsr.MaxY - svgsr.MinY));
					break;
				}
			if (vectorAdapter.InitResult != VectorAdapterInitResults.Opened)
				{
				string msg = "Ошибка записи файла изображения: ";

				switch (vectorAdapter.InitResult)
					{
					case VectorAdapterInitResults.CannotCreateFile:
						msg += ("не удалось создать файл в указанном расположении");
						break;

					case VectorAdapterInitResults.IncorrectImageSize:
						msg += ("данные в файле скрипта не позволяют сформировать двумерное изображение");
						break;

					default:	// f.e., Closed, NotInited
						throw new Exception (ProgramDescription.AssemblyExceptionMessage);
					}

				MessageBox.Show (msg, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			// Кривые
			for (int i = 0; i < svgsr.LinesX.Count; i++)
				{
				vectorAdapter.OpenGroup ();

				for (int j = 0; j < svgsr.LinesX[i].Count - 1; j++)
					{
					vectorAdapter.DrawLine (svgsr.LinesX[i][j], svgsr.LinesY[i][j], svgsr.LinesX[i][j + 1], svgsr.LinesY[i][j + 1],
						svgsr.LinesWidths[i], svgsr.LinesColors[i]);
					}

				vectorAdapter.CloseGroup ();
				}

			// Ось Ox
			if (svgsr.DrawOx)
				{
				// Группировка
				vectorAdapter.OpenGroup ();

				// Ось
				vectorAdapter.DrawLine (svgsr.MinX, svgsr.OxOffset, svgsr.MaxX, svgsr.OxOffset, svgsr.OxWidth, svgsr.OxColor);

				// Засечки
				for (int i = 0; i < svgsr.OxNotchesOffsets.Count; i++)
					{
					vectorAdapter.DrawLine (svgsr.OxNotchesOffsets[i], svgsr.OxOffset - svgsr.OxNotchesSizes[i] / 2.0,
						svgsr.OxNotchesOffsets[i], svgsr.OxOffset + svgsr.OxNotchesSizes[i] / 2.0, svgsr.OxWidth, svgsr.OxColor);
					}

				// Завершение
				vectorAdapter.CloseGroup ();
				}

			// Ось Oy
			if (svgsr.DrawOy)
				{
				// Группировка
				vectorAdapter.OpenGroup ();

				// Ось
				vectorAdapter.DrawLine (svgsr.OyOffset, svgsr.MinY, svgsr.OyOffset, svgsr.MaxY, svgsr.OyWidth, svgsr.OyColor);

				// Засечки
				for (int i = 0; i < svgsr.OyNotchesOffsets.Count; i++)
					{
					vectorAdapter.DrawLine (svgsr.OyOffset - svgsr.OyNotchesSizes[i] / 2.0f, svgsr.OyNotchesOffsets[i],
						svgsr.OyOffset + svgsr.OyNotchesSizes[i] / 2.0f, svgsr.OyNotchesOffsets[i], svgsr.OyWidth, svgsr.OyColor);
					}

				// Завершение
				vectorAdapter.CloseGroup ();
				}

			// Текстовые подписи
			for (int i = 0; i < svgsr.Texts.Count; i++)
				{
				vectorAdapter.DrawText (svgsr.TextX[i], svgsr.TextY[i], svgsr.Texts[i], svgsr.TextFonts[i], svgsr.TextColors[i]);
				}

			// Исходный скрипт в качестве комментария
			vectorAdapter.AddComment (svgsr.SourceScript);

			// Завершено
			vectorAdapter.CloseFile ();

			if (showSuccessMessage)
				{
				MessageBox.Show ("Файл изображения успешно сформирован", ProgramDescription.AssemblyTitle,
					 MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}

		// Сохранение образца сценария
		private void SaveSample_Click (object sender, EventArgs e)
			{
			SSDialog.ShowDialog ();
			}

		private void SSDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Попытка создания файла образца
			FileStream FS = null;
			try
				{
				FS = new FileStream (SSDialog.FileName, FileMode.Create);
				}
			catch
				{
				MessageBox.Show ("Не удалось создать файл образца. Возможно, указанное расположение защищено от записи",
					ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
				}

			FS.Write (GeomagDataDrawer.Properties.SVGGenerator.Sample, 0, GeomagDataDrawer.Properties.SVGGenerator.Sample.Length);

			FS.Close ();
			}

		// Отображение справки
		private void MainForm_HelpButtonClicked (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Отмена обработки события вызова справки
			e.Cancel = true;

			// Отображение
			MessageBox.Show (ProgramDescription.AssemblyTitle + " v " + ProgramDescription.AssemblyVersion + "\n" +
				ProgramDescription.AssemblyDescription + "\n" + ProgramDescription.AssemblyCopyright + "\n" +
				ProgramDescription.AssemblyLastUpdate, ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
