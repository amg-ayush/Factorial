using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер факториала
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FactorialController : ControllerBase
    {
        private readonly IFactorialService factorialService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="factorialService">Сервис для вычисления значения факториал числа</param>
        public FactorialController(IFactorialService factorialService)
        {
            this.factorialService = factorialService;
        }

        /// <summary>
        /// Вычисляет факториал числа
        /// </summary>
        /// <param name="number">Число</param>
        [HttpGet(nameof(CalculateFact))]
        public async Task<long> CalculateFact(long number)
        {
            long result = await factorialService.CalculateAsync(number);
            return result;
        }
    }
}
