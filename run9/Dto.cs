using System.Linq.Expressions;

namespace TMT.External.Grpc
{
    // =================================================================
    //  Store
    // =================================================================
    public class Store
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class StoreGrpcRequestDto
    {
        public Expression<Func<Store, bool>> Predicates { get; set; }
    }

    public class StoreGetListParam
    {
        public bool IsUnicode { get; set; }
        public bool? IsActive { get; set; }
        public List<long> StoreIds { get; set; } = new();
        public string SearchTextToLower { get; set; } = string.Empty;
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
    }

    // =================================================================
    //  Tag
    // =================================================================
    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class TagGrpcRequestDto
    {
        public Expression<Func<Tag, bool>> Predicates { get; set; } = _ => true;
    }

    public class TagGetListParam
    {
        public bool? IsActive { get; set; }
        public bool IsUnicode { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string SearchTextToLower { get; set; } = string.Empty;
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
    }

    // =================================================================
    //  Category
    // =================================================================
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CategoryGrpcRequestDto
    {
        public Expression<Func<Category, bool>> Predicates { get; set; } = _ => true;
    }

    public class CategoryGetListParam
    {
        public bool? IsActive { get; set; }
        public List<long> StoreIds { get; set; } = new();
        public bool IsHO { get; set; }
        public bool IsUnicode { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string SearchTextToLower { get; set; } = string.Empty;
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
    }

    public class GetCategoryByIdsAndStoresRequestDto
    {
        public List<long> CategoryIds { get; set; } = new();
        public List<long> StoreIds { get; set; } = new();
    }

    // =================================================================
    //  ProductAttribute
    // =================================================================
    public class ProductAttribute
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ProductAttributeGrpcRequestDto
    {
        public Expression<Func<ProductAttribute, bool>> Predicates { get; set; } = _ => true;
    }

    // =================================================================
    //  ProductMedia
    // =================================================================
    public class ProductMedia
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    public class ProductMediaGrpcRequestDto
    {
        public Expression<Func<ProductMedia, bool>> Predicates { get; set; } = _ => true;
    }

    // =================================================================
    //  ProductModel
    // =================================================================
    public class ProductModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ProductModelGrpcRequestDto
    {
        public Expression<Func<ProductModel, bool>> Predicates { get; set; } = _ => true;
    }

    public class ProductModelGetListParam
    {
        public bool? IsActive { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
    }

    public class ProductModelGetListRequestDto
    {
        public long StoreId { get; set; }
        public List<long> ProductModelIds { get; set; } = new();
    }

    // =================================================================
    //  Product
    // =================================================================
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ProductGrpcRequestDto
    {
        public Expression<Func<Product, bool>> Predicates { get; set; } = _ => true;
    }

    public class ProductGetListParam
    {
        public List<long>? CategoryId { get; set; }
        public List<long>? ProductId { get; set; }
        public bool? IsActive { get; set; }
        public List<long>? AttachIds { get; set; }
        public long? ViewId { get; set; }
        public long? StoreId { get; set; }

        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
    }

    // =================================================================
    //  Customer + Group + Tag
    // =================================================================
    public class Customer
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CustomerGrpcRequestDto
    {
        public Expression<Func<Customer, bool>> Predicates { get; set; } = _ => true;
    }

    public class CustomerGetListParam
    {
        public bool? IsActive { get; set; }
        public List<long> TagIds { get; set; } = new();
        public List<long> CustomerGroupIds { get; set; } = new();
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public bool? IsNewAddress { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
        public bool IncludeTotalRecord { get; set; }
    }

    public enum ApplyForObject
    {
        Other = 0,
        CustomerGroup = 1,
        Tag = 2
    }

    public class GetCustomersByTagOrGroupRequestDto
    {
        public ApplyForObject ApplyFor { get; set; }
        public List<long> Ids { get; set; } = new();
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
    }

    public class GetCustomerDetailRequestDto
    {
        public bool? IsActive { get; set; }
        public List<long> TagIds { get; set; } = new();
        public List<long> CustomerGroupIds { get; set; } = new();
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public bool? IsNewAddress { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
        public bool IncludeTotalRecord { get; set; }
    }

    public class CustomerGroup
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CustomerGroupGrpcRequestDto
    {
        public Expression<Func<CustomerGroup, bool>> Predicates { get; set; } = _ => true;
    }

    public class CustomerGroupGetListParam
    {
        public bool? IsActive { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; } = string.Empty;
        public string SearchText { get; set; } = string.Empty;
    }

    public class CustomerTag
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CustomerTagGrpcRequestDto
    {
        public Expression<Func<CustomerTag, bool>> Predicates { get; set; } = _ => true;
    }

    public class GetTagOfCustomerTagByCustomerRequestDto
    {
        public List<long> CustomerIds { get; set; } = new();
    }
}
