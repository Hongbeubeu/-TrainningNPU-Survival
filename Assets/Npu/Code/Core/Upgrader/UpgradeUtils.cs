using System;
using Npu.Core;

namespace Npu
{

    public static class UpgradeUtils
    {
        public static SecuredDouble GetBulkCost_Linear(SecuredDouble a, SecuredDouble b, int currentLevel,
            int levelCount, double discount = 1)
        {
            return (a * levelCount + b * (2 * currentLevel + levelCount - 1) * levelCount / 2) * discount;
        }

        public static long GetMaxUpgrades_Linear(SecuredDouble a, SecuredDouble b, int currentLevel,
            SecuredDouble money, double discount = 1)
        {
            double A = b / 2;
            double B = a + b * (2 * currentLevel - 1) / 2;
            var C = -money / discount;
            var DELTA = B * B - 4 * A * C;
            var root = (-B + Math.Sqrt(DELTA)) / (2 * A);
            return (long) Math.Floor(root);
        }

        public static (long levels, SecuredDouble cost) GetUpgrades_Linear(SecuredDouble a, SecuredDouble b,
            int currentLevel, SecuredDouble money, int maxLvls, double discount = 1)
        {
            var cost = GetBulkCost_Linear(a, b, currentLevel, maxLvls, discount);
            if (cost <= money) return (maxLvls, cost);

            var lvls = GetMaxUpgrades_Linear(a, b, currentLevel, money, discount);
            return (lvls, lvls == 0 ? cost : GetBulkCost_Linear(a, b, currentLevel, (int) lvls, discount));
        }

        public static SecuredDouble GetBulkCost(SecuredDouble a, SecuredDouble b, int currentLevel, int levelCount,
            double discount = 1)
        {
            return a * b.Pow(currentLevel) * (b.Pow(levelCount) - 1) / (b - 1) * discount;
        }

        public static long GetMaxUpgrades(SecuredDouble a, SecuredDouble b, int currentLevel, SecuredDouble money,
            double discount = 1)
        {
            var n = Math.Log10(money * (b - 1) / (a * b.Pow(currentLevel) * discount) + 1) / Math.Log10(b);
            return (long) Math.Floor(n);
        }

        public static (long levels, SecuredDouble cost) GetUpgrades(SecuredDouble a, SecuredDouble b, int currentLevel,
            SecuredDouble money, int maxLvls, double discount = 1)
        {
            var cost = GetBulkCost(a, b, currentLevel, maxLvls, discount);
            if (cost <= money) return (maxLvls, cost);

            var lvls = GetMaxUpgrades(a, b, currentLevel, money, discount);
            return (lvls, lvls == 0 ? cost : GetBulkCost(a, b, currentLevel, (int) lvls, discount));
        }
    }

}