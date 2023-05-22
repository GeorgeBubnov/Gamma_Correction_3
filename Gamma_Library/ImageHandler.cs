using System.Collections.Generic;
using System.Drawing;

namespace Gamma_Library
{
    //Указатель на функцию обработки прогресса выполнения задачи
    public delegate void ProgressDelegate(int number, double percent, int width, int height);
    public interface ImageHandler
    {
        //получение осмысленного имени обработчика
        string HandlerName { get; }
        //Инициализация параметров обработчика
        void init(SortedList<string, object> parameters);
        //Установка изображения-источника
        Bitmap Source { set; }
        //Получение изображения-результата
        Bitmap Result { get; }
        //Запуск обработки
        void startHandle(ProgressDelegate progress);
    }
}
