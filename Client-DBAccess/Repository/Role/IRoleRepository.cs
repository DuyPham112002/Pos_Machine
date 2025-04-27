using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Role
{
    /// <summary>
    /// Generic Coding: Cho phép định nghĩa các lớp và giao diện mà không cần chỉ rõ kiểu dữ liệu từ đầu, giúp mã nguồn linh hoạt và dễ bảo trì.
    /// IRepository<T>: Là một interface tổng quát định nghĩa các phương thức CRUD cơ bản cho bất kỳ kiểu dữ liệu nào.
    /// IRoleRepository : IRepository<Entities.Role>: Định nghĩa một repository cụ thể cho thực thể Role, 
    /// kế thừa tất cả các phương thức CRUD từ IRepository, nhưng áp dụng cho Role.
    /// </summary>
    public interface IRoleRepository : IRepository<Entities.Role>
    /// public interface IRoleRepository: Định nghĩa một interface công khai tên là IRoleRepository.
    /// : IRepository<Entities.Role>: IRoleRepository kế thừa từ IRepository<Entities.Role>.Ở đây, 
    /// IRepository là một interface tổng quát(generic interface), và Entities.Role là kiểu dữ liệu cụ thể được áp dụng cho IRepository.
    {

    }
}
