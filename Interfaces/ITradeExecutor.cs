﻿using BackTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTester.Interfaces
{
    public interface ITradeExecutor
    {
        TradeResult ExecuteTrade(TradeSignal signal);
    }

}