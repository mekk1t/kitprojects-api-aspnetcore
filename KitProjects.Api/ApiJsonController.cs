using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace KitProjects.Api.AspNetCore
{
    /// <summary>
    /// Базовый API контроллер, который принимает и отдает данные в формате application/json.
    /// </summary>
    /// <remarks>
    /// Базовый маршрут - api/[controller].
    /// </remarks>
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/[controller]", Name = "[controller]_[action]")]
    public abstract class ApiJsonController : ControllerBase
    {
        private readonly ILogger<ApiJsonController> _logger;

        /// <summary>
        /// Создает объект <see cref="ApiJsonController"/>.
        /// </summary>
        /// <param name="logger">Логгер для учета ошибок.</param>
        protected ApiJsonController(ILogger<ApiJsonController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Возвращает ответ сервера с единственным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="statusCode">Статус-код ответа.</param>
        /// <returns>
        /// Ответ сервера со статус-кодом <paramref name="statusCode"/> и сообщением об ошибке
        /// в формате <see cref="ApiErrorResponse"/>.
        /// </returns>
        protected IActionResult ApiError(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
            StatusCode((int)statusCode, new ApiErrorResponse(new[] { message }));

        /// <summary>
        /// Возвращает ответ сервера со списком сообщений об ошибках.
        /// </summary>
        /// <param name="messages">Сообщения об ошибках.</param>
        /// <param name="statusCode">Статус-код ответа.</param>
        /// <returns>
        /// Ответ сервера со статус-кодом <paramref name="statusCode"/> и списком сообщений об ошибках
        /// в формате <see cref="ApiErrorResponse"/>.
        /// </returns>
        protected IActionResult ApiError(string[] messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
            StatusCode((int)statusCode, new ApiErrorResponse(messages));

        /// <summary>
        /// Обрабатывает запрос, не требующий ответа сервера и оборачивает ошибки в <see cref="ApiErrorResponse"/>.
        /// </summary>
        /// <remarks>
        /// Стандартный статус-код -- 204. Его можно перезаписать через параметр <paramref name="statusCode"/>.
        /// </remarks>
        /// <param name="action">Набор действий для обработки.</param>
        /// <param name="statusCode">Статус-код ответа.</param>
        /// <returns>Ответ сервера со статус-кодом <see cref="HttpStatusCode.NoContent"/> и пустым телом.</returns>
        protected IActionResult ProcessRequest(Action action, HttpStatusCode statusCode = HttpStatusCode.NoContent)
        {
            try
            {
                action();
                return StatusCode((int)statusCode);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex.ToString());
                return ApiError("Произошла ошибка на стороне сервера.", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return ApiError(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Обрабатывает запрос данных на сервер и оборачивает ошибки в <see cref="ApiErrorResponse"/>.
        /// </summary>
        /// <param name="function">Функция, возвращающая ответ в формате <typeparamref name="TResult"/>.</param>
        /// <returns>
        /// Если <paramref name="function"/> возвращает какие-либо данные,
        /// то ответ сервера со статус-кодом <see cref="HttpStatusCode.OK"/> и телом ответа <typeparamref name="TResult"/>.
        /// <br></br>
        /// Иначе <see cref="ApiErrorResponse"/> со статус-кодом <see cref="HttpStatusCode.NotFound"/>.
        /// </returns>
        protected IActionResult ProcessRequest<TResult>(Func<TResult> function)
        {
            try
            {
                var result = function();
                if (result == null)
                    return ApiError("Не удалось получить данные по запросу.", HttpStatusCode.NotFound);

                return Ok(result);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex.ToString());
                return ApiError("Произошла ошибка на стороне сервера.", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return ApiError(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}