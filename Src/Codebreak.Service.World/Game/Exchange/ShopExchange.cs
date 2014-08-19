﻿using Codebreak.Service.World.Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Exchange
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ShopExchange : ExchangeBase
    {
        private EntityBase _buyer, _shop;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyer"></param>
        /// <param name="shop"></param>
        public ShopExchange(EntityBase buyer, EntityBase shop)
            : base(ExchangeTypeEnum.EXCHANGE_SHOP)
        {
            _buyer = buyer;
            _shop = shop;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Create()
        {
            base.Create();
            base.Dispatch(WorldMessage.EXCHANCE_ITEMS_LIST(_shop));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string SerializeAs_ExchangeCreate()
        {
            return _shop.Id.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="templateId"></param>
        /// <param name="quantity"></param>
        public override void BuyItem(EntityBase entity, int templateId, int quantity)
        {
            if (quantity < 1)
            {
                Logger.Debug("ShopExchange unable to buy, quantity < 0 : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_BUY_ERROR());
                return;
            }

            var template = _shop.ShopItems.Find(x => x.Id == templateId);

            if (template == null)
            {
                Logger.Debug("ShopExchange unable to buy null template : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_BUY_ERROR());
                return;
            }

            var price = template.Price * quantity;

            if (entity.Inventory.Kamas < price)
            {
                Logger.Debug("ShopExchange no enought kamas to buy item : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_BUY_ERROR());
                return;
            }

            var instance = template.CreateItem(quantity);
            if (instance == null)
            {
                Logger.Debug("ShopExchange error while creating object : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_BUY_ERROR());
                return;
            }

            entity.Inventory.SubKamas(price);
            entity.Inventory.AddItem(instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="guid"></param>
        /// <param name="quantity"></param>
        public override void SellItem(EntityBase entity, long guid, int quantity)
        {
            if (quantity < 1)
            {
                Logger.Debug("ShopExchange unable to sell, quantity < 1 : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_SELL_ERROR());
                return;
            }

            var item = entity.Inventory.Items.Find(entry => entry.Id == guid);

            if (item == null)
            {
                Logger.Debug("ShopExchange unable to sell null item : " + entity.Name);
                entity.Dispatch(WorldMessage.EXCHANGE_SELL_ERROR());
                return;
            }

            if (quantity > item.Quantity)
                quantity = item.Quantity;

            var price = (item.GetTemplate().Price / 10) * quantity;

            entity.Inventory.RemoveItem(guid, quantity);
            entity.Inventory.AddKamas(price);
        }
    }
}