using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace SoundStore.Infrastructure.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class DynamicSearchHelper
    {
        /// <summary>
        /// Áp dụng các điều kiện filter dựa trên các giá trị của filter object.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng cần filter (ví dụ: Product, Category,...)</typeparam>
        /// <typeparam name="TFilter">Kiểu dữ liệu của filter (ví dụ: ProductFilter, CategoryFilter,...)</typeparam>
        /// <param name="query">IQueryable cần filter</param>
        /// <param name="filter">Filter object chứa các điều kiện cần tìm kiếm</param>
        /// <returns>IQueryable đã được áp dụng các điều kiện filter</returns>
        public static IQueryable<T> ApplyFilters<T, TFilter>(this IQueryable<T> query, TFilter filter)
        {
            if (filter == null)
                return query;

            // Tạo parameter đại diện cho đối tượng T trong biểu thức lambda (x => ...)
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            Expression predicateBody = null;

            // Duyệt qua từng thuộc tính của filter object
            foreach (PropertyInfo filterProp in typeof(TFilter).GetProperties())
            {
                // Lấy giá trị của thuộc tính trên filter, nếu null thì bỏ qua
                object filterValue = filterProp.GetValue(filter);
                if (filterValue == null)
                    continue;

                // Tìm thuộc tính tương ứng trong đối tượng T, giả định tên thuộc tính giống nhau
                PropertyInfo entityProp = typeof(T).GetProperty(filterProp.Name);
                if (entityProp == null)
                    continue;

                // Tạo biểu thức truy cập thuộc tính: x.PropertyName
                MemberExpression left = Expression.Property(parameter, entityProp);
                // Tạo biểu thức chứa giá trị cần so sánh
                ConstantExpression right = Expression.Constant(filterValue, entityProp.PropertyType);
                // Tạo điều kiện so sánh: x.PropertyName == filterValue
                Expression condition = Expression.Equal(left, right);

                // Nếu đã có predicate rồi thì kết hợp với điều kiện hiện tại bằng phép AndAlso
                if (predicateBody == null)
                    predicateBody = condition;
                else
                    predicateBody = Expression.AndAlso(predicateBody, condition);
            }

            // Nếu không có điều kiện nào được thiết lập thì trả về query ban đầu
            if (predicateBody == null)
                return query;

            // Tạo lambda expression cho predicate: x => (điều kiện)
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

            return query.Where(lambda);
        }
    }
}
