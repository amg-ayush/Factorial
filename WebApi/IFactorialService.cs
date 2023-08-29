namespace WebApi
{
    /// <summary>
    /// Интерфейс реализующий вычисление факториал числа
    /// </summary>
    public interface IFactorialService
    {
        /// <summary>
        /// Вычисляет значение факториала числа
        /// </summary>
        /// <param name="number">Число</param>
        Task<long> CalculateAsync(long number);
    }
}
