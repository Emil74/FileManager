using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace lesson_9
{
    class Program
    {
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDTH = 120;
        private static string currentDir = Directory.GetCurrentDirectory();
        private static List<string> listComand = new List<string>();

        /*
         * historyPointer
         Это. Индех listCommand: когда мы нажимаем Enter и если 
        этот элемент добпвлен тогда historyPointer++ 
        например я добавил  3 комманд (historyPointer =3)
        когда я нажимаю 1 (2,3,4....) раз UpArrow 
        (Я написал там historyPointer--)historyPointer=2  
        listCommand[2]= наш последний комманд и
        а потом когда я нажимаю 1 (2,3,4...) раз DownArrow (Там написал historyPointer++) 
         */
        static int historyPointer = 0;
        static void Main(string[] args)
        {
            File.Delete(currentDir + @"\errors\random_name_exception.txt");//удалит этот файл когда начинает программа 

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager";
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.Title = currentDir;

            DrawWindow(0, 0, WINDOW_WIDTH, 18);
            DrawWindow(0, 18, WINDOW_WIDTH, 8);
            if (Properties.Settings.Default.Command != "")
                ParseCommandString(Properties.Settings.Default.Command);
            UpdateConsole();

            Console.ReadKey(true);
        }

        /// <summary>
        /// обновляеть консола
        /// </summary>
        static void UpdateConsole()
        {
            DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
            ProcessEnterCommand(WINDOW_WIDTH);
        }

        /// <summary>
        /// Позиция курсора
        /// </summary>
        /// <returns></returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }

        /// <summary>
        /// методы кнопки
        ///  
        /// </summary>
        /// <param name="width">Width</param>
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();

            ConsoleKeyInfo key;
            char keyy;
            do
            {
                key = Console.ReadKey();
                keyy = key.KeyChar;

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.UpArrow && key.Key != ConsoleKey.DownArrow)
                {
                    command.Append(keyy);
                }

                (int currentLeft, int currentTop) = GetCursorPosition();

                if (currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }
                if (key.Key == ConsoleKey.Backspace/*ConsoleKey.Backspace*/)
                {
                    if (command.Length > 0)
                        command.Remove(command.Length - 1, 1);

                    if (currentLeft >= left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        Console.SetCursorPosition(left, top);
                    }
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);

                    if (listComand.Count == 0)   //если listCommand пусто тогда возвращает последный command
                    {
                        if (Properties.Settings.Default.Command != "")
                            ParseCommandString(Properties.Settings.Default.Command);
                    }

                    historyPointer--;    //каждый раз нажимаю UpArrow historyPointer--
                    if (historyPointer == -1)
                    {
                        DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                        historyPointer = 0;
                    }

                    DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);

                    if (historyPointer > listComand.Count)
                    {
                        historyPointer = listComand.Count - 1;
                    }
                    Console.Write(listComand[historyPointer]);
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                    if (listComand.Count == 0)
                    {
                        if (Properties.Settings.Default.Command != "")
                            ParseCommandString(Properties.Settings.Default.Command);
                    }

                    historyPointer++;
                    if (historyPointer < listComand.Count)
                    {
                        DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                        Console.Write(listComand[historyPointer]);
                    }
                    else if (historyPointer == listComand.Count)
                    {
                        DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                    }
                    else if (historyPointer > listComand.Count)
                    {
                        DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                        historyPointer = listComand.Count;
                    }
                    else
                    {
                        Console.Write(listComand[historyPointer - 1]);
                        DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
            ListCommand(command.ToString());
        }

        /// <summary>
        /// добавить комманд List<string>
        /// </summary>
        /// <param name="command">command каторый мы написали</param>
        static void ListCommand(string command)
        {
            command = command.Trim();
            if (string.IsNullOrWhiteSpace(command))
            {
                DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);

            }

            if (command == "") //например я добавил 3 комманд и нажимаю UpArrow и потом Enter она возвращает command=""
                               //он принимает вес комманд как текуший директоря
            {
                if (historyPointer != listComand.Count)
                {
                    ParseCommandString(listComand[historyPointer]);
                }
                else if (historyPointer == listComand.Count)
                {
                    DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
                }
            }

            if (command == "\0")  //например я добавил 3 комманд и нажимаю DownArrow и потом Enter  возвращает command="\0"
                                  //он принимаеть все комманду как директория
            {
                ParseCommandString(listComand[historyPointer]);
            }

            if (!listComand.Contains(command))
            {
                listComand.Add(command);
                historyPointer = listComand.Count;
            }
            else
            {
                historyPointer = listComand.Count;
                for (int i = 0; i < listComand.Count; i++)
                {
                    if (listComand[i] == command)
                    {
                        ParseCommandString(listComand[i]);
                    }
                }
            }

            ParseCommandString(listComand[historyPointer - 1]);
        }

        /// <summary>
        /// cd C:\Source --Директория
         /*Вывод дерева файловой системы с условием “пейджинга”          
            ls C:\Source -p 2
            Копирование каталога
            cp C:\Source D:\Target
            Копирование файла
            cp C:\Source\source.txt D:\Target\target.txt
            Удаление каталога рекурсивно
            rm C:\Source
            Удаление файла
            rm C:\Source\source.txt
            Вывод информации
            file C:\Source\source.txt
         */
        /// Я неаписал так когда последные комманды cd или ls ... сохраняет последный комманд.можно все комманды так написат...
        /// 
        /// </summary>
        /// <param name="command">наш комманд</param>
        static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            if (commandParams.Length > 0)
            {
                switch (commandParams[0])
                {
                    case "cd":
                        try
                        {
                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {
                                currentDir = commandParams[1];
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                Properties.Settings.Default.Command = command;
                                Properties.Settings.Default.Save();
                            }
                            else
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);

                                if (!Directory.Exists("errors"))
                                {
                                    Directory.CreateDirectory("errors");
                                }
                                throw new Exception(/*commandParams[1]*/ command + "--Нет такой файл");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.SetCursorPosition(1, 19);

                            File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                            Console.Write(ex.Message);
                        }
                        break;

                    case "ls":
                        try
                        {

                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {

                                if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    DrawTree(new DirectoryInfo(commandParams[1]), n);
                                    if (n != 0)
                                    {
                                        Properties.Settings.Default.Command = command;
                                        Properties.Settings.Default.Save();
                                    }
                                    else
                                    {
                                        throw new Exception(command + "--Нет такая страница");
                                    }
                                }
                                else
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), 1);
                                    /*  Properties.Settings.Default.Command = command;
                                      Properties.Settings.Default.Save();*/
                                }
                            }
                            else
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                throw new Exception(command + "--Нет такой файл");
                            }
                        }
                        catch (Exception ex)
                        {
                            DrawWindow(0, 0, WINDOW_WIDTH, 18);
                            Console.SetCursorPosition(1, 19);

                            File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                            Console.Write(command + " " + "--Нет такая страница");
                        }
                        break;

                    case "cp":
                        if (Directory.Exists(commandParams[1]))
                        {
                            try
                            {
                                if (commandParams.Length > 1 && Directory.Exists(commandParams[1]) && Directory.Exists(commandParams[2]))
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    CopyDirectory(commandParams[1], commandParams[2], true);
                                }
                                else
                                {
                                    // Проверьте, существует ли исходный каталог                               
                                    throw new DirectoryNotFoundException(command + "--Исходный каталог не найден");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);

                                Console.SetCursorPosition(1, 19);
                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }
                        else
                        {
                            //Copy File
                            try
                            {
                                if (commandParams.Length > 1 && File.Exists(commandParams[1]) && File.Exists(commandParams[2]))
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    CopyFile(commandParams[1], commandParams[2]);
                                }
                                else
                                {
                                    throw new FileNotFoundException(command + "--Исходный файл не найден");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);

                                Console.SetCursorPosition(1, 19);
                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }

                        break;

                    case "rm":
                        if (Directory.Exists(commandParams[1]))
                        {
                            //delete Folder
                            try
                            {
                                if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    deleteFolder(commandParams[1]);
                                }
                                else
                                {
                                    throw new Exception(commandParams[1] + "--Нет такой фолдер");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                Console.SetCursorPosition(1, 19);

                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }

                        //delete File
                        else
                        {
                            try
                            {
                                if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    deleteFile(commandParams[1]);
                                }
                                else
                                {
                                    throw new Exception(commandParams[1] + "--Нет такой файл");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                Console.SetCursorPosition(1, 19);
                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }

                        break;

                    case "file":
                        //Info Folder
                        if (Directory.Exists(commandParams[1]))
                        {
                            try
                            {
                                if (commandParams.Length > 1)
                                {
                                    DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    InfoFolder(commandParams[1]);
                                }
                                else
                                {
                                    throw new Exception(command + "--Нет файл");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                Console.SetCursorPosition(1, 19);

                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }
                        //Info File
                        else
                        {
                            try
                            {
                                if (commandParams.Length > 1)
                                {
                                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                    DrawWindow(0, 0, WINDOW_WIDTH, 18);

                                    InfoFile(commandParams[1]);
                                }
                                else
                                {
                                    throw new Exception(command + "--Нет файл");
                                }
                            }
                            catch (Exception ex)
                            {
                                DrawWindow(0, 0, WINDOW_WIDTH, 18);
                                DrawWindow(0, 18, WINDOW_WIDTH, 8);
                                Console.SetCursorPosition(1, 19);
                                File.AppendAllText(@"errors/random_name_exception.txt", ex.Message + "\n");
                                Console.Write(ex.Message);
                            }
                        }

                        break;

                    case "exit":
                        Environment.Exit(1);
                        break;

                    default:
                        DrawWindow(0, 0, WINDOW_WIDTH, 18);
                        DrawWindow(0, 18, WINDOW_WIDTH, 8);

                        Console.SetCursorPosition(1, 19);
                        if (command == "")
                            Console.Write("Введите команд");

                        else
                            Console.Write(command + "--Нет такая команда");
                        break;
                }
            }
            UpdateConsole();
        }

        /// <summary>
        /// Информация файла
        /// </summary>
        /// <param name="path"></param>
        static void InfoFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            Console.SetCursorPosition(1, 19);
            Console.WriteLine($"Имя Файла {fileInfo.Name}");
            Console.SetCursorPosition(1, 20);
            Console.WriteLine($"Время создания: {fileInfo.CreationTime}");
            Console.SetCursorPosition(1, 21);
            Console.WriteLine($"Размер: {fileInfo.Length} КБ");
        }

        /// <summary>
        /// Информация фолдера
        /// </summary>
        /// <param name="path"></param>
        static void InfoFolder(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            Console.SetCursorPosition(1, 19);
            Console.WriteLine($"Имя фолдера {dirInfo.Name}");
            Console.SetCursorPosition(1, 20);
            Console.WriteLine($"Время создания: {dirInfo.CreationTime}");
            Console.SetCursorPosition(1, 21);
            Console.WriteLine($"Полный имя фолдера: {dirInfo} ");
        }

        /// <summary>
        /// удалит файл
        /// </summary>
        /// <param name="file"></param>
        static void deleteFile(string file)
        {
            File.Delete(file);
        }

        /// <summary>
        /// удалит фолдер
        /// </summary>
        /// <param name="folder"></param>
        static void deleteFolder(string folder)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                DirectoryInfo[] diA = di.GetDirectories();
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo f in fi)
                {
                    f.Delete();
                }

                foreach (FileInfo df in fi)
                {
                    if (df.Length == 0)
                        return;
                    deleteFolder(df.FullName);
                }

                if (di.GetDirectories().Length == 0 && di.GetFiles().Length == 0) di.Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// копироват файла
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destinationFile"></param>
        static void CopyFile(string sourceFile, string destinationFile)
        {
            FileInfo fileInf = new FileInfo(sourceFile);
            if (fileInf.Exists)
            {
                fileInf.CopyTo(destinationFile, true);
            }
        }

        /// <summary>
        /// Копирование каталогов
        /// </summary>
        /// <param name="sourceDir">исходный каталог</param>
        /// <param name="destinationDir">каталог назначения</param>
        /// <param name="recursive">скопировать рекурсивно</param>
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Получить информацию об исходном каталоге
            var dir = new DirectoryInfo(sourceDir);

            // Кэшируйте каталоги, прежде чем мы начнем копирование
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Получить файлы в исходном каталоге и скопировать в целевой каталог
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    DrawWindow(0, 0, WINDOW_WIDTH, 18);
                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                    string targetFilePath = Path.Combine(destinationDir, file.Name);
                    file.CopyTo(targetFilePath);
                }
                catch (Exception)
                {
                    DrawWindow(0, 0, WINDOW_WIDTH, 18);
                    DrawWindow(0, 18, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 19);
                    Console.Write("Этот файл уже сущесвует");
                }
            }

            // Если рекурсивно и копируются подкаталоги, рекурсивно вызовите этот метод
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// Отрисовать делево каталогов
        /// </summary>
        /// <param name="dir">Директория</param>
        /// <param name="page">Страница</param>
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            DrawWindow(0, 0, WINDOW_WIDTH, 18);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 16;


            string[] lines = tree.ToString().Split(new char[] { '\n' });

            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            try
            {
                if (page > pageTotal || page == 0)
                {
                    Console.SetCursorPosition(1, 19);
                    throw new Exception(" Нет такая страница");
                }

                else
                {
                    //footer
                    NumberPage(lines, page, pageLines);
                    string footer = $"╡ {page} of {pageTotal} elements { Properties.Settings.Default.numberElement} ╞";
                    Console.SetCursorPosition(WINDOW_WIDTH / 2 - footer.Length / 2, 17);
                    Console.WriteLine(footer);
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(1, 19);
                Console.Write(ex.Message);
            }

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);

                    Console.WriteLine(lines[i]);
                }
            }
        }
        /// <summary>
        /// нумбер страница
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="page"></param>
        /// <param name="pageLines"></param>
        static void NumberPage(string[] lines, int page, int pageLines)
        {
            int line = lines.Length - 1;

            int c = line / 16;
            int b = line % 16;
            if (page <= c)
            {
                Properties.Settings.Default.numberElement = pageLines;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.numberElement = b;
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="dir"></param>
        /// <param name="indent"></param>
        /// <param name="lastDirectory"></param>
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }

            tree.Append($"{dir.Name}\n"); //<---------------------- ПЕРЕХОД НА СЛЕД СТРОКУ

            //TODO: Добавляем отображение файлов
            FileInfo[] subFiles = dir.GetFiles();
            for (int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length - 1)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }

            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
        }

        /// <summary>
        /// Отрисовать консоль
        /// </summary>
        /// <param name="dir">Текущая директория</param>
        /// <param name="x">Начальная позиция по оси X</param>
        /// <param name="y">Начальная позиция по оси Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height / 2);
            Console.Write($"{dir}>");
        }

        /// <summary>
        /// Отрисовать окно
        /// </summary>
        /// <param name="x">Начальная позиция по оси X</param>
        /// <param name="y">Начальная позиция по оси Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        static void DrawWindow(int x, int y, int width, int height)
        {
            Console.SetCursorPosition(x, y);
            // header - шапка
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++) // 2 - уголки
                Console.Write("═");
            Console.Write("╗");

            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("║");
                for (int j = x + 1; j < x + width - 1; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("║");
            }

            // footer - подвал
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++) // 2 - уголки
                Console.Write("═");
            Console.Write("╝");
            Console.SetCursorPosition(x, y);
        }
    }
}
