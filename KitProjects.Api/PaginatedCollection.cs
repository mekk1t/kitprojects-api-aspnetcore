using System;

namespace KitProjects.Api.AspNetCore
{
    /// <summary>
    /// Класс для представления коллекции данных с параметрами пагинации.
    /// </summary>
    /// <remarks>
    /// Поддерживается только динамическая пагинация через признак наличия дальнейших данных по выборке.
    /// </remarks>
    public class PaginatedCollection<T>
    {
        /// <summary>
        /// Коллекция данных.
        /// </summary>
        public T[] Data { get; }
        /// <summary>
        /// Признак наличия данных дальше по выборке.
        /// </summary>
        public bool ThereIsMoreData { get; }

        /// <summary>
        /// Создает коллекцию данных с параметрами пагинации.
        /// </summary>
        /// <param name="data">Коллекция данных.</param>
        /// <param name="thereIsMoreData">Есть ли по выборке ещё данные?</param>
        public PaginatedCollection(T[] data, bool thereIsMoreData)
        {
            Data = data ?? Array.Empty<T>();
            ThereIsMoreData = thereIsMoreData;
        }
    }
}
