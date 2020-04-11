using System;
using System.Windows.Forms;

namespace GeomagDataDrawer
	{
	static class SVGGeneratorProgram
		{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main (string[] args)
			{
			// Подготовка к запуску
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			SVGGeneratorForm mainForm = null;

			// Контроль аргументов
			string sourcePath = "", destPath = "", format = "";
			if ((args.Length >= 1) && (args.Length <= 3))
				{
				// Запрос справки
				if ((args[0] == "/?") || (args[0] == "-?"))
					{
					MessageBox.Show ("Использование командной строки:\n\n" +
						"SVGGenerator [Имя файла сценария] [Имя файла изображения] [Тип изображения {SVG|EMF}]\n\n" +
						"При отсутствии второго параметра имя файла изображения формируется на основе имени файла сценария в формате SVG\n" +
						"При отсутствии или некорректном указании третьего параметра изображение сохраняется в формате SVG\n" +
						"При отсутствии всех параметров программа запускается в обычном режиме",
						ProgramDescription.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
					}

				// Выбор варианта
				sourcePath = args[0];
				destPath = args[0] + ".svg";
				format = "svg";
				if (args.Length > 1)
					{
					destPath = args[1];
					}
				if (args.Length > 2)
					{
					format = args[2];
					}

				mainForm = new SVGGeneratorForm (sourcePath, destPath, format);
				}
			else
				{
				mainForm = new SVGGeneratorForm ();
				}
			}
		}
	}
