using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace KP.Api.AspNetCore
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
        private readonly bool _catchAllExceptions;

        /// <summary>
        /// Создает объект <see cref="ApiJsonController"/>.
        /// </summary>
        /// <param name="logger">Логгер для учета ошибок.</param>
        /// <param name="catchAllExceptions">Ловить все исключения?</param>
        protected ApiJsonController(ILogger<ApiJsonController> logger, bool catchAllExceptions = false)
        {
            _logger = logger;
            _catchAllExceptions = catchAllExceptions;
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
        protected IActionResult ExecuteAction(Action action, HttpStatusCode statusCode = HttpStatusCode.NoContent) =>
            Wrap(() => LoggerWrap(() =>
            {
                action();
                return StatusCode((int)statusCode);
            }));

        /// <summary>
        /// Обрабатывает запрос на информацию об одиночном объекте. Ответ заворачивается в <see cref="ApiObjectResponse{T}"/>.
        /// </summary>
        /// <remarks>
        /// Если объект равен <see langword="null"/>, возвращается сообщение об ошибке со статус-кодом <see cref="HttpStatusCode.NotFound"/>. <br></br>
        /// Прочие ошибки обрабатываются и логгируются - возвращается сообщение об ошибке со статусо-кодом <see cref="HttpStatusCode.InternalServerError"/>.
        /// </remarks>
        /// <typeparam name="TResult">Тип данных объекта.</typeparam>
        /// <param name="function">Функция, которая выдает запрашиваемый объект.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> с телом запроса в формате <typeparamref name="TResult" />,
        /// обернутым в <see cref="ApiObjectResponse{TResult}"/>.
        /// </returns>
        protected IActionResult ExecuteObjectRequest<TResult>(Func<TResult> function) =>
            Wrap(() => LoggerWrap(() =>
            {
                var result = function();
                if (result == null)
                    return ApiError("Не удалось получить данные по запросу.", HttpStatusCode.NotFound);

                return Ok(new ApiObjectResponse<TResult>(result));
            }));

        /// <summary>
        /// Обрабатывает запрос на получение статической коллекции данных в формате <typeparamref name="TResult"/>.
        /// Итоговая коллекция будет обернута в <see cref="ApiCollectionResponse{T}"/>.
        /// </summary>
        /// <remarks>
        /// Этот метод никогда не вернет в коллекции данных <see langword="null"/>. Пустые или <see langword="null"/>-коллекции будут
        /// обернуты в пустой массив данных.
        /// </remarks>
        /// <typeparam name="TResult">Тип данных в результирующей коллекции.</typeparam>
        /// <param name="function">Функция, выдающая коллекцию данных.</param>
        /// <returns>Коллекцию данных в формате <typeparamref name="TResult"/>, обернутую в <see cref="ApiCollectionResponse{T}"/>.</returns>
        protected IActionResult ExecuteCollectionRequest<TResult>(Func<IEnumerable<TResult>> function) =>
            Wrap(() => LoggerWrap(() =>
            {
                var result = function();
                return Ok(new ApiCollectionResponse<TResult>(result));
            }));

        /// <summary>
        /// Обрабатывает получение <see cref="IActionResult"/>.
        /// </summary>
        /// <param name="getActionResult">Функция, возвращающая <see cref="IActionResult"/>.</param>
        /// <returns>Ответ API в формате <see cref="IActionResult"/>.</returns>
        protected abstract IActionResult Wrap(Func<IActionResult> getActionResult);

        private IActionResult LoggerWrap(Func<IActionResult> getActionResult)
        {
            try
            {
                return getActionResult();
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, ex.ToString());
                return ApiError("Произошла ошибка на стороне сервера", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex) when (_catchAllExceptions)
            {
                _logger.LogError(ex, ex.ToString());
                return ApiError(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}