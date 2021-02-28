using Dapper;
using Orders.Domain.DataAccess.Repositories;
using Orders.Domain.Entities;
using Orders.Domain.Entities.Client;
using Orders.Domain.Entities.DTO;
using Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Infrastructure.DataAccess.Database
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public OrderRepository(IMySqlConnHelper mySqlConnHelper)
        {
            _mySqlConnHelper = mySqlConnHelper;
        }
        public async Task<int> AddAsync(Order order)
        {
            var query = $@"INSERT INTO `Vanlune`.`Orders`
                        (`accountId`,
                        `productId`,
                        `quantity`,
                        `price`,
                        `amount`,
                        `idStatus`,
                        `email`,
                        `phone`)
                        VALUES
                        (@{nameof(Order.User.Id)},
                        @productId,
                        @{nameof(Order.Quantity)},
                        @{nameof(Order.Product.Price)},
                        @{nameof(Order.Amount)},
                        @{nameof(Order.Status)},
                        @{nameof(Order.User.Email)},
                        @{nameof(Order.User.Phone)});";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                order.User.Id,
                productId = order.Product.Id,
                order.Quantity,
                order.Product.Price,
                order.Amount,
                order.Status,
                order.User.Email,
                order.User.Phone
            });

            return result.Single();
        }

        public async Task AddAllAsync(IEnumerable<Order> orders)
        {
            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                foreach(var order in orders)
                {
                    var query = $@"INSERT INTO `Vanlune`.`Orders`
                        (`accountId`,
                        `productId`,
                        `quantity`,
                        `price`,
                        `amount`,
                        `idStatus`,
                        `email`,
                        `phone`)
                        VALUES
                        (@{nameof(Order.User.Id)},
                        @productId,
                        @{nameof(Order.Quantity)},
                        @{nameof(Order.Product.Price)},
                        @{nameof(Order.Amount)},
                        @{nameof(Order.Status)},
                        @{nameof(Order.User.Email)},
                        @{nameof(Order.User.Phone)});";

                    await connection.ExecuteAsync(query, new
                    {
                        order.User.Id,
                        productId = order.Product.Id,
                        order.Quantity,
                        order.Product.Price,
                        order.Amount,
                        order.Status,
                        order.User.Email,
                        order.User.Phone
                    }).ConfigureAwait(false);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var query = $@"SELECT 
                            O.`id`          AS {nameof(Order.Id)},
                            O.`quantity`    AS {nameof(Order.Quantity)},
                            O.`price`       AS {nameof(Order.Price)},
                            O.`amount`      AS {nameof(Order.Amount)},
                            O.`idStatus`    AS {nameof(Order.Status)},
                            O.email         AS {nameof(Order.User.Email)},
                            O.phone         AS {nameof(Order.User.Phone)},
                            A.id            AS {nameof(User.Id)},
                            A.name          AS {nameof(User.Name)},
                            A.email         AS {nameof(User.Email)},
                            A.phone         AS {nameof(User.Phone)},
                            A.country       AS {nameof(User.Country)},
                            A.characters    AS {nameof(User.Characters)},
                            P.id            AS {nameof(ProductDto.Id)},
                            P.title         AS {nameof(ProductDto.Title)},
                            P.description   AS {nameof(ProductDto.Description)},
                            P.sale          AS {nameof(ProductDto.Sale)},
                            P.price         AS {nameof(ProductDto.Price)},
                            P.quantity      AS {nameof(ProductDto.Quantity)},
                            P.discount      AS {nameof(ProductDto.Discount)},
                            P.images_src    AS {nameof(ProductDto.Image.Src)}
                            FROM `Vanlune`.`Orders` AS O
                            JOIN `Vanlune`.`Accounts` AS A ON A.id = O.accountId
                            JOIN `Vanlune`.`Products` AS P ON P.id = O.productId";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, ProductDto, Order>(query,
                (order, user, product) => 
                {
                    if (user != null)
                        order.User = user;
                    order.Product = product;

                    return order;
                }, splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByEmail(string email)
        {
            var query = $@"SELECT 
                            O.`id`          AS {nameof(Order.Id)},
                            O.`quantity`    AS {nameof(Order.Quantity)},
                            O.`price`       AS {nameof(Order.Price)},
                            O.`amount`      AS {nameof(Order.Amount)},
                            O.`idStatus`    AS {nameof(Order.Status)},
                            O.email         AS {nameof(Order.User.Email)},
                            O.phone         AS {nameof(Order.User.Phone)},
                            A.id            AS {nameof(User.Id)},
                            A.name          AS {nameof(User.Name)},
                            A.email         AS {nameof(User.Email)},
                            A.phone         AS {nameof(User.Phone)},
                            A.country       AS {nameof(User.Country)},
                            A.characters    AS {nameof(User.Characters)},
                            P.id            AS {nameof(ProductDto.Id)},
                            P.title         AS {nameof(ProductDto.Title)},
                            P.description   AS {nameof(ProductDto.Description)},
                            P.sale          AS {nameof(ProductDto.Sale)},
                            P.price         AS {nameof(ProductDto.Price)},
                            P.quantity      AS {nameof(ProductDto.Quantity)},
                            P.discount      AS {nameof(ProductDto.Discount)},
                            P.images_src    AS {nameof(ProductDto.Image.Src)}
                            FROM `Vanlune`.`Orders` AS O
                            JOIN `Vanlune`.`Accounts` AS A ON A.id = O.accountId
                            JOIN `Vanlune`.`Products` AS P ON P.id = O.productId
                            WHERE O.email = @email";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, ProductDto, Order>(query,
                (order, user, product) =>
                {
                    if (user != null)
                        order.User = user;
                    order.Product = product;

                    return order;
                }, 
                new { email },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(Status status)
        {
            var query = $@"SELECT 
                            O.`id`          AS {nameof(Order.Id)},
                            O.`quantity`    AS {nameof(Order.Quantity)},
                            O.`price`       AS {nameof(Order.Price)},
                            O.`amount`      AS {nameof(Order.Amount)},
                            O.`idStatus`    AS {nameof(Order.Status)},
                            O.email         AS {nameof(Order.User.Email)},
                            O.phone         AS {nameof(Order.User.Phone)},
                            A.id            AS {nameof(User.Id)},
                            A.name          AS {nameof(User.Name)},
                            A.email         AS {nameof(User.Email)},
                            A.phone         AS {nameof(User.Phone)},
                            A.country       AS {nameof(User.Country)},
                            A.characters    AS {nameof(User.Characters)},
                            P.id            AS {nameof(ProductDto.Id)},
                            P.title         AS {nameof(ProductDto.Title)},
                            P.description   AS {nameof(ProductDto.Description)},
                            P.sale          AS {nameof(ProductDto.Sale)},
                            P.price         AS {nameof(ProductDto.Price)},
                            P.quantity      AS {nameof(ProductDto.Quantity)},
                            P.discount      AS {nameof(ProductDto.Discount)},
                            P.images_src    AS {nameof(ProductDto.Image.Src)}
                            FROM `Vanlune`.`Orders` AS O
                            JOIN `Vanlune`.`Accounts` AS A ON A.id = O.accountId
                            JOIN `Vanlune`.`Products` AS P ON P.id = O.productId
                            WHERE O.status = @status";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, ProductDto, Order>(query,
                (order, user, product) =>
                {
                    if (user != null)
                        order.User = user;
                    order.Product = product;

                    return order;
                },
                new { status },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            var query = $@"SELECT 
                            O.`id`          AS {nameof(Order.Id)},
                            O.`quantity`    AS {nameof(Order.Quantity)},
                            O.`price`       AS {nameof(Order.Price)},
                            O.`amount`      AS {nameof(Order.Amount)},
                            O.`idStatus`    AS {nameof(Order.Status)},
                            O.email         AS {nameof(Order.User.Email)},
                            O.phone         AS {nameof(Order.User.Phone)},
                            A.id            AS {nameof(User.Id)},
                            A.name          AS {nameof(User.Name)},
                            A.email         AS {nameof(User.Email)},
                            A.phone         AS {nameof(User.Phone)},
                            A.country       AS {nameof(User.Country)},
                            A.characters    AS {nameof(User.Characters)},
                            P.id            AS {nameof(ProductDto.Id)},
                            P.title         AS {nameof(ProductDto.Title)},
                            P.description   AS {nameof(ProductDto.Description)},
                            P.sale          AS {nameof(ProductDto.Sale)},
                            P.price         AS {nameof(ProductDto.Price)},
                            P.quantity      AS {nameof(ProductDto.Quantity)},
                            P.discount      AS {nameof(ProductDto.Discount)},
                            P.images_src    AS {nameof(ProductDto.Image.Src)}
                            FROM `Vanlune`.`Orders` AS O
                            JOIN `Vanlune`.`Accounts` AS A ON A.id = O.accountId
                            JOIN `Vanlune`.`Products` AS P ON P.id = O.productId
                            WHERE A.id = @userId";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, ProductDto, Order>(query,
                (order, user, product) =>
                {
                    if (user != null)
                        order.User = user;
                    order.Product = product;

                    return order;
                },
                new { userId },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)}");

            return result;
        }
    }
}
