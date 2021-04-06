using Dapper;
using Newtonsoft.Json;
using Orders.Domain.DataAccess.Repositories;
using Orders.Domain.Entities;
using Orders.Domain.Entities.Client;
using Orders.Domain.Enums;
using Orders.Infraestructure.Logging;
using Orders.Infrastructure.Extensions;
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
        private readonly ILogger _logger;
        private readonly IMySqlConnHelper _mySqlConnHelper;

        private readonly string QueryBase = $@"
                            SELECT 
                            O.`id`          AS {nameof(Order.Id)},
                            O.`quantity`    AS {nameof(Order.Quantity)},
                            O.`price`       AS {nameof(Order.Price)},
                            O.`amount`      AS {nameof(Order.Amount)},
                            O.`idStatus`    AS {nameof(Order.Status)},
                            O.createdAt     AS {nameof(Order.CreatedAt)},
                            O.updatedAt     AS {nameof(Order.UpdatedAt)},
                            O.variant       AS {nameof(Order.Variant)},
                            O.payment       AS {nameof(Order.Payment)},
                            O.discount      AS {nameof(Order.Discount)},
                            O.customizes    AS {nameof(Order.Customizes)},
                            A.id            AS {nameof(User.Id)},
                            A.name          AS {nameof(User.Name)},
                            A.email         AS {nameof(User.Email)},
                            A.phone         AS {nameof(User.Phone)},
                            A.country       AS {nameof(User.Country)},
                            A.characters    AS {nameof(User.Characters)},
                            P.id            AS {nameof(Product.Id)},
                            P.title         AS {nameof(Product.Title)},
                            P.description   AS {nameof(Product.Description)},
                            P.images_src    AS {nameof(Images.Src)},
                            C.id            AS {nameof(Category.Id)},
                            C.name          AS {nameof(Category.Name)},
                            G.id            AS {nameof(Game.Id)},
                            G.name          AS {nameof(Game.Name)},
                            AT.id           AS {nameof(User.Id)},
                            AT.name         AS {nameof(User.Name)},
                            AT.email        AS {nameof(User.Email)},
                            AT.phone        AS {nameof(User.Phone)},
                            AT.country      AS {nameof(User.Country)}
                            FROM `Vanlune`.`Orders`    AS O
                            LEFT JOIN `Vanlune`.`Accounts`  AS A  ON A.id = O.accountId
                            JOIN `Vanlune`.`Products`  AS P  ON P.id = O.productId
                            LEFT JOIN  Vanlune.Category     AS C  ON P.idCategory = C.id
                            LEFT JOIN `Vanlune`.`Games`     AS G  ON C.idGame = G.id
                            LEFT JOIN `Vanlune`.`Accounts`  AS AT ON AT.id = O.attendedBy";

        public OrderRepository(IMySqlConnHelper mySqlConnHelper,
            ILogger logger)
        {
            _mySqlConnHelper = mySqlConnHelper;
            _logger = logger;

            SqlMapper.AddTypeHandler(new DapperCharsTypeHandler());
            SqlMapper.AddTypeHandler(new DapperVariantsTypeHandler());
            SqlMapper.AddTypeHandler(new DapperCustomizesTypeHandler());
        }
        public async Task<int> AddAsync(Order order)
        {
            var query = $@"INSERT INTO `Vanlune`.`Orders`
                        (accountId,
                        `productId`,
                        `quantity`,
                        `price`,
                        `amount`,
                        `idStatus`,
                        `payment`,
                        `variant`,
                        `discount`
                        `paymentStatus`,
                        `externalId`)
                        VALUES
                        (@accountId,
                        @productId,
                        @{nameof(Order.Quantity)},
                        @{nameof(Order.Price)},
                        @{nameof(Order.Amount)},
                        @{nameof(Order.Status)},
                        @{nameof(Order.Payment)},
                        @{nameof(Order.Variant)},
                        @{nameof(Order.Discount)},
                        @{nameof(Order.PaymentStatus)},
                        @{nameof(Order.ExternalId)});
                        SELECT LAST_INSERT_ID();";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                accountId = order.User.Id,
                productId = order.Product.Id,
                order.Quantity,
                order.Price,
                order.Amount,
                order.Status,
                order.Payment,
                Variant = JsonConvert.SerializeObject(order.Variant),
                order.Discount,
                order.PaymentStatus,
                order.ExternalId
            });

            return result.Single();
        }

        public async Task<List<int>> AddAllAsync(IEnumerable<Order> orders)
        {
            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var createdOrders = new List<int>();
            var transaction = connection.BeginTransaction();

            try
            {
                foreach(var order in orders)
                {
                    var query = $@"INSERT INTO `Vanlune`.`Orders`
                        (accountId,
                        `productId`,
                        `quantity`,
                        `price`,
                        `amount`,
                        `idStatus`,
                        `payment`,
                        `variant`,
                        `discount`,
                        `customizes`,
                        `paymentStatus`,
                        `externalId`)
                        VALUES
                        (@accountId,
                        @productId,
                        @{nameof(Order.Quantity)},
                        @{nameof(Order.Price)},
                        @{nameof(Order.Amount)},
                        @{nameof(Order.Status)},
                        @{nameof(Order.Payment)},
                        @{nameof(Order.Variant)},
                        @{nameof(Order.Discount)},
                        @{nameof(Order.Customizes)},
                        @{nameof(Order.PaymentStatus)},
                        @{nameof(Order.ExternalId)});
                        SELECT LAST_INSERT_ID();";

                    var result = await connection.QueryAsync<int>(query, new
                    {
                        accountId = order.User.Id,
                        productId = order.Product.Id,
                        order.Quantity,
                        order.Price,
                        order.Amount,
                        order.Status,
                        order.Payment,
                        Variant = JsonConvert.SerializeObject(order.Variant),
                        order.Product.Discount,
                        customizes = JsonConvert.SerializeObject(order.Customizes),
                        order.PaymentStatus,
                        order.ExternalId
                    });

                    createdOrders.Add(result.Single());
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                createdOrders.Clear();
                transaction.Rollback();
            }

            return createdOrders;
        }

        public async Task UpdateOrder(Order order, string newExternalId = null)
        {
            var query = new StringBuilder();
            query.Append("UPDATE `Vanlune`.`Orders` SET ");
            var inTerms = new DynamicParameters();

            var hasProductId = order.Product?.Id > 0;
            if (hasProductId)
            {
                query.Append($" productId = @productId ");
                inTerms.Add("@productId", order.Product.Id);
            }

            var hasPrice = order.Price != 0;
            if (hasPrice)
            {
                if (hasProductId)
                    query.Append(",");
                query.Append($" price = @{nameof(Order.Price)} ");
                inTerms.Add($"@{nameof(Order.Price)}", order.Price);
            }

            var hasAmount = order.Amount != 0;
            if (hasAmount)
            {
                if (hasProductId || hasPrice)
                    query.Append(",");
                query.Append($" amount = @{nameof(Order.Amount)} ");
                inTerms.Add($"@{nameof(Order.Amount)}", order.Amount);
            }

            var hasStatus = order.Status != null;
            if (hasStatus)
            {
                if (hasProductId || hasPrice || hasAmount)
                    query.Append(",");
                query.Append($" idStatus = @{nameof(Order.Status)} ");
                inTerms.Add($"@{nameof(Order.Status)}", order.Status);
            }

            var hasPayment = order.Payment != null;
            if (hasPayment)
            {
                if (hasProductId || hasPrice || hasAmount || hasStatus)
                    query.Append(",");
                query.Append($" payment = @{nameof(Order.Payment)} ");
                inTerms.Add($"@{nameof(Order.Payment)}", order.Payment);
            }

            var hasDiscount = order.Discount > 0;
            if (hasDiscount)
            {
                if (hasProductId || hasPrice || hasAmount || hasStatus || hasPayment)
                    query.Append(",");
                query.Append($" discount = @{nameof(Order.Discount)} ");
                inTerms.Add($"@{nameof(Order.Discount)}", order.Discount);
            }

            var hasAttendent = order.Attendent?.Id > 0;
            if (hasAttendent)
            {
                if (hasProductId || hasPrice || hasAmount || hasStatus || hasPayment || hasDiscount)
                    query.Append(",");
                query.Append($" attendedBy = @{nameof(Order.Attendent.Id)} ");
                inTerms.Add($"@{nameof(Order.Attendent.Id)}", order.Attendent.Id);
            }

            var hasPaymentStatus = !string.IsNullOrEmpty(order.PaymentStatus);
            if (hasPaymentStatus)
            {
                if (hasProductId || hasPrice || hasAmount || hasStatus || hasPayment || hasDiscount || hasAttendent)
                    query.Append(",");
                query.Append($" paymentStatus = @{nameof(Order.PaymentStatus)} ");
                inTerms.Add($"@{nameof(Order.PaymentStatus)}", order.PaymentStatus);
            }

            var hasNewExternalId = !string.IsNullOrEmpty(newExternalId);
            if (hasNewExternalId)
            {
                if (hasProductId || hasPrice || hasAmount || hasStatus || hasPayment || hasDiscount || hasAttendent || hasPaymentStatus)
                    query.Append(",");
                query.Append($" externalId = @newExternalId ");
                inTerms.Add($"@newExternalId", newExternalId);
            }

            query.Append($" WHERE ");

            var hasId = order.Id > 0;
            if (hasId)
            {
                query.Append($" id=@orderId ");
                inTerms.Add($"@orderId", order.Id);
            }

            var hasExternalId = !string.IsNullOrEmpty(order.ExternalId);
            if (hasExternalId)
            {
                if (hasId) query.Append($" AND ");
                query.Append($" externalId=@externalId ");
                inTerms.Add($"@externalId", order.ExternalId);
            }
            try
            {

                using var connection = _mySqlConnHelper.MySqlConnection();

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                await connection.ExecuteAsync(query.ToString(), inTerms).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByFilters(IDictionary<string, string> filters)
        {
            var query = new StringBuilder();
            query.Append(QueryBase);
            query.Append(" WHERE ");

            var inTerms = new DynamicParameters();
            var hasId = filters.ContainsKey("id") && !string.IsNullOrEmpty(filters["id"]);
            if (hasId)
            {
                query.Append(" O.`id`=@id ");
                inTerms.Add("@id", filters["id"]);
            }
            
            var hasCategory = filters.ContainsKey("category") && !string.IsNullOrEmpty(filters["category"]);
            if (hasCategory)
            {
                if (hasId) query.Append(" AND ");
                query.Append(" P.`idCategory`=@category ");
                inTerms.Add("@category", filters["category"]);
            }

            var hasCategoryName = filters.ContainsKey("categoryName") && !string.IsNullOrEmpty(filters["categoryName"]);
            if (hasCategoryName)
            {
                if (hasId || hasCategory) query.Append(" AND ");
                query.Append(" C.name=@categoryName ");
                inTerms.Add("@categoryName", filters["categoryName"]);
            }

            var hasGame = filters.ContainsKey("game") && !string.IsNullOrEmpty(filters["game"]);
            if (hasGame)
            {
                if (hasId || hasCategory || hasCategoryName) query.Append(" AND ");
                query.Append(" P.`idGames`=@game ");
                inTerms.Add("@game", filters["game"]);
            }

            var hasTitle = filters.ContainsKey("title") && !string.IsNullOrEmpty(filters["title"]);
            if (hasTitle)
            {
                if (hasId || hasCategory || hasCategoryName || hasGame) query.Append(" AND ");
                query.Append(" P.`title`=@title ");
                inTerms.Add("@title", filters["title"]);
            }

            var hasAttendentId = filters.ContainsKey("attendedById") && !string.IsNullOrEmpty(filters["attendedById"]);
            if (hasAttendentId)
            {
                if (hasId || hasCategory || hasCategoryName || hasGame || hasTitle) query.Append(" AND ");
                query.Append(" O.`attendedBy`=@attendedById ");
                inTerms.Add("@attendedById", filters["attendedById"]);
            }

            var hasAttendentName = filters.ContainsKey("attendedByName") && !string.IsNullOrEmpty(filters["attendedByName"]);
            if (hasAttendentName)
            {
                if (hasId || hasCategory || hasCategoryName || hasGame || hasTitle || hasAttendentId) 
                    query.Append(" AND ");
                query.Append(" AT.name=@attendedByName ");
                inTerms.Add("@attendedByName", filters["attendedByName"]);
            }

            var hasStatus = filters.ContainsKey("status") && !string.IsNullOrEmpty(filters["status"]);
            if (hasStatus)
            {
                if (hasId || hasCategory || hasCategoryName || 
                    hasGame || hasTitle || hasAttendentId || 
                    hasAttendentName)
                    query.Append(" AND ");
                query.Append(" O.`idStatus`=@status ");
                inTerms.Add("@status", filters["status"]);
            }

            var hasAmount = filters.ContainsKey("amount") && !string.IsNullOrEmpty(filters["amount"]);
            if (hasAmount)
            {
                if (hasId || hasCategory || hasCategoryName || 
                    hasGame || hasTitle || hasAttendentId || 
                    hasAttendentName || hasStatus)
                    query.Append(" AND ");
                query.Append(" O.`amount` >= @amount ");
                inTerms.Add("@amount", filters["amount"]);
            }

            var hasPayment = filters.ContainsKey("payment") && !string.IsNullOrEmpty(filters["payment"]);
            if (hasPayment)
            {
                if (hasId || hasCategory || hasCategoryName || 
                    hasGame || hasTitle || hasAttendentId || 
                    hasAttendentName || hasStatus || hasAmount)
                    query.Append(" AND ");
                query.Append(" O.payment >= @payment ");
                inTerms.Add("@payment", filters["payment"]);
            }

            var hasDiscount = filters.ContainsKey("discount") && !string.IsNullOrEmpty(filters["discount"]);
            if (hasDiscount)
            {
                if (hasId || hasCategory || hasCategoryName || 
                    hasGame || hasTitle || hasAttendentId || 
                    hasAttendentName || hasStatus || hasAmount || 
                    hasPayment)
                    query.Append(" AND ");
                query.Append(" O.discount >= 0 ");
            }

            var hasAttendentEmail = filters.ContainsKey("attendentEmail") && !string.IsNullOrEmpty(filters["attendentEmail"]);
            if (hasAttendentEmail)
            {
                if (hasId || hasCategory || hasCategoryName ||
                    hasGame || hasTitle || hasAttendentId ||
                    hasAttendentName || hasStatus || hasAmount ||
                    hasPayment || hasDiscount)
                    query.Append(" AND ");
                query.Append(" AT.email >= @attendentEmail ");
                inTerms.Add("@attendentEmail", filters["attendentEmail"]);
            }

            var hasStartDate = filters.ContainsKey("startDate") && !string.IsNullOrEmpty(filters["startDate"]);
            if (hasStartDate)
            {
                if (hasId || hasCategory || hasCategoryName ||
                    hasGame || hasTitle || hasAttendentId ||
                    hasAttendentName || hasStatus || hasAmount ||
                    hasPayment || hasDiscount || hasAttendentEmail)
                    query.Append(" AND ");
                query.Append(" AT.createdAt >= @startDate ");
                inTerms.Add("@startDate", filters["startDate"]);
            }

            var hasEndDate = filters.ContainsKey("endDate") && !string.IsNullOrEmpty(filters["endDate"]);
            if (hasEndDate)
            {
                if (hasId || hasCategory || hasCategoryName ||
                    hasGame || hasTitle || hasAttendentId ||
                    hasAttendentName || hasStatus || hasAmount ||
                    hasPayment || hasDiscount || hasAttendentEmail ||
                    hasStartDate)
                    query.Append(" AND ");
                query.Append(" AT.createdAt <= @endDate ");
                inTerms.Add("@endDate", filters["endDate"]);
            }

            var hasPaymentStatus = filters.ContainsKey("paymentStatus") && !string.IsNullOrEmpty(filters["paymentStatus"]);
            if (hasPaymentStatus)
            {
                if (hasId || hasCategory || hasCategoryName ||
                    hasGame || hasTitle || hasAttendentId ||
                    hasAttendentName || hasStatus || hasAmount ||
                    hasPayment || hasDiscount || hasAttendentEmail ||
                    hasStartDate || hasEndDate)
                    query.Append(" AND ");
                query.Append(" O.paymentStatus = @paymentStatus ");
                inTerms.Add("@paymentStatus", filters["paymentStatus"]);
            }

            var hasExternalId = filters.ContainsKey("externalId") && !string.IsNullOrEmpty(filters["externalId"]);
            if (hasExternalId)
            {
                if (hasId || hasCategory || hasCategoryName ||
                    hasGame || hasTitle || hasAttendentId ||
                    hasAttendentName || hasStatus || hasAmount ||
                    hasPayment || hasDiscount || hasAttendentEmail ||
                    hasStartDate || hasEndDate || hasPaymentStatus)
                    query.Append(" AND ");
                query.Append(" O.externalId = @externalId ");
                inTerms.Add("@externalId", filters["externalId"]);
            }

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, Product, Images, Category, Game, User,Order>(query.ToString(),
                (order, user, product, image, category, game, attendent) =>
                {
                    order.User = user;
                    order.Product = product;
                    order.Attendent = attendent;

                    if (order.Product != null)
                    {
                        order.Product.Game = game;
                        order.Product.Category = category;
                    }
                    if (image != null && !string.IsNullOrEmpty(image.Src))
                        order.Product.Images = new Images[] { image };

                    return order;
                },
                inTerms,
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)},{nameof(Images.Src)},{nameof(Category.Id)},{nameof(Game.Id)},{nameof(User.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var query = $@"{QueryBase}";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, Product, Images, Game, Order>(query,
                (order, user, product, image, game) =>
                {
                    order.User = user;
                    order.Product = product;
                    order.Product.Game = game;

                    if (image != null && !string.IsNullOrEmpty(image.Src))
                        order.Product.Images = new Images[] { image };

                    return order;
                },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)},{nameof(Images.Src)},{nameof(Game.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByEmail(string email)
        {
            var query = $@"{QueryBase}
                            WHERE A.email = @email";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, Product, Images, Game, Order>(query,
                (order, user, product, image, game) =>
                {
                    order.User = user;
                    order.Product = product;
                    order.Product.Game = game;

                    if (image != null && !string.IsNullOrEmpty(image.Src))
                        order.Product.Images = new Images[] { image };

                    return order;
                },
                new { email },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)},{nameof(Images.Src)},{nameof(Game.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(Status status)
        {
            var query = $@"{QueryBase}
                            WHERE O.`idStatus` = @status";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, Product, Images, Game, Order>(query,
                (order, user, product, image, game) =>
                {
                    order.User = user;
                    order.Product = product;
                    order.Product.Game = game;

                    if (image != null && !string.IsNullOrEmpty(image.Src))
                        order.Product.Images = new Images[] { image };

                    return order;
                },
                new { status },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)},{nameof(Images.Src)},{nameof(Game.Id)}");

            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            var query = $@"{QueryBase}
                            WHERE A.id = @userId";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Order, User, Product, Images, Game, Order>(query,
                (order, user, product, image, game) =>
                {
                    order.User = user;
                    order.Product = product;
                    order.Product.Game = game;
                    
                    if (image != null && !string.IsNullOrEmpty(image.Src))
                        order.Product.Images = new Images[] { image };                    

                    return order;
                },
                new { userId },
                splitOn: $"{nameof(Order.Id)},{nameof(User.Id)},{nameof(Product.Id)},{nameof(Images.Src)},{nameof(Game.Id)}");

            return result;
        }
    }
}
