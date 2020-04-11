using System;
using System.Threading;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	/// <summary>
	/// Класс-описатель программы
	/// </summary>
	public static class Program
		{
		/// <summary>
		/// Главная точка входа для приложения
		/// </summary>
		[STAThread]
		public static void Main (string[] args)
			{
			// Проверка запуска единственной копии
			bool result;
			Mutex instance = new Mutex (true, ProgramDescription.AssemblyTitle, out result);
			if (!result)
				{
				MessageBox.Show (string.Format (LanguageProvider.GetText ("ProgramLaunchedError", SupportedLanguages.ru_ru),
					ProgramDescription.AssemblyTitle), ProgramDescription.AssemblyTitle, MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
				return;
				}

			// Запуск программы в случае уникальности
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			Application.Run (new TableMergerForm ());
			}
		}
	}
