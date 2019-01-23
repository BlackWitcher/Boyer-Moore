using System.Collections.Generic;

namespace SearchString
{
    public static class StringExtension
    {
        /// <summary>
        /// Класс для хранения таблицы с последним вхождением каждого искомого символа в строке поиска.
        /// </summary>
        /// <remarks>
        /// Таблица по сути это словарь вида ключ - значение, но использовать KeyValuePair<char, int> показалось лишним, так как элементы таблицы
        /// будут модифициваться в процессе просмотра текста поиска, а KeyValuePair иммутабельно, и каждый раз делать
        /// пересоздавать newKeyValuePair = new KeyValuePair<char, int>(oldKeyValuePair.Key, newValue) мне кажется менее очевидным и наглядным 
        /// нежели свой простенький мутабельный nested-класс
        /// </remarks>
        private class CharIndex
        {
            /// <summary>
            /// Это буква из поискового шаблона
            /// </summary>
            public char Letter { get; set; }

            /// <summary>
            /// Это индекс последнего вхождения буквы в тексте, среди которого ищем шаблон
            /// </summary>
            public int LastIndex { get; set; }
        }

        //Табличка для хранения индексов последнего вхождения символа в поисковом тексте
        private static readonly Dictionary<char, CharIndex> _lastFoundTable = new Dictionary<char, CharIndex>();
        public static int[] SearchString(this string text, string pattern)
        {
            //Список позиций вхождения подстроки в тексте (все индексы, где шаблон был найден в тексте, т.е. результаты поиска)
            List<int> positions = new List<int>();

            _lastFoundTable.Clear();

            //Вычислим длину шаблона (исключительно ради удобства)
            int pl = pattern.Length - 1; //pl = Pattern Lenght
            
            //Вычисляем индексы последнего вхождения каждого символа искомой строки и загоняем всё это в таблицу
            int i = pl;
            foreach (var ch in pattern)
            {
                if (_lastFoundTable.ContainsKey(ch))
                    _lastFoundTable[ch].LastIndex = i; //Обновляем индекс символа
                else
                    _lastFoundTable.Add(ch, new CharIndex { Letter = ch, LastIndex = i }); //Такого символа еще не было -> добавляем.
                i--;
            }

            //Можно было бы вновь длину шаблона обозвать за i, но что бы не путаться, пусть будет так:
            int j = pl;
            //А теперь основной цикл поиска по всему тексту
            while (j <= text.Length - 1)
            {
                var patternLastChar = pattern[pl];

                if (text[j] != patternLastChar)
                {
                    //Аналитика 1
                    //Если у нас последний символ текста не совпадает с последним символом шаблона, и этот символ отсутствует в шаблоне,
                    //тогда сдвигаем указатель на длину поискового шаблона:
                    if (!_lastFoundTable.ContainsKey(text[j]))
                    {
                        j += pl;
                    }

                    //Аналитика 2
                    //Смотрим нашу таблицу с индексами на предмет получения индекса последнего вхождения текущего символа в тексте
                    //и двигаем указатель к этому индексу.
                    if (j <= text.Length - 1 && _lastFoundTable.ContainsKey(text[j]))
                    {
                        CharIndex tmp = _lastFoundTable[text[j]];
                        if (tmp != null) j += tmp.LastIndex;
                    }
                }

                int k = pl; //Длина шаблона - 1
                int m = j; //Указатель теперь в m
                if (j <= text.Length - 1)
                {
                    while (k >= 0)
                    {
                        //Аналитика 3.1.
                        //Сравним символ из текста с символом шаблона в обратном порядке. Если они совпадают, то выполним проверку, а не закончился ли у нас шаблон?
                        //Если шаблон у нас закончился (все его символы просмотрели), то, учтоним, если указатель m + длина шаблона не выходят за границы текста, то мы нашли совпадение!
                        //в этом случае сдвигаемся влево на длину шаблона. Ну и запоминаем позицию вхождения.
                        if (text[m] == pattern[k])
                        {
                            if (k == 0 && text[m] == pattern[k])
                            {
                                if (m + pattern.Length <= text.Length)
                                    positions.Add(m);
                                j += pl;
                            }
                            m--; k--;
                            continue;
                        }

                        if (!_lastFoundTable.ContainsKey(text[m]))
                        {
                            //Аналитика 3.2                            
                            //Если у нас встретился символ в тексте, который не содержится в шаблоне, то мы сдвигем указатель на величину 
                            //к индексу этого несовпадающего символа + число совпадающих символов., 
                            j += k + (j - m);
                            break;
                        }
                        k--;
                    }
                }
                j++;
            }

            //Подготовка и возврат результата:
            if (positions.Count == 0) positions.Add(-1);
            return positions.ToArray();
        }
    }
}
