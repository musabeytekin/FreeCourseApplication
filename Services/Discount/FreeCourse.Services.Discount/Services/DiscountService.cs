using System.Data;
using Dapper;
using FreeCourse.Shared.DTOs;
using Npgsql;

namespace FreeCourse.Services.Discount.Services;

public class DiscountService : IDiscountService
{
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _dbConnection;

    public DiscountService(IConfiguration configuration)
    {
        _configuration = configuration;
        _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
    }

    public async Task<Response<List<Models.Discount>>> GetAll()
    {
        var dicounts = await _dbConnection.QueryAsync<Models.Discount>("Select * From Discounts");
        return Response<List<Models.Discount>>.Success(dicounts.ToList(), 200);
    }

    public async Task<Response<Models.Discount>> GetById(int id)
    {
        var discount =
            await _dbConnection.QueryFirstOrDefaultAsync<Models.Discount>("Select * From discounts Where Id=@Id",
                new { Id = id });
        if (discount is null)
        {
            return Response<Models.Discount>.Fail("Discount not found", 404);
        }

        return Response<Models.Discount>.Success(discount, 200);
    }

    public async Task<Response<NoContent>> Save(Models.Discount discount)
    {
        var saveStatus = await _dbConnection.ExecuteAsync(
            "INSERT INTO discounts (UserId,Rate,Code) VALUES (@UserId,@Rate,@Code)",
            discount);
        if (saveStatus > 0)
        {
            return Response<NoContent>.Success(204);
        }

        return Response<NoContent>.Fail("An error occurred while adding", 500);
    }

    public async Task<Response<NoContent>> Update(Models.Discount discount)
    {
        var updateStatus = await _dbConnection.ExecuteAsync(
            "UPDATE discounts SET UserId=@UserId,Rate=@Rate,Code=@Code WHERE Id=@Id",
            discount);
        if (updateStatus > 0)
        {
            return Response<NoContent>.Success(204);
        }

        return Response<NoContent>.Fail("An error occurred while updating", 500);
    }

    public async Task<Response<NoContent>> Delete(int id)
    {
        var deleteStatus = await _dbConnection.ExecuteAsync("DELETE FROM discounts WHERE Id=@Id", new { Id = id });
        if (deleteStatus > 0)
        {
            return Response<NoContent>.Success(204);
        }

        return Response<NoContent>.Fail("Discount not found", 404);
    }

    public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
    {
        var discount = await _dbConnection.QueryFirstOrDefaultAsync<Models.Discount>(
            "Select * From discounts Where UserId=@UserId and Code=@Code",
            new { UserId = userId, Code = code });
        if (discount is null)
        {
            return Response<Models.Discount>.Fail("Discount not found", 404);
        }

        return Response<Models.Discount>.Success(discount, 200);
    }
}