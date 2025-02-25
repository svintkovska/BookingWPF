﻿using AutoMapper;
using BLL.ModelsDTO;
using DAL.Data;
using DAL.Data.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService
    {
        EFAppContext context = new EFAppContext();
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemsRepository _orderItemsRepository;
        private readonly BasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;

        public OrderService()
        {
            _orderRepository = new OrderRepository(context);
            _orderItemsRepository = new OrderItemsRepository(context);
            _basketRepository = new BasketRepository(context);
            _productRepository = new ProductRepository(context);
        }

        public List<BasketDTO> GetBasketsByUserId(int userId)
        {
            var baskets = _basketRepository.GetAll().Where(i => i.UserId == userId).ToList();
            List<BasketDTO> list = new List<BasketDTO>();

            foreach (var item in baskets)
            {
                var translateObj = new MapperConfiguration(map => map.CreateMap<BasketEntity, BasketDTO>()).CreateMapper();
                var basketDTO = translateObj.Map<BasketEntity, BasketDTO>(item);
                list.Add(basketDTO);
            }
            return list;
        }
        public async Task AddToBasket(BasketDTO item)
        {
            if (item != null)
            {

                var translateObj = new MapperConfiguration(map => map.CreateMap<BasketDTO, BasketEntity>()).CreateMapper();
                var entity = translateObj.Map<BasketDTO, BasketEntity>(item);

                await _basketRepository.Create(entity);
            }
        }

        public async Task UpdateBasket(BasketDTO item)
        {
            if (item != null)
            {
                var translateObj = new MapperConfiguration(map => map.CreateMap<BasketDTO, BasketEntity>()).CreateMapper();
                var entity = translateObj.Map<BasketDTO, BasketEntity>(item);
                await _basketRepository.Update(entity);
            }
        }
        public async Task ClearBasket()
        {
            var baskets =  _basketRepository.GetAll().AsNoTracking().ToList();
            foreach (var item in baskets)
            {
                var basket = new BasketEntity()
                {
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    Count = item.Count
                };
                context.Baskets.Remove(basket);
            }
            await  context.SaveChangesAsync();
        }
        public async Task DeleteFromBasket(int userId, int productId)
        {
            await _basketRepository.Delete(userId, productId);
        }

        public async Task<int> CreateOrder(int userId)
        {
            var order = new OrderDTO()
            {
                UserId = userId,
                DateCreated = DateTime.Now
            };
            var translateObj = new MapperConfiguration(map => map.CreateMap<OrderDTO, OrderEntity>()).CreateMapper();
            var entity = translateObj.Map<OrderDTO, OrderEntity>(order);

            await _orderRepository.Create(entity);
            return entity.Id;
        }

        public async Task CreateOrderItems(int userId, int orderId)
        {
            var basketQuery = await context.Baskets.AsQueryable().Where(u => u.UserId == userId).ToListAsync() ;

            var orderItems = new List<OrderItemDTO>();

            foreach (var item in basketQuery)
            {
                var product = await context.Products.AsQueryable().Where(p => p.Id == item.ProductId).FirstOrDefaultAsync();
                var price = product.IsOnDiscount ? product.DiscountPrice : product.Price;
                var order = new OrderItemDTO()
                {
                    ProductId = item.ProductId,
                    DateCreated = DateTime.Now,
                    Count = item.Count,
                    Price = price,
                    OrderId = orderId
                };
                var translateObj = new MapperConfiguration(map => map.CreateMap<OrderItemDTO, OrderItemEntity>()).CreateMapper();
                var entity = translateObj.Map<OrderItemDTO, OrderItemEntity>(order);
               await  _orderItemsRepository.Create(entity);
            }
        }

        public List<OrderDTO> GetOrderHistory(int userId)
        {
            var orders = _orderRepository.GetAll().Where(u => u.UserId == userId);
            var list = new List<OrderDTO>();
            foreach (var order in orders)
            {
                var translateObj = new MapperConfiguration(map => map.CreateMap<OrderEntity, OrderDTO>()).CreateMapper();
                var orderDTO = translateObj.Map<OrderEntity, OrderDTO>(order);
                list.Add(orderDTO);
            }
            return list;
        }

        public List<OrderItemDTO> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = _orderItemsRepository.GetAll().Where(o => o.OrderId == orderId);
            var list = new List<OrderItemDTO>();
            foreach (var item in orderItems)
            {
                var translateObj = new MapperConfiguration(map => map.CreateMap<OrderItemEntity, OrderItemDTO>()).CreateMapper();
                var itemDTO = translateObj.Map<OrderItemEntity, OrderItemDTO>(item);
                list.Add(itemDTO);
            }
            return list;
        }
    }
}
