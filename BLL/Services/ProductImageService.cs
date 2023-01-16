﻿using AutoMapper;
using BLL.Interfaces;
using BLL.ModelsDTO;
using DAL.Data;
using DAL.Data.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductImageService : IService<ProductImageDTO>
    {
        private readonly IProductImagesRepository _productImgRepository;
        public ProductImageService()
        {
            EFAppContext context = new EFAppContext();
            _productImgRepository = new ProductImageRepository(context);
        }
        public async Task<int> Create(ProductImageDTO item)
        {
            if (item != null)
            {
                var entity = TranslateProductImageDTOToProductImageEntity(item);
                await _productImgRepository.Create(entity);
                return entity.Id;
            }
            return 0;
        }

        public async Task Delete(int? id)
        {
            if (id != null)
            {
               await _productImgRepository.Delete((int)id);
            };
        }

        public async Task <ProductImageDTO> Find(int? id)
        {
            if (id != null)
            {
                ProductImageEntity item = await _productImgRepository.GetById((int)id);
                return TranslateProductImageEntityToProductImageDTO(item);
            }
            return null;
        }

        public IList<ProductImageDTO> GetAll()
        {
            var list = new List<ProductImageDTO>();
            foreach (var cat in _productImgRepository.GetAll())
            {
                list.Add(TranslateProductImageEntityToProductImageDTO(cat));
            }
            return list;
        }

        public async Task Update(int id, ProductImageDTO item)
        {
            if (item != null)
            {
               await _productImgRepository.Update(id, TranslateProductImageDTOToProductImageEntity(item));
            }
        }

        private ProductImageEntity TranslateProductImageDTOToProductImageEntity(ProductImageDTO productImgDTO)
        {
            var translateObj = new MapperConfiguration(map => map.CreateMap<ProductImageDTO, ProductImageEntity>()).CreateMapper();
            if (productImgDTO != null)
                return new ProductImageEntity()
                {
                    Id = productImgDTO.Id,
                    Name = productImgDTO.Name,
                    Priority = productImgDTO.Priority,
                    DateCreated = productImgDTO.DateCreated,
                    IsDelete = productImgDTO.IsDelete,
                    ProductId = productImgDTO.ProductId
                };
            return null;
        }

        private ProductImageDTO TranslateProductImageEntityToProductImageDTO(ProductImageEntity productImgEntity)
        {
            var translateObj = new MapperConfiguration(map => map.CreateMap<ProductImageEntity, ProductImageDTO>()).CreateMapper();

            if (productImgEntity != null)
                return new ProductImageDTO()
                {
                    Id = productImgEntity.Id,
                    Name = productImgEntity.Name,
                    Priority = productImgEntity.Priority,
                    DateCreated = productImgEntity.DateCreated,
                    IsDelete = productImgEntity.IsDelete
                };
            return null;
        }
    }
}
