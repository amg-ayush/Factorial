namespace WorkerService.Utils
{
    /// <summary>
    /// Вспомашательный класс для работы с числами
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Получение факториала числа в текстовом формате
        /// </summary>
        /// <param name="number">Число</param>
        public static string GetFactorialText(this long number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (number <= 1)
                return "1";

            List<int> resultList = new List<int>() { 1 };

            for (int i = 2; i <= number; i++)
                resultList = GetProduct(resultList, GetList(i));

            return string.Concat(resultList);
        }

        // Получение результата умножения двух чисел представленных списками
        private static List<int> GetProduct(List<int> firstList, List<int> secondList)
        {
            firstList.Reverse();
            secondList.Reverse();

            List<int> resultList = new List<int>() { 0 };
            List<int> list = new List<int>();

            for (int i = 0; i < secondList.Count; ++i)
            {
                bool twoNum = false;
                foreach (int num in firstList)
                {
                    int product = secondList[i] * num;
                    if (twoNum)
                    {
                        product += list[0];
                        list[0] = product % 10;
                    }
                    else
                    {
                        list.Insert(0, product % 10);
                    }

                    if (product > 9)
                    {
                        list.Insert(0, product / 10);
                        twoNum = true;
                    }
                    else
                    {
                        twoNum = false;
                    }
                }

                for (int j = 0; j < i; ++j)
                    list.Add(0);

                resultList = GetSum(new List<int>(resultList), new List<int>(list));

                list.Clear();
            }

            return resultList;
        }

        // Получение суммы двух чисел представленных списками
        private static List<int> GetSum(List<int> firstList, List<int> secondList)
        {
            while (secondList.Count < firstList.Count)
                secondList.Insert(0, 0);
            while (firstList.Count < secondList.Count)
                firstList.Insert(0, 0);

            int dec = 0;
            for (int i = firstList.Count - 1; i >= 0; --i)
            {
                int total = firstList[i] + secondList[i] + dec;
                secondList[i] = total % 10;
                dec = total / 10;
            }
            if (dec > 0)
                secondList.Insert(0, dec);

            return secondList;
        }

        // Получение списка цифр на основе одного числа
        private static List<int> GetList(int num)
        {
            if (num < 10)
                return new List<int>() { num };

            List<int> resultList = new List<int>();
            while (num > 0)
            {
                resultList.Insert(0, num % 10);
                num /= 10;
            }

            return resultList;
        }
    }
}
