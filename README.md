File Manager
Информация команды

cd C:\Source –Директория

 /*Вывод дерева файловой системы с условием пейджинга”                                                
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
                   file C:\Source

![image](https://user-images.githubusercontent.com/58603557/166884796-7074c630-bac4-441a-b7be-25b72444ac39.png)

Это. Индекс listCommand: когда мы нажимаем Enter и если этот элемент добавлен тогда historyPointer++ например я 
добавил  3 команд (historyPointer =3)когда я нажимаю 1 (2,3,4....) раз UpArrow (Я написал там historyPointer--)
historyPointer=2  listCommand[2]= наш последний команд и а потом когда я нажимаю 1 (2,3,4...) раз DownArrow 
(Там написал historyPointer++) historyPointer =3 .... Я  просто добавил индекс каждую елементун

например добавьте 3(2,1) команд потом нажмите UpArrow потом Enter так тоже раблтает(как  powerShell)
