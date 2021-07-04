using System.Collections.Generic;

namespace KitProjects.Api.AspNetCore
{
    /// <summary>
    /// Ответ от сервера с информацией об ошибках.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Список сообщений об ошибках.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Создает ответ от сервера с информацией об ошибках.
        /// </summary>
        /// <param name="errors">Список сообщений об ошибках.</param>
        public ApiErrorResponse(IReadOnlyList<string> errors)
        {
            Errors = errors;
        }
    }
}
