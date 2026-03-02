using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Motion
{
    /// <summary>
    /// 运动控制卡接口
    /// 定义通用的运动控制功能，兼容不同厂商的控制卡
    /// </summary>
    public interface IMotionControlCard
    {
        /// <summary>
        /// 初始化控制卡
        /// </summary>
        /// <returns>初始化成功返回true，失败返回false</returns>
        bool Initialize();

        /// <summary>
        /// 关闭控制卡连接
        /// </summary>
        /// <returns>关闭成功返回true，失败返回false</returns>
        bool Close();

        /// <summary>
        /// 设置脉冲输出模式
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="mode">脉冲模式：0-脉冲+方向，1-CCW/CW</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        bool SetPulseMode(int axis, int mode);

        /// <summary>
        /// 设置加速度
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="acceleration">加速度</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        bool SetAcceleration(int axis, double acceleration);

        /// <summary>
        /// 设置减速度
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="deceleration">减速度</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        bool SetDeceleration(int axis, double deceleration);

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="velocity">速度</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        bool SetVelocity(int axis, double velocity);

        /// <summary>
        /// 相对运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="distance">移动距离（脉冲数）</param>
        /// <returns>启动运动成功返回true，失败返回false</returns>
        bool MoveRelative(int axis, int distance);

        /// <summary>
        /// 绝对运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">目标位置（脉冲数）</param>
        /// <returns>启动运动成功返回true，失败返回false</returns>
        bool MoveAbsolute(int axis, int position);

        /// <summary>
        /// 停止运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="immediate">是否立即停止</param>
        /// <returns>停止成功返回true，失败返回false</returns>
        bool Stop(int axis, bool immediate = true);

        /// <summary>
        /// 急停所有轴
        /// </summary>
        /// <returns>急停成功返回true，失败返回false</returns>
        bool EmergencyStop();

        /// <summary>
        /// 获取轴状态
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>轴状态对象</returns>
        AxisStatus GetAxisStatus(int axis);

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>当前位置（脉冲数）</returns>
        int GetCurrentPosition(int axis);

        /// <summary>
        /// 设置当前位置为原点
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>设置成功返回true，失败返回false</returns>
        bool SetHomePosition(int axis);

        /// <summary>
        /// 回原点运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="direction">回零方向：0-正方向，1-负方向</param>
        /// <param name="velocity">回零速度</param>
        /// <returns>启动回零成功返回true，失败返回false</returns>
        bool GoHome(int axis, int direction, double velocity);

        /// <summary>
        /// 读取输入信号状态
        /// </summary>
        /// <param name="inputIndex">输入点编号</param>
        /// <returns>输入状态：true-高电平，false-低电平</returns>
        bool ReadInput(int inputIndex);

        /// <summary>
        /// 写入输出信号状态
        /// </summary>
        /// <param name="outputIndex">输出点编号</param>
        /// <param name="state">输出状态：true-高电平，false-低电平</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        bool WriteOutput(int outputIndex, bool state);

        /// <summary>
        /// 读取所有输入状态
        /// </summary>
        /// <returns>输入状态数组</returns>
        bool[] ReadAllInputs();

        /// <summary>
        /// 写入所有输出状态
        /// </summary>
        /// <param name="states">输出状态数组</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        bool WriteAllOutputs(bool[] states);
    }
}
