using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class SVGGeneratorForm: Form
		{
		// Переменные
		private bool showSuccessMessage = true;

		/// <summary>
		/// Конструктор. Запускает главную форму программы
		/// </summary>
		public SVGGeneratorForm ()
			{
			InitializeComponent ();

			// Настройка контролов
			this.Text = ProgramDescription.AssemblyTitle;

			// Запуск
			LocalizeForm ();
			this.ShowDialog ();
			}


#if false

		// Using the command line:
		//
		// VectorImageGenerator [Script file name] [Image file name] [Image type {SVG | EMF}]
		// 
		// • If the second parameter is absent, image name wiil be generated from the name of the script file in SVG format;
		// • If the third parameter is missing or incorrectly specified, image will be saved in SVG format;
		// • If all parameters are missing, the application starts in normal mode

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
			GenerateImage.Enabled = BExit.Enabled = OFSelect.Enabled = SFSelect.Enabled =
				SaveSample.Enabled = false;

			// Выполнение
			this.Show ();
			this.Visible = false;
			GenerateImage_Click (null, null);
			this.Close ();
			}

#endif

		// Выбор входного файла
		private void OFSelect_Click (object sender, EventArgs e)
			{
			OFDialog.ShowDialog ();
			}

		private void OFDialog_FileOk (object sender, CancelEventArgs e)
			{
			OFName.Text = OFDialog.FileName;
			}

		// Выбор выходного файла
		private void SFSelect_Click (object sender, EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			SFName.Text = SFDialog.FileName;
			}

		// Выход
		private void BExit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Генерация
		private void GenerateImage_Click (object sender, EventArgs e)
			{
			// Контроль параметров
			if (OFName.Text == "")
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "InputFileNotSpecified");
				return;
				}

			if (SFName.Text == "")
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "OutputFileNotSpecified");
				return;
				}

			// Чтение файла сценария
			SVGScriptReader svgsr = new SVGScriptReader (OFName.Text);
			if (svgsr.InitResult != SVGScriptReader.InitResults.Ok)
				{
				string msg = RDLocale.GetText ("VIG_ScriptReadingError");

				switch (svgsr.InitResult)
					{
					case SVGScriptReader.InitResults.BrokenLineColor:
					case SVGScriptReader.InitResults.BrokenLinePoint:
					case SVGScriptReader.InitResults.BrokenLineWidth:

					case SVGScriptReader.InitResults.BrokenOxColor:
					case SVGScriptReader.InitResults.BrokenOxNotch:
					case SVGScriptReader.InitResults.BrokenOxOffset:
					case SVGScriptReader.InitResults.BrokenOxWidth:

					case SVGScriptReader.InitResults.BrokenOyColor:
					case SVGScriptReader.InitResults.BrokenOyNotch:
					case SVGScriptReader.InitResults.BrokenOyOffset:
					case SVGScriptReader.InitResults.BrokenOyWidth:

					case SVGScriptReader.InitResults.BrokenText:
						msg += (string.Format (RDLocale.GetText ("VIG_" + svgsr.InitResult.ToString ()),
							svgsr.CurrentLine));
						break;

					case SVGScriptReader.InitResults.CannotCreateTMP:
						msg = string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
							"?");
						break;

					case SVGScriptReader.InitResults.IncludeDeepOverflow:
						msg += RDLocale.GetText ("VIG_" + svgsr.InitResult.ToString ());
						break;

					case SVGScriptReader.InitResults.FileNotAvailable:
					case SVGScriptReader.InitResults.CannotIncludeFile:
						msg = string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_LoadFailure_Fmt),
							OFName.Text);
						break;

					default:    // f.e., NotInited
						throw new Exception ("Internal error occurred. Debug is required at point 1");
					}

				RDGenerics.MessageBox (RDMessageTypes.Warning_Left, msg);
				return;
				}

			// Запись файла SVG
			IVectorAdapter vectorAdapter;
			switch (SFDialog.FilterIndex)
				{
				// EMF
				case 2:
					vectorAdapter = new EMFAdapter (SFName.Text, (uint)(svgsr.MaxX - svgsr.MinX),
						(uint)(svgsr.MaxY - svgsr.MinY));
					break;

				// SVG
				default:
					vectorAdapter = new SVGAdapter (SFName.Text, (uint)(svgsr.MaxX - svgsr.MinX),
						(uint)(svgsr.MaxY - svgsr.MinY));
					break;
				}
			if (vectorAdapter.InitResult != VectorAdapterInitResults.Opened)
				{
				string msg = RDLocale.GetText ("FileWritingError");

				switch (vectorAdapter.InitResult)
					{
					case VectorAdapterInitResults.CannotCreateFile:
						msg = string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
							SFName.Text);
						break;

					case VectorAdapterInitResults.IncorrectImageSize:
						msg += RDLocale.GetText ("VIG_" + vectorAdapter.InitResult.ToString ());
						break;

					default:    // f.e., Closed, NotInited
						throw new Exception ("Internal error occurred. Debug is required at point 4");
					}

				RDGenerics.MessageBox (RDMessageTypes.Warning_Left, msg);
				return;
				}

			// Кривые
			for (int i = 0; i < svgsr.LinesX.Count; i++)
				{
				vectorAdapter.OpenGroup ();

				for (int j = 0; j < svgsr.LinesX[i].Count - 1; j++)
					{
					vectorAdapter.DrawLine (svgsr.LinesX[i][j], svgsr.LinesY[i][j], svgsr.LinesX[i][j + 1],
						svgsr.LinesY[i][j + 1], svgsr.LinesWidths[i], svgsr.LinesColors[i]);
					}

				vectorAdapter.CloseGroup ();
				}

			// Ось Ox
			if (svgsr.DrawOx)
				{
				// Группировка
				vectorAdapter.OpenGroup ();

				// Ось
				vectorAdapter.DrawLine (svgsr.MinX, svgsr.OxOffset, svgsr.MaxX, svgsr.OxOffset, svgsr.OxWidth,
					svgsr.OxColor);

				// Засечки
				for (int i = 0; i < svgsr.OxNotchesOffsets.Count; i++)
					{
					vectorAdapter.DrawLine (svgsr.OxNotchesOffsets[i], svgsr.OxOffset - svgsr.OxNotchesSizes[i] / 2.0,
						svgsr.OxNotchesOffsets[i], svgsr.OxOffset + svgsr.OxNotchesSizes[i] / 2.0, svgsr.OxWidth,
						svgsr.OxColor);
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
				vectorAdapter.DrawLine (svgsr.OyOffset, svgsr.MinY, svgsr.OyOffset, svgsr.MaxY, svgsr.OyWidth,
					svgsr.OyColor);

				// Засечки
				for (int i = 0; i < svgsr.OyNotchesOffsets.Count; i++)
					{
					vectorAdapter.DrawLine (svgsr.OyOffset - svgsr.OyNotchesSizes[i] / 2.0f, svgsr.OyNotchesOffsets[i],
						svgsr.OyOffset + svgsr.OyNotchesSizes[i] / 2.0f, svgsr.OyNotchesOffsets[i], svgsr.OyWidth,
						svgsr.OyColor);
					}

				// Завершение
				vectorAdapter.CloseGroup ();
				}

			// Текстовые подписи
			for (int i = 0; i < svgsr.Texts.Count; i++)
				{
				vectorAdapter.DrawText (svgsr.TextX[i], svgsr.TextY[i], svgsr.Texts[i], svgsr.TextFonts[i],
					svgsr.TextColors[i]);
				}

			// Исходный скрипт в качестве комментария
			vectorAdapter.AddComment (svgsr.SourceScript);

			// Завершено
			vectorAdapter.CloseFile ();

			if (showSuccessMessage)
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Success_Center, "FileCreated");
			}

		// Сохранение образца сценария
		private void SaveSample_Click (object sender, EventArgs e)
			{
			SSDialog.ShowDialog ();
			}

		private void SSDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Попытка создания файла образца
			FileStream FS = null;
			try
				{
				FS = new FileStream (SSDialog.FileName, FileMode.Create);
				}
			catch
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "CannotCreateSample");
				return;
				}

			if (RDLocale.IsCurrentLanguageRuRu)
				FS.Write (RD_AAOW.Properties.GeomagDataDrawer.Sample_ru_ru, 0,
					RD_AAOW.Properties.GeomagDataDrawer.Sample_ru_ru.Length);
			else
				FS.Write (RD_AAOW.Properties.GeomagDataDrawer.Sample_en_us, 0,
					RD_AAOW.Properties.GeomagDataDrawer.Sample_en_us.Length);

			FS.Close ();
			}

		// Выбор языка интерфейса
		private void LocalizeForm ()
			{
			// Локализация
			OFDialog.Title = OFLabel.Text = RDLocale.GetText ("VIG_OFDialogTitle");
			OFDialog.Filter = SSDialog.Filter = RDLocale.GetText ("VIG_OFDialogFilter");

			SFDialog.Title = SFLabel.Text = RDLocale.GetText ("VIG_SFDialogTitle");
			SFDialog.Filter = RDLocale.GetText ("VIG_SFDialogFilter");

			SSDialog.Title = RDLocale.GetText ("VIG_SSDialogTitle");

			SaveSample.Text = RDLocale.GetText ("VIG_SaveSampleText");
			GenerateImage.Text = RDLocale.GetText ("VIG_GenerateImageText");
			BExit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);
			}
		}
	}
