using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Motion
{
    /// <summary>
    /// 运动控制卡工厂类
    /// 用于创建特定类型的控制卡实例
    /// </summary>
    public static  class MotionCardFactory
    {
        /// <summary>
        /// 创建控制卡实例
        /// </summary>
        /// <param name="cardType">控制卡类型</param>
        /// <param name="cardId">控制卡ID（多卡时使用）</param>
        /// <returns>控制卡接口实例</returns>
        public static IMotionControlCard CreateCard(CardType cardType, int cardId = 0)
        {
            switch (cardType)
            {
                case CardType.LeadShine:
                    return new LeadShineCard(cardId);
                case CardType.ZMotion:
                    return new ZMotionCard(cardId);
                default:
                    throw new ArgumentException("不支持的控制卡类型");
            }
        }
    }
}
