using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<T> Items { get; }
        /// <summary>
        /// Признак наличия данных дальше по выборке.
        /// </summary>
        public bool ThereAreMoreItems { get; }

        /// <summary>
        /// Создает коллекцию данных с параметрами пагинации.
        /// </summary>
        /// <param name="data">Коллекция данных.</param>
        /// <param name="thereIsMoreData">Есть ли по выборке ещё данные?</param>
        public PaginatedCollection(IEnumerable<T> data, bool thereIsMoreData)
        {
            Items = data ?? Enumerable.Empty<T>();
            ThereAreMoreItems = thereIsMoreData;
        }
    }
}
