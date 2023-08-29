namespace WebApi
{
    /// <summary>
    /// Сервис вычисляющий факториал числа
    /// </summary>
    public class FactorialService : IFactorialService
    {
        /// <contentfrom cref="IFactorialService.CalculateAsync(long)" />
        public async Task<long> CalculateAsync(long number)
        {
            RpcClient rpcClient = new RpcClient("InQueue");
            string response = rpcClient.CalculateFactorial(number.ToString());
            rpcClient.Close();
            long.TryParse(response, out long result);
            return result;
        }
    }
}
