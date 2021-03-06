﻿using System;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces;
using DarkLight.Framework.Interfaces.Common;

namespace DarkLight.Infrastructure.Filter
{
    public class TradeEventFilter : IFilter<TradeEvent>
    {
        private readonly string _key;


        public TradeEventFilter(string key)
        {
            _key = key;
        }

        #region Implementation of IFilter<LinkedEvent>

        public bool IsPassedBy(TradeEvent message)
        {
            if (message.Key == _key)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        #endregion

    }
}