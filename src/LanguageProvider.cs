using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Поддерживаемые языки интерфейса программы
	/// </summary>
	public enum SupportedLanguages
		{
		/// <summary>
		/// Русский (Россия)
		/// </summary>
		ru_ru,

		/// <summary>
		/// Английский (США)
		/// </summary>
		en_us
		}

	/// <summary>
	/// Класс обеспечивает передачу в формы локализованных подписей контролов
	/// </summary>
	public static class LanguageProvider
		{
		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа Form</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Form Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа TabPage</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabPage Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех пунктов, входящих в состав указанного меню (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа MenuStrip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (MenuStrip Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Items.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Items[i].Name, Language) != null)
					{
					Container.Items[i].Text = LanguageProvider.GetControlText (Container.Name, Container.Items[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех пунктов, входящих в состав указанного меню (Text и ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа ToolStripMenuItem</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (ToolStripMenuItem Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.DropDownItems.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.DropDownItems[i].Name, Language) != null)
					{
					Container.DropDownItems[i].Text = LanguageProvider.GetControlText (Container.Name,
						Container.DropDownItems[i].Name, Language);
					}
				if (LanguageProvider.GetControlText (Container.Name, Container.DropDownItems[i].Name + "_TT", Language) != null)
					{
					Container.DropDownItems[i].ToolTipText = LanguageProvider.GetControlText (Container.Name,
						Container.DropDownItems[i].Name + "_TT", Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа TabControl</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabControl Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.TabPages.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.TabPages[i].Name + "_TT", Language) != null)
					{
					Container.TabPages[i].ToolTipText = LanguageProvider.GetControlText (Container.Name,
						Container.TabPages[i].Name + "_TT", Language);
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа Form</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Form Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], LanguageProvider.GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа TabPage</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (TabPage Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], LanguageProvider.GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (Text и ToolTipText)
		/// </summary>
		/// <param name="Container">Контейнер типа Panel</param>
		/// <param name="TextContainer">Контейнер подписей типа ToolTip</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Panel Container, ToolTip TextContainer, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}

				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name + "_TT", Language) != null)
					{
					TextContainer.SetToolTip (Container.Controls[i], LanguageProvider.GetControlText (Container.Name,
						Container.Controls[i].Name + "_TT", Language));
					}
				}
			}

		/// <summary>
		/// Метод устанавливает локализованные подписи для всех контролов, входящих в состав указанного контейнера (только Text)
		/// </summary>
		/// <param name="Container">Контейнер типа Panel</param>
		/// <param name="Language">Требуемый язык локализации</param>
		public static void SetControlsText (Panel Container, SupportedLanguages Language)
			{
			for (int i = 0; i < Container.Controls.Count; i++)
				{
				if (LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language) != null)
					{
					Container.Controls[i].Text = LanguageProvider.GetControlText (Container.Name, Container.Controls[i].Name, Language);
					}
				}
			}

		/// <summary>
		/// Метод возвращает локализованную подпись указанного контрола
		/// </summary>
		/// <param name="FormName">Имя формы, содержащей контрол</param>
		/// <param name="ControlName">Имя контрола</param>
		/// <param name="Language">Требуемый язык локализации</param>
		/// <returns>Подпись или текст контрола</returns>
		public static string GetControlText (string FormName, string ControlName, SupportedLanguages Language)
			{
			return GetText (FormName + "_" + ControlName, Language);
			}

		/// <summary>
		/// Метод возвращает локализованный текст по указанному идентификатору
		/// </summary>
		/// <param name="ErrorName">Идентификатор текстового фрагмента</param>
		/// <param name="Language">Требуемый язык локализации</param>
		/// <returns>Локализованный текстовый фрагмент</returns>
		public static string GetText (string ErrorName, SupportedLanguages Language)
			{
			switch (Language)
				{
				case SupportedLanguages.en_us:
					return GeomagDataDrawer.Lang_EnUs.ResourceManager.GetString (ErrorName);

				case SupportedLanguages.ru_ru:	// Считается базовым
				default:
					return GeomagDataDrawer.Lang_RuRu.ResourceManager.GetString (ErrorName);
				}
			}
		}
	}
