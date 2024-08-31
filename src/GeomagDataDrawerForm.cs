using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает главную форму программы
	/// </summary>
	public partial class GeomagDataDrawerForm: Form
		{
		// Создание класса-представителя данных диаграммы (фиктивная инициализация)
		private DiagramData dd = new DiagramData ("", DataInputTypes.Unknown, 0);

		// Состояние загрузки значений параметров в контролы
		private bool loading = false, selectionMode = false;

		// Аксессор конфигурации программы (загрузка или инициализация)
		private ConfigAccessor ca = new ConfigAccessor ();

		private MarkersLoader ml = new MarkersLoader ();        // Загрузчик маркеров
		private Graphics drawField;                             // Основное поле отрисовки программы
		private List<int> selectedInidces = new List<int> ();   // Текущий список выбранных кривых
		private bool selecting = false;                         // Состояние загрузки списка выбранных кривых
		private int oldMouseX, oldMouseY;                       // Промежуточные координаты указателя мыши

		#region Настройка и функционирование главной формы

		/// <summary>
		/// Конструктор главной формы программы
		/// </summary>
		/// <param name="SentFileName">Имя файла, полученное от операционной системе при запуске программы</param>
		/// <param name="SentFileType">Предопределённый тип полученного файла</param>
		public GeomagDataDrawerForm (string SentFileName, DataInputTypes SentFileType)
			{
			// Инициализация и локализация формы
			InitializeComponent ();

			// Блокировка формата отображения чисел (для фиксации на NaN, десятичной точке и т. п.;
			// иначе в редакторе невозможно выставить точки разрыва)
			Application.CurrentCulture = RDLocale.GetCulture (RDLanguages.en_us);

			// Настройка списков
			for (int i = 0; i < 4; i++)
				{
				OxPlacementCombo.Items.Add (i.ToString ());
				OyPlacementCombo.Items.Add (i.ToString ());

				if (i < 3)
					{
					OxFormatCombo.Items.Add (i.ToString ());
					OyFormatCombo.Items.Add (i.ToString ());
					}
				if (i < 3)
					{
					LineStyleCombo.Items.Add (i.ToString ());
					}
				}

			MLanguage.Items.AddRange (RDLocale.LanguagesNames);
			try
				{
				MLanguage.SelectedIndex = (int)RDLocale.CurrentLanguage;
				}
			catch
				{
				MLanguage.SelectedIndex = 0;
				}

			// Установка заголовка программы и параметров окна
			this.Text = ProgramDescription.AssemblyTitle;
			this.MinimumSize = new Size ((int)ConfigAccessor.MinWidth, (int)ConfigAccessor.MinHeight);
			RDGenerics.LoadWindowDimensions (this);

			if (!RDGenerics.AppHasAccessRights (false, false))
				this.Text += RDLocale.GetDefaultText (RDLDefaultTexts.Message_LimitedFunctionality);

			// Потеря фокуса полем настройки
			MainTabControl_Leave (MainTabControl, null);

			#region Настройка контролов

			// Диалоги
			StyleFontDialog.MaxSize = (int)DiagramStyle.MaxFontSize;
			StyleFontDialog.MinSize = (int)DiagramStyle.MinFontSize;

			// Общие параметры
			LeftOffset.Minimum = TopOffset.Minimum = 0;
			LeftOffset.Maximum = DiagramStyle.MaxImageWidth;
			TopOffset.Maximum = DiagramStyle.MaxImageHeight;

			loading = true;
			ImageWidth.Minimum = DiagramStyle.MinImageWidth;
			ImageHeight.Minimum = DiagramStyle.MinImageHeight;
			ImageWidth.Maximum = DiagramStyle.MaxImageWidth;
			ImageHeight.Maximum = DiagramStyle.MaxImageHeight;
			loading = false;

			LineName.MaxLength = (int)DiagramStyle.MaxLineNameLength;

			// Настройки осей
			OxPrimaryDiv.Minimum = DiagramStyle.MinPrimaryDivisions;
			OxPrimaryDiv.Maximum = DiagramStyle.MaxPrimaryDivisions;
			OyPrimaryDiv.Minimum = DiagramStyle.MinPrimaryDivisions;
			OyPrimaryDiv.Maximum = DiagramStyle.MaxPrimaryDivisions;
			OxSecondaryDiv.Minimum = DiagramStyle.MinSecondaryDivisions;
			OxSecondaryDiv.Maximum = DiagramStyle.MaxSecondaryDivisions;
			OySecondaryDiv.Minimum = DiagramStyle.MinSecondaryDivisions;
			OySecondaryDiv.Maximum = DiagramStyle.MaxSecondaryDivisions;
			AxesWidth.Minimum = DiagramStyle.MinLineWidth;
			AxesWidth.Maximum = DiagramStyle.MaxLineWidth;

			// Настройки сетки
			GridWidth.Minimum = DiagramStyle.MinLineWidth;
			GridWidth.Maximum = DiagramStyle.MaxLineWidth;

			// Настройки линии кривой
			LineWidth.Minimum = DiagramStyle.MinLineWidth;
			LineWidth.Maximum = DiagramStyle.MaxLineWidth;
			LineMarker.Minimum = 1;
			LineMarker.Maximum = ml.MarkersCount;

			// Настройка эффекта выделения
			MainSelector.Parent = DiagramBox;

			#endregion

			#region Загрузка диаграммы

			if (SentFileType == DataInputTypes.Unspecified)
				{
				// Загрузка стандартного файла данных при старте
				if (ca.ForceUsingBackupDataFile)
					{
					dd = new DiagramData (RDGenerics.AppStartupPath + ConfigAccessor.BackupDataFileName,
						DataInputTypes.GDD, 0);
					}
				}
			else
				{
				// Тестовое открытие файла данных
				DiagramData ddt = null;
				if (SentFileType == DataInputTypes.Unknown)
					ddt = new DiagramData (SentFileName, 2, 0);
				else
					ddt = new DiagramData (SentFileName, SentFileType, 0);

				// Контроль результата (до настоящей загрузки результат BrokenFile допустим по признаку имён столбцов)
				if ((ddt.InitResult != DiagramDataInitResults.Ok) && (ddt.InitResult !=
					DiagramDataInitResults.BrokenFile))
					{
					RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
						DiagramData.GetDataLoadError (ddt.InitResult, SentFileName));
					return;
					}

				// Обновление параметров загрузки
				if (!CheckFileLoadingParameters (SentFileType))
					return;

				// Контрольная загрузка файла, переданного из операционной системы
				if (SentFileType == DataInputTypes.Unknown)
					dd = new DiagramData (SentFileName, ca.ExpectedColumnsCount, ca.SkippedLinesCount);
				else
					dd = new DiagramData (SentFileName, SentFileType, ca.SkippedLinesCount);
				}

			// Контроль результата
			if (dd.InitResult == DiagramDataInitResults.Ok)
				{
				for (int i = 0; i < dd.LinesCount; i++)
					{
					LineNamesList.Items.Add (dd.GetDataColumnName (dd.GetStyle (i).YColumnNumber) + " @ " +
						dd.GetDataColumnName (dd.GetStyle (i).XColumnNumber));
					}
				for (int i = 0; i < dd.AdditionalObjectsCount; i++)
					{
					LineNamesList.Items.Add (dd.GetStyle (i + (int)dd.LinesCount).LineName);
					}
				if (LineNamesList.Items.Count != 0)
					{
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = 0;
					}

				ChangeControlsState (true);

				if (ca.ForceShowDiagram && (SentFileType != DataInputTypes.GDD) &&
					(SentFileType != DataInputTypes.Unspecified))
					{
					AddFirstColumns ();
					}

				Redraw ();
				}
			else if (SentFileType != DataInputTypes.Unspecified)
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
					DiagramData.GetDataLoadError (dd.InitResult, SentFileName));
				}

			#endregion
			}

		// Метод выполняет локализацию формы
		private void LocalizeForm ()
			{
			// Поля настройки
			for (int i = 0; i < MainTabControl.TabPages.Count; i++)
				{
				RDLocale.SetControlsText (MainTabControl.TabPages[i]);
				RDLocale.SetControlsText (MainTabControl.TabPages[i], MainToolTip);
				}

			// Меню программы
			RDLocale.SetControlsText (GeomagDataDrawerFormMenuStrip);
			RDLocale.SetControlsText (MFile);
			MExit.Text = RDLocale.GetDefaultText (RDLDefaultTexts.Button_Exit);

			RDLocale.SetControlsText (MOperations);
			RDLocale.SetControlsText (MUpperHelp);
			RDLocale.SetControlsText (MAdditional);

			// Кнопки управления
			RDLocale.SetControlsText (this, MainToolTip);

			// Панель настроек
			RDLocale.SetControlsText (MainTabControl);

			// Контролы и диалоги
			OFDialog.Filter = string.Format (RDLocale.GetControlText (this.Name, "OFDialog_F"),
				ProgramDescription.AssemblyTitle, ProgramDescription.AppDataExtension);
			OFDialog.Title = RDLocale.GetControlText (this.Name, "OFDialog");
			SFDialog.Filter = string.Format (RDLocale.GetControlText (this.Name, "SFDialog_F"),
				ProgramDescription.AssemblyTitle, ProgramDescription.AppDataExtension);
			SFDialog.Title = RDLocale.GetControlText (this.Name, "SFDialog");
			OpenImageDialog.Filter = RDLocale.GetControlText (this.Name, "OIDialog_F");
			OpenImageDialog.Title = RDLocale.GetControlText (this.Name, "OIDialog");

			LoadStyleDialog.Filter = SaveStyleDialog.Filter = string.Format (RDLocale.GetControlText (this.Name,
				"StyleDialog_F"), ProgramDescription.AppStyleExtension);
			LoadStyleDialog.Title = RDLocale.GetControlText (this.Name, "LoadStyleDialog");
			SaveStyleDialog.Title = RDLocale.GetControlText (this.Name, "SaveStyleDialog");

			MainToolTip.ToolTipTitle = RDLocale.GetControlText (this.Name, "MainToolTip");

			for (int i = 0; i < 4; i++)
				{
				OxPlacementCombo.Items[i] = RDLocale.GetControlText (this.Name, "OxPlacement_" + i.ToString ());
				OyPlacementCombo.Items[i] = RDLocale.GetControlText (this.Name, "OyPlacement_" + i.ToString ());

				if (i < 3)
					{
					OxFormatCombo.Items[i] = RDLocale.GetControlText (this.Name, "OxFormat_" + i.ToString ());
					OyFormatCombo.Items[i] = RDLocale.GetControlText (this.Name, "OxFormat_" + i.ToString ());
					}
				if (i < 3)
					{
					LineStyleCombo.Items[i] = RDLocale.GetControlText (this.Name, "LineStyle_" + i.ToString ());
					}
				}
			}

		// Метод вызывает формы настройки параметров загрузки файлов данных
		// Возвращает false, если была нажата кнопка Cancel
		private bool CheckFileLoadingParameters (DataInputTypes InputType)
			{
			// Обработка случая извлечения данных
			UnknownFileParametersSelector ufps = null;
			if (InputType == DataInputTypes.Unknown)
				{
				ufps = new UnknownFileParametersSelector (ca.ExpectedColumnsCount, false);

				if (ufps.Cancelled)
					{
					return false;
					}

				// Сохранение изменившегося значения
				ca.ExpectedColumnsCount = ufps.DataColumnsCount;
				}

			// Обработка случаев, требующих указания количества строк, используемых для поиска имён столбцов данных
			ColumnsNamesSelector cns = null;
			if (InputType != DataInputTypes.GDD)
				{
				cns = new ColumnsNamesSelector (ca.SkippedLinesCount);

				if (cns.Cancelled)
					return false;

				// Сохранение изменившегося значения
				ca.SkippedLinesCount = cns.SkippedRowsCount;
				}

			return true;
			}

		// Обработка изменения размера окна
		private void MainForm_Resize (object sender, EventArgs e)
			{
			// Изменение размера поля отрисовки
			HorScroll.Width = DiagramBox.Width = this.Width - 55;
			VertScroll.Height = DiagramBox.Height = this.Height - 65 - DiagramBox.Top;

			VertScroll.Left = this.Width - 35;
			HorScroll.Top = this.Height - 45 - HorScroll.Height;

			// Перерисовка
			Redraw ();
			}

		// Сворачивание панелей
		private void MainForm_Click (object sender, EventArgs e)
			{
			MainTabControl_Leave (MainTabControl, null);
			LineNamesList_Leave (LineNamesList, null);
			}

		// Активация/деактивация контролов при старте программы/закрытии диаграммы
		private void ChangeControlsState (bool NewState)
			{
			// Окно программы и меню
			if (DiagramBox.Enabled != NewState)
				{
				AddColumn.Enabled = MAddColumn.Enabled = MAddColumnCmd.Enabled = MMergeLines.Enabled =

				DeleteColumn.Enabled = MReplaceColumn.Enabled = MDeleteColumn.Enabled =

				LineNamesList.Enabled = DrawLine.Enabled =
				DiagramBox.Enabled = HorScroll.Enabled = VertScroll.Enabled =
				MLoadStyle.Enabled = MSaveStyle.Enabled = MResetStyle.Enabled = MSaveTemplate.Enabled =
				MSaveDataFile.Enabled = MSaveDiagramImage.Enabled = MRedactData.Enabled = NewState;

				MRestoreTemplate.Enabled = MReplaceTemplate.Enabled = NewState &&
					RDGenerics.AppHasAccessRights (false, false);
				}

			bool isAnObject = LineNamesList.SelectedIndex >= dd.LinesCount;
			DiagramAdditionalObjects objectType = (isAnObject ? dd.GetObjectType ((uint)LineNamesList.SelectedIndex -
				dd.LinesCount) : DiagramAdditionalObjects.Text);
			bool hasItems = (LineNamesList.Items.Count != 0);

			// Основные
			AutoTextOffset.Enabled = SwitchXY.Enabled = (NewState && !isAnObject && hasItems);

			SwapX.Enabled = SwapY.Enabled = Label01.Enabled = MaxX.Enabled = MinX.Enabled = MaxY.Enabled =
				MinY.Enabled = (NewState && hasItems && !isAnObject);

			Label02.Enabled = Label03.Enabled = Label04.Enabled = Label05.Enabled =
			Label06.Enabled = LeftOffset.Enabled = TopOffset.Enabled = ImageHeight.Enabled = ImageWidth.Enabled =
			AlignAsRow.Enabled = AlignAsColumn.Enabled = (NewState && hasItems);

			// Оси
			OxSecondaryDiv.Enabled = OySecondaryDiv.Enabled = OxPlacementCombo.Enabled = OyPlacementCombo.Enabled =
			AutoDivisions.Enabled = AxesWidth.Enabled = AxesColor.Enabled = AxesColorTurnOff.Enabled =
			OxFormatCombo.Enabled = OyFormatCombo.Enabled = (NewState && !isAnObject && hasItems);

			OxPrimaryDiv.Enabled = OyPrimaryDiv.Enabled = (!AutoDivisions.Checked && hasItems);

			// Шрифты
			AxesFont.Enabled = AxesFontColor.Enabled = (NewState && !isAnObject && hasItems);

			Label27.Enabled = TextFont.Enabled = TextFontColor.Enabled = (NewState && hasItems &&
				(!isAnObject || isAnObject && (objectType == DiagramAdditionalObjects.Text)));

			// Текстовая подпись
			Label07.Enabled = Label08.Enabled = Label09.Enabled = Label10.Enabled =
			OxTextOffset.Enabled = OyTextOffset.Enabled = LineName.Enabled =
			LineNameLeftOffset.Enabled = LineNameTopOffset.Enabled = (!AutoTextOffset.Checked && hasItems);

			if (isAnObject && (objectType == DiagramAdditionalObjects.Text))
				{
				Label09.Enabled = LineName.Enabled = (NewState && hasItems);
				}

			// Сетка
			GridWidth.Enabled = GridPrimaryColor.Enabled = GridSecondaryColor.Enabled =
			GridColorTurnOff.Enabled = (NewState && !isAnObject && hasItems);

			// Линия
			LineMarker.Enabled = (LineStyleCombo.SelectedIndex > 0) && hasItems;

			LineStyleCombo.Enabled = (NewState && !isAnObject && hasItems);

			Label28.Enabled = Label29.Enabled = LineWidth.Enabled = (NewState && hasItems &&
				(!isAnObject || isAnObject &&
				((objectType == DiagramAdditionalObjects.LineH) ||
				(objectType == DiagramAdditionalObjects.LineV) ||
				(objectType == DiagramAdditionalObjects.LineNWtoSE) ||
				(objectType == DiagramAdditionalObjects.LineSWtoNE) ||
				(objectType == DiagramAdditionalObjects.Rectangle) ||
				(objectType == DiagramAdditionalObjects.Ellipse))));

			LineColor.Enabled = (NewState && hasItems && (!isAnObject || isAnObject &&
				((objectType == DiagramAdditionalObjects.LineH) ||
				(objectType == DiagramAdditionalObjects.LineV) ||
				(objectType == DiagramAdditionalObjects.LineNWtoSE) ||
				(objectType == DiagramAdditionalObjects.LineSWtoNE) ||
				(objectType == DiagramAdditionalObjects.Rectangle) ||
				(objectType == DiagramAdditionalObjects.Ellipse) ||
				(objectType == DiagramAdditionalObjects.FilledRectangle) ||
				(objectType == DiagramAdditionalObjects.FilledEllipse))));

			// Лейблы
			Label11.Enabled = Label12.Enabled = Label13.Enabled = Label14.Enabled = Label15.Enabled =
			Label16.Enabled = Label17.Enabled = Label18.Enabled = Label19.Enabled = Label20.Enabled =
			Label21.Enabled = Label22.Enabled = Label23.Enabled = Label24.Enabled = Label25.Enabled =
			Label26.Enabled = Label30.Enabled = Label31.Enabled = (NewState && !isAnObject && hasItems);
			}

		// Перерисовка диаграммы
		private void Redraw ()
			{
			// Контроль на наличие источника изображения
			if (dd == null)
				{
				return;
				}

			// Создание изображения, если оно не было создано
			if (DiagramBox.BackgroundImage != null)
				{
				DiagramBox.BackgroundImage.Dispose ();
				}
			if (drawField != null)
				{
				drawField.Dispose ();
				}
			DiagramBox.BackgroundImage = new Bitmap ((int)dd.DiagramWidth, (int)dd.DiagramHeight);
			drawField = Graphics.FromImage (DiagramBox.BackgroundImage);

			// Пересчёт параметров полос прокрутки
			HorScroll.Maximum = ((dd.DiagramWidth - DiagramBox.Width < 0) ? 0 : (int)(dd.DiagramWidth - DiagramBox.Width));
			VertScroll.Maximum = ((dd.DiagramHeight - DiagramBox.Height < 0) ? 0 : (int)(dd.DiagramHeight - DiagramBox.Height));

			// Перерисовка
			if (dd != null)     // Предотвращение перерисовки при запуске программы
				{
				dd.DrawAllDiagrams (drawField, LineNamesList.SelectedIndices);
				Image im = (Image)DiagramBox.BackgroundImage.Clone ();
				drawField.Clear (DiagramStyle.ImageBackColor);
				drawField.DrawImage (im, -HorScroll.Value, -VertScroll.Value);
				im.Dispose ();
				}
			}

		// Изменение положения бегунков
		private void HorScroll_Scroll (object sender, ScrollEventArgs e)
			{
			if (e.Type == ScrollEventType.EndScroll)
				{
				Redraw ();
				}
			}

		private void VertScroll_Scroll (object sender, ScrollEventArgs e)
			{
			if (e.Type == ScrollEventType.EndScroll)
				{
				Redraw ();
				}
			}

		// Смещение кривой "перетаскиванием" или выделение группы кривых
		private void DiagramBox_MouseUp (object sender, MouseEventArgs e)
			{
			// Выделение диаграмм
			if (selectionMode)
				{
				selectionMode = false;
				MainSelector.Visible = false;

				// Сброс списка
				LineNamesList.SelectedItems.Clear ();

				// Поиск подходящих кривых
				Rectangle r1 = new Rectangle (Math.Min (e.X, oldMouseX) + HorScroll.Value, Math.Min (e.Y, oldMouseY) +
					VertScroll.Value, Math.Max (e.X, oldMouseX) - Math.Min (e.X, oldMouseX),
					Math.Max (e.Y, oldMouseY) - Math.Min (e.Y, oldMouseY));
				for (int i = 0; i < LineNamesList.Items.Count; i++)
					{
					Rectangle r2 = new Rectangle ((int)dd.GetStyle (i).DiagramImageLeftOffset,
						(int)dd.GetStyle (i).DiagramImageTopOffset,
						(int)dd.GetStyle (i).DiagramImageWidth, (int)dd.GetStyle (i).DiagramImageHeight);

					if (r1.IntersectsWith (r2))
						LineNamesList.SelectedIndices.Add (i);
					}

				// Отображение списка
				LineNamesList_MouseClick (sender, e);
				LineNamesList.Select ();
				}

			// Смещение по полю
			else
				{
				// Смещение по горизонтали
				if (HorScroll.Value + oldMouseX - e.X > HorScroll.Maximum)
					{
					HorScroll.Value = HorScroll.Maximum;
					}
				else if (HorScroll.Value + oldMouseX - e.X < HorScroll.Minimum)
					{
					HorScroll.Value = HorScroll.Minimum;
					}
				else
					{
					HorScroll.Value += (oldMouseX - e.X);
					}

				// Смещение по вертикали
				if (VertScroll.Value + oldMouseY - e.Y > VertScroll.Maximum)
					{
					VertScroll.Value = VertScroll.Maximum;
					}
				else if (VertScroll.Value + oldMouseY - e.Y < VertScroll.Minimum)
					{
					VertScroll.Value = VertScroll.Minimum;
					}
				else
					{
					VertScroll.Value += (oldMouseY - e.Y);
					}

				// Перерисовка (не пересекается с заданием положения изображения
				// и случайными нажатиями)
				if ((oldMouseX != e.X) || (oldMouseY != e.Y))
					{
					Redraw ();
					}
				}
			}

		// Изменение эффекта выделения
		private void DiagramBox_MouseMove (object sender, MouseEventArgs e)
			{
			if (selectionMode)
				{
				MainSelector.Left = Math.Min (oldMouseX, e.X);
				MainSelector.Top = Math.Min (oldMouseY, e.Y);
				MainSelector.Width = Math.Max (oldMouseX, e.X) - MainSelector.Left;
				MainSelector.Height = Math.Max (oldMouseY, e.Y) - MainSelector.Top;
				}
			}

		// Задание положения и размера изображения кривой, а также положения его подписей или начало выделения
		private void DiagramBox_MouseDown (object sender, MouseEventArgs e)
			{
			// Потеря фокуса
			MainTabControl_Leave (MainTabControl, null);
			LineNamesList_Leave (LineNamesList, null);

			if (e.Button == MouseButtons.Middle)
				selectionMode = true;

			// Обработка движения по полю диаграммы или выделения кривых
			if (e.Clicks == 1)
				{
				oldMouseX = e.X;
				oldMouseY = e.Y;
				}

			// Эффект выделения, если требуется
			if (selectionMode)
				{
				MainSelector.Visible = true;
				MainSelector.Left = oldMouseX;
				MainSelector.Top = oldMouseY;
				MainSelector.Width = MainSelector.Height = 0;
				}

			// Обработка позиционирования диаграмм
			if (e.Clicks == 2)
				{
				// Смещение подписей осей и диаграммы
				if (Control.ModifierKeys == Keys.Control)
					{
					// Смещение подписи диаграммы
					if (e.Button == MouseButtons.Left)
						{
						if (e.X + HorScroll.Value - LeftOffset.Value > LineNameLeftOffset.Maximum)
							LineNameLeftOffset.Value = LineNameLeftOffset.Maximum;
						else if (e.X + HorScroll.Value - LeftOffset.Value < LineNameLeftOffset.Minimum)
							LineNameLeftOffset.Value = LineNameLeftOffset.Minimum;
						else
							LineNameLeftOffset.Value = e.X + HorScroll.Value - LeftOffset.Value;

						if (e.Y + VertScroll.Value - TopOffset.Value > LineNameTopOffset.Maximum)
							LineNameTopOffset.Value = LineNameTopOffset.Maximum;
						else if (e.Y + VertScroll.Value - TopOffset.Value < LineNameTopOffset.Minimum)
							LineNameTopOffset.Value = LineNameTopOffset.Minimum;
						else
							LineNameTopOffset.Value = e.Y + VertScroll.Value - TopOffset.Value;
						}

					// Смещение подписей осей
					if (e.Button == MouseButtons.Right)
						{
						if (e.X + HorScroll.Value - LeftOffset.Value > OyTextOffset.Maximum)
							OyTextOffset.Value = OyTextOffset.Maximum;
						else if (e.X + HorScroll.Value - LeftOffset.Value < OyTextOffset.Minimum)
							OyTextOffset.Value = OyTextOffset.Minimum;
						else
							OyTextOffset.Value = e.X + HorScroll.Value - LeftOffset.Value;

						if (e.Y + VertScroll.Value - TopOffset.Value > OxTextOffset.Maximum)
							OxTextOffset.Value = OxTextOffset.Maximum;
						else if (e.Y + VertScroll.Value - TopOffset.Value < OxTextOffset.Minimum)
							OxTextOffset.Value = OxTextOffset.Minimum;
						else
							OxTextOffset.Value = e.Y + VertScroll.Value - TopOffset.Value;
						}
					}

				// Положение и размер диаграммы
				else
					{
					// Положение
					if (e.Button == MouseButtons.Left)
						{
						if (e.X + HorScroll.Value > LeftOffset.Maximum)
							LeftOffset.Value = LeftOffset.Maximum;
						else if (e.X + HorScroll.Value < LeftOffset.Minimum)
							LeftOffset.Value = LeftOffset.Minimum;
						else
							LeftOffset.Value = e.X + HorScroll.Value;

						if (e.Y + VertScroll.Value > TopOffset.Maximum)
							TopOffset.Value = TopOffset.Maximum;
						else if (e.Y + VertScroll.Value < TopOffset.Minimum)
							TopOffset.Value = TopOffset.Minimum;
						else
							TopOffset.Value = e.Y + VertScroll.Value;
						}

					// Размер
					if (e.Button == MouseButtons.Right)
						{
						if (e.X + HorScroll.Value - LeftOffset.Value > ImageWidth.Maximum)
							ImageWidth.Value = ImageWidth.Maximum;
						else if (e.X + HorScroll.Value - LeftOffset.Value < ImageWidth.Minimum)
							ImageWidth.Value = ImageWidth.Minimum;
						else
							ImageWidth.Value = e.X + HorScroll.Value - LeftOffset.Value;

						if (e.Y + VertScroll.Value - TopOffset.Value > ImageHeight.Maximum)
							ImageHeight.Value = ImageHeight.Maximum;
						else if (e.Y + VertScroll.Value - TopOffset.Value < ImageHeight.Minimum)
							ImageHeight.Value = ImageHeight.Minimum;
						else
							ImageHeight.Value = e.Y + VertScroll.Value - TopOffset.Value;
						}
					}
				}
			}

		// Завершение работы
		private void MainForm_FormClosing (object sender, FormClosingEventArgs e)
			{
			// Перезапись автосохраняемого файла данных
			if (ca.ForceUsingBackupDataFile && (dd != null) && (dd.InitResult == DiagramDataInitResults.Ok))
				{
				// Возвращаемый результат не имеет значения
				dd.SaveDataFile (RDGenerics.AppStartupPath + ConfigAccessor.BackupDataFileName,
					DataOutputTypes.GDD, true);
				}

			// Подтверждение
			if (ca.ForceExitConfirmation ||
				!ca.ForceUsingBackupDataFile && (dd != null) && (dd.InitResult == DiagramDataInitResults.Ok))
				{
				if (RDGenerics.LocalizedMessageBox (ca.ForceUsingBackupDataFile ? RDMessageTypes.Question_Center :
					RDMessageTypes.Warning_Center, ca.ForceUsingBackupDataFile ? "ApplicationExit" :
					"ApplicationExitNoBackup", RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) ==
					RDMessageButtons.ButtonTwo)
					{
					e.Cancel = true;    // Отмена закрытия окна
					}
				}

			// Сохранение конфигурации
			RDGenerics.SaveWindowDimensions (this);
			}

		#endregion

		#region Меню программы «Файл»

		// Выбор файла данных
		private void MOpenDataFile_Click (object sender, EventArgs e)
			{
			// Защита
			if ((dd != null) && (dd.InitResult == DiagramDataInitResults.Ok) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "AbortChanges",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne))
				return;

			OFDialog.FileName = "";
			OFDialog.ShowDialog ();
			}

		// Файл выбран
		private void OFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Тестовое открытие файла данных
			DiagramData ddt = null;
			if (OFDialog.FilterIndex == (int)DataInputTypes.Unknown)
				ddt = new DiagramData (OFDialog.FileName, 2, 0);
			else
				ddt = new DiagramData (OFDialog.FileName, (DataInputTypes)OFDialog.FilterIndex, 0);

			// Контроль результата (до настоящей загрузки результат BrokenFile допустим по признаку имён столбцов)
			if ((ddt.InitResult != DiagramDataInitResults.Ok) && (ddt.InitResult != DiagramDataInitResults.BrokenFile))
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
					DiagramData.GetDataLoadError (ddt.InitResult, OFDialog.FileName));
				return;
				}

			// Обновление параметров загрузки файлов
			if (!CheckFileLoadingParameters ((DataInputTypes)OFDialog.FilterIndex))
				return;

			// Контрольное открытие
			if (OFDialog.FilterIndex == (int)DataInputTypes.Unknown)
				dd = new DiagramData (OFDialog.FileName, ca.ExpectedColumnsCount, ca.SkippedLinesCount);
			else
				dd = new DiagramData (OFDialog.FileName, (DataInputTypes)OFDialog.FilterIndex, ca.SkippedLinesCount);

			if (dd.InitResult != DiagramDataInitResults.Ok)
				{
				// Файл точно с ошибками, или выбрано некорректное количество строк для поиска имён
				RDGenerics.MessageBox (RDMessageTypes.Warning_Left,
					DiagramData.GetDataLoadError (dd.InitResult, OFDialog.FileName));
				return;
				}

			// Сброс списка кривых
			LineNamesList.Items.Clear ();

			// Загрузка
			if (OFDialog.FilterIndex == (int)DataInputTypes.Unknown)
				{
				if (ca.ForceShowDiagram)
					AddFirstColumns ();
				}
			else
				{
				// GDD
				if (OFDialog.FilterIndex == (int)DataInputTypes.GDD)
					{
					for (int i = 0; i < dd.LinesCount; i++)
						{
						LineNamesList.Items.Add (dd.GetDataColumnName (dd.GetStyle (i).YColumnNumber) + " @ " +
							dd.GetDataColumnName (dd.GetStyle (i).XColumnNumber));
						}
					for (int i = 0; i < dd.AdditionalObjectsCount; i++)
						{
						LineNamesList.Items.Add (dd.GetStyle (i + (int)dd.LinesCount).LineName);
						}
					if (LineNamesList.Items.Count != 0)
						{
						LineNamesList.SelectedItems.Clear ();
						LineNamesList.SelectedIndex = 0;
						}
					}

				// Остальные
				else
					{
					if (ca.ForceShowDiagram)
						AddFirstColumns ();
					}
				}

			// Включение заблокированных контролов
			ChangeControlsState (true);

			// Перерисовка
			Redraw ();
			}

		// Метод добавляет (по возможности) первые столбцы на диаграмму
		private void AddFirstColumns ()
			{
			// Получение параметров
			ColumnsAdderCmd cad = new ColumnsAdderCmd (dd.DataColumnsCount, true);
			if (!cad.LoadParametersFile (RDGenerics.AppStartupPath + ConfigAccessor.LineParametersFileName))
				{
				if (!cad.CreateParametersFile (RDGenerics.AppStartupPath + ConfigAccessor.LineParametersFileName))
					return;

				cad.LoadParametersFile (RDGenerics.AppStartupPath + ConfigAccessor.LineParametersFileName);
				}

			// Контроль
			if (cad.XColumnNumber.Count == 0)
				return;

			// Добавление
			for (int i = 0; i < cad.XColumnNumber.Count; i++)
				{
				int res = dd.AddDiagram (cad.XColumnNumber[i], cad.YColumnNumber[i]);
				if ((res < 0) && (res != -3))
					{
					continue;
					}
				else if (res == -3)
					{
					RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
						string.Format (RDLocale.GetText ("LinesOverloadError"), DiagramData.MaxLines));
					break;
					}

				// Обновление списка названий кривых
				LineNamesList.Items.Add (dd.GetDataColumnName (cad.YColumnNumber[i]) + " @ " +
					dd.GetDataColumnName (cad.XColumnNumber[i]));
				LineNamesList.SelectedItems.Clear ();
				LineNamesList.SelectedIndex = LineNamesList.Items.Count - 1;

				// Установка параметров
				ImageWidth.Value = cad.ImageWidth[i];
				ImageHeight.Value = cad.ImageHeight[i];
				LeftOffset.Value = cad.ImageLeft[i];
				TopOffset.Value = cad.ImageTop[i];

				if (!(AutoTextOffset.Checked = cad.AutoNameOffset[i]))
					{
					LineName.Text = cad.LineName[i];
					LineNameLeftOffset.Value = cad.LineNameLeftOffset[i];
					LineNameTopOffset.Value = cad.LineNameTopOffset[i];
					}
				}
			}

		// Сохранение файла данных
		private void MSaveDataFile_Click (object sender, EventArgs e)
			{
			SFDialog.FileName = "";
			SFDialog.ShowDialog ();
			}

		private void SFDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Сохранение
			if (dd.SaveDataFile (SFDialog.FileName, (DataOutputTypes)SFDialog.FilterIndex,
				ca.ForceSavingColumnNames) < 0)
				{
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					SFDialog.FileName));
				}
			}

		// Сохранение изображения
		private void MSaveDiagramImage_Click (object sender, EventArgs e)
			{
			SavePicture sp = new SavePicture (dd, false);
			}

		// Настройки программы
		private void MProgramSettings_Click (object sender, EventArgs e)
			{
			// Выполнение настройки
			ProgramSettings ps = new ProgramSettings ();
			ps.Dispose ();

			// Перезагрузка параметров
			ca = new ConfigAccessor ();
			}

		// Выход из программы
		private void MExit_Click (object sender, EventArgs e)
			{
			this.Close ();
			}

		// Генерация кривой
		private void MGenerate_Click (object sender, EventArgs e)
			{
			// Защита
			if ((dd != null) && (dd.InitResult == DiagramDataInitResults.Ok) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "AbortChanges",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne))
				return;

			// Генерация данных по формуле
			FormulaEvaluator fe = new FormulaEvaluator ();
			if (fe.Cancelled)
				{
				fe.Dispose ();
				return;
				}

			// Создание диаграммы
			dd = new DiagramData (fe.X, fe.Y, fe.ColumnsNames);
			fe.Dispose ();

			if (dd.InitResult != DiagramDataInitResults.Ok)
				throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (2)");

			// Сброс списка кривых
			LineNamesList.Items.Clear ();

			// Включение заблокированных контролов
			ChangeControlsState (true);

			// Загрузка
			if (ca.ForceShowDiagram)
				AddFirstColumns ();

			// Перерисовка
			Redraw ();
			}

		// Закрытие диаграммы
		private void MClose_Click (object sender, EventArgs e)
			{
			// Защита
			if ((dd != null) && (dd.InitResult == DiagramDataInitResults.Ok) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "AbortChanges",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne))
				return;

			// Снятие инициализации и блокировка контролов
			if (dd.InitResult != DiagramDataInitResults.Ok)
				return;

			dd = new DiagramData ("", DataInputTypes.Unknown, 0);
			LineNamesList.Items.Clear ();
			ChangeControlsState (false);
			Redraw ();
			}

		// Загрузка из буфера обмена
		private void MLoadFromClipboard_Click (object sender, EventArgs e)
			{
			// Защита
			if ((dd != null) && (dd.InitResult == DiagramDataInitResults.Ok) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "AbortChanges",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne))
				return;

			// Обновление параметров загрузки файлов
			if (!CheckFileLoadingParameters (DataInputTypes.Unknown))
				return;

			// Контрольное открытие
			DiagramData ddt = new DiagramData (ca.ExpectedColumnsCount, ca.SkippedLinesCount);
			if (ddt.InitResult != DiagramDataInitResults.Ok)
				{
				// Файл точно с ошибками, или выбрано некорректное количество строк для поиска имён
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "ClipboardLoadError");
				return;
				}

			// Создание диаграммы
			dd = new DiagramData (ca.ExpectedColumnsCount, ca.SkippedLinesCount);

			// Сброс списка кривых
			LineNamesList.Items.Clear ();

			// Включение заблокированных контролов
			ChangeControlsState (true);

			// Загрузка
			if (ca.ForceShowDiagram)
				AddFirstColumns ();

			// Перерисовка
			Redraw ();
			}

		// Генерация кривой
		private void MProcessImage_Click (object sender, EventArgs e)
			{
			// Защита
			if ((dd != null) && (dd.InitResult == DiagramDataInitResults.Ok) &&
				(RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "AbortChanges",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne))
				return;

			// Получение изображения
			if (OpenImageDialog.ShowDialog () != DialogResult.OK)
				return;

			Bitmap b;
			try
				{
				b = (Bitmap)Image.FromFile (OpenImageDialog.FileName);
				}
			catch
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "NotAnImageError");
				return;
				}

			// Генерация данных по формуле
			ImageProcessor ip = new ImageProcessor (b);
			if (ip.Cancelled)
				{
				ip.Dispose ();
				return;
				}

			// Создание диаграммы
			dd = new DiagramData (ip.X, ip.Y, ip.ColumnsNames);
			ip.Dispose ();

			if (dd.InitResult != DiagramDataInitResults.Ok)
				throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (3)");

			// Сброс списка кривых
			LineNamesList.Items.Clear ();

			// Включение заблокированных контролов
			ChangeControlsState (true);

			// Загрузка
			if (ca.ForceShowDiagram)
				AddFirstColumns ();

			// Перерисовка
			Redraw ();
			}

		#endregion

		#region Меню программы «Справка»

		// О программе
		private void MAbout_Click (object sender, EventArgs e)
			{
			RDGenerics.ShowAbout (false);
			}

		// Изменение языка интерфейса
		private void MLanguage_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Обновление конфигурации
			RDLocale.CurrentLanguage = (RDLanguages)MLanguage.SelectedIndex;

			// Релокализация формы
			LocalizeForm ();
			}

		#endregion

		#region Меню программы «Операции»

		// Загрузка стиля из файла
		private void MLoadStyle_Click (object sender, EventArgs e)
			{
			// Вызов диалога
			LoadStyleDialog.FileName = "";
			LoadStyleDialog.ShowDialog ();
			}

		private void LoadStyleDialog_FileOk (object sender, CancelEventArgs e)
			{
			// Выбор варианта использования стилей
			switch (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Left, "StyleLoadingType",
				RDLDefaultTexts.Button_Apply, RDLDefaultTexts.Button_Add, RDLDefaultTexts.Button_Cancel))
				{
				// Применение к выделенным кривым
				case RDMessageButtons.ButtonOne:
					// Контроль
					if (LineNamesList.SelectedIndices.Count == 0)
						{
						RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LinesForLoadingStylesNSError");
						return;
						}

					// Загрузка
					if (dd.LoadStyle (LoadStyleDialog.FileName, LineNamesList.SelectedIndices) < 0)
						{
						RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "BrokenStyleError");
						return;
						}
					break;

				// Добавление кривых
				case RDMessageButtons.ButtonTwo:
					// Загрузка
					int oldLinesCount = LineNamesList.Items.Count;

					if (dd.LoadStyle (LoadStyleDialog.FileName) < 0)
						RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "BrokenStyleError");

					// Обновление списка названий кривых (файл стиля заведомо непустой)
					for (int i = oldLinesCount; i < dd.LinesCount; i++)
						{
						LineNamesList.Items.Insert (i, dd.GetDataColumnName (dd.GetStyle (i).YColumnNumber) + " @ " +
							dd.GetDataColumnName (dd.GetStyle (i).XColumnNumber));
						}
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = (int)dd.LinesCount - 1;

					break;

				// Отмена
				case RDMessageButtons.ButtonThree:
					return;
				}

			// Перерисовка
			LineNames_SelectedIndexChanged (LineNamesList, null);
			}

		// Сохранение стиля в файл
		private void MSaveStyle_Click (object sender, EventArgs e)
			{
			// Контроль
			if (LineNamesList.SelectedIndices.Count == 0)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LinesForSavingStylesNSError");
				return;
				}

			// Вызов диалога
			SaveStyleDialog.FileName = "";
			SaveStyleDialog.ShowDialog ();
			}

		private void SaveStyleDialog_FileOk (object sender, CancelEventArgs e)
			{
			if (dd.SaveStyle (SaveStyleDialog.FileName, LineNamesList.SelectedIndices) < 0)
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "StyleSaveError");
			}

		// Сброс стиля диаграммы
		private void MResetStyle_Click (object sender, EventArgs e)
			{
			// Контроль
			if (LineNamesList.SelectedIndices.Count == 0)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LinesForResetStylesNSError");
				return;
				}

			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "StylesReset",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonOne)
				{
				// Сброс
				for (int i = 0; i < LineNamesList.SelectedIndices.Count; i++)
					dd.ResetStyle ((uint)LineNamesList.SelectedIndices[i]);

				// Перерисовка
				LineNames_SelectedIndexChanged (LineNamesList, null);
				}
			}

		// Сохранение шаблона добавления кривых
		private void MSaveTemplate_Click (object sender, EventArgs e)
			{
			// Инициализация
			ColumnsAdderCmd cad = new ColumnsAdderCmd (dd.DataColumnsCount, true);
			switch (cad.SaveParametersFile (dd))
				{
				case -1:
					throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (9)");

				case -2:
					RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
						string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt), "?"));
					break;

				default:
					// Файл сохранён
					// Сохранение отменено
					break;
				}
			}

		// Восстановление шаблона добавления кривых
		private void MReplaceTemplate_Click (object sender, EventArgs e)
			{
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "TemplateReplace",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne)
				return;

			if (!ColumnsAdderCmd.WriteParametersFile (dd, RDGenerics.AppStartupPath +
				ConfigAccessor.LineParametersFileName))
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					ConfigAccessor.LineParametersFileName));
			}

		// Восстановление шаблона добавления кривых
		private void MRestoreTemplate_Click (object sender, EventArgs e)
			{
			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "TemplateReset",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) != RDMessageButtons.ButtonOne)
				return;

			if (!ColumnsAdderCmd.CreateDefaultParametersFile (RDGenerics.AppStartupPath +
				ConfigAccessor.LineParametersFileName))
				RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
					string.Format (RDLocale.GetDefaultText (RDLDefaultTexts.Message_SaveFailure_Fmt),
					ConfigAccessor.LineParametersFileName));
			}

		// Редактирование данных диаграммы
		private void MRedactData_Click (object sender, EventArgs e)
			{
			// Сохранение стилей
			List<DiagramStyle> linesStyles = new List<DiagramStyle> ();
			List<DiagramStyle> objectsStyles = new List<DiagramStyle> ();
			List<DiagramAdditionalObjects> objectsTypes = new List<DiagramAdditionalObjects> ();

			for (int i = 0; i < dd.LinesCount; i++)
				{
				linesStyles.Add (new DiagramStyle (dd.GetStyle (i)));
				}
			for (int i = 0; i < dd.AdditionalObjectsCount; i++)
				{
				objectsTypes.Add (dd.GetObjectType ((uint)i));
				objectsStyles.Add (new DiagramStyle (dd.GetStyle (i + (int)dd.LinesCount)));
				}

			// Запуск
			DiagramDataEditor dde = new DiagramDataEditor (dd);

			// Обработка
			if (!dde.Cancelled)
				{
				// Сброс списка кривых
				LineNamesList.Items.Clear ();

				// Загрузка данных
				dd = new DiagramData (dde.ResultTable);

				// Добавление столбцов и восстановление стилей
				for (int i = 0; i < linesStyles.Count; i++)
					{
					dd.AddDiagram (linesStyles[i].XColumnNumber, linesStyles[i].YColumnNumber);

					dd.LoadStyle (i, linesStyles[i]);

					LineNamesList.Items.Add (dd.GetDataColumnName (linesStyles[i].YColumnNumber) + " @ " +
						dd.GetDataColumnName (linesStyles[i].XColumnNumber));
					}
				for (int i = 0; i < objectsStyles.Count; i++)
					{
					dd.AddObject (objectsTypes[i]);

					dd.LoadStyle (i + (int)dd.LinesCount, objectsStyles[i]);

					LineNamesList.Items.Add (objectsStyles[i].LineName);
					}

				// Обновление состояния
				if (LineNamesList.Items.Count != 0)
					{
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = LineNamesList.Items.Count - 1;

					// Перерисовка
					Redraw ();
					}
				}

			// Обнуление списков
			linesStyles.Clear ();
			objectsTypes.Clear ();
			objectsStyles.Clear ();
			}

		#endregion

		#region Меню программы «Дополнительно»

		// Склеивание таблиц данных
		private void MMergeTables_Click (object sender, EventArgs e)
			{
			// Вызов диалога
			TablesMergerForm tmf = new TablesMergerForm ();
			tmf.Dispose ();
			}

		// Генерация векторного изображения
		private void MGenerateVI_Click (object sender, EventArgs e)
			{
			// Вызов диалога
			SVGGeneratorForm svggf = new SVGGeneratorForm ();
			svggf.Dispose ();
			}

		#endregion

		#region Управление диаграммой

		// Метод добавляет кривую на диаграмму
		private void AddColumn_Click (object sender, EventArgs e)
			{
			// Запуск выбора
			ColumnsAdder cad = new ColumnsAdder (dd);

			if (!cad.Cancelled)
				{
				int res;
				if (cad.IsNewObjectADiagram)
					{
					res = dd.AddDiagram (cad.XColumnNumber, cad.YColumnNumber);
					}
				else
					{
					res = dd.AddObject (cad.AdditionalObjectType);
					}

				if ((res < 0) && (res != -3))
					{
					throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (9)");
					}
				else if (res == -3)
					{
					if (cad.IsNewObjectADiagram)
						{
						RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
							string.Format (RDLocale.GetText ("LinesOverloadError"),
							DiagramData.MaxLines));
						}
					else
						{
						RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
							string.Format (RDLocale.GetText ("ObjectsOverloadError"),
							DiagramData.MaxAdditionalObjects));
						}
					return;
					}

				// Обновление списка названий кривых
				if (cad.IsNewObjectADiagram)
					{
					LineNamesList.Items.Insert ((int)dd.LinesCount - 1, dd.GetDataColumnName (cad.YColumnNumber) +
						" @ " + dd.GetDataColumnName (cad.XColumnNumber));
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = (int)dd.LinesCount - 1;
					}
				else
					{
					LineNamesList.Items.Add (dd.GetStyle ((int)dd.LinesCount +
						(int)dd.AdditionalObjectsCount - 1).LineName);
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = LineNamesList.Items.Count - 1;
					}

				// Перерисовка
				Redraw ();
				}
			}

		// Метод удаляет кривую с диаграммы
		private void DeleteColumn_Click (object sender, EventArgs e)
			{
			// Контроль
			if (LineNamesList.SelectedIndices.Count == 0)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LinesForDeleteNSError");
				return;
				}

			if (RDGenerics.LocalizedMessageBox (RDMessageTypes.Question_Center, "LinesDelete",
				RDLDefaultTexts.Button_YesNoFocus, RDLDefaultTexts.Button_No) == RDMessageButtons.ButtonTwo)
				{
				return;
				}

			// Формирование массива удаляемых индексов
			List<int> lineIndices = new List<int> ();
			List<int> objectIndices = new List<int> ();
			for (int i = 0; i < LineNamesList.SelectedIndices.Count; i++)
				{
				if (LineNamesList.SelectedIndices[i] < dd.LinesCount)
					lineIndices.Add (LineNamesList.SelectedIndices[i]);
				else
					objectIndices.Add (LineNamesList.SelectedIndices[i] - (int)dd.LinesCount);
				}

			// Удаление
			int offset = 0;
			for (int i = 0; i < lineIndices.Count; i++)
				{
				// Удаление диаграммы
				dd.DeleteDiagram ((uint)(lineIndices[i] - offset));

				// Удаление имени
				LineNamesList.Items.RemoveAt (lineIndices[i] - offset);

				// Увеличение смещения по списку
				offset++;
				}

			offset = 0;
			for (int i = 0; i < objectIndices.Count; i++)
				{
				// Удаление диаграммы
				dd.DeleteObject ((uint)(objectIndices[i] - offset));

				// Удаление имени
				LineNamesList.Items.RemoveAt (objectIndices[i] - offset + (int)dd.LinesCount);

				// Увеличение смещения по списку
				offset++;
				}

			// Перерисовка
			Redraw ();

			// Блокировка кнопки удаления, если необходимо
			if (LineNamesList.Items.Count != 0)
				{
				LineNamesList.SelectedItems.Clear ();
				LineNamesList.SelectedIndex = 0;
				}
			}

		// Метод добавляет кривую на диаграмму с помощью строки параметров
		private void AddColumnCmd_Click (object sender, EventArgs e)
			{
			// Запуск выбора
			ColumnsAdderCmd cac = new ColumnsAdderCmd (dd.DataColumnsCount, false);

			if (!cac.Cancelled)
				{
				// Добавление
				for (int i = 0; i < cac.AutoNameOffset.Count; i++)
					{
					int res = dd.AddDiagram (cac.XColumnNumber[i], cac.YColumnNumber[i]);
					if ((res < 0) && (res != -3))
						{
						throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (10)");
						}
					else if (res == -3)
						{
						RDGenerics.MessageBox (RDMessageTypes.Warning_Center,
							string.Format (RDLocale.GetText ("LinesOverloadError"),
							DiagramData.MaxLines));
						break;
						}

					// Обновление списка названий кривых
					LineNamesList.Items.Insert ((int)dd.LinesCount - 1,
						dd.GetDataColumnName (cac.YColumnNumber[i]) + " @ " +
						dd.GetDataColumnName (cac.XColumnNumber[i]));
					LineNamesList.SelectedItems.Clear ();
					LineNamesList.SelectedIndex = (int)dd.LinesCount - 1;

					// Установка параметров
					ImageWidth.Value = cac.ImageWidth[i];
					ImageHeight.Value = cac.ImageHeight[i];
					LeftOffset.Value = cac.ImageLeft[i];
					TopOffset.Value = cac.ImageTop[i];

					if (!cac.AutoNameOffset[i])
						{
						AutoTextOffset.Checked = cac.AutoNameOffset[i];
						LineName.Text = cac.LineName[i];
						LineNameLeftOffset.Value = cac.LineNameLeftOffset[i];
						LineNameTopOffset.Value = cac.LineNameTopOffset[i];
						}
					}

				// Перерисовка
				Redraw ();
				}
			}

		// Загрузка значений в остальные поля
		private void LineNames_SelectedIndexChanged (object sender, EventArgs e)
			{
			// Отмена операции, если поле для настройки не выбрано
			if (LineNamesList.SelectedIndices.Count == 0)
				return;

			// Блокировка обработчиков событий _ChangeValue для всех контролов
			loading = true;

			// Защита от незапланированных границ
			try
				{
				// Установка ограничений
				MaxX.DecimalPlaces = MinX.DecimalPlaces = (dd.XRoundPos ((uint)LineNamesList.SelectedIndex) < 2) ?
					(-dd.XRoundPos ((uint)LineNamesList.SelectedIndex) + 2) : 0;
				MaxX.Increment = MinX.Increment = (decimal)Math.Pow (10.0, -(double)MaxX.DecimalPlaces);
				MaxY.DecimalPlaces = MinY.DecimalPlaces = (dd.YRoundPos ((uint)LineNamesList.SelectedIndex) < 2) ?
					(-dd.YRoundPos ((uint)LineNamesList.SelectedIndex) + 2) : 0;
				MaxY.Increment = MinY.Increment = (decimal)Math.Pow (10.0, -(double)MaxY.DecimalPlaces);

				// Загрузка значений в контролы
				DrawLine.Checked = dd.GetStyle (LineNamesList.SelectedIndex).AllowDrawing;

				try
					{
					MinX.Value = (decimal)dd.GetStyle (LineNamesList.SelectedIndex).MinX;
					MaxX.Value = (decimal)dd.GetStyle (LineNamesList.SelectedIndex).MaxX;
					MinY.Value = (decimal)dd.GetStyle (LineNamesList.SelectedIndex).MinY;
					MaxY.Value = (decimal)dd.GetStyle (LineNamesList.SelectedIndex).MaxY;
					}
				catch
					{
					}

				LeftOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).DiagramImageLeftOffset;
				TopOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).DiagramImageTopOffset;

				ImageWidth.Value = dd.GetStyle (LineNamesList.SelectedIndex).DiagramImageWidth;
				ImageHeight.Value = dd.GetStyle (LineNamesList.SelectedIndex).DiagramImageHeight;

				SwitchXY.Checked = dd.GetStyle (LineNamesList.SelectedIndex).SwitchXY;

				AutoTextOffset.Checked = dd.GetStyle (LineNamesList.SelectedIndex).AutoTextOffset;
				OxTextOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).OxTextOffset;
				OyTextOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).OyTextOffset;
				LineName.Text = dd.GetStyle (LineNamesList.SelectedIndex).LineName;
				LineNameLeftOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).LineNameLeftOffset;
				LineNameTopOffset.Value = dd.GetStyle (LineNamesList.SelectedIndex).LineNameTopOffset;

				// Оси
				AutoDivisions.Checked = dd.GetStyle (LineNamesList.SelectedIndex).AutoPrimaryDivisions;
				OxPrimaryDiv.Value = dd.GetStyle (LineNamesList.SelectedIndex).XPrimaryDivisions;
				OyPrimaryDiv.Value = dd.GetStyle (LineNamesList.SelectedIndex).YPrimaryDivisions;
				OxSecondaryDiv.Value = dd.GetStyle (LineNamesList.SelectedIndex).XSecondaryDivisions;
				OySecondaryDiv.Value = dd.GetStyle (LineNamesList.SelectedIndex).YSecondaryDivisions;
				AxesWidth.Value = dd.GetStyle (LineNamesList.SelectedIndex).AxesLinesWidth;
				AxesColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).AxesColor;
				OxPlacementCombo.SelectedIndex = (int)dd.GetStyle (LineNamesList.SelectedIndex).OxPlacement;
				OyPlacementCombo.SelectedIndex = (int)dd.GetStyle (LineNamesList.SelectedIndex).OyPlacement;
				OxFormatCombo.SelectedIndex = (int)dd.GetStyle (LineNamesList.SelectedIndex).OxFormat;
				OyFormatCombo.SelectedIndex = (int)dd.GetStyle (LineNamesList.SelectedIndex).OyFormat;

				// Шрифты
				AxesFont.Font = dd.GetStyle (LineNamesList.SelectedIndex).AxesFont;
				AxesFontColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).AxesFontColor;
				TextFont.Font = dd.GetStyle (LineNamesList.SelectedIndex).TextFont;
				TextFontColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).TextFontColor;

				// Сетка
				GridWidth.Value = dd.GetStyle (LineNamesList.SelectedIndex).GridLinesWidth;
				GridPrimaryColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).PrimaryGridColor;
				GridSecondaryColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).SecondaryGridColor;

				// Линия
				LineWidth.Value = dd.GetStyle (LineNamesList.SelectedIndex).LineWidth;
				LineColor.BackColor = dd.GetStyle (LineNamesList.SelectedIndex).LineColor;
				LineStyleCombo.SelectedIndex = (int)dd.GetStyle (LineNamesList.SelectedIndex).LineDrawingFormat;
				LineMarker.Value = ((dd.GetStyle (LineNamesList.SelectedIndex).LineMarkerNumber > ml.MarkersCount) ?
					1 : dd.GetStyle (LineNamesList.SelectedIndex).LineMarkerNumber);
				}
			catch
				{
				throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (12)");
				}

			// Обновление ограничений
			ImageWidth_ValueChanged (ImageWidth, null);
			ImageHeight_ValueChanged (ImageHeight, null);

			// Обновление состояния контролов
			ChangeControlsState (true);

			// Возврат состояния
			loading = false;

			if (!selecting)
				{
				// Сохранение индексов выбранных кривых
				selectedInidces.Clear ();
				for (int i = 0; i < LineNamesList.SelectedIndices.Count; i++)
					selectedInidces.Add (LineNamesList.SelectedIndices[i]);

				// Перерисовка (необходима для отображения выделения кривых)
				Redraw ();
				}
			}

		// Совмещение кривых
		private void MergeLines_Click (object sender, EventArgs e)
			{
			// Контроль
			if (LineNamesList.SelectedIndices.Count != 2)
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "MergingError");
				return;
				}

			// Запрос параметров совмещения
			DiagramMerger dm = new DiagramMerger (LineNamesList.SelectedItems[0].ToString (),
				LineNamesList.SelectedItems[1].ToString ());
			if (dm.Cancelled)
				return;

			// Обработка варианта совмещения
			int fromIdx, toIdx;
			if (dm.MergeVariant == DiagramMerger.MergeVariants.FirstLine)
				{
				fromIdx = LineNamesList.SelectedIndices[0];
				toIdx = LineNamesList.SelectedIndices[1];
				}
			else
				{
				fromIdx = LineNamesList.SelectedIndices[1];
				toIdx = LineNamesList.SelectedIndices[0];
				}

			dd.GetStyle (toIdx).DiagramImageLeftOffset = dd.GetStyle (fromIdx).DiagramImageLeftOffset;
			dd.GetStyle (toIdx).DiagramImageTopOffset = dd.GetStyle (fromIdx).DiagramImageTopOffset;
			dd.GetStyle (toIdx).DiagramImageWidth = dd.GetStyle (fromIdx).DiagramImageWidth;
			dd.GetStyle (toIdx).DiagramImageHeight = dd.GetStyle (fromIdx).DiagramImageHeight;

			// Приведение к общему виду
			dd.GetStyle (toIdx).SwitchXY = dd.GetStyle (fromIdx).SwitchXY;
			dd.GetStyle (toIdx).AutoTextOffset = dd.GetStyle (fromIdx).AutoTextOffset = true;
			dd.GetStyle (toIdx).AutoPrimaryDivisions = dd.GetStyle (fromIdx).AutoPrimaryDivisions = false;
			dd.GetStyle (toIdx).AxesColor = dd.GetStyle (fromIdx).AxesColor;
			dd.GetStyle (toIdx).AxesFont = dd.GetStyle (fromIdx).AxesFont;
			dd.GetStyle (toIdx).AxesLinesWidth = dd.GetStyle (fromIdx).AxesLinesWidth;

			// Обработка осей
			bool ox = (dm.MergeAxe == DiagramMerger.MergeAxes.Ox);
			bool oy = (dm.MergeAxe == DiagramMerger.MergeAxes.Oy);
			if (dm.MergeAxe == DiagramMerger.MergeAxes.Both)
				ox = oy = true;

			if (ox)
				{
				dd.GetStyle (toIdx).OxFormat = dd.GetStyle (fromIdx).OxFormat;
				dd.GetStyle (toIdx).OxPlacement = dd.GetStyle (fromIdx).OxPlacement =
					AxesPlacements.RightBottom;
				dd.GetStyle (toIdx).XPrimaryDivisions = dd.GetStyle (fromIdx).XPrimaryDivisions;
				dd.GetStyle (toIdx).XSecondaryDivisions = dd.GetStyle (fromIdx).XSecondaryDivisions;

				dd.GetStyle (toIdx).MinX = dd.GetStyle (fromIdx).MinX =
					Math.Min (dd.GetStyle (toIdx).MinX, dd.GetStyle (fromIdx).MinX);
				dd.GetStyle (toIdx).MaxX = dd.GetStyle (fromIdx).MaxX =
					Math.Max (dd.GetStyle (toIdx).MaxX, dd.GetStyle (fromIdx).MaxX);
				}
			else
				{
				dd.GetStyle (toIdx).OxPlacement = AxesPlacements.LeftTop;
				dd.GetStyle (fromIdx).OxPlacement = AxesPlacements.RightBottom;
				}

			if (oy)
				{
				dd.GetStyle (toIdx).OyFormat = dd.GetStyle (fromIdx).OyFormat;
				dd.GetStyle (toIdx).OyPlacement = dd.GetStyle (fromIdx).OyPlacement =
					AxesPlacements.LeftTop;
				dd.GetStyle (toIdx).YPrimaryDivisions = dd.GetStyle (fromIdx).YPrimaryDivisions;
				dd.GetStyle (toIdx).YSecondaryDivisions = dd.GetStyle (fromIdx).YSecondaryDivisions;

				dd.GetStyle (toIdx).MinY = dd.GetStyle (fromIdx).MinY =
					Math.Min (dd.GetStyle (toIdx).MinY, dd.GetStyle (fromIdx).MinY);
				dd.GetStyle (toIdx).MaxY = dd.GetStyle (fromIdx).MaxY =
					Math.Max (dd.GetStyle (toIdx).MaxY, dd.GetStyle (fromIdx).MaxY);
				}
			else
				{
				dd.GetStyle (toIdx).OyPlacement = AxesPlacements.LeftTop;
				dd.GetStyle (fromIdx).OyPlacement = AxesPlacements.RightBottom;
				}

			// Обработка окраски
			if (dm.PaintAxes)
				{
				dd.GetStyle (toIdx).AxesFontColor = dd.GetStyle (toIdx).TextFontColor =
					dd.GetStyle (toIdx).LineColor;
				dd.GetStyle (fromIdx).AxesFontColor = dd.GetStyle (fromIdx).TextFontColor =
					dd.GetStyle (fromIdx).LineColor;
				}

			// Перерисовка
			LineNames_SelectedIndexChanged (LineNamesList, null);
			}

		// Замена данных кривой
		private void ReplaceColumn_Click (object sender, EventArgs e)
			{
			// Контроль
			if ((LineNamesList.SelectedIndices.Count == 0) || (LineNamesList.SelectedIndex >= dd.LinesCount))
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LineForReplaceNSError");
				return;
				}

			// Запуск выбора
			ColumnsAdder cad = new ColumnsAdder (dd, (int)dd.GetStyle (LineNamesList.SelectedIndex).XColumnNumber,
				(int)dd.GetStyle (LineNamesList.SelectedIndex).YColumnNumber);

			if (!cad.Cancelled)
				{
				int res = dd.ReplaceDiagram ((uint)LineNamesList.SelectedIndex, cad.XColumnNumber, cad.YColumnNumber);
				if (res < 0)
					{
					throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (14)");
					}

				// Обновление списка названий кривых
				LineNamesList.Items[LineNamesList.SelectedIndex] = dd.GetDataColumnName (cad.YColumnNumber) +
					" @ " + dd.GetDataColumnName (cad.XColumnNumber);

				// Перерисовка
				Redraw ();
				}
			}

		#endregion

		#region Настройка диаграммы

		// Общий метод обновления значений параметров
		private void UpdateDiagramParameters (Control Sender)
			{
			// Блокировка обработки в случае, если программа находится в состоянии выбора кривой для настройки
			if (loading)
				return;

			// Обновление значений
			try
				{
				for (int i = 0; i < LineNamesList.SelectedIndices.Count; i++)
					{
					// Объединяемые параметры
					switch (Sender.Name)
						{
						// Общие параметры
						case "MinX":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).MinX = (double)MinX.Value;
							break;

						case "MaxX":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).MaxX = (double)MaxX.Value;
							break;

						case "MinY":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).MinY = (double)MinY.Value;
							break;

						case "MaxY":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).MaxY = (double)MaxY.Value;
							break;

						case "LeftOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).DiagramImageLeftOffset = (uint)LeftOffset.Value;
							break;

						case "TopOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).DiagramImageTopOffset = (uint)TopOffset.Value;
							break;

						case "ImageWidth":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).DiagramImageWidth = (uint)ImageWidth.Value;
							break;

						case "ImageHeight":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).DiagramImageHeight = (uint)ImageHeight.Value;
							break;

						case "SwitchXY":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).SwitchXY = SwitchXY.Checked;
							break;

						case "DrawLine":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AllowDrawing = DrawLine.Checked;
							break;

						// Оси
						case "AutoDivisions":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AutoPrimaryDivisions = AutoDivisions.Checked;
							break;

						case "OxPrimaryDiv":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).XPrimaryDivisions = (uint)OxPrimaryDiv.Value;
							break;

						case "OyPrimaryDiv":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).YPrimaryDivisions = (uint)OyPrimaryDiv.Value;
							break;

						case "OxSecondaryDiv":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).XSecondaryDivisions = (uint)OxSecondaryDiv.Value;
							break;

						case "OySecondaryDiv":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).YSecondaryDivisions = (uint)OySecondaryDiv.Value;
							break;

						case "AxesWidth":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AxesLinesWidth = (uint)AxesWidth.Value;
							break;

						case "AxesColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AxesColor = AxesColor.BackColor;
							break;

						case "OxPlacementCombo":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OxPlacement =
								(AxesPlacements)OxPlacementCombo.SelectedIndex;
							break;

						case "OyPlacementCombo":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OyPlacement =
								(AxesPlacements)OyPlacementCombo.SelectedIndex;
							break;

						case "OxFormatCombo":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OxFormat =
								(NumbersFormat)OxFormatCombo.SelectedIndex;
							break;

						case "OyFormatCombo":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OyFormat =
								(NumbersFormat)OyFormatCombo.SelectedIndex;
							break;

						// Шрифты
						case "AxesFont":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AxesFont = AxesFont.Font;
							break;

						case "AxesFontColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AxesFontColor = AxesFontColor.BackColor;
							break;

						case "TextFont":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).TextFont = TextFont.Font;
							break;

						case "TextFontColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).TextFontColor = TextFontColor.BackColor;
							break;

						// Сетка
						case "GridWidth":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).GridLinesWidth = (uint)GridWidth.Value;
							break;

						case "GridPrimaryColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).PrimaryGridColor = GridPrimaryColor.BackColor;
							break;

						case "GridSecondaryColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).SecondaryGridColor = GridSecondaryColor.BackColor;
							break;

						// Линия
						case "LineWidth":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineWidth = (uint)LineWidth.Value;
							break;

						case "LineColor":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineColor = LineColor.BackColor;
							break;

						case "LineStyleCombo":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineDrawingFormat =
								(DrawingLinesFormats)LineStyleCombo.SelectedIndex;
							break;

						case "LineMarker":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineMarkerNumber = (uint)LineMarker.Value;
							break;

						// Параметры, перенесённые из необъединяемых
						case "AutoTextOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).AutoTextOffset = AutoTextOffset.Checked;
							break;

						case "OxTextOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OxTextOffset = (uint)OxTextOffset.Value;
							break;

						case "OyTextOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).OyTextOffset = (uint)OyTextOffset.Value;
							break;

						case "LineNameLeftOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineNameLeftOffset = (uint)LineNameLeftOffset.Value;
							break;

						case "LineNameTopOffset":
							dd.GetStyle (LineNamesList.SelectedIndices[i]).LineNameTopOffset = (uint)LineNameTopOffset.Value;
							break;

						// Необъединяемые параметры
						case "LineName":
							dd.GetStyle (LineNamesList.SelectedIndex).LineName = LineName.Text;
							break;

						// Любое нештатное значение
						default:
							throw new Exception (RDLocale.GetText ("ExceptionMessage") + " (13)");
						}
					}
				}
			// Вряд ли, но на всякий случай
			catch
				{
				RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "LinesForSettingNSError");
				}

			// Перерисовка
			Redraw ();
			}

		// Изменение основных настроек диаграммы
		private void UniSettings_ValueChanged (object sender, EventArgs e)
			{
			UpdateDiagramParameters ((Control)sender);
			}

		private void ImageWidth_ValueChanged (object sender, EventArgs e)
			{
			// Установка новых ограничений
			OyTextOffset.Maximum = ImageWidth.Value * (decimal)(DiagramStyle.MaxOyTextMargin);
			LineNameLeftOffset.Maximum = ImageWidth.Value * (decimal)(DiagramStyle.MaxLineNameLeftMargin);

			UpdateDiagramParameters ((Control)sender);
			}

		private void ImageHeight_ValueChanged (object sender, EventArgs e)
			{
			// Установка новых ограничений
			OxTextOffset.Maximum = ImageHeight.Value * (decimal)(DiagramStyle.MaxOxTextMargin);
			LineNameTopOffset.Maximum = ImageHeight.Value * (decimal)(DiagramStyle.MaxLineNameTopMargin);

			UpdateDiagramParameters ((Control)sender);
			}

		private void AlignAsRow_Click (object sender, EventArgs e)
			{
			AlignLines (true);
			}

		private void AlignAsColumn_Click (object sender, EventArgs e)
			{
			AlignLines (false);
			}

		private void AlignLines (bool InRow)
			{
			// Сохранение списка обрабатываемых индексов
			List<int> indices = new List<int> ();
			for (int i = 0; i < LineNamesList.SelectedIndices.Count; i++)
				{
				indices.Add (LineNamesList.SelectedIndices[i]);
				}

			// Обработка
			for (int i = 1; i < indices.Count; i++)
				{
				// Выбор кривой
				LineNamesList.SelectedIndices.Clear ();
				LineNamesList.SelectedIndices.Add (indices[i]);

				// Установка параметров
				if (InRow)
					{
					LeftOffset.Value = dd.GetStyle (indices[i - 1]).DiagramImageLeftOffset + dd.GetStyle (indices[i - 1]).DiagramImageWidth;
					TopOffset.Value = dd.GetStyle (indices[i - 1]).DiagramImageTopOffset;
					}
				else
					{
					LeftOffset.Value = dd.GetStyle (indices[i - 1]).DiagramImageLeftOffset;
					TopOffset.Value = dd.GetStyle (indices[i - 1]).DiagramImageTopOffset + dd.GetStyle (indices[i - 1]).DiagramImageHeight;
					}
				}

			// Завершено
			indices.Clear ();
			}

		private void AutoTextOffset_CheckedChanged (object sender, EventArgs e)
			{
			UpdateDiagramParameters ((Control)sender);

			Label07.Enabled = Label08.Enabled = Label09.Enabled = Label10.Enabled =
			OxTextOffset.Enabled = OyTextOffset.Enabled = LineName.Enabled =
			LineNameLeftOffset.Enabled = LineNameTopOffset.Enabled = !AutoTextOffset.Checked;
			}

		// Моментальный разворот диапазонов
		private void SwapX_Click (object sender, EventArgs e)
			{
			decimal x = MaxX.Value;
			MaxX.Value = MinX.Value;
			MinX.Value = x;
			}

		private void SwapY_Click (object sender, EventArgs e)
			{
			decimal y = MaxY.Value;
			MaxY.Value = MinY.Value;
			MinY.Value = y;
			}

		// Настройки осей
		private void AutoDivisions_CheckedChanged (object sender, EventArgs e)
			{
			UpdateDiagramParameters ((Control)sender);
			OxPrimaryDiv.Enabled = OyPrimaryDiv.Enabled = !AutoDivisions.Checked;
			UniSettings_ValueChanged (OxPrimaryDiv, null);
			UniSettings_ValueChanged (OyPrimaryDiv, null);
			}

		// Изменение числа делений
		private void AxesColorTurnOff_Click (object sender, EventArgs e)
			{
			AxesColor.BackColor = DiagramStyle.ImageBackColor;
			}

		private void AxesColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = AxesColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			AxesColor.BackColor = ColorSelectDialog.Color;
			}

		// Изменение шрифтов подписей
		private void RunFontSelectCycle ()
			{
			// Вызов окна шрифтов с контролем успешности выбора
			bool success = false;
			while (!success)
				{
				success = true;

				try
					{
					if (StyleFontDialog.ShowDialog () == DialogResult.Cancel)
						{
						return;
						}
					}
				catch
					{
					RDGenerics.LocalizedMessageBox (RDMessageTypes.Warning_Center, "FontSelectError");
					success = false;
					}
				}
			}

		private void TextFont_Click (object sender, EventArgs e)
			{
			StyleFontDialog.Font = TextFont.Font;
			RunFontSelectCycle ();
			TextFont.Font = StyleFontDialog.Font;
			}

		private void AxesFont_Click (object sender, EventArgs e)
			{
			StyleFontDialog.Font = AxesFont.Font;
			RunFontSelectCycle ();
			AxesFont.Font = StyleFontDialog.Font;
			}

		// Изменение цветов подписей
		private void TextFontColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = TextFontColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			TextFontColor.BackColor = ColorSelectDialog.Color;
			}

		private void AxesFontColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = AxesFontColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			AxesFontColor.BackColor = ColorSelectDialog.Color;
			}

		// Изменение цветов линий сетки
		private void GridPrimaryColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = GridPrimaryColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			GridPrimaryColor.BackColor = ColorSelectDialog.Color;
			}

		private void GridSecondaryColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = GridSecondaryColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			GridSecondaryColor.BackColor = ColorSelectDialog.Color;
			}

		private void GridColorTurnOff_Click (object sender, EventArgs e)
			{
			GridPrimaryColor.BackColor = DiagramStyle.ImageBackColor;
			GridSecondaryColor.BackColor = DiagramStyle.ImageBackColor;
			}

		// Изменение цвета линии
		private void LineColor_Click (object sender, EventArgs e)
			{
			ColorSelectDialog.Color = LineColor.BackColor;
			ColorSelectDialog.ShowDialog ();
			LineColor.BackColor = ColorSelectDialog.Color;
			}

		private void LineColor_BackColorChanged (object sender, EventArgs e)
			{
			if (LineMarkerImage.BackgroundImage != null)
				LineMarkerImage.BackgroundImage.Dispose ();

			LineMarkerImage.BackgroundImage = ml.GetMarker ((uint)LineMarker.Value - 1, LineColor.BackColor);
			UpdateDiagramParameters ((Control)sender);
			}

		// Изменение формата линии
		private void LineStyleCombo_SelectedIndexChanged (object sender, EventArgs e)
			{
			LineMarker.Enabled = (LineStyleCombo.SelectedIndex > 0);
			UpdateDiagramParameters ((Control)sender);
			}

		// Изменение маркера
		private void LineMarker_ValueChanged (object sender, EventArgs e)
			{
			if (LineMarkerImage.BackgroundImage != null)
				LineMarkerImage.BackgroundImage.Dispose ();

			LineMarkerImage.BackgroundImage = ml.GetMarker ((uint)LineMarker.Value - 1, LineColor.BackColor);
			UpdateDiagramParameters ((Control)sender);
			}

		#endregion

		#region Обработка эффектов панели настроек и списка кривых

		// Панели настроек
		private void MainTabControl_MouseHover (object sender, EventArgs e)
			{
			MainTabControl_MouseClick (null, null);
			}

		private void MainTabControl_MouseClick (object sender, MouseEventArgs e)
			{
			MainTabControl.Height = (int)ConfigAccessor.MinHeight - 375;
			}

		private void MainTabControl_Leave (object sender, EventArgs e)
			{
			MainTabControl.Height = DiagramBox.Top - MainTabControl.Top;
			}

		// Список кривых
		private void LineNamesList_MouseHover (object sender, EventArgs e)
			{
			LineNamesList_MouseClick (null, null);
			}

		private void LineNamesList_MouseClick (object sender, MouseEventArgs e)
			{
			if (LineNamesList.Height != LineNamesList.ItemHeight * 10 + 4)
				{
				LineNamesList.Height = LineNamesList.ItemHeight * 10 + 4;

				// Следующий код позволяет избегать сброса списка выделенных кривых в поле LineNamesList при
				// щелчке мышью
				selecting = true;

				if (LineNamesList.Items.Count >= selectedInidces.Count) // По неизвестной причине последний элемент в selectedIndices
																		// не удаляется при опустошении LineNamesList.Items
					{
					LineNamesList.SelectedIndices.Clear ();
					for (int i = 0; i < selectedInidces.Count; i++)
						{
						LineNamesList.SelectedIndices.Add (selectedInidces[i]);
						}
					}

				selecting = false;
				}
			}

		private void LineNamesList_Leave (object sender, EventArgs e)
			{
			LineNamesList.Height = LineNamesList.ItemHeight * 3 + 4;
			}

		#endregion
		}
	}
