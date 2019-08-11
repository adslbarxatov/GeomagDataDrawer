using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс описывает форму выбора размера и сохранения конечного изображения
	/// </summary>
	public partial class SavePicture:Form
		{
		// Переменные
		private List<DiagramStyle> lineStyles = new List<DiagramStyle> ();	// Специальные стили, используемые для сохранения изображения
		private List<DiagramStyle> objectStyles = new List<DiagramStyle> ();
		private uint imageWidth, imageHeight;	// Новый размер диаграммы
		private DiagramData diagramData;		// Данные диаграммы
		private SupportedLanguages language;	// Язык локализации

		/// <summary>
		/// Конструктор. Принимает отрисовываемые данные и стиль отрисовки
		/// </summary>
		/// <param name="DiagramData">Данные диаграммы</param>
		/// <param name="Language">Язык локализации</param>
		/// <param name="Silent">Флаг 'тихого' режима (без настроек)</param>
		public SavePicture (DiagramData DiagramData, SupportedLanguages Language, bool Silent)
			{
			// Инициализация и локализация формы
			InitializeComponent ();
			LanguageProvider.SetControlsText (this, Language);
			SaveButton.Text = LanguageProvider.GetText ("SaveButton", Language);
			AbortButton.Text = LanguageProvider.GetText ("AbortButton", Language);
			this.Text = LanguageProvider.GetControlText (this.Name, "T", Language);

			// Передача значений
			for (int i = 0; i < DiagramData.LinesCount; i++)
				{
				lineStyles.Add (new DiagramStyle (DiagramData.GetStyle (i)));
				}
			for (int i = 0; i < DiagramData.AdditionalObjectsCount; i++)
				{
				objectStyles.Add (new DiagramStyle (DiagramData.GetStyle (i + (int)DiagramData.LinesCount)));
				}

			diagramData = DiagramData;
			imageWidth = DiagramData.DiagramWidth;
			imageHeight = DiagramData.DiagramHeight;
			language = Language;

			// Настройка контролов
			SFDialog.Title = LanguageProvider.GetControlText (this.Name, "SFDialog", Language);

			ImageScale.Minimum = (decimal)DiagramStyle.MinScale;
			ImageScale.Maximum = (decimal)DiagramStyle.MaxScale;

			CustomSize.Checked = true;

			// Запуск
			if (!Silent)
				this.ShowDialog ();
			}

		// Отмена
		private void SaveAbort_Click (object sender, System.EventArgs e)
			{
			this.Close ();
			}

		// Сохранить
		private void ImageSave_Click (object sender, System.EventArgs e)
			{
			SFDialog.ShowDialog ();
			}

		/// <summary>
		/// Метод сохраняет изображение в 'тихом' режиме
		/// </summary>
		/// <param name="Path"></param>
		/// <param name="Type"></param>
		public void SaveImage (string Path, ImageOutputTypes Type)
			{
			// Настройка триггеров
			SFDialog.FileName = Path;

			switch (Type)
				{
				case ImageOutputTypes.PNG:
					break;

				default:
				case ImageOutputTypes.SVG:
					VectorImage.Checked = true;
					break;

				/*case ImageOutputTypes.EMF:
					VectorImage.Checked = true;
					SFDialog.FilterIndex = 2;
					break;*/
				}

			// Сохранение
			SFDialog_FileOk (null, null);
			}

		// Файл выбран
		private void SFDialog_FileOk (object sender, System.ComponentModel.CancelEventArgs e)
			{
			// Переменные
			double scale = (1.0 / 2.54) * 300.0;	// Множитель для пересчёта сантиметров в пиксели
			// 100/254 дюймов на сантиметр, 300 пикселей на дюйм
			double horizScale, vertScale, endScale = 1.0;

			#region Сохранение растрового изображения
			if (!VectorImage.Checked)
				{
				// Вариант с пользовательским размером
				if (CustomSize.Checked)
					{
					endScale = (double)ImageScale.Value;

					imageWidth = (uint)((double)imageWidth * endScale);
					imageHeight = (uint)((double)imageHeight * endScale);
					}

				// Вариант A4, альбомная
				if (A4Horiz.Checked)
					{
					horizScale = 29.7 * scale / (double)imageWidth;
					vertScale = 21.0 * scale / (double)imageHeight;
					endScale = System.Math.Min (horizScale, vertScale);

					imageWidth = (uint)(29.7 * scale);
					imageHeight = (uint)(21.0 * scale);
					}

				// Вариант A4, книжная
				if (A4Vert.Checked)
					{
					horizScale = 21.0 * scale / (double)imageWidth;
					vertScale = 29.7 * scale / (double)imageHeight;
					endScale = System.Math.Min (horizScale, vertScale);

					imageWidth = (uint)(21.0 * scale);
					imageHeight = (uint)(29.7 * scale);
					}

				// Вариант A3, альбомная
				if (A3Horiz.Checked)
					{
					horizScale = 42.0 * scale / (double)imageWidth;
					vertScale = 29.7 * scale / (double)imageHeight;
					endScale = System.Math.Min (horizScale, vertScale);

					imageWidth = (uint)(42.0 * scale);
					imageHeight = (uint)(29.7 * scale);
					}

				// Вариант A3, книжная
				if (A3Vert.Checked)
					{
					horizScale = 29.7 * scale / (double)imageWidth;
					vertScale = 42.0 * scale / (double)imageHeight;
					endScale = System.Math.Min (horizScale, vertScale);

					imageWidth = (uint)(29.7 * scale);
					imageHeight = (uint)(42.0 * scale);
					}

				// Масштабирование изображения
				for (int i = 0; i < lineStyles.Count; i++)
					{
					lineStyles[i].ApplyLineScale ((float)endScale);
					}
				for (int i = 0; i < objectStyles.Count; i++)
					{
					objectStyles[i].ApplyObjectScale ((float)endScale);
					}

				// Формирование изображения
				Bitmap b = new Bitmap ((int)imageWidth, (int)imageHeight);
				Graphics g = Graphics.FromImage (b);
				diagramData.DrawAllDiagrams (imageWidth, imageHeight, lineStyles, objectStyles, g, null);

				// Сохранение
				try
					{
					b.Save (SFDialog.FileName, ImageFormat.Png);
					}
				catch
					{
					MessageBox.Show (LanguageProvider.GetText ("ImageSaveError", language),
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				b.Dispose ();
				g.Dispose ();
				}
			#endregion

			#region Сохранение векторного изображения
			else
				{
				switch (SFDialog.FilterIndex)
					{
					// EMF
					case 2:
						EMFAdapter emfa = new EMFAdapter (SFDialog.FileName, (uint)((decimal)imageWidth * ImageScale.Value),
							(uint)((decimal)imageHeight * ImageScale.Value));

						if ((emfa.InitResult != VectorAdapterInitResults.Opened) ||
							(diagramData.DrawAllDiagrams (emfa) < 0))
							{
							MessageBox.Show (LanguageProvider.GetText ("ImageSaveError", language),
								ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						break;

					// SVG
					default:
						SVGAdapter svga = new SVGAdapter (SFDialog.FileName, (uint)((decimal)imageWidth * ImageScale.Value),
							(uint)((decimal)imageHeight * ImageScale.Value));

						if ((svga.InitResult != VectorAdapterInitResults.Opened) ||
							(diagramData.DrawAllDiagrams (svga) < 0))
							{
							MessageBox.Show (LanguageProvider.GetText ("ImageSaveError", language),
								ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						break;
					}
				}
			#endregion

			// Закрытие окна
			this.Close ();
			}

		// Выбор варианта
		private void CustomSize_CheckedChanged (object sender, System.EventArgs e)
			{
			ImageScale.Enabled = true;
			SFDialog.Filter = LanguageProvider.GetControlText (this.Name, "SFDialog_FR", language);
			}

		private void A4Horiz_CheckedChanged (object sender, System.EventArgs e)
			{
			ImageScale.Enabled = false;
			SFDialog.Filter = LanguageProvider.GetControlText (this.Name, "SFDialog_FR", language);
			}

		private void A4Vert_CheckedChanged (object sender, System.EventArgs e)
			{
			A4Horiz_CheckedChanged (sender, e);
			}

		private void A3Horiz_CheckedChanged (object sender, System.EventArgs e)
			{
			A4Horiz_CheckedChanged (sender, e);
			}

		private void A3Vert_CheckedChanged (object sender, System.EventArgs e)
			{
			A4Horiz_CheckedChanged (sender, e);
			}

		private void VectorImage_CheckedChanged (object sender, System.EventArgs e)
			{
			ImageScale.Enabled = false;
			SFDialog.Filter = LanguageProvider.GetControlText (this.Name, "SFDialog_FV", language);
			}
		}
	}
