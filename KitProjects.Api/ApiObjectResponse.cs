namespace KitProjects.Api.AspNetCore
{
    /// <summary>
    /// Ответ от сервера, в котором данные обернуты в поле Data.
    /// </summary>
    /// <typeparam name="T">Тип данных в ответе сервера.</typeparam>
    public class ApiObjectResponse<T>
    {
        /// <summary>
        /// Данные в формате <typeparamref name="T"/>.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Создает ответ от сервера для обертки данных в поле Data.
        /// </summary>
        /// <param name="data">Данные для обертки.</param>
        public ApiObjectResponse(T data)
        {
            Data = data;
        }
    }
}
